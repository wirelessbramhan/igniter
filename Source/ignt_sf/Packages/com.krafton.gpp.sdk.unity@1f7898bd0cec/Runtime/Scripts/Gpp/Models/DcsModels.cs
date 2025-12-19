using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Gpp.Models
{
    public class DcsModels
    {
        public enum AppUpdateUserAction
        {
            None,
            MandatoryCancel,
            OptionalLater
        }

        [DataContract]
        public class AppUpdateInfoResult : AppUpdateInfo
        {
            [DataMember(Name = "userActionType")]
            public AppUpdateUserAction UserActionType { get; set; } = AppUpdateUserAction.None;
        }

        [DataContract]
        public class AppUpdateInfo
        {
            [DataMember(Name = "mandatory")]
            public bool Mandatory { get; set; }

            [DataMember(Name = "updateAvailable")]
            public bool UpdateAvailable { get; set; }

            [DataMember(Name = "latestVersion")]
            public string LatestVersion { get; set; }

            [DataMember(Name = "message")]
            public string Message { get; set; }

            [DataMember(Name = "url")]
            public string Url { get; set; }

            [DataMember(Name = "showOptionalUpdate")]
            public bool ShowOptionalUpdate { get; set; }
        }

        [DataContract]
        public class SdkConfigInfo
        {
            [DataMember(Name = "value")]
            public SdkConfigInfoValues Value { get; set; }
        }

        [DataContract]
        public class SdkConfigInfoValues
        {
            [DataMember(Name = "countryCode")]
            public string CountryCode { get; set; }

            [DataMember(Name = "bAllowCopyDeviceId")]
            public bool AllowCopyDeviceId { get; set; }

            [DataMember(Name = "bUseSyncOwnership")]
            public string UseSyncOwnership { get; set; }

            [DataMember(Name = "HttpResponseSlowTime")]
            public int HttpResponseSlowTime { get; set; } = 5;

            [DataMember(Name = "isReportUserError")]
            public bool IsReportUserError { get; set; } = true;
        }
    }

    public enum DcsDataAccess
    {
        PostLogin,
        PreLogin
    }

    [DataContract]
    public class PublicDCSInfo
    {
        [DataMember(Name = "activation")]
        public string Activation { get; set; }

        [DataMember(Name = "activeCondition")]
        public ActiveCondition ActiveCondition { get; set; }

        [DataMember(Name = "createdAt")]
        public DateTime CreatedAt { get; set; }

        [DataMember(Name = "dataAccess")]
        public string DataAccess { get; set; }

        [DataMember(Name = "isActive")]
        public bool IsActive { get; set; }

        [DataMember(Name = "isArchived")]
        public bool IsArchived { get; set; }

        [DataMember(Name = "key")]
        public string Key { get; set; }

        [DataMember(Name = "namespace")]
        public string Namespace { get; set; }

        [DataMember(Name = "updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [DataMember(Name = "value")]
        public Dictionary<string, object> Value { get; set; }

        [DataMember(Name = "version")]
        public string Version { get; set; }
    }

    [DataContract]
    public class ActiveCondition
    {
        [DataMember(Name = "changedAt")]
        public string ChangedAt { get; set; }

        [DataMember(Name = "schedules")]
        public List<Schedule> Schedules { get; set; }
    }

    [DataContract]
    public class Schedule
    {
        [DataMember(Name = "start")]
        public string Start { get; set; }

        [DataMember(Name = "end")]
        public string End { get; set; }
    }
}