using System;
using System.Runtime.Serialization;

namespace Gpp.Config.Extensions
{
    [DataContract]
    public class BrazeConfig : IEquatable<BrazeConfig>
    {
        [DataMember(Name = "android_api_key")]
        public string AndroidApiKey { get; set; } = "";

        [DataMember(Name = "android_endpoint")]
        public string AndroidEndpoint { get; set; } = "";

        [DataMember(Name = "ios_api_key")]
        public string iOSApiKey { get; set; } = "";

        [DataMember(Name = "ios_endpoint")]
        public string iOSEndPoint { get; set; } = "";

        [DataMember(Name = "sender_id")]
        public string SenderId { get; set; } = "";

        public bool Equals(BrazeConfig other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return AndroidApiKey == other.AndroidApiKey
                   && AndroidEndpoint == other.AndroidEndpoint
                   && iOSApiKey == other.iOSApiKey
                   && iOSEndPoint == other.iOSEndPoint
                   && SenderId == other.SenderId;
        }
    }
}