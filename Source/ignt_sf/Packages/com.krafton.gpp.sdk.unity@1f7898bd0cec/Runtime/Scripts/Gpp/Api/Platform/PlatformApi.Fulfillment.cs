using System.Collections;
using Gpp.Core;
using Gpp.Models;
using Gpp.Network;

namespace Gpp.Api.Platform
{
    internal partial class PlatformApi
    {
        private const string PlatformFulfillmentRedeemUrl = "/public/namespaces/{namespace}/users/{userId}/fulfillment/code";

        private IEnumerator RequestPostRedeemCode(FulFillCodeRequest fulFillCodeRequest, ResultCallback<FulfillmentResult> callback)
        {
            var request = HttpRequestBuilder
                .CreatePost(ServiceUrl + PlatformFulfillmentRedeemUrl)
                .WithPathParam("namespace", Namespace)
                .WithPathParam("userId", Session.UserId)
                .WithBearerAuth(Session.AccessToken)
                .WithContentType(MediaType.ApplicationJson)
                .WithJsonBody(fulFillCodeRequest)
                .Accepts(MediaType.ApplicationJson)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
                    var result = response.TryParseJson<FulfillmentResult>();
                    callback.Try(result);
                }
            );
        }
    }
}