using System.Collections;
using Gpp.Core;
using Gpp.Extension;
using Gpp.Models;
using Gpp.Network;

namespace Gpp.Api.Payment
{
    internal partial class PaymentApi
    {
        private const string CheckoutUrl = "/v1/public/namespaces/{namespace}/checkouts";
        private const string PaymentHistoryUrl = "/v1/public/namespaces/{namespace}/payment-history-url";

        private IEnumerator RequestCheckout(CheckoutRequest checkoutRequest, ResultCallback<CheckoutResponse> callback)
        {
            var request = HttpRequestBuilder
                .CreatePost(ServiceUrl + CheckoutUrl)
                .WithPathParam("namespace", Namespace)
                .WithJsonBody(checkoutRequest)
                .WithBearerAuth(Session.AccessToken)
                .SetDisableRetry()
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
            {
                var result = response.TryParseJson<CheckoutResponse>();

                if (result.IsError)
                {
                    if (result.Error.Code is ErrorCode.PaymentNotSupportCountry)
                    {
                        callback.TryError(ErrorCode.NotSupportCountry, "Not supported country.");
                        return;
                    }
                }

                callback.Try(result);
            });
        }

        private IEnumerator RequestPaymentHistory(ResultCallback<PaymentHistoryResponse> callback)
        {
            var request = HttpRequestBuilder
                .CreateGet(ServiceUrl + PaymentHistoryUrl)
                .WithPathParam("namespace", Namespace)
                .WithQueryParam("customScheme", $"kraftonsdk{GppSDK.GetConfig().Namespace}")
                .WithBearerAuth(Session.AccessToken)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
                    var result = response.TryParseJson<PaymentHistoryResponse>();
                    callback.Try(result);
                }
            );
        }
    }
}