using System;
using System.Collections;
using Gpp.Extension;
using Gpp.Log;
using Gpp.Utils;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.CompilerServices;

namespace Gpp.Datadog
{
    public static class DatadogManager
    {   
        private const string RemoteLogSuffix = "client_logs/datadog";
        private const int TIMEOUT_SECONDS = 3;
        
        internal static void ReportException(string log, string trace)
        {
            var gppClientRemoteLog = new GppClientRemoteLog()
            {
                StackTrace = trace,
                Log = log,
                LogLevel = nameof(LogType.Exception)
            };
            
            GppSDK.GetCoroutineRunner()?.Run(SendLog(gppClientRemoteLog.ToUtf8Json()));
        }
        
        internal static void ReportLog(GppClientRemoteLog remoteLog, [CallerMemberName] string callerMemberName = "")
        {
            if (remoteLog is null)
            {
                return;
            }

            if (IsReportUserError() is false)
            {
                return;
            }
            
            remoteLog.FunctionName = callerMemberName;
            GppSDK.GetCoroutineRunner()?.Run(SendLog(remoteLog.ToUtf8Json()));
        }

        private static bool IsReportUserError()
        {   
            var remoteConfig = GppSDK.GetSession()?.RemoteSdkConfig;
            return remoteConfig is null || remoteConfig.IsReportUserError;
        }
        
        private static string GetRemoteLogPath()
        {
            return GppSDK.GetConfig() is { BaseUrl: var baseUrl } && string.IsNullOrWhiteSpace(baseUrl) is false
                ? $"{GppUtil.EnsureTrailingSlash(baseUrl)}{RemoteLogSuffix}"
                : null;
        }
        
        private static IEnumerator SendLog(byte[] data)
        {
            if (data is null || data.Length == 0)
            {
                yield break;
            }
            
            UnityWebRequest request = null;
            
            try
            {
                request = new UnityWebRequest(GetRemoteLogPath(), "POST");
                request.uploadHandler = new UploadHandlerRaw(data);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.timeout = TIMEOUT_SECONDS;
            }
            catch (Exception e)
            {
                GppLog.LogWarning($"Failed to send log: {e}");
                request?.Dispose();
                yield break;
            }

            yield return request.SendWebRequest();

            try
            {
                if (request.result is not UnityWebRequest.Result.Success)
                {
                    GppLog.LogWarning($"Failed to send log: {request.error}");
                }
            }
            finally
            {
                request?.Dispose();
            }
        }
    }
}
