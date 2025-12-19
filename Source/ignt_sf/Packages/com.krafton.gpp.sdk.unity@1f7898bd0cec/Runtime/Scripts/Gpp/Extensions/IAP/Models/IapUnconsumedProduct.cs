using Gpp.Models;

namespace Gpp.Extensions.IAP.Models
{
    public class IapUnconsumedProduct
    {
        public IapProduct ProductInfo { get; set; }
        public StoreReceipt ReceiptInfo { get; set; }
    }
}