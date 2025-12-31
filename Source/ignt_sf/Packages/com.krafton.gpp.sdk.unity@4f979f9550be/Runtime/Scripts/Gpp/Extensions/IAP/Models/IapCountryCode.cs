using System.Runtime.Serialization;

namespace Gpp.Extensions.IAP.Models
{
    [DataContract]
    public class IapCountryCode
    {
        [DataMember(Name = "countryCode")]
        public string CountryCode { get; set; }
    }
}