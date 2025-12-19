using System.Collections.Generic;
using Gpp.Core;
using Gpp.Models;

namespace Gpp.Api.Platform
{
    internal partial class PlatformApi : GppApi
    {
        protected override string GetServiceName()
        {
            return "platform";
        }

        internal void GetStoreProducts(string platform, ResultCallback<StoreProduct> callback)
        {
            Run(RequestGetStoreProducts(platform, callback));
        }

        internal void CreateReservation(Reservation reservation, ResultCallback<IapUid> callback)
        {
            Run(RequestPostReservation(reservation, callback));
        }

        internal void CreateRepayReservation(string iapType, string price, string currency, string storeCountry, int checkCount, string restrictionId, string refundTransactionId, ResultCallback<IapUid> callback)
        {
            Run(RequestPostRepayReservation(iapType, price, currency, storeCountry, checkCount, restrictionId, refundTransactionId, callback));
        }

        internal void SetReservationFailed(string uid, string store, string reason, ResultCallback callback)
        {
            Run(RequestSetReservationFailed(uid, store, reason, callback));
        }

        internal void SetReservationCanceled(string uid, string store, string reason, ResultCallback callback)
        {
            Run(RequestSetReservationCanceled(uid, store, reason, callback));
        }

        internal void SetJapaneseAge(string ruleId, string ageLevel, ResultCallback callback)
        {
            Dictionary<string, string> property = new Dictionary<string, string>
            {
                ["ruleId"] = ruleId,
                ["value"] = ageLevel
            };
            Run(RequestPatchLimitation(property, callback));
        }

        internal void VerifyStoreReceipt(StoreReceipt receipt, string platform, ResultCallback<VerifiedReceipt> callback)
        {
            Run(RequestVerifyStoreReceipt(receipt, platform, callback));
        }

        internal void VerifyStoreReceipts(List<StoreReceipt> receipts, string platform, ResultCallback<Dictionary<string, VerifiedReceipt>> callback)
        {
            Run(RequestVerifyStoreReceipts(receipts, platform, callback));
        }

        internal void GetStoreMappers(string platform, ResultCallback<MapperIapProductData> callback)
        {
            Run(RequestGetStoreMappers(platform, callback));
        }

        internal void StartSteamPurchase(string payload, string desc, string steamUserId, MapperIapProduct steamProduct, ResultCallback<VerifiedReceipt> callback)
        {
            Run(RequestStartSteamPurchase(payload, desc, steamUserId, steamProduct, callback));
        }

        internal void VerifySteamIAPOrder(SteamIapOrder steamIapOrder, ResultCallback<VerifiedReceipt> callback)
        {
            Run(RequestPostSteamIAPOrder(steamIapOrder, callback));
        }

        internal void VerifyEpicIAPOrder(EpicIapOrder epicIapOrder, ResultCallback<VerifiedReceipt> callback)
        {
            Run(RequestPostEpicIAPOrder(epicIapOrder, callback));
        }

        internal void GetEntitlements(ResultCallback<Entitlements> callback, bool isDurable)
        {
            Run(RequestGetEntitlements(callback, isDurable));
        }

        internal void UseCouponCode(string code, string region, string language, ResultCallback<FulfillmentResult> callback)
        {
            FulFillCodeRequest fulFillCodeRequest = new FulFillCodeRequest
            {
                code = code,
                region = region,
                language = language
            };

            Run(RequestPostRedeemCode(fulFillCodeRequest, callback));
        }

        internal void GetRestrictions(ResultCallback<RefundRestriction[]> callback)
        {
            Run(RequestGetRestrictions(callback));
        }

        internal void SetRestrictionsActionResults(List<RestrictionFailedResult> results, ResultCallback callback)
        {
            Run(RequestPostRestrictionsActionResults(results, callback));
        }

        internal void OwnerShipSync(string appId, string steamId, ResultCallback<long> callback)
        {
            Run(RequestOwnerShipSync(appId, steamId, callback));
        }

        internal void AppleOwnerShipSync(string bundleId, string appleUserId, string jwsRepresentation, ResultCallback<long> callback)
        {
            Run(RequestAppleOwnerShipSync(bundleId, appleUserId, jwsRepresentation, callback));
        }

        internal void EpicOwnerShipSync(string epicUserId, ResultCallback<EpicRefreshEntitlement> callback)
        {
            Run(RequestOwnerShipSync(epicUserId, callback));
        }
    }
}