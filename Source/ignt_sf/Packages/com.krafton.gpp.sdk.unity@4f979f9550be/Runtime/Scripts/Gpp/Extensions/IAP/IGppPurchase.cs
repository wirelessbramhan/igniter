using System.Collections.Generic;
using Gpp.Constants;
using Gpp.Core;
using Gpp.Extensions.IAP.Models;
using Gpp.Models;

namespace Gpp.IAP
{
    internal interface IGppPurchase
    {
        public StoreType GetStoreType();
        public void Init(ResultCallback callback);
        public void GetProducts(List<string> productIds, ResultCallback<List<Extensions.IAP.Models.IapProduct>> callback);
        public void GetUnconsumedReceipts(ResultCallback<List<IapReceipt>> callback);
        public void Purchase(string itemId, string payload, ResultCallback<IapReceipt> callback);
        public void Consume(string itemId, ResultCallback callback);
        public void GetStoreCountryCode(ResultCallback<IapCountryCode> callback);
        public void ConsumeAll(ResultCallback callback);
    }
}