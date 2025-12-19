using System;
using System.Collections.Generic;
using Gpp.Extensions.IAP.Models;
using Gpp.Models;

namespace Gpp.Core
{
    public interface IGppEventListener
    {
        public void NotifyBrokenToken()
        {
        }

        public void NotifyDuplicatedSession()
        {
        }

        public void NotifyDeletedAccount()
        {
        }

        public void NotifyRefreshSurvey(List<Survey> surveyList)
        {
        }

        public void NotifyMergedAccount(string oldUserId, string newUserId)
        {
        }

        public void NotifyExternalPurchase(List<IapPurchase> purchaseList)
        {
        }

        public void NotifyRestoredPurchase(List<IapPurchase> purchaseList)
        {
        }

        public void NotifyOwnerShipUpdated(Entitlements entitlements)
        {
        }

        public void NotifyCheckoutResult(CheckoutResult checkoutResult)
        {            
        }
    }
}