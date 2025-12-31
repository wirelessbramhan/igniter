using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Gpp.Core;
using Gpp.Extension;
using Gpp.Log;
using Gpp.Models;
using Gpp.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace Gpp.Telemetry
{
    public enum TelemetryType
    {
        GameTelemetry,
        GameNoAuthTelemetry,
        KpiTelemetry,
        KpiNoAuthTelemetry
    }
    internal class TelemetryManager : MonoBehaviour
    {
        public const int TELEMETRY_INTERVAL_DEFAULT_SECONDS = 60;
        private const int TELEMETRY_INTERVAL_MIN_SECONDS = 10;
        private const int ONE_KB = 1024;
        /// <summary>
        /// 100KB
        /// </summary>
        private const int MAX_TELEMETRY_SIZE = ONE_KB * 100;
        /// <summary>
        /// 2MB
        /// </summary>
        private const int MAX_MEMORY_SIZE = ONE_KB * ONE_KB * 2;
        
        private readonly ConcurrentQueue<Tuple<TelemetryType, TelemetryBody, ResultCallback>> _telemetryQueue = new();
        private bool _isTelemetryJobStarted;
        private string _telemetryPath;
        private int _telemetryIntervalSeconds;
        
        private void Awake()
        {   
            _telemetryPath = Path.Combine(Application.persistentDataPath, string.Concat(GppSDK.GetConfig()?.ClientId ?? "", "-ev"));
            
            if (File.Exists(_telemetryPath))
            {
                GetLoadTemFile();
            }
        }

        private void GetLoadTemFile()
        {
            ResUtil.ReadAllText(_telemetryPath, (fileString) =>
            {
                File.Delete(_telemetryPath);
                
                if (string.IsNullOrEmpty(fileString))
                    return;

                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects
                };

                try
                {
                    var loadedLogs = JsonConvert.DeserializeObject<List<SaveTelemetryBody>>(fileString, settings);

                    if (loadedLogs == null || loadedLogs.Count == 0)
                        return;
                    
                    foreach (var log in loadedLogs)
                    {
                        _telemetryQueue.Enqueue(new Tuple<TelemetryType, TelemetryBody, ResultCallback>(log.TelemetryType, log.TelemetryBody, r => { }));
                    }
                }
                catch (Exception ex)
                {
                    GppLog.Log($"Failed to read cached telemetry logs: {ex}");
                }
            });
        }

        internal void Flush()
        {
            StartCoroutine(FlushAllTelemetry());
        }
        
        internal void Send(TelemetryType telemetryType, TelemetryBody telemetryBody, ResultCallback callback, bool isImmediately = false)
        {
            if (IsTelemetrySizeOverLimit(telemetryBody))
            {
                GppLog.Log("Telemetry was ignored because its size exceeded 1MB.");
                return;
            }
            
            if (isImmediately)
            {
                ImmediatelySend(telemetryType, telemetryBody, callback);
            }
            else
            {
                ScheduleSend(telemetryType, telemetryBody, callback);
            }
        }
        
        private bool IsTelemetrySizeOverLimit(TelemetryBody body)
        {   
            return GetSize(body) > MAX_TELEMETRY_SIZE;
        }
        
        private bool IsMemorySizeOverLimit()
        {
            var memorySize = _telemetryQueue.Sum(tuple => GetSize(tuple.Item2));
            return memorySize > MAX_MEMORY_SIZE;
        }
        
        private int GetSize(TelemetryBody body)
        {
            return Encoding.UTF8.GetByteCount(body.ToJsonString());
        }
        
        private int GetTelemetryIntervalSeconds()
        {
            if (_telemetryIntervalSeconds is 0) _telemetryIntervalSeconds = GppSDK.GetConfig()?.TelemetryIntervalSeconds ?? 60;
            if (_telemetryIntervalSeconds < TELEMETRY_INTERVAL_MIN_SECONDS) _telemetryIntervalSeconds = TELEMETRY_INTERVAL_MIN_SECONDS;
            
            return _telemetryIntervalSeconds;
        }
        
        private void ScheduleSend(TelemetryType telemetryType, TelemetryBody telemetryBody, ResultCallback callback)
        {
            if (_isTelemetryJobStarted is false)
            {
                StartTelemetryScheduler();
            }

            if (IsMemorySizeOverLimit())
            {
                GppLog.Log("Telemetry was ignored because its size exceeded 2MB.");
                return;
            }
            
            _telemetryQueue.Enqueue(new Tuple<TelemetryType, TelemetryBody, ResultCallback>(telemetryType, telemetryBody, callback));
        }
        
        private void ImmediatelySend(TelemetryType telemetryType, TelemetryBody telemetryBody, ResultCallback callback)
        {
            switch (telemetryType)
            {
                case TelemetryType.GameTelemetry:
                {
                    if (telemetryBody is GameTelemetryBody body)
                    {
                        if (GppSDK.IsLoggedIn)
                        {
                            if (string.IsNullOrEmpty(body.AccountId)) body.AccountId = GppSDK.GetSession().UserId;
                            if (string.IsNullOrEmpty(body.KraftonId)) body.KraftonId = GppSDK.GetSession().cachedTokenData.KraftonId;
                        }
                        
                        GppSDK.GetTelemetryApi().SendGameTelemetry(body, result =>
                        {
                            if (result.IsError) _telemetryQueue.Enqueue(new Tuple<TelemetryType, TelemetryBody, ResultCallback>(TelemetryType.GameTelemetry, telemetryBody, callback));
                            callback?.Invoke(result);
                        });
                    }
                    
                    break;
                }
                case TelemetryType.GameNoAuthTelemetry:
                {
                    if (telemetryBody is GameTelemetryBody body)
                    {
                        GppSDK.GetTelemetryApi().SendGameNoAuthTelemetry(body, result =>
                        {
                            if (result.IsError) _telemetryQueue.Enqueue(new Tuple<TelemetryType, TelemetryBody, ResultCallback>(TelemetryType.GameNoAuthTelemetry, telemetryBody, callback));
                            callback?.Invoke(result);
                        });
                    }
                    
                    break;
                }
                case TelemetryType.KpiTelemetry:
                {
                    if (telemetryBody is KpiTelemetryBody body)
                    {
                        if (string.IsNullOrEmpty(body.NetworkOperator)) body.NetworkOperator = null;

                        if (GppSDK.IsLoggedIn)
                        {
                            if (string.IsNullOrEmpty(body.AccountId)) body.AccountId = GppSDK.GetSession().UserId;
                        }
                        
                        GppSDK.GetTelemetryApi().SendKpiTelemetry(body, result =>
                        {
                            if (result.IsError) _telemetryQueue.Enqueue(new Tuple<TelemetryType, TelemetryBody, ResultCallback>(TelemetryType.KpiTelemetry, telemetryBody, callback));
                            callback?.Invoke(result);
                        });
                    }
                    break;
                }
                case TelemetryType.KpiNoAuthTelemetry:
                {
                    if (telemetryBody is KpiTelemetryBody body)
                    {
                        if (string.IsNullOrEmpty(body.NetworkOperator)) body.NetworkOperator = null;
                        
                        GppSDK.GetTelemetryApi().SendKpiNoAuthTelemetry(body, result =>
                        {
                            if (result.IsError) _telemetryQueue.Enqueue(new Tuple<TelemetryType, TelemetryBody, ResultCallback>(TelemetryType.KpiNoAuthTelemetry, telemetryBody, callback));
                            callback?.Invoke(result);
                        });
                    }
                    
                    break;
                }
            }
        }
        
        private IEnumerator RunPeriodicTelemetry()
        {   
            while (true)
            {
                yield return new WaitForSeconds(GetTelemetryIntervalSeconds());

                if (_telemetryQueue.IsEmpty) continue;

                yield return FlushAllTelemetry();
            }
        }

        private IEnumerator FlushAllTelemetry()
        {
            var telemetryList = new List<Tuple<TelemetryType, TelemetryBody, ResultCallback>>();

            while (!_telemetryQueue.IsEmpty)
            {
                if (_telemetryQueue.TryDequeue(out var deQueueResult))
                {
                    telemetryList.Add(deQueueResult);
                }
            }
            
            var gameTelemetryList = new List<Tuple<GameTelemetryBody, ResultCallback>>();
            var gameNoAuthTelemetryList = new List<Tuple<GameTelemetryBody, ResultCallback>>();
            var kpiTelemetryList = new List<Tuple<KpiTelemetryBody, ResultCallback>>();
            var kpiNoAuthTelemetryList = new List<Tuple<KpiTelemetryBody, ResultCallback>>();
            
            telemetryList.ForEach(x =>
            {
                switch (x.Item1)
                {
                    case TelemetryType.GameTelemetry:
                        if (x.Item2 is GameTelemetryBody gameTelemetryBody)
                        {
                            gameTelemetryList.Add(new Tuple<GameTelemetryBody, ResultCallback>(gameTelemetryBody, x.Item3));
                        }
                        break;
                    case TelemetryType.GameNoAuthTelemetry:
                        if (x.Item2 is GameTelemetryBody gameNoAuthTelemetryBody)
                        {
                            gameNoAuthTelemetryList.Add(new Tuple<GameTelemetryBody, ResultCallback>(gameNoAuthTelemetryBody, x.Item3));
                        }
                        break;
                    case TelemetryType.KpiTelemetry:
                        if (x.Item2 is KpiTelemetryBody kpiTelemetryBody)
                        {
                            kpiTelemetryList.Add(new Tuple<KpiTelemetryBody, ResultCallback>(kpiTelemetryBody, x.Item3));
                        }
                        break;
                    case TelemetryType.KpiNoAuthTelemetry:
                        if (x.Item2 is KpiTelemetryBody kpiNoAuthTelemetry)
                        {
                            kpiNoAuthTelemetryList.Add(new Tuple<KpiTelemetryBody, ResultCallback>(kpiNoAuthTelemetry, x.Item3));
                        }
                        break;
                }
            });
            
            if (gameNoAuthTelemetryList.Count != 0)
            {
                yield return SendTelemetry(TelemetryType.GameNoAuthTelemetry, gameNoAuthTelemetryList.Select(x => x.Item1).ToList(), (result, sendCount) =>
                {
                    for (var i = sendCount; i < gameNoAuthTelemetryList.Count; i++)
                        _telemetryQueue.Enqueue(new Tuple<TelemetryType, TelemetryBody, ResultCallback>(TelemetryType.GameNoAuthTelemetry, gameNoAuthTelemetryList[i].Item1, gameNoAuthTelemetryList[i].Item2));

                    for (var i = 0; i < sendCount; i++)
                        gameNoAuthTelemetryList[i].Item2?.Invoke(result);
                });
            }
            
            if (kpiNoAuthTelemetryList.Count != 0)
            {
                yield return SendTelemetry(TelemetryType.KpiNoAuthTelemetry, kpiNoAuthTelemetryList.Select(x => x.Item1).ToList(), (result, sendCount) =>
                {
                    for (var i = sendCount; i < kpiNoAuthTelemetryList.Count; i++)
                        _telemetryQueue.Enqueue(new Tuple<TelemetryType, TelemetryBody, ResultCallback>(TelemetryType.KpiNoAuthTelemetry, kpiNoAuthTelemetryList[i].Item1, kpiNoAuthTelemetryList[i].Item2));

                    for (var i = 0; i < sendCount; i++)
                        kpiNoAuthTelemetryList[i].Item2?.Invoke(result);
                });
            }
            
            if (!GppSDK.IsLoggedIn) yield break;
            
            if (gameTelemetryList.Count != 0)
            {
                yield return SendTelemetry(TelemetryType.GameTelemetry, gameTelemetryList.Select(x => x.Item1).ToList(), (result, sendCount) =>
                {
                    for (var i = sendCount; i < gameTelemetryList.Count; i++)
                        _telemetryQueue.Enqueue(new Tuple<TelemetryType, TelemetryBody, ResultCallback>(TelemetryType.GameTelemetry, gameTelemetryList[i].Item1, gameTelemetryList[i].Item2));

                    for (var i = 0; i < sendCount; i++)
                        gameTelemetryList[i].Item2?.Invoke(result);
                });
            }
            
            if (kpiTelemetryList.Count != 0)
            {
                yield return SendTelemetry(TelemetryType.KpiTelemetry, kpiTelemetryList.Select(x => x.Item1).ToList(), (result, sendCount) =>
                {
                    for (var i = sendCount; i < kpiTelemetryList.Count; i++)
                        _telemetryQueue.Enqueue(new Tuple<TelemetryType, TelemetryBody, ResultCallback>(TelemetryType.KpiTelemetry, kpiTelemetryList[i].Item1, kpiTelemetryList[i].Item2));

                    for (var i = 0; i < sendCount; i++)
                        kpiTelemetryList[i].Item2?.Invoke(result);
                });
            }
        }
        
        private IEnumerator SendTelemetry<T>(TelemetryType telemetryType, List<T> telemetryBodies, Action<Result, int> callback) where T : TelemetryBody
        {
            telemetryBodies.RemoveAll(x => x is null);

            if (telemetryBodies.Count == 0)
            {
                yield break;
            }
            
            var totalSize = 0;
            var sendList = telemetryBodies
                .TakeWhile(x => (totalSize += Encoding.UTF8.GetByteCount(x.ToJsonString())) < MAX_TELEMETRY_SIZE)
                .ToList();
            
            telemetryBodies.RemoveRange(0, sendList.Count);

            yield return RequestTelemetry(telemetryType, sendList, result =>
            {
                if (result.IsError)
                    telemetryBodies.InsertRange(0, sendList);

                GppLog.Log($"{telemetryType} :: telemetryBodies count :: {telemetryBodies.Count}");
                callback(result, sendList.Count);
            });
        }

        private IEnumerator RequestTelemetry<T>(TelemetryType telemetryType, List<T> telemetryBodies, ResultCallback callback) where T : TelemetryBody
        {
            if (telemetryType == TelemetryType.GameTelemetry)
                yield return GppSDK.GetTelemetryApi().RequestPostGameLog(telemetryBodies as List<GameTelemetryBody>, callback);
            else if (telemetryType == TelemetryType.GameNoAuthTelemetry)
                yield return GppSDK.GetTelemetryApi().RequestPostGameNoAuthLog(telemetryBodies as List<GameTelemetryBody>, callback);
            else if (telemetryType == TelemetryType.KpiTelemetry)
                yield return GppSDK.GetTelemetryApi().RequestPostKpiLog(telemetryBodies as List<KpiTelemetryBody>, callback);
            else
                yield return GppSDK.GetTelemetryApi().RequestPostKpiNoAuthLog(telemetryBodies as List<KpiTelemetryBody>, callback);
        }

        internal void StartTelemetryScheduler()
        {
            if (_isTelemetryJobStarted)
            {
                return;
            }

            StartCoroutine(RunPeriodicTelemetry());
            _isTelemetryJobStarted = true;
        }
        
        private void OnApplicationQuit()
        {
            if (_telemetryQueue.Count == 0) return;

            var logsToSave = _telemetryQueue.Select(tuple => new SaveTelemetryBody { TelemetryType = tuple.Item1, TelemetryBody = tuple.Item2 }).ToList();
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
            };
            
            File.WriteAllText(_telemetryPath, JsonConvert.SerializeObject(logsToSave, settings));
            GppLog.Log($"GppSDK have stored {logsToSave.Count} telemetry data entries. They will be sent after the next login");
        }
    }
}