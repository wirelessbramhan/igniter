using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Gpp.Core;
using Gpp.Extension;
using Gpp.Log;
using Gpp.Models;
using Gpp.Network;
using Gpp.Utils;
using UnityEngine;
using UnityEngine.Assertions;

namespace Gpp.Api
{
    internal class Lobby
    {
        internal event Action Connected;
        internal event ResultCallback<DisconnectNotify> Disconnecting;
        internal event ResultCallback<DisconnectNotify> DuplicatedSession;
        internal event ResultCallback<DisconnectNotify> AccountDeletion;
        internal event Action<WsCloseCode> Disconnected;
        internal event ResultCallback<Notification> OnNotification;
        internal event ResultCallback<RefreshSurvey> OnRefreshSurvey;

        private readonly int _pingDelay;
        private readonly int _backoffDelay;
        private readonly int _maxDelay;
        private readonly int _totalTimeout;

        private readonly Dictionary<long, Action<ErrorCode, string>> _responseCallbacks = new();

        private readonly string _websocketUrl;
        private readonly LoginSession _session;
        private readonly CoroutineRunner _coroutineRunner;
        private readonly object _syncToken = new();
        private readonly IWebSocket _webSocket;
        private bool _reconnectsOnClose;
        private WsCloseCode _wsCloseCode = WsCloseCode.NotSet;
        private long _id;
        private LobbySessionId _lobbySessionId;
        private Coroutine _maintainConnectionCoroutine;

        public event EventHandler OnRetryAttemptFailed;

        internal Lobby(IWebSocket webSocket, int pingDelay = 4000, int backoffDelay = 1000, int maxDelay = 30000, int totalTimeout = 60000)
        {
            Assert.IsNotNull(webSocket);

            _websocketUrl = $"{GppUtil.ConvertUrlToWebSocketUrl(GppSDK.GetConfig().BaseUrl)}/lobby/";
            _session = GppSDK.GetSession();
            _coroutineRunner = GppSDK.GetCoroutineRunner();
            _webSocket = webSocket;
            _pingDelay = pingDelay;
            _backoffDelay = backoffDelay;
            _maxDelay = maxDelay;
            _totalTimeout = totalTimeout;
            _reconnectsOnClose = false;
            _lobbySessionId = new LobbySessionId();

            Disconnecting += LobbyEventHandler.LobbyDisconnectingHandler;
            DuplicatedSession += LobbyEventHandler.LobbyDuplicatedSessionHandler;
            OnNotification += LobbyEventHandler.LobbyNotificationHandler;
            Disconnected += LobbyEventHandler.LobbyDisconnectedHandler;
            AccountDeletion += LobbyEventHandler.LobbyAccountDeletionHandler;
            OnRefreshSurvey += LobbyEventHandler.LobbyRefreshSurveyHandler;

            _webSocket.OnOpen += HandleOnOpen;
            _webSocket.OnMessage += HandleOnMessage;
            _webSocket.OnClose += HandleOnClose;
        }

        public bool IsConnected => _webSocket.ReadyState == WsState.Open;

        public void RestoreLobby(bool keepExisting = false)
        {
            _coroutineRunner.Run(ReconnectToRestore(keepExisting));
        }

        private IEnumerator ReconnectToRestore(bool keepExisting = false)
        {
            yield return new WaitForSeconds(30f);
            Connect(keepExisting);
        }

        public void Reconnect()
        {
            if (IsConnected)
            {
                Disconnect();
            }
            Connect();
        }

        public void Connect(bool keepExisting = false)
        {
            if (!_session.IsLoggedIn())
            {
                GppLog.Log("Need to be logged in to connect to the lobby.");
                return;
            }

            if (IsConnected)
            {
                GppLog.Log("Already lobby is connected!.");
                return;
            }

            if (_webSocket.ReadyState == WsState.Connecting)
            {
                return;
            }

            if (string.IsNullOrEmpty(_session.AccessToken))
            {
                return;
            }

            _wsCloseCode = WsCloseCode.NotSet;
            ConnectLobbyAsync(keepExisting);
        }

        private async void ConnectLobbyAsync(bool keepExisting)
        {
            // MainThread가 아닌 Thread에서는 유니티에서 제공하는 API(DeviceId, Operation)를 사용할 수 없으므로 여기서 CustomHeader를 생성한다.
            Dictionary<string, string> customHeaders = WebSocket.GetWebSocketCustomHeaders(_lobbySessionId.lobbySessionID);
            await Task.Run(() =>
                {
                    _webSocket.Connect($"{_websocketUrl}?keepExisting={keepExisting.ToString().ToLower()}", _session.AccessToken, null, customHeaders);
                }
            );
            if (_wsCloseCode == WsCloseCode.NotSet || IsReConnectable(_wsCloseCode))
            {
                StartMaintainConnection();
            }
        }

