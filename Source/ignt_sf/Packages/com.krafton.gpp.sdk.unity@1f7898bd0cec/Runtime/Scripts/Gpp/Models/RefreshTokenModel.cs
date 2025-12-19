using System.Runtime.Serialization;

namespace Gpp.Models
{
    [DataContract]
    public class RefreshTokenModel
    {
        [DataMember(Name = "account_id")]
        public string AccountId { get; set; }

        [DataMember(Name = "app_version")]
        public string AppVersion { get; set; }

        [DataMember(Name = "client_id")]
        public string ClientId { get; set; }

        [DataMember(Name = "device")]
        public string Device { get; set; }

        [DataMember(Name = "exp")]
        public long Exp { get; set; }

        [DataMember(Name = "iat")]
        public long Iat { get; set; }

        [DataMember(Name = "is_full_kid")]
        public bool IsFullKid { get; set; }

        [DataMember(Name = "iss")]
        public string Iss { get; set; }

        [DataMember(Name = "krafton_id")]
        public string KraftonId { get; set; }

        [DataMember(Name = "namespace")]
        public string Namespace { get; set; }

        [DataMember(Name = "os")]
        public string Os { get; set; }

        [DataMember(Name = "os_detail")]
        public string OsDetail { get; set; }

        [DataMember(Name = "platform_id")]
        public string PlatformId { get; set; }

        [DataMember(Name = "publisher")]
        public string Publisher { get; set; }

        [DataMember(Name = "store")]
        public string Store { get; set; }

        [DataMember(Name = "sub")]
        public string Sub { get; set; }
    }

}