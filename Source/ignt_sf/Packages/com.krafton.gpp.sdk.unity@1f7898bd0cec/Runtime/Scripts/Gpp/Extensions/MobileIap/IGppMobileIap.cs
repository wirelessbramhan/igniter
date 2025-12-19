using System.Collections.Generic;
using Gpp.Constants;
using Gpp.Core;
using Gpp.Extensions.IAP.Models;
using Gpp.Models;

namespace Gpp.Extensions.IAP
{
    public interface IGppIap
    {
        public void Init(ResultCallback callback, StoreType store = StoreType.None);
        public void GetProducts(ResultCallback<List<Models.IapProduct>> callback);
        public void GetUnconsumedProducts(ResultCallback<List<IapUnconsumedProduct>> callback);
        public void Purchase(string productId, ResultCallback<IapPurchase> callback, IapTestMode mode = IapTestMode.Normal);
        public void RepayPurchase(string productId, string restrictionId, string refundTransactionId, ResultCallback<IapPurchase> callback, IapTestMode mode = IapTestMode.Normal);
        public void Restore(ResultCallback<List<IapPurchase>> callback);
        public void Restore(List<StoreReceipt> receiptList, ResultCallback<List<IapPurchase>> callback);
        public void GetProducts(List<string> productIds, ResultCallback<List<Models.IapProduct>> callback);
        public void ConsumeAll(ResultCallback callback);
        public void GetStoreCountryCode(ResultCallback<IapCountryCode> callback);
    }
}