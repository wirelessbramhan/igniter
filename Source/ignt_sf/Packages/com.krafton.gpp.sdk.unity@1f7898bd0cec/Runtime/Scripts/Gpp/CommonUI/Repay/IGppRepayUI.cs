using System;
using System.Collections;
using System.Collections.Generic;
using Gpp.Extensions.IAP.Models;
using Gpp.Models;
using UnityEngine;

namespace Gpp.CommonUI.GppRepay
{
    public interface IGppRepay
    {
        void SetRestrictionProducts(RefundRestriction[] refundRestrictions, List<Extensions.IAP.Models.IapProduct> iapProducts, bool isGetProductsError);
        void SetSuccessRepayCallback(Action onSuccess);
        void SetFailRepayCallback(Action<GameObject> onFail);
    }
}
