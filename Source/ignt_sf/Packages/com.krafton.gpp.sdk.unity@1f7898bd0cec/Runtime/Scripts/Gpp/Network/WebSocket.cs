using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using Gpp.CommonUI;
using Gpp.Extension;
using Gpp.Log;
using Gpp.Network;
using Gpp.Utils;
using UnityEngine;
using Gpp.WebSocketSharp;

namespace Gpp.Core
{
    internal class WebSocket : IWebSocket
    {
        private WebSocketSharp.WebSocket webSocket;
        private bool isProxySet;
        private string proxyUrl;
        private string proxyUsername;
        private string proxyPassword;

        public event OnOpenHandler OnOpen;
        public event OnMessageHandler OnMessage;
        public event OnErrorHandler OnError;
        public event OnCloseHandler OnClose;

        ~WebSocket()
        {
            OnOpen = null;
            OnMessage = null;
            OnError = null;
            OnClose = null;
        }

        public WsState ReadyState
        {
            get
            {
                if (webSocket == null)
                {
                    return WsState.Closed;
                }

                switch (webSocket.ReadyState)
                {
                    case WebSocketState.Open: return WsState.Open;
                    case WebSocketState.Closed: return WsState.Closed;
                    case WebSocketState.Closing: return WsState.Closing;
                    case WebSocketState.Connecting: return WsState.Connecting;
                    default: throw new WebSocketInvalidStateException("Unrecognized websocket ready state.");
                }
            }
        }

        public void SetProxy(string url, string username, string password)
        {
            proxyUrl = url;
            proxyUsername = username;
            proxyPassword = password;
            isProxySet = true;
        }

        public static Dictionary<string, string> GetWebSocketCustomHeaders(string sessionId)
        {
#if UNITY_ANDROID
            string operatingSystem = "Android";
#elif UNITY_IOS
            string operatingSystem = "IOS";
#elif UNITY_PS5
            string operatingSystem = "PS5";
#else
            string operatingSystem = "Windows";
#endif
            var session = GppSDK.GetSession();
            return new Dictionary<string, string>
            {
                ["x-gpp-app-id"] = $"{GppSDK.GetConfig()?.Namespace ?? ""};{GppSDK.GetOptions().AppVersion}",
                ["x-gpp-device-id"] = GppDeviceProvider.GetGppDeviceId(),
                ["x-gpp-sdk-version"] = GppSDK.SdkVersion,
                ["x-gpp-analytics-id"] = session.AnalyticsId,
                ["x-gpp-ua"] = $"{GppSDK.SdkVersion};{session.LanguageCode};{session.CountryCode};unity;{GppUtil.GetDevice()};{operatingSystem};{GppUtil.GetOSDetail()};{SystemInfo.deviceModel}",
                ["x-gpp-model"] = SystemInfo.deviceModel,
                ["x-gpp-lang-code-device"] = PreciseLocale.GetLanguage(),
                ["x-gpp-country-code-device"] = session.GetSystemCountry(),
                ["x-gpp-analytics-id"] = session.AnalyticsId,
                ["x-gpp-store"] = GppSDK.GetOptions().Store.ToString(),
                ["x-ab-lobbysessionid"] = sessionId
            };
        }

        public void Connect(string url)
        {
            Connect(url, null, null);
        }

        public void Connect(string url, string protocols, string sessionId, Dictionary<string, string> customHeaders = null)
        {
            try
            {
                webSocket = string.IsNullOrEmpty(protocols) ? new WebSocketSharp.WebSocket(url) : new WebSocketSharp.WebSocket(url, protocols);
                webSocket.SslConfiguration.EnabledSslProtocols = SslProtocols.Tls11 | SslProtocols.Tls12;

                if (customHeaders != null)
                {
                    foreach (var kv in customHeaders)
                    {
                        webSocket.SetUserHeader(kv.Key, kv.Value);
                    }
                }

                SetProxy();
                SetHandlers();
            }
            catch (Exception e)
            {
                throw new WebSocketUnexpectedException("Websocket cannot be created.", e);
            }

            CheckAndConnect();
        }

        private void CheckAndConnect()
        {
            switch (webSocket.ReadyState)
            {
                case WebSocketState.Open: return;
                case WebSocketState.Closing: throw new WebSocketInvalidStateException("WebSocket is closing.");
                case WebSocketState.Closed: break;
                default:

                    try
                    {
                        webSocket.Connect();
                    }
                    catch (Exception e)
                    {
                        throw new WebSocketUnexpectedException("Websocket failed to connect.", e);
                    }

                    break;
            }
        }

        private void SetProxy()
        {
            if (isProxySet)
            {
                webSocket.SetProxy(proxyUrl, proxyUsername, proxyPassword);
            }
        }

        private void SetHandlers()
        {
            webSocket.OnOpen -= WebSocket_OnOpen;
            webSocket.OnOpen += WebSocket_OnOpen;

            webSocket.OnMessage -= WebSocket_OnMessage;
            webSocket.OnMessage += WebSocket_OnMessage;

            webSocket.OnError -= WebSocket_OnError;
            webSocket.OnError += WebSocket_OnError;

            webSocket.OnClose -= WebSocket_OnClose;
            webSocket.OnClose += WebSocket_OnClose;
        }

        private void WebSocket_OnOpen(object sender, EventArgs ev)
        {
            OnOpen?.Invoke();
        }

        private void WebSocket_OnMessage(object sender, MessageEventArgs ev)
        {
            if (ev.RawData == null) return;
            OnMessage?.Invoke(ev.Data);
        }

        private void WebSocket_OnError(object sender, ErrorEventArgs ev)
        {
            OnError?.Invoke(ev.Exception.Message);
        }

        private void WebSocket_OnClose(object sender, CloseEventArgs ev)
        {
            OnClose?.Invoke(ev.Code);
            OnMessage?.Invoke(ev.Reason);
        }

        public void Send(string message)
        {
            if (webSocket.ReadyState != WebSocketState.Open)
                throw new WebSocketInvalidStateException("Websocket is not open.");

            try
            {
                GppLog.LogWebSocketReq(message);
                webSocket.SendAsync(message, null);
            }
            catch (Exception e)
            {
                throw new WebSocketUnexpectedException("Unexpected websocket exception.", e);
            }
        }

        public void Ping()
        {
            webSocket.SendAsync("", null);
        }

        public void Close(WsCloseCode code = WsCloseCode.Normal, string reason = null)
        {
            if (webSocket.ReadyState == WebSocketState.Closing ||
                webSocket.ReadyState == WebSocketState.Closed)
            {
                return;
            }

            try
            {
                webSocket.Close((ushort)code, reason);
            }
            catch (Exception e)
            {
                throw new WebSocketUnexpectedException("Failed to close the connection.", e);
            }
        }
    }
}