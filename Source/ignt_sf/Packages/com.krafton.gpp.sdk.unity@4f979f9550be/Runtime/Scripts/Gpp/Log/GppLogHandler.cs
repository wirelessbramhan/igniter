using System;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Gpp.Log
{
    internal class GppLogHandler : ILogHandler
    {
        private readonly string _logFilePath;
        private readonly object _lockObject = new();
        private const string DateFormat = "yyyy-MM-dd HH:mm:ss";

        internal GppLogHandler()
        {
            var date = DateTime.Now.ToString("yyMMdd");
            var fileName = $"sdk_log_{date}.log";
            _logFilePath = Path.Combine(Application.persistentDataPath, fileName);
        }

        private void Write(string message)
        {
            if (string.IsNullOrEmpty(_logFilePath))
            {
                return;
            }

            try
            {
                lock (_lockObject)
                {
                    using var writer = new StreamWriter(_logFilePath, true);
                    writer.WriteLine(message);
                }
            }
            catch (Exception)
            {
                
            }
        }

        public void LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            Debug.unityLogger.logHandler.LogFormat(logType, context, format, args);
            Write($"{Now()} {logType} {string.Format(format, args)}");
        }

        public void LogException(Exception exception, Object context)
        {
            Debug.unityLogger.LogException(exception, context);
            Write($"{Now()} {exception.Message}");
        }

        private static string Now()
        {
            return DateTime.Now.ToString(DateFormat);
        }
    }
}