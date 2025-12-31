using System;
using System.Collections;
using System.Collections.Generic;
using Gpp.Constants;
using Gpp.Core;
using Gpp.Extension;
using Gpp.Log;
using Gpp.Models;
using Gpp.Network;
using UnityEngine;

namespace Gpp.Auth
{
    internal class AuthWebSocket
    {
        private IWebSocket ws;
        private const int DefaultPingSeconds = 40;
        private Coroutine pingCoroutine;
        private readonly CoroutineRunner coroutineRunner;

        public Action<AuthWebSocketResult> onReceive;
        public Action onConnected;
        public Action<string> onConnectError;
        public Action onClosed;

        private readonly Dictionary<AuthMsgType, bool> managedMessages = new()
        {
            { AuthMsgType.MSG_DEVICE_AUTH_GRANT_USER_CODE_CONSUMED, false },
            { AuthMsgType.MSG_DEVICE_AUTH_GRANT_COMPLETE, false },
            { AuthMsgType.MSG_DEVICE_AUTH_HAS_ISSUE, false },
            { AuthMsgType.MSG_USER_STATUS_CHECK_STARTED, false },
            { AuthMsgType.MSG_USER_STATUS_HAS_ISSUE, false },
            { AuthMsgType.MSG_USER_STATUS_RESOLVED, false }
        };

        internal AuthWebSocket()
        {
            coroutineRunner = GppSDK.GetCoroutineRunner();
            ws = new WebSocket();
            ws.OnOpen += OnOpen;
            ws.OnMessage += OnMessage;
            ws.OnError += OnError;
            ws.OnClose += OnClose;
        }

        private void OnOpen()
        {
            GppLog.Log("OnOpen AuthWebSocket.");
            pingCoroutine = coroutineRunner.Run(SendPingEverySeconds(DefaultPingSeconds));
            onConnected?.Invoke();
        }

        private void OnMessage(string data)
        {
            var convertedData = new AuthWebSocketResult(data);
            if (managedMessages.ContainsKey(convertedData.Message))
            {
                if (managedMessages[convertedData.Message])
                {
                    return;
                }

                managedMessages[convertedData.Message] = true;
            }
            GppLog.Log($"WebSocket Received Message: {data}");
            onReceive?.Invoke(convertedData);
        }

        private void OnError(string reason)
        {
            onConnectError?.Invoke(reason);
        }

        private void OnClose(ushort wsCode)
        {
            GppLog.Log($"WebSocket Closed. {wsCode}");
            onClosed?.Invoke();
        }

        public void Connect(string url)
        {
            if (ws.ReadyState != WsState.Open)
            {
                ws.Connect(url);
            }
        }

        private IEnumerator SendPingEverySeconds(int sec)
        {
            while (ws is { ReadyState: WsState.Open })
            {
                ws.Send(AuthMsgType.MSG_PING.ToString());
                yield return new WaitForSeconds(sec);
            }
        }

        public void Close()
        {
            GppSyncContext.RunOnUnityThread(() =>
                {
                    if (ws == null)
                    {
                        return;
                    }

                    ws.OnOpen -= OnOpen;
                    ws.OnMessage -= OnMessage;
                    ws.OnError -= OnError;
                    ws.OnClose -= OnClose;
                    if (ws.ReadyState == WsState.Open)
                    {
                        ws.Close();
                    }

                    ws = null;
                    if (pingCoroutine == null)
                    {
                        return;
                    }

                    if (coroutineRunner == null) return;
                    coroutineRunner.Stop(pingCoroutine);
                    pingCoroutine = null;
                }
            );
        }
    }
}