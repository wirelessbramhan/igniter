using System.Runtime.Serialization;

namespace Gpp.Extensions.IAP.Models
{
    [DataContract]
    public class IapPurchase
    {
        [DataMember(Name = "transactionId")]
        public string TransactionId;

        [DataMember(Name = "entitlementId")]
        public string EntitlementId;

        [DataMember(Name = "productId")]
        public string ProductId;
    }
}