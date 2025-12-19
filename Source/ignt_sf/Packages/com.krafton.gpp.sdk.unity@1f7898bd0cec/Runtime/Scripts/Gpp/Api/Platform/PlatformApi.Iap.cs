using System;
using System.Collections;
using System.Collections.Generic;
using Gpp.Core;
using Gpp.Models;
using Gpp.Network;

namespace Gpp.Api.Platform
{
    internal partial class PlatformApi
    {
        private const string PlatformIAPStoreProductsUrl = "/public/namespaces/{namespace}/store_products";
        private const string PlatformIAPReservationUrl = "/public/v2/iap/reservation";
        private const string PlatformIAPLimitationUrl = "/public/v1/namespaces/{namespace}/users/{userId}/iap/limitation/property";
        private const string PlatformIAPVerifyReceiptUrl = "/public/namespaces/{namespace}/users/{userId}/iap_order/{provider}";
        private const string PlatformIAPGetStoreMappers = "/public/namespaces/{namespace}/sku_mappers";
        private const string PlatformIAPStartSteamPurchase = "/public/namespaces/{namespace}/users/{userId}/iap/startPurchase/steam";
        private const string PlatformIAPVerifySteamIAPOrder = "/public/namespaces/{namespace}/users/{userId}/iap_order/steam";
        private const string PlatformIAPReservationCanceledUrl = "/public/v2/namespaces/{namespace}/users/{userId}/iap/reservation/{uid}/cancel";
        private const string PlatformIAPReservationFailedUrl = "/public/v2/namespaces/{namespace}/users/{userId}/iap/reservation/{uid}/fail";
        private const string PlatformIAPVerifyEpicIAPOrder = "/public/namespaces/{namespace}/users/{userId}/iap_order/epic";
        private IEnumerator RequestGetStoreProducts(string platform, ResultCallback<StoreProduct> callback)
        {
            var request = HttpRequestBuilder
                .CreateGet(ServiceUrl + PlatformIAPStoreProductsUrl)
                .WithPathParam("namespace", Namespace)
                .WithQueryParam("provider", platform)
                .WithBearerAuth(Session.AccessToken)
                .Accepts(MediaType.ApplicationJson)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
                    var result = response.TryParseJson<StoreProduct>();
                    callback.Try(result);
                }
            );
        }

        private IEnumerator RequestPostReservation(Reservation reservation, ResultCallback<IapUid> callback)
        {
            var request = HttpRequestBuilder
                .CreatePost(ServiceUrl + PlatformIAPReservationUrl)
                .WithBearerAuth(Session.AccessToken)
                .WithContentType(MediaType.ApplicationJson)
                .WithJsonBody(reservation)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
                    var result = response.TryParseJson<IapUid>();
                    callback.Try(result);
                }
            );
        }

        private IEnumerator RequestPostRepayReservation(string iapType, string price, string currency, string storeCountry, int checkCount, string restrictionId, string refundTransactionId, ResultCallback<IapUid> callback)
        {
            Dictionary<string, string> body = new Dictionary<string, string>
            {
                ["store"] = iapType,
                ["price"] = price,
                ["currency"] = currency,
                ["checkCount"] = checkCount.ToString(),
                ["storeCountry"] = storeCountry,
                ["category"] = "REPAYMENT",
                ["restrictionId"] = restrictionId,
                ["refundTransactionId"] = refundTransactionId
            };
            var request = HttpRequestBuilder
                .CreatePost(ServiceUrl + PlatformIAPReservationUrl)
                .WithBearerAuth(Session.AccessToken)
                .WithContentType(MediaType.ApplicationJson)
                .WithJsonBody(body)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
                    var result = response.TryParseJson<IapUid>();
                    callback.Try(result);
                }
            );
        }

        private IEnumerator RequestPatchLimitation(Dictionary<string, string> property, ResultCallback callback)
        {
            var request = HttpRequestBuilder
                .CreatePatch(ServiceUrl + PlatformIAPLimitationUrl)
                .WithPathParam("namespace", Namespace)
                .WithPathParam("userId", Session.UserId)
                .WithBearerAuth(Session.AccessToken)
                .WithContentType(MediaType.ApplicationJson)
                .WithJsonBody(property)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
                    var result = response.TryParse();
                    callback.Try(result);
                }
            );
        }

        private IEnumerator RequestVerifyStoreReceipt(StoreReceipt receipt, string platform, ResultCallback<VerifiedReceipt> callback)
        {
            var request = HttpRequestBuilder
                .CreatePost(ServiceUrl + PlatformIAPVerifyReceiptUrl)
                .WithPathParam("namespace", Namespace)
                .WithPathParam("userId", Session.UserId)
                .WithPathParam("provider", platform.ToLower())
                .WithBearerAuth(Session.AccessToken)
                .WithJsonBody(receipt)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
                    var result = response.TryParseJson<VerifiedReceipt>();
                    callback.Try(result);
                }
            );
        }

        private IEnumerator RequestVerifyStoreReceipts(List<StoreReceipt> receipts, string platform, ResultCallback<Dictionary<string, VerifiedReceipt>> callback)
        {
            Dictionary<string, VerifiedReceipt> verifiedReceipts = new Dictionary<string, VerifiedReceipt>();
            foreach (var receipt in receipts)
            {
                yield return RequestVerifyStoreReceipt(receipt, platform, result =>
                    {
                        if (!result.IsError)
                        {
                            verifiedReceipts.Add(receipt.productId, result.Value);
                        }
                        else
                        {
                            if (result.Error.Code == ErrorCode.EntitlementAlreadyExist)
                            {
                                verifiedReceipts.Add(receipt.productId, new VerifiedReceipt { transactionId = receipt.transactionId });
                            }
                        }
                    }
                );
            }

            callback.TryOk(verifiedReceipts);
        }

        private IEnumerator RequestGetStoreMappers(string platform, ResultCallback<MapperIapProductData> callback)
        {
            var request = HttpRequestBuilder
                .CreateGet(ServiceUrl + PlatformIAPGetStoreMappers)
                .WithPathParam("namespace", Namespace)
                .WithQueryParam("providers", platform.ToUpper())
                .WithBearerAuth(Session.AccessToken)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
                    var result = response.TryParseJson<MapperIapProductData>();
                    callback.Try(result);
                }
            );
        }

        private IEnumerator RequestStartSteamPurchase(string payload, string desc, string steamUserId, MapperIapProduct steamProduct, ResultCallback<VerifiedReceipt> callback)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("orderId", payload);
            dictionary.Add("itemCount", 1);
            dictionary.Add("storeUserId", steamUserId);
            dictionary.Add("lang", GppSDK.GetSession().LanguageCode);
            dictionary.Add("currency", steamProduct.currency);

            SteamIAPItem[] steamIAPItems = new SteamIAPItem[1];
            steamIAPItems[0] = new SteamIAPItem()
            {
                itemId = Convert.ToInt64(steamProduct.providers["STEAM"]),
                qty = 1,
                amount = Convert.ToInt32(steamProduct.price),
                description = desc,
                priceInfoSignature = steamProduct.priceInfoSignature
            };

            dictionary.Add("items", steamIAPItems);

            var request = HttpRequestBuilder
                .CreatePost(ServiceUrl + PlatformIAPStartSteamPurchase)
                .WithPathParam("namespace", Namespace)
                .WithPathParam("userId", Session.UserId)
                .WithBearerAuth(Session.AccessToken)
                .WithJsonBody(dictionary)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
                    var result = response.TryParseJson<VerifiedReceipt>();
                    callback.Try(result);
                }
            );
        }

        private IEnumerator RequestPostSteamIAPOrder(SteamIapOrder steamIapOrder, ResultCallback<VerifiedReceipt> callback)
        {
            var request = HttpRequestBuilder
                .CreatePost(ServiceUrl + PlatformIAPVerifySteamIAPOrder)
                .WithPathParam("namespace", Namespace)
                .WithPathParam("userId", Session.UserId)
                .WithJsonBody(steamIapOrder)
                .WithBearerAuth(Session.AccessToken)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
                    var result = response.TryParseJson<VerifiedReceipt>();
                    callback.Try(result);
                }
            );
        }

        private IEnumerator RequestSetReservationFailed(string uid, string store, string reason, ResultCallback callback)
        {
            Dictionary<string, string> dictionary = new()
            {
                ["store"] = store,
                ["reason"] = reason
            };

            var request = HttpRequestBuilder
                .CreatePost(ServiceUrl + PlatformIAPReservationFailedUrl)
                .WithPathParam("namespace", Namespace)
                .WithPathParam("userId", Session.UserId)
                .WithPathParam("uid", uid)
                .WithJsonBody(dictionary)
                .WithBearerAuth(Session.AccessToken)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
                    var result = response.TryParse();
                    callback.Try(result);
                }
            );
        }

        private IEnumerator RequestSetReservationCanceled(string uid, string store, string reason, ResultCallback callback)
        {
            Dictionary<string, string> dictionary = new()
            {
                ["store"] = store,
                ["reason"] = reason
            };

            var request = HttpRequestBuilder
                .CreatePost(ServiceUrl + PlatformIAPReservationCanceledUrl)
                .WithPathParam("namespace", Namespace)
                .WithPathParam("userId", Session.UserId)
                .WithPathParam("uid", uid)
                .WithJsonBody(dictionary)
                .WithBearerAuth(Session.AccessToken)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
                    var result = response.TryParse();
                    callback.Try(result);
                }
            );
        }

        private IEnumerator RequestPostEpicIAPOrder(EpicIapOrder epicIapOrder, ResultCallback<VerifiedReceipt> callback)
        {
            var request = HttpRequestBuilder
                .CreatePost(ServiceUrl + PlatformIAPVerifyEpicIAPOrder)
                .WithPathParam("namespace", Namespace)
                .WithPathParam("userId", Session.UserId)
                .WithJsonBody(epicIapOrder)
                .WithBearerAuth(Session.AccessToken)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
                    var result = response.TryParseJson<VerifiedReceipt>();
                    callback.Try(result);
                }
            );
        }
    }
}