using System.Collections;
using Gpp.Core;
using Gpp.Models;
using Gpp.Network;

namespace Gpp.Api.Legal
{
    internal partial class LegalApi
    {
        private const string LegalPoliciesUrl = "/public/policies/namespaces/{namespace}/countries/{country}/clients/{clientId}";

        private IEnumerator RequestGetLegalPoliciesByNamespaceCountryClientIdWithTags(LegalPolicyType legalPolicyType, string[] tags, ResultCallback<PublicPolicy[]> callback)
        {
            var httpBuilder = HttpRequestBuilder
                .CreateGet(ServiceUrl + LegalPoliciesUrl)
                .WithPathParam("namespace", Namespace)
                .WithPathParam("country", GppSDK.GetSession().cachedTokenData.Country)
                .WithQueryParam("tags", tags == null ? "" : string.Join(",", tags))
                .WithPathParam("clientId", GppSDK.GetConfig().ClientId)
                .WithQueryParam("policyType", legalPolicyType == LegalPolicyType.EMPTY ? "" : legalPolicyType.ToString())
                .Accepts(MediaType.ApplicationJson);

            if (!string.IsNullOrEmpty(Session.AccessToken))
            {
                httpBuilder.WithBearerAuth(Session.AccessToken);
            }

            var request = httpBuilder.GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
                    var result = response.TryParseJson<PublicPolicy[]>();
                    callback.Try(result);
                }
            );
        }
    }
}