using Gpp.Core;
using Gpp.Extensions.IAP;
using Gpp.Models;

namespace Gpp.Kps
{
    internal class CatalogService
    {
        public static void GetOffers(ResultCallback<CatalogOfferResponse> callback)
        {
            if (MobileIapExt.CanUse())
            {
                MobileIapExt.Impl().GetStoreCountryCode(result =>
                {
                    if (result.IsError)
                    {
                        callback.TryError(result.Error);
                    }
                    else
                    {
                        var req = new CatalogOfferRequest
                        {
                            AccountCreationCountry = GppSDK.GetSession().CountryCode,
                            AccountPreferredLanguage = "", // Token에 언어 정보는 항상 없음
                            DeviceCountry = GppSDK.GetSession().GetSystemCountry(),
                            DeviceLanguage = PreciseLocale.GetLanguage(),
                            StorefrontCountry = result.Value.CountryCode,
                            StorefrontLanguage = "", // Store의 Language는 항상 없음
                        };

                        GppSDK.GetCatalogApi().GetOffers(req, callback);
                    }
                });
            }
            else
            {
                callback.TryError(ErrorCode.NotSupportPlatform);
            }
        }
    }
}