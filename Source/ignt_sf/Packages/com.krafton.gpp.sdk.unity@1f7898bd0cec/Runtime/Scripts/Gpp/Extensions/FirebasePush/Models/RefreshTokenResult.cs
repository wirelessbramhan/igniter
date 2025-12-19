using System.Runtime.Serialization;

namespace Scripts.Gpp.Extensions.FirebasePush.Models
{
    [DataContract]
    public class RefreshTokenResult
    {
        [DataMember(Name = "refreshToken")]
        public string RefreshToken { get; set; } = "";
    }
}