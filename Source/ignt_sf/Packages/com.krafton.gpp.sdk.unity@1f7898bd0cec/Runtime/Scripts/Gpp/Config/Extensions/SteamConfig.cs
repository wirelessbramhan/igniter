using System;
using System.Runtime.Serialization;

namespace Gpp.Config.Extensions
{
    [DataContract]
    public class SteamConfig : IEquatable<SteamConfig>
    {
        [DataMember(Name = "steam_app_id")]
        public string SteamAppId { get; set; } = "";

        public bool Equals(SteamConfig other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return SteamAppId == other.SteamAppId;
        }
    }
}