using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using AOT;

using Gpp.Core;
using Gpp.Extension;
using Gpp.Extensions.IAP;
using Gpp.Log;
using Gpp.Models;
using Gpp.Telemetry;

using UnityEngine;

namespace Gpp.Kps
{
    /// <summary>
    /// 결제 서비스 처리 클래스
    /// </summary>
    public static class PaymentService
    {
        // iOS 네이티브 브리지
        private delegate void DeepLinkCallback(string message);

        private static DeepLinkCallback _deepLinkHandler;
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void gppCommon_setAppSchemeListener(DeepLinkCallback callback);
#endif
        public static void InitKps()
        {
            _deepLinkHandler = OnDeepLinkReceived;

#if UNITY_IOS && !UNITY_EDITOR
            gppCommon_setAppSchemeListener(HandleDeepLinkNative);
#endif

            GppLog.Log(" KPS initialized");
        }

        [MonoPInvokeCallback(typeof(DeepLinkCallback))]
        private static void HandleDeepLinkNative(string message)
        {
            _deepLinkHandler?.Invoke(message);
        }

        private static void OnDeepLinkReceived(string message)
        {
            GppSyncContext.RunOnUnityThread(() =>
            {
                GppLog.Log($" DeepLink received: {message}");

                try
                {
                    var checkoutResult = ParseCheckoutResultFromUrl(message);
                    if (checkoutResult != null)
                    {
                        GppLog.Log($" Checkout completed: {checkoutResult.ToPrettyJsonString()}");
                        GppSDK.GetEventListener().NotifyCheckoutResult(checkoutResult);
                    }
                }
                catch (Exception ex)
                {
                    GppLog.Log($" Failed to parse checkout result: {ex.Message}");
                }
            });
        }

        private static CheckoutResult ParseCheckoutResultFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                GppLog.LogWarning(" URL is null or empty");
                return null;
            }

            // URL에서 ResultCode 결정
            CheckoutResultCode resultCode = url switch
            {
                var u when u.Contains("kps/complete") => CheckoutResultCode.Complete,
                var u when u.Contains("kps/cancel") => CheckoutResultCode.Cancel,
                var u when u.Contains("kps/close") => CheckoutResultCode.Close,
                _ => CheckoutResultCode.Unknown
            };

            if (resultCode == CheckoutResultCode.Unknown)
            {
                GppLog.LogWarning($" Unknown KPS path in URL: {url}");
            }

            var result = new CheckoutResult
            {
                ResultCode = resultCode
            };

            // complete인 경우에만 쿼리 파라미터를 파싱
            if (resultCode == CheckoutResultCode.Complete)
            {
                var queryIndex = url.IndexOf('?');
                if (queryIndex >= 0)
                {
                    var queryString = url.Substring(queryIndex + 1);
                    var queryParams = ParseQueryString(queryString);

                    result.Id = GetQueryParam(queryParams, "id");
                    result.SkuId = GetQueryParam(queryParams, "skuId");
                    result.Price = GetQueryParam(queryParams, "price");
                    result.Currency = GetQueryParam(queryParams, "currency");
                    result.Name = GetQueryParam(queryParams, "name");
                    result.ImageUrl = GetQueryParam(queryParams, "imageUrl");
                    result.Quantity = GetQueryParam(queryParams, "quantity");
                    result.Description = GetQueryParam(queryParams, "description");
                    result.PurchaseId = GetQueryParam(queryParams, "purchaseId");
                    result.CheckoutId = GetQueryParam(queryParams, "checkoutId");
                    result.PurchaseToken = GetQueryParam(queryParams, "purchaseToken");
                }
                else
                {
                    GppLog.LogWarning(" Complete result but no query parameters found");
                }
            }

