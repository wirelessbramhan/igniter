using System.Diagnostics;
using System.Runtime.Serialization;
using Gpp.Utils;
using UnityEngine;

namespace Gpp.Log
{
    [DataContract]
    internal class GppClientRemoteLog
    {
        [DataMember(Name = "gppsdk-envname")]
        internal string EnvName;

        [DataMember(Name = "gppsdk-errorcode")]
        internal string ErrorCode;

        [DataMember(Name = "gppsdk-functionname")]
        internal string FunctionName;

        [DataMember(Name = "gppsdk-gamename")]
        internal string GameName;

        [DataMember(Name = "gppsdk-hashpid")]
        internal string HashId;

        [DataMember(Name = "gppsdk-log")]
        internal string Log;

        [DataMember(Name = "gppsdk-loglevel")]
        internal string LogLevel;

        [DataMember(Name = "gppsdk-platformtype")]
        internal string PlatformType;

        [DataMember(Name = "gppsdk-processid")]
        internal string ProcessId;

        [DataMember(Name = "gppsdk-stacktrace")]
        internal string StackTrace;

        [DataMember(Name = "gppsdk-userid")]
        internal string UserId;

        [DataMember(Name = "gppsdk-ver")]
        internal string Version;
        
        //--------------------------------------------------------------------------------
        //  2025.11.14
        //  Configure the same specifications as the Unreal SDK
        //  https://krafton.atlassian.net/wiki/spaces/GPP/pages/660061258/2025-07+Datadog
        //--------------------------------------------------------------------------------
        
        [DataMember(Name = "gppsdk-analytics-id")]
        internal string AnalyticsId;
        
        [DataMember(Name = "gppsdk-app-ver")]
        internal string AppVersion;
        
        [DataMember(Name = "gppsdk-deviceid")]
        internal string DeviceId;
        
        [DataMember(Name = "gppsdk-storetype")]
        internal string StoreType;
        
        [DataMember(Name = "gppsdk-device-name")]
        internal string DeviceName;
        
        [DataMember(Name = "gppsdk-os-detail")]
        internal string OsDetail;
        
        //--------------------------------------------------------------------------------
        //  Unity does not implement it.
        //--------------------------------------------------------------------------------
        //  * gppsdk-category-group
        //  * gppsdk-before-deviceid
        //--------------------------------------------------------------------------------
        
        internal GppClientRemoteLog()
        {
            var config = GppSDK.GetConfig();
            EnvName = config?.Stage ?? string.Empty;
            GameName = config?.Namespace ?? string.Empty;
            
            var options = GppSDK.GetOptions();
            StoreType = options?.Store.ToString() ?? string.Empty;

            var session = GppSDK.GetSession();
            UserId = session?.UserId ?? string.Empty;
            AnalyticsId = session?.AnalyticsId ?? string.Empty;
            
            HashId = GppLog.HashId;
            PlatformType = GppUtil.GetOperationSystem();
            Version = GppSDK.SdkVersion;

            using var process = Process.GetCurrentProcess();
            ProcessId = process.Id.ToString();

            AppVersion = Application.version;
            DeviceId = SystemInfo.deviceUniqueIdentifier;
            DeviceName = SystemInfo.deviceName;
            OsDetail = SystemInfo.operatingSystem;
        }
    }
}