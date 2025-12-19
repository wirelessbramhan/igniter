using System;
using System.Runtime.Serialization;

namespace Gpp.Config.Extensions
{
    [DataContract]
    public class FirebasePushConfig : IEquatable<FirebasePushConfig>
    {
        [DataMember(Name = "project_id")]
        public string ProjectId { get; set; } = "";

        [DataMember(Name = "sender_id")]
        public string SenderId { get; set; } = "";

        [DataMember(Name = "api_key")]
        public string ApiKey { get; set; } = "";

        [DataMember(Name = "android_app_id")]
        public string AndroidAppId { get; set; } = "";

        [DataMember(Name = "ios_app_id")]
        public string IOSAppId { get; set; } = "";

        public bool Equals(FirebasePushConfig other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ProjectId == other.ProjectId
                   && SenderId == other.SenderId
                   && ApiKey == other.ApiKey
                   && AndroidAppId == other.AndroidAppId
                   && IOSAppId == other.IOSAppId;
        }
    }
}