using System.Runtime.Serialization;

namespace Scripts.Gpp.Extensions.FirebasePush.Models
{
    [DataContract]
    public class PushTokenResult
    {
        [DataMember(Name = "pushToken")]
        public string PushToken { get; set; } = "";
    }

    [DataContract]
    public class PushGrantedResult
    {
        [DataMember(Name = "granted")]
        public string IsGranted { get; set; } = "";
    }
}