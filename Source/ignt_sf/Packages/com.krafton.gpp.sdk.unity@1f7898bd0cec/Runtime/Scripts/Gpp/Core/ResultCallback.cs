using System.Runtime.CompilerServices;
using Gpp.Datadog;
using Gpp.Log;

namespace Gpp.Core
{
    public delegate void ResultCallback<T>(Result<T> result);

    public delegate void ResultCallback(Result result);

    internal static class ResultCallbackExtension
    {
        public static void Try<T>(this ResultCallback<T> callback, Result<T> param)
        {
            callback?.Invoke(param);
        }

        public static void TryOk<T>(this ResultCallback<T> callback, T value)
        {
            callback?.Invoke(Result<T>.CreateOk(value));
        }
        
        public static void Try(this ResultCallback callback, Result param)
        {
            callback?.Invoke(param);
        }
        
        public static void TryOk(this ResultCallback callback)
        {
            callback?.Invoke(Result.CreateOk());
        }
        
        public static void TryError<T>(this ResultCallback<T> callback, Error error, bool isSendDatadog = true, [CallerMemberName] string callerName = "")
        {
            var result = Result<T>.CreateError(error);
            
            if (isSendDatadog)
            {
                SendDatadog(result.Error?.Code ?? ErrorCode.None, result.Error?.Message ?? "", callerName);
            }
            
            callback?.Invoke(result);
        }
        
        public static void TryError<T>(this ResultCallback<T> callback, ErrorCode errorCode, string errorMessage = null, bool isSendDatadog = true, [CallerMemberName] string callerName = "")
        {
            var result = Result<T>.CreateError(errorCode, errorMessage);
            
            if (isSendDatadog)
            {
                SendDatadog(result.Error?.Code ?? ErrorCode.None, result.Error?.Message ?? "", callerName);
            }
            
            callback?.Invoke(result);
        }
        
        public static void TryError<T>(this ResultCallback<T> callback, object value, ErrorCode errorCode, string errorMessage = null, bool isSendDatadog = true, [CallerMemberName] string callerName = "")
        {
            var result = Result<T>.CreateError(value, errorCode, errorMessage);
            
            if (isSendDatadog)
            {
                SendDatadog(result.Error?.Code ?? ErrorCode.None, result.Error?.Message ?? "", callerName);
            }
            
            callback?.Invoke(result);
        }

        public static void TryError(this ResultCallback callback, Error error, bool isSendDatadog = true, [CallerMemberName] string callerName = "")
        {
            var result = Result.CreateError(error);
            
            if (isSendDatadog)
            {
                SendDatadog(result.Error?.Code ?? ErrorCode.None, result.Error?.Message ?? "", callerName);
            }
            
            callback?.Invoke(result);
        }
        
        public static void TryError(this ResultCallback callback, ErrorCode errorCode, string errorMessage = null, bool isSendDatadog = true, [CallerMemberName] string callerName = "")
        {
            var result = Result.CreateError(errorCode, errorMessage);
            
            if (isSendDatadog)
            {
                SendDatadog(result.Error?.Code ?? ErrorCode.None, result.Error?.Message ?? "", callerName);
            }
            
            callback?.Invoke(result);
        }

        private static void SendDatadog(ErrorCode errorCode, string errorMessage = null, string callerName = "")
        {
            var remoteLog = new GppClientRemoteLog()
            {
                ErrorCode = ((int)errorCode).ToString(),
                Log = errorMessage
            };
            DatadogManager.ReportLog(remoteLog, callerName);
        }
    }
}