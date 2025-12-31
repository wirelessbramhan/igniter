using System.Collections;
using Gpp.Core;
using Gpp.Models;
using Gpp.Network;

namespace Gpp.Api
{
    internal partial class IamApi
    {
        private const string IamExternalLinkUrl = "/ext/namespaces/{namespace}/redirect";

        private IEnumerator RequestExternalLink(string keyword, ResultCallback<ExternalLink> callback)
        {
            IHttpRequest request;
            
            if (GppSDK.GetSession().IsLoggedIn())
            {
                request = HttpRequestBuilder.CreateGet(GppSDK.GetConfig().BaseUrl + IamExternalLinkUrl)
                    .Accepts(MediaType.ApplicationJson)
                    .WithPathParam("namespace", Namespace)
                    .WithBearerAuth(Session.AccessToken)
                    .WithQueryParam("keyword", keyword)
                    .WithQueryParam("ui_locales", GppSDK.GetSession().LanguageCode)
                    .GetResult();
            }
            else
            {
                request = HttpRequestBuilder.CreateGet(GppSDK.GetConfig().BaseUrl + IamExternalLinkUrl)
                    .Accepts(MediaType.ApplicationJson)
                    .WithPathParam("namespace", Namespace)
                    .WithQueryParam("keyword", keyword)
                    .WithQueryParam("ui_locales", GppSDK.GetSession().LanguageCode)
                    .GetResult();
            }
            
            yield return HttpClient.SendRequest(request, (response, _) =>
            {
                var result = response.TryParseJson<ExternalLink>();
                callback.Try(result);
            });
        }
    }
}