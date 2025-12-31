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
        void SetRestrictionProductList(List<RefundRestriction> refundRestrictions, List<RefundRestriction> purchasedRefunds, List<Extensions.IAP.Models.IapProduct> iapProducts);
        void SetSuccessRepayCallback(Action<RefundRestriction> onSuccess);
        void SetFailRepayCallback(Action<GameObject> onFail);
        void DrawCommonErrorMessage(string key, Action<GameObject> onFail);
    }
}
