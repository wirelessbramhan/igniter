using System.Collections;
using Gpp.Core;
using Gpp.Models;
using Gpp.Network;

namespace Gpp.Api.Legal
{
    internal partial class LegalApi
    {
        private const string LegalAgreementsPoliciesUrl = "/public/agreements/policies";

        private IEnumerator RequestPostBulkAcceptPolicyVersions(AcceptLegalRequest[] acceptAgreementRequests, ResultCallback<AcceptLegalResponse> callback)
        {
            var request = HttpRequestBuilder
                .CreatePost(ServiceUrl + LegalAgreementsPoliciesUrl)
                .WithBearerAuth(Session.AccessToken)
                .WithContentType(MediaType.ApplicationJson)
                .WithJsonBody(acceptAgreementRequests)
                .Accepts(MediaType.ApplicationJson)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
                    var result = response.TryParseJson<AcceptLegalResponse>();
                    callback.Try(result);
                }
            );
        }
    }
}