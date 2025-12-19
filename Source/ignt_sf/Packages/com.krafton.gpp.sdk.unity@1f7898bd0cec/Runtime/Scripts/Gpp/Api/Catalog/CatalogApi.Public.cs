using System.Collections;
using Gpp.Core;
using Gpp.Extension;
using Gpp.Models;
using Gpp.Network;

namespace Gpp.Api.Catalog
{
    // https://krafton.atlassian.net/wiki/spaces/GPP/pages/743944791/Catalog+Public+API+for+KPS+Purchase
    internal partial class CatalogApi
    {
        private const string Offers = "/public/v1/namespaces/{namespace}/offers";
        private const string OfferById = "/public/v1/namespaces/{namespace}/offers/{offerId}";

        private IEnumerator RequestGetOffers(CatalogOfferRequest offerRequest, ResultCallback<CatalogOfferResponse> callback)
        {
            var request = HttpRequestBuilder
                .CreateGet(ServiceUrl + Offers)
                .WithPathParam("namespace", Namespace)
                .WithQueries(offerRequest.ToDictionary())
                .WithBearerAuth(Session.AccessToken)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
                    var result = response.TryParseJson<CatalogOfferResponse>();

                    // 성공한 경우 Offers 데이터를 캐시에 저장
                    if (!result.IsError && result.Value?.Offers != null)
                    {
                        CacheOffers(result.Value.Offers);
                    }

                    callback.Try(result);
                }
            );
        }
    }
}