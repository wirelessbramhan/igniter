using System.Collections;
using Gpp.Core;
using Gpp.Models;
using Gpp.Network;

namespace Gpp.Api.Platform
{
    internal partial class PlatformApi
    {
        private const string PlatformEntitlementDurable = "/public/v1/namespaces/{namespace}/users/{userId}/entitlements/durable";
        private const string PlatformEntitlementConsumable = "/public/v1/namespaces/{namespace}/users/{userId}/entitlements/consumable";

        private IEnumerator RequestGetEntitlements(ResultCallback<Entitlements> callback, bool isDurable)
        {
            var request = HttpRequestBuilder
                .CreateGet($"{ServiceUrl}{(isDurable ? PlatformEntitlementDurable : PlatformEntitlementConsumable)}")
                .WithPathParam("namespace", Namespace)
                .WithPathParam("userId", Session.UserId)
                .WithQueryParam("limit", "200")
                .WithBearerAuth(Session.AccessToken)
                .Accepts(MediaType.ApplicationJson)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
                    var result = response.TryParseJson<Entitlements>();
                    callback.Try(result);
                }
            );
        }
    }
}