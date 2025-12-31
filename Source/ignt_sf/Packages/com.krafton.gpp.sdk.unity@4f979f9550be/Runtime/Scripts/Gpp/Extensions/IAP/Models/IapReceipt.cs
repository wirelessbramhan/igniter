using System.Runtime.Serialization;

namespace Gpp.Extensions.IAP.Models
{
    [DataContract]
    public class IapReceipt
    {
        [DataMember(Name = "orderId", EmitDefaultValue = false)]
        public string OrderId { get; set; }

        [DataMember(Name = "purchaseToken", EmitDefaultValue = false)]
        public string PurchaseToken { get; set; }

        [DataMember(Name = "packageName", EmitDefaultValue = false)]
        public string PackageName { get; set; }

        [DataMember(Name = "productId", EmitDefaultValue = false)]
        public string ProductId { get; set; }

        [DataMember(Name = "purchaseTime", EmitDefaultValue = false)]
        public string PurchaseTime { get; set; }

        [DataMember(Name = "paymentId", EmitDefaultValue = false)]
        public string PaymentId { get; set; }

        [DataMember(Name = "purchaseId", EmitDefaultValue = false)]
        public string PurchaseId { get; set; }

        [DataMember(Name = "payload", EmitDefaultValue = false)]
        public string Payload { get; set; }
    }

    [DataContract]
    public class PcIapReceipt
    {

    }
}