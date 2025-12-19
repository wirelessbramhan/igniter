using System;
using System.Collections.Generic;

namespace Gpp.Network
{
    public delegate void OnOpenHandler();

    public delegate void OnMessageHandler(string data);

    public delegate void OnErrorHandler(string errorMsg);

    public delegate void OnCloseHandler(ushort closeCode);

    public enum WsState
    {
        Connecting,
        Open,
        Closing,
        Closed
    }

    public enum WsCloseCode
    {
        /* Do NOT use NotSet - it's only purpose is to indicate that the close code cannot be parsed. */
        NotSet = 0,
        Normal = 1000,
        Away = 1001,
        ProtocolError = 1002,
        UnsupportedData = 1003,
        Undefined = 1004,
        NoStatus = 1005,
        Abnormal = 1006,
        InvalidData = 1007,
        PolicyViolation = 1008,
        TooBig = 1009,
        MandatoryExtension = 1010,
        ServerError = 1011,
        ServiceRestart = 1012,
        TryAgainLater = 1013,
        BadGateway = 1014,
        TlsHandshakeFailure = 1015,
        ClosedByServer = 4000,
        TokenRevoked = 4001,
        DisconnectedByServer = 4003,
        DisconnectByMergeDeletion = 4004,
        DisconnectKeepExistingCode = 4005,
    }

    public abstract class WebSocketException : Exception
    {
        protected WebSocketException(string message, Exception innerException = null) : base(message, innerException)
        {
        }
    }

    public class WebSocketUnexpectedException : WebSocketException
    {
        public WebSocketUnexpectedException(string message, Exception innerException = null) : base(message, innerException)
        {
        }
    }

    public class WebSocketInvalidStateException : WebSocketException
    {
        public WebSocketInvalidStateException(string message, Exception innerException = null) : base(message, innerException)
        {
        }
    }

    public interface IWebSocket
    {
        void Connect(string url);
        void Connect(string url, string protocols, string sessionId, Dictionary<string, string> customHeaders = null);
        void Close(WsCloseCode code = WsCloseCode.Normal, string reason = null);
        void Send(string message);

        void Ping();
        WsState ReadyState { get; }
        event OnOpenHandler OnOpen;
        event OnMessageHandler OnMessage;
        event OnErrorHandler OnError;
        event OnCloseHandler OnClose;
    }
}