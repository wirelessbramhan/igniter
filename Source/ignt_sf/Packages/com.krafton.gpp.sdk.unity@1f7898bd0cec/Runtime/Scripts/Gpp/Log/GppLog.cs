using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Gpp.Extension;
using Gpp.Network;
using UnityEngine;
using UnityEngine.Networking;

namespace Gpp.Log
{
    internal static class GppLog
    {
        private static readonly Lazy<ILogger> Logger = new(() => new Logger(new GppLogHandler()));
        private const string Tag = "[GPP]";

        internal static string HashId { get; private set; }

        static GppLog()
        {
            HashId = GetHashId();
            SetFilterLogType(LogType.Log);
        }

        private static void SetFilterLogType(LogType type)
        {
            Logger.Value.filterLogType = type;
        }


        private static string GetHashId()
        {
            try
            {
                var md5 = MD5.Create();
                var encodedPw = Encoding.UTF8.GetBytes(Process.GetCurrentProcess().Id.ToString());
                var hash = md5.ComputeHash(encodedPw);
                var hashString = hash.Select(x => x.ToString("x2"))
                    .Aggregate((cur, next) => cur + next);

                hashString = hashString.Substring(0, 5);
                return hashString;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static void Log(object message)
        {
            if (GppSDK.GetConfig() is null)
            {
                return;
            }
            if (!GppSDK.GetConfig().EnableDebugLog)
            {
                return;
            }
            Logger.Value.Log(LogType.Log, Tag, message);
        }

        public static void LogWarning(object message)
        {
            Logger.Value.Log(LogType.Warning, Tag, message);
        }

        public static void LogException(object message)
        {
            Logger.Value.Log(LogType.Exception, Tag, message);
        }

        private static Dictionary<string, string> tempColorDict = new();
        
        private static string GetRandomColor(string key)
        {
            if (tempColorDict.TryGetValue(key, out var color))
            {
                return color;
            }
            
            // Hue range 0.2 ~ 0.85 to avoid red colors (0 ~ 0.15 is red/orange range)
            return tempColorDict[key] = $"#{ColorUtility.ToHtmlStringRGB(UnityEngine.Random.ColorHSV(0.2f, 0.85f, 0.7f, 1f, 1f, 1f))}";
        }
        
        public static void LogHttpReq(IHttpRequest request, UnityWebRequest unityWebRequest)
        {
            if (GppSDK.GetConfig() is null)
            {
                return;
            }
            if (!GppSDK.GetConfig().EnableDebugLog)
            {
                return;
            }
            var httpLog = $"HTTP <color={GetRandomColor(unityWebRequest.url)}> >> {request.Method} {unityWebRequest.url}</color>\n";
            if (request.BodyBytes != null)
            {
                httpLog += $"{Encoding.UTF8.GetString(request.BodyBytes)}\n\n";
            }

            httpLog = request.Headers.Aggregate(httpLog, (current, item) => current + (item.Key + " : " + item.Value + "\n"));
            Logger.Value.Log(Tag, httpLog);
        }

        public static void LogHttpRes(UnityWebRequest unityWebRequest)
        {
            if (GppSDK.GetConfig() is null)
            {
                return;
            }
            if (!GppSDK.GetConfig().EnableDebugLog)
            {
                return;
            }
            var httpLog = $"HTTP <color={GetRandomColor(unityWebRequest.url)}> << {unityWebRequest.method} {unityWebRequest.responseCode}({unityWebRequest.responseCode.ParseHttpCodeToString()}) {unityWebRequest.url}</color>\n" +
                          $"{unityWebRequest.downloadHandler.text}\n" +
                          $"\n{unityWebRequest.GetResponseHeaders().ToPrettyJsonString()}\n";
            Logger.Value.Log(Tag, httpLog);
        }

        public static void LogWebSocketReq(string message)
        {
            if (GppSDK.GetConfig() is null)
            {
                return;
            }
            if (!GppSDK.GetConfig().EnableDebugLog)
            {
                return;
            }
            var log = $"WebSocket Send Message: {message}";
            Logger.Value.Log(Tag, log);
        }

        public static void LogWebSocketRes(string message)
        {
            if (GppSDK.GetConfig() is null)
            {
                return;
            }
            if (!GppSDK.GetConfig().EnableDebugLog)
            {
                return;
            }
            var log = $"WebSocket Receive Message: {message}";
            Logger.Value.Log(Tag, log);
        }
        
        private static string ParseHttpCodeToString(this long httpCode)
        {
            return httpCode switch
            {
                100 => "Continue",
                101 => "Switching Protocols",
                200 => "OK",
                201 => "Created",
                202 => "Accepted",
                203 => "Non-Authoritative Information",
                204 => "No Content",
                205 => "Reset Content",
                206 => "Partial Content",
                300 => "Multiple Choices",
                301 => "Moved Permanently",
                302 => "Found",
                303 => "See Other",
                304 => "Not Modified",
                305 => "Use Proxy",
                307 => "Temporary Redirect",
                400 => "Bad Request",
                401 => "Unauthorized",
                402 => "Payment Required",
                403 => "Forbidden",
                404 => "Not Found",
                405 => "Method Not Allowed",
                406 => "Not Acceptable",
                407 => "Proxy Authentication Required",
                408 => "Request Timeout",
                409 => "Conflict",
                410 => "Gone",
                411 => "Length Required",
                412 => "Precondition Failed",
                413 => "Request Entity Too Large",
                414 => "Request-URI Too Long",
                415 => "Unsupported Media Type",
                416 => "Requested Range Not Satisfiable",
                417 => "Expectation Failed",
                500 => "Internal Server Error",
                501 => "Not Implemented",
                502 => "Bad Gateway",
                503 => "Service Unavailable",
                504 => "Gateway Timeout",
                505 => "HTTP Version Not Supported",
                _ => "Unknown HTTP Code"
            };
        }
    }
}