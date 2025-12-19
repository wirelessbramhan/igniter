using System;
using System.Runtime.Serialization;

namespace Gpp.Models
{
    [DataContract]
    public class TokenData
    {
        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; } = "";

        [DataMember(Name = "refresh_token")]
        public string RefreshToken { get; set; } = "";

        [DataMember(Name = "refresh_expires_in")]
        public int RefreshTokenExpiresIn { get; set; }

        [DataMember(Name = "expires_in")]
        public int AccessTokenExpiresIn { get; set; }

        [DataMember(Name = "user_id")]
        public string UserId { get; set; } = "";

        [DataMember(Name = "display_name")]
        public string Nickname { get; set; } = "";

        [DataMember(Name = "namespace")]
        public string Namespace { get; set; }

        [DataMember(Name = "is_comply")]
        public bool IsComply { get; set; }

        [DataMember(Name = "is_full_kid")]
        public bool IsFullKid { get; set; }

        [DataMember(Name = "platform_id")]
        public string PlatformType { get; set; } = "";

        [DataMember(Name = "platform_user_id")]
        public string PlatformUserId { get; set; } = "";

        [DataMember(Name = "krafton_id")]
        public string KraftonId { get; set; } = "";

        [DataMember(Name = "krafton_tag")]
        public string KraftonTag { get; set; } = "";

        [DataMember]
        public bool PushEnabled { get; set; }

        [DataMember]
        public bool NightPushEnabled { get; set; }

        [DataMember(Name = "country")]
        public string Country { get; set; } = "";

        [DataMember(Name = "game_server_id")]
        public string GameServerId { get; set; } = "";

        [DataMember(Name = "server_time")]
        public string ServerTime { get; set; } = "";
    }

    [Serializable]
    public class RefreshTokenData
    {
        public string refresh_token { get; set; }
        public int expiration_date { get; set; }
    }

    [DataContract]
    public class Permission
    {
        [DataMember]
        public int action { get; set; }
        [DataMember]
        public string resource { get; set; }
        [DataMember]
        public int schedAction { get; set; }
        [DataMember]
        public string schedCron { get; set; }
        [DataMember]
        public string[] schedRange { get; set; }
    }

    [DataContract]
    public class AccountLinkedPlatform
    {
        [DataMember(Name = "namespace")]
        public string namespace_ { get; set; }
        [DataMember]
        public string platformUserId { get; set; }
    }

    [DataContract]
    public class ExternalLink
    {
        [DataMember(Name = "redirectTo")]
        public string RedirectTo { get; set; }
    }
}