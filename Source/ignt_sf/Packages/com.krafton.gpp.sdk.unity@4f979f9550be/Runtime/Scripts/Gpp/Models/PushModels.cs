using System.Runtime.Serialization;

namespace Gpp.Models
{
    [DataContract]
    public class RegisterToken
    {
        [DataMember(Name = "country")]
        public string Country { get; set; }

        [DataMember(Name = "device_id")]
        public string DeviceId { get; set; }

        [DataMember(Name = "language")]
        public string Language { get; set; }

        [DataMember(Name = "platform")]
        public string Platform { get; set; }

        [DataMember(Name = "token")]
        public string Token { get; set; }
    }
    
    [DataContract]
    public class DeleteToken
    {
        [DataMember(Name = "device_id")]
        public string DeviceId { get; set; }
    }
}