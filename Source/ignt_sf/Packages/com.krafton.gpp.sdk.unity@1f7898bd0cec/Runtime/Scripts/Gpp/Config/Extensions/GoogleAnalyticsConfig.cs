using System;
using System.Runtime.Serialization;

namespace Gpp.Config.Extensions
{
    [DataContract]
    public class GoogleAnalyticsConfig : IEquatable<GoogleAnalyticsConfig>
    {
        [DataMember(Name = "project_id")]
        public string ProjectId { get; set; } = "";

        [DataMember(Name = "web_client_id")]
        public string WebClientId { get; set; } = "";

        [DataMember(Name = "api_key")]
        public string ApiKey { get; set; } = "";

        [DataMember(Name = "android_app_id")]
        public string AndroidAppId { get; set; } = "";

        [DataMember(Name = "ios_app_id")]
        public string IOSAppId { get; set; } = "";

        public bool Equals(GoogleAnalyticsConfig other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ProjectId == other.ProjectId
                   && WebClientId == other.WebClientId
                   && ApiKey == other.ApiKey
                   && AndroidAppId == other.AndroidAppId
                   && IOSAppId == other.IOSAppId;
        }
    }
}