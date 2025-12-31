using System.Collections;
using System.Collections.Generic;
using Gpp.Constants;
using Gpp.Core;
using Gpp.Extensions.IAP.Models;
using Gpp.Models;
using UnityEngine;

namespace Gpp.IAP
{
    internal interface IGppPCPurchase
    {
        public StoreType GetStoreType();
        public void GetProducts(ResultCallback<List<IapProduct>> callback);
        public void Purchase(string itemId, ResultCallback<IapPurchase> callback);
    }
}

