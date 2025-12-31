using System.Runtime.Serialization;
using Gpp.Core;
using Gpp.Utils;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Gpp.Log;
using Gpp.Telemetry;
using Newtonsoft.Json;
using UnityEngine.Analytics;

namespace Gpp.Models
{
    [DataContract]
    public abstract class TelemetryBody
    {
        [DataMember(Name = "created_time")]
        public long CreatedTime = GppUtil.UnixTimeMillisecondsNow();

        [DataMember(Name = "env")]
        public string Env { get; set; }

        [DataMember(Name = "eventnamespace")]
        public string Namespace { get; set; }

        [DataMember(Name = "log_type")]
        public string LogType { get; set; }

        [DataMember(Name = "event_at")]
        public string EventAt { get; set; }

        [DataMember(Name = "client_sdk_version")]
        public string SdkVersion = GppSDK.SdkVersion;

        [DataMember(Name = "payload")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object Payload { get; set; }

        [DataMember(Name = "queue_time")]
        public string QueueTime { get; set; }

        [DataMember(Name = "telemetry_location")]
        public string TelemetryLocation = "client";

        [DataMember(Name = "additional_fields")]
        public Dictionary<string, object> additionalFields = new Dictionary<string, object>();


        public JObject ToJObject()
        {
            JObject jObj = JObject.FromObject(this);
            jObj.Remove("created_time");
            jObj.Remove("additional_fields");
            if (additionalFields != null && additionalFields.Count != 0)
            {
                foreach (var pair in additionalFields)
                {
                    JToken valueToken = (pair.Value == null) ? new JValue("") : JToken.FromObject(pair.Value);
                    jObj.Add(pair.Key, valueToken);
                }
            }
            return jObj;
        }

        public void SetAdditionalFields(Dictionary<string, object> keyValuePairs)
        {
            additionalFields = keyValuePairs;
        }

        public void CalculateQueueTime()
        {
            QueueTime = (GppUtil.UnixTimeMillisecondsNow() - CreatedTime).ToString();
        }
    }

    [DataContract]
    public class GameTelemetryBody : TelemetryBody
    {
        [DataMember(Name = "krafton_id")] // (없으면 서버에서 채움. mandatory X)
        public string KraftonId { get; set; }

        [DataMember(Name = "account_id")] // gpp user id (없으면 서버에서 채움. mandatory X)
        public string AccountId { get; set; }


        public static GameTelemetryBody CreateDefault()
        {
            if (GppSDK.GetConfig() is null)
            {
                GppLog.LogWarning("GppConfig is not set");
                return null;
            }
            return new GameTelemetryBody
            {
                Env = GppSDK.GetConfig()?.Stage ?? "",
                Namespace = GppSDK.GetConfig()?.Namespace ?? "",
                EventAt = GppUtil.GetCurrentTimeInISO8601(),
                KraftonId = GppSDK.GetSession().cachedTokenData?.KraftonId ?? "",
                AccountId = GppSDK.GetSession().UserId ?? "",
                QueueTime = "0",
            };
        }
    }

    [DataContract]
    public class KpiTelemetryBody : TelemetryBody
    {
        [DataMember(Name = "gpp_id")]
        public string AccountId { get; set; }

        [DataMember(Name = "analytics_id")]
        public string AnalyticsId { get; set; }
        
        [DataMember(Name = "telemetry_id")]
        public string TelemetryId { get; set; }

        [DataMember(Name = "os")]
        public string OS { get; set; }

        [DataMember(Name = "model")]
        public string DeviceModel { get; set; }

        [DataMember(Name = "store")]
        public string Store { get; set; }

        [DataMember(Name = "package_info")]
        public string InstallPackageInfo { get; set; }

        [DataMember(Name = "network_operator")]
        public string NetworkOperator { get; set; }

        [DataMember(Name = "device_id")]
        public string DeviceId { get; set; }

        [DataMember(Name = "lang_code")]
        public string LangCode { get; set; }

        [DataMember(Name = "app_version")]
        public string AppVersion { get; set; }

        [DataMember(Name = "country_code_device")]
        public string CountryCodeDevice { get; set; }

        [DataMember(Name = "lang_code_device")]
        public string LanguageCodeDevice { get; set; }

        [DataMember(Name = "os_detail")]
        public string OSDetail { get; set; }

        [DataMember(Name = "device")]
        public string Device { get; set; }
        
        [DataMember(Name = "game_server_id")]
        public string GameServerId { get; set; }
        
        internal static int telemetryIdNo;
        
        public static KpiTelemetryBody CreateDefault()
        {
            if (GppSDK.GetConfig() is null)
            {
                GppLog.LogWarning("GppConfig is not set");
                return null;
            }
            return new KpiTelemetryBody
            {
                Env = GppSDK.GetConfig()?.Stage ?? "",
                AnalyticsId = GppSDK.GetSession().AnalyticsId,
                TelemetryId = string.Concat(GppSDK.GetSession().AnalyticsId, "#", telemetryIdNo++),
                Namespace = GppSDK.GetConfig()?.Namespace ?? "",
                EventAt = GppUtil.GetCurrentTimeInISO8601(),
                DeviceId = GppDeviceProvider.GetGppDeviceId(),
                LangCode = GppSDK.GetSession()?.LanguageCode ?? "",
                AppVersion = Application.version,
                OS = PlatformUtil.IsSteamDeck() ? "SteamOS" : GppUtil.GetOperationSystem(),
#if UNITY_PS5
                DeviceModel = "PS5",
#else
                DeviceModel = SystemInfo.deviceModel,
#endif
                InstallPackageInfo = GppNativeUtil.GetInstallSource(),
                NetworkOperator = GppNativeUtil.GetCarrierName(),                
                AccountId = GppSDK.GetSession().UserId ?? "",
                Store = GppSDK.GetOptions().Store.ToString(),
                QueueTime = "0",
                Device = GppUtil.GetDevice(),
                CountryCodeDevice = GppSDK.GetSession().GetSystemCountry(),
                LanguageCodeDevice = PreciseLocale.GetLanguage(),
                OSDetail = GppUtil.GetOSDetail(),
                GameServerId = GppSDK.GetSession()?.GameServerId,
            };
        }
    }

    [DataContract] 
    public class SaveTelemetryBody
    {
        [DataMember(Name = "telemetry_type")]
        public TelemetryType TelemetryType;

        [DataMember(Name = "telemetry_body")]
        public TelemetryBody TelemetryBody;
    }
}