        private void StartMaintainConnection()
        {
            _reconnectsOnClose = true;
            _maintainConnectionCoroutine = _coroutineRunner.Run(MaintainConnection(_backoffDelay, _maxDelay, _totalTimeout));
        }

        private void StopMaintainConnection()
        {
            _reconnectsOnClose = false;

            if (_maintainConnectionCoroutine == null) return;

            _coroutineRunner.Stop(_maintainConnectionCoroutine);
            _maintainConnectionCoroutine = null;
        }

        private IEnumerator MaintainConnection(int backoffDelay, int maxDelay, int totalTimeout)
        {
            while (true)
            {
                switch (_webSocket.ReadyState)
                {
                    case WsState.Open:
                        _webSocket.Ping();

                        yield return new WaitForSeconds(_pingDelay / 1000f);

                        break;
                    case WsState.Connecting:
                        while (_webSocket.ReadyState == WsState.Connecting)
                        {
                            yield return new WaitForSeconds(1f);
                        }

                        break;
                    case WsState.Closing:
                        while (_webSocket.ReadyState == WsState.Closing)
                        {
                            yield return new WaitForSeconds(1f);
                        }

                        break;
                    case WsState.Closed:
                        var firstClosedTime = DateTime.Now;
                        var timeout = TimeSpan.FromSeconds(totalTimeout / 1000f);

                        while (_reconnectsOnClose && _webSocket.ReadyState == WsState.Closed && DateTime.Now - firstClosedTime < timeout)
                        {
                            yield return new WaitUntil(() => Application.internetReachability != NetworkReachability.NotReachable);
                            _webSocket.Connect($"{_websocketUrl}?keepExisting=true", _session.AccessToken, _lobbySessionId.lobbySessionID, WebSocket.GetWebSocketCustomHeaders(_lobbySessionId.lobbySessionID));
                            yield return new WaitForSeconds(backoffDelay / 1000f);
                        }

                        if (_webSocket.ReadyState == WsState.Closed && Application.internetReachability != NetworkReachability.NotReachable)
                        {
                            RaiseOnRetryAttemptFailed();
                            yield break;
                        }

                        break;
                }
            }
        }

        public void Disconnect()
        {
            if (!IsConnected) return;

            StopMaintainConnection();

            try
            {
                if (_session != null)
                {
                    _session.RefreshTokenCallback -= RefreshTokenCallback;
                }

                if (_webSocket.ReadyState is WsState.Open or WsState.Connecting)
                {
                    _webSocket.Close();
                }
            }
            catch (Exception ex)
            {
                GppLog.LogWarning(ex);
            }
        }

        protected virtual void RaiseOnRetryAttemptFailed()
        {
            OnRetryAttemptFailed?.Invoke(this, EventArgs.Empty);
        }

        private void RefreshToken(string newAccessToken, ResultCallback callback)
        {
            SendRequest(MessageType.refreshTokenRequest, new RefreshAccessTokenRequest { token = newAccessToken }, callback);
        }

        private long GenerateId()
        {
            lock (_syncToken)
            {
                if (_id < long.MaxValue)
                {
                    _id++;
                }
                else
                {
                    _id = 0;
                }
            }

            return _id;
        }

        private void SendRequest<T, TU>(MessageType requestType, T requestPayload, ResultCallback<TU> callback)
            where T : class, new() where TU : class, new()
        {
            long messageId = GenerateId();
            var writer = new StringWriter();
            LobbyParser.WriteHeader(writer, requestType, messageId);
            LobbyParser.WritePayload(writer, requestPayload);

            _responseCallbacks[messageId] = (errorCode, response) =>
            {
                Result<TU> result;

                if (errorCode != ErrorCode.None)
                {
                    result = Result<TU>.CreateError(errorCode);
                }
                else
                {
                    errorCode = LobbyParser.ReadPayload(response, out TU responsePayload);

                    result = errorCode != ErrorCode.None
                        ? Result<TU>.CreateError(errorCode)
                        : Result<TU>.CreateOk(responsePayload);
                }

                _coroutineRunner.Run(() => callback.Try(result));
            };

            _webSocket.Send(writer.ToString());
        }

        private void SendRequest<T>(MessageType requestType, T requestPayload, ResultCallback callback) where T : class, new()
        {
            long messageId = GenerateId();
            var writer = new StringWriter();
            LobbyParser.WriteHeader(writer, requestType, messageId);
            LobbyParser.WritePayload(writer, requestPayload);

            _responseCallbacks[messageId] = (errorCode, _) =>
            {
                Result result = errorCode != ErrorCode.None ? Result.CreateError(errorCode) : Result.CreateOk();
                _coroutineRunner.Run(() => callback.Try(result));
            };

            _webSocket.Send(writer.ToString());
        }