            return result;
        }

        /// <summary>
        /// 쿼리 스트링을 파싱하여 딕셔너리로 변환
        /// </summary>
        private static Dictionary<string, string> ParseQueryString(string queryString)
        {
            var result = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(queryString))
                return result;

            var pairs = queryString.Split('&');
            foreach (var pair in pairs)
            {
                var keyValue = pair.Split(new[] { '=' }, 2);
                if (keyValue.Length == 2)
                {
                    var key = Uri.UnescapeDataString(keyValue[0]);
                    var value = Uri.UnescapeDataString(keyValue[1]);
                    result[key] = value;
                }
            }

            return result;
        }

        /// <summary>
        /// 쿼리 파라미터에서 값 가져오기
        /// </summary>
        private static string GetQueryParam(Dictionary<string, string> queryParams, string key)
        {
            return queryParams.TryGetValue(key, out var value) ? value : null;
        }

        public static void PaymentHistory(ResultCallback callback)
        {
            if (callback == null)
            {
                GppLog.LogWarning(" GetPaymentHistory called with null callback");
                return;
            }

            GppSDK.GetPaymentApi().PaymentHistory(result =>
            {
                if (result.IsError)
                {
                    GppLog.LogWarning($"Payment history failed: {result.Error?.Message}");
                    callback.TryError(result.Error);
                    return;
                }

                var url = result.Value?.Url;
                if (string.IsNullOrEmpty(url))
                {
                    callback.TryError(new Error(ErrorCode.BadRequest, "Payment history URL is missing"));
                    return;
                }

                Application.OpenURL(url);
                callback.TryOk();
            });
        }

        public static void Checkout(string offerId, ResultCallback<CheckoutResponse> callback)
        {
            if (callback == null)
            {
                GppLog.LogWarning("Checkout called with null callback");
                return;
            }

            if (string.IsNullOrEmpty(offerId))
            {
                callback.TryError(new Error(ErrorCode.OfferIdEmpty, "Offer ID is required"));
                return;
            }

            PublicOffer offer = GppSDK.GetCatalogApi().GetCachedOfferById(offerId);
            if (offer == null)
            {
                callback.TryError(new Error(ErrorCode.OfferIdMismatch, "Offer ID is mismatch"));
                return;
            }

            string checkoutId = GppTokenProvider.CreateGuestUserId();
            GppTelemetry.SendKpsPaymentChannelSelect(checkoutId, offer);

            if (MobileIapExt.CanUse())
            {
                MobileIapExt.Impl().GetStoreCountryCode(result =>
                {
                    if (result.IsError)
                    {
                        GppLog.LogWarning($"GetStoreCountryCode failed: {result.Error.Code}");
                    }

                    string platform = "ios";
#if UNITY_ANDROID
                    platform = "android";
#endif
                    Dictionary<String, String> metadata = new Dictionary<string, string>
                    {
                        { "checkout_id", checkoutId }
                    };
                    var checkoutRequest = new CheckoutRequest
                    {
                        OfferId = offerId,
                        Price = offer.Price.FinalAmount,
                        Currency = offer.Price.Currency,
                        StoreCountry = result.Value?.CountryCode ?? "",
                        StoreLanguage = "", // StoreLanguage은 받아올 수 없기 때문에 항상 빈값
                        Platform = platform,
                        Metadata = metadata,
                        CustomScheme = $"kraftonsdk{GppSDK.GetConfig().Namespace}"
                    };

                    GppLog.Log($" Processing checkout for : {checkoutRequest.ToPrettyJsonString()}");
                    GppSDK.GetPaymentApi().Checkout(checkoutRequest, result =>
                    {
                        if (result.IsError)
                        {
                            GppLog.Log($" Checkout failed: {result.Error?.Message}");
                            callback.TryError(result.Error);
                            return;
                        }

                        var redirectUrl = result.Value?.RedirectUrl;
                        if (string.IsNullOrEmpty(redirectUrl))
                        {
                            callback.TryError(new Error(ErrorCode.BadRequest, "Checkout redirect URL is missing"));
                            return;
                        }

                        Application.OpenURL(redirectUrl);
                        callback.TryOk(result.Value);
                    });
                });
            }
            else
            {
                callback.TryError(ErrorCode.NotSupportPlatform);
            }
        }
    }
}