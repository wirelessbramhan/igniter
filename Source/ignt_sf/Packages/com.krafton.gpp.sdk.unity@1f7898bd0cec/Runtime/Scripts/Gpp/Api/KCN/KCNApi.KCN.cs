using System.Collections;
using System.Collections.Generic;
using Gpp.Core;
using Gpp.Extension;
using Gpp.Models;
using Gpp.Network;
using UnityEngine;

namespace Gpp.Api
{
    internal partial class KCNApi
    {
        private const string IamKCNCodeUrl = "/v1/ns/{namespace}/users/me/code";

        private IEnumerator RequestGetLastCreatorCode(ResultCallback<KCNResult> callback)
        {
            var request = HttpRequestBuilder.CreateGet(ServiceUrl + IamKCNCodeUrl)
                .WithPathParam("namespace", Namespace)
                .WithBearerAuth();

            yield return HttpClient.SendRequest(request.GetResult(), (response, _) =>
            {
                var result = response.TryParseJson<KCNResult>();
                callback.Invoke(result);
            });
        }

        private IEnumerator RequestSetCreatorCode(string code, string[] associatedPurchases, ResultCallback<KCNResult> callback)
        {
            Dictionary<string, object> body = new Dictionary<string, object>
            {
                { "associated_purchases", associatedPurchases },
                { "code", code }                
            };

            var request = HttpRequestBuilder.CreatePost(ServiceUrl + IamKCNCodeUrl)
                .WithPathParam("namespace", Namespace)
                .WithContentType(MediaType.ApplicationJson)
                .WithBody(body.ToJsonString())
                .WithBearerAuth();

            yield return HttpClient.SendRequest(request.GetResult(), (response, _) =>
            {
                var result = response.TryParseJson<KCNResult>();
                callback.Invoke(result);
            });
        }

        private IEnumerator RequestDeleteCreatorCode(ResultCallback callback)
        {
            var request = HttpRequestBuilder.CreateDelete(ServiceUrl + IamKCNCodeUrl)
                            .WithPathParam("namespace", Namespace)
                            .WithBearerAuth();

            yield return HttpClient.SendRequest(request.GetResult(), (response, _) =>
            {
                var result = response.TryParse();
                callback.Invoke(result);
            });
        }
    }
}