        private void SendRequest<TU>(MessageType requestType, ResultCallback<TU> callback) where TU : class, new()
        {
            long messageId = GenerateId();
            var writer = new StringWriter();
            LobbyParser.WriteHeader(writer, requestType, messageId);

            _responseCallbacks[messageId] = (errorCode, response) =>
            {
                Result<TU> result;

                if (errorCode != ErrorCode.None)
                {
                    result = Result<TU>.CreateError(errorCode);
                }
                else
                {
                    errorCode = LobbyParser.ReadPayload(response, out TU responsePayload);

                    result = errorCode != ErrorCode.None
                        ? Result<TU>.CreateError(errorCode)
                        : Result<TU>.CreateOk(responsePayload);
                }

                _coroutineRunner.Run(() => callback.Try(result));
            };

            _webSocket.Send(writer.ToString());
        }

        private void SendRequest(MessageType requestType, ResultCallback callback)
        {
            long messageId = GenerateId();
            var writer = new StringWriter();
            LobbyParser.WriteHeader(writer, requestType, messageId);

            _responseCallbacks[messageId] = (errorCode, _) =>
            {
                Result result = errorCode != ErrorCode.None ? Result.CreateError(errorCode) : Result.CreateOk();

                _coroutineRunner.Run(() => callback.Try(result));
            };

            _webSocket.Send(writer.ToString());
        }

        private void HandleOnOpen()
        {
            _coroutineRunner.Run(
                () =>
                {
                    Action handler = Connected;

                    handler?.Invoke();

                    if (_session != null)
                    {
                        _session.RefreshTokenCallback += RefreshTokenCallback;
                    }
                }
            );
        }

        private void RefreshTokenCallback(string token)
        {
            RefreshToken(token, result =>
                {
                    GppLog.Log($"Send refresh token event to lobby: {result.ToPrettyJsonString()}");
                }
            );
        }

        private void HandleOnClose(ushort closeCode)
        {
            var code = (WsCloseCode)closeCode;
            _wsCloseCode = code;
            _coroutineRunner.Run(
                () =>
                {
                    if (!IsReConnectable(code))
                    {
                        StopMaintainConnection();
                    }

                    Action<WsCloseCode> handler = Disconnected;

                    if (handler != null)
                    {
                        handler(code);
                    }

                    if (_session != null)
                    {
                        _session.RefreshTokenCallback -= RefreshTokenCallback;
                    }
                }
            );
        }

        private void HandleOnMessage(string message)
        {
            GppLog.LogWebSocketRes(message);

            ErrorCode errorCode = LobbyParser.ReadHeader(message, out var messageType, out var messageId);

            switch (messageType)
            {
                case MessageType.messageNotif:
                    HandleNotification(message, OnNotification);
                    break;
                case MessageType.connectNotif:
                    LobbyParser.ReadPayload(message, out _lobbySessionId);
                    break;
                case MessageType.disconnectNotif:
                    HandleNotification(message, Disconnecting);
                    break;
                case MessageType.accountDeletionNotif:
                    HandleNotification(message, AccountDeletion);
                    break;
                case MessageType.disconnectByDuplicatedLoggedInNotif:
                    HandleNotification(message, DuplicatedSession);
                    break;

                case MessageType.surveyRefreshNotif:
                    HandleNotification(message, OnRefreshSurvey);
                    break;
                default:
                    if (messageId != -1 && _responseCallbacks.TryGetValue(messageId, out var handler))
                    {
                        _responseCallbacks.Remove(messageId);
                        handler(errorCode, message);
                    }

                    break;
            }
        }

        private void HandleNotification<T>(string message, ResultCallback<T> handler) where T : class, new()
        {
            if (handler == null)
            {
                return;
            }

            ErrorCode errorCode = LobbyParser.ReadPayload(message, out T payload);

            if (errorCode != ErrorCode.None)
            {
                _coroutineRunner.Run(() => handler(Result<T>.CreateError(errorCode)));
            }
            else
            {
                _coroutineRunner.Run(() => handler(Result<T>.CreateOk(payload)));
            }
        }

        private static bool IsReConnectable(WsCloseCode code)
        {
            switch (code)
            {
                case WsCloseCode.Abnormal:
                case WsCloseCode.ServerError:
                case WsCloseCode.ServiceRestart:
                case WsCloseCode.TryAgainLater:
                case WsCloseCode.TlsHandshakeFailure: return true;
                default:
                    {
                        return false;
                    }
            }
        }
    }
}