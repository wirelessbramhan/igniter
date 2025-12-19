using System.Collections;
using Gpp.Core;
using Gpp.Models;
using Gpp.Network;

namespace Gpp.Api.Basic
{
    internal partial class BasicApi
    {
        private const string BasicDeletionInfo = "/v1/public/namespaces/{namespace}/features/accounts/deletion";

        private IEnumerator RequestGetAccountDeletionConfig(ResultCallback<AccountDeletionConfig> callback)
        {
            var request = HttpRequestBuilder
                .CreateGet(ServiceUrl + BasicDeletionInfo)
                .WithPathParam("namespace", Namespace)
                .WithBearerAuth(Session.AccessToken)
                .Accepts(MediaType.ApplicationJson)
                .GetResult();
            
            yield return HttpClient.SendRequest(request, (response, _) =>
            {
                var result = response.TryParseJson<AccountDeletionConfig>();
                callback.Try(result);
            });
        }
    }
}