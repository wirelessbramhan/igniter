using System;
using System.Runtime.Serialization;

namespace Gpp.Config.Extensions
{
    [DataContract]
    public class GooglePlayGamesConfig : IEquatable<GooglePlayGamesConfig>
    {
        [DataMember(Name = "web_client_id")]
        public string WebClientId { get; set; } = "";

        public bool Equals(GooglePlayGamesConfig other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return WebClientId == other.WebClientId;
        }
    }
}