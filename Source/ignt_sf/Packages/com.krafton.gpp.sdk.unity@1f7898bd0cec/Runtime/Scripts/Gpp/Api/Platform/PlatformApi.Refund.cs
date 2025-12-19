using System.Collections;
using System.Collections.Generic;
using Gpp.Core;
using Gpp.Extension;
using Gpp.Models;
using Gpp.Network;
using UnityEngine;

namespace Gpp.Api.Platform
{
    internal partial class PlatformApi
    {
        private const string PlatformRefundGetRestrictionsUrl = "/public/namespaces/{namespace}/users/{userId}/restrictions";
        private const string PlatformRefundGetRestrictionsActionResultsUrl = "/public/namespaces/{namespace}/users/{userId}/restrictions/actionResults";

        private IEnumerator RequestGetRestrictions(ResultCallback<RefundRestriction[]> callback)
        {
            var request = HttpRequestBuilder
                .CreateGet(ServiceUrl + PlatformRefundGetRestrictionsUrl)
                .WithPathParam("namespace", Namespace)
                .WithPathParam("userId", Session.UserId)
                .WithQueryParam("type", "REPAYMENT")
                .WithBearerAuth(Session.AccessToken)
                .Accepts(MediaType.ApplicationJson)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
                    var result = response.TryParseJson<RefundRestriction[]>();
                    callback.Try(result);
                }
            );
        }

        private IEnumerator RequestPostRestrictionsActionResults(List<RestrictionFailedResult> results, ResultCallback callback)
        {
            Dictionary<string, object> resultDictionary = new Dictionary<string, object>()
            {
                ["results"] = results.ToArray()
            };

            var request = HttpRequestBuilder
                .CreatePost(ServiceUrl + PlatformRefundGetRestrictionsActionResultsUrl)
                .WithPathParam("namespace", Namespace)
                .WithPathParam("userId", Session.UserId)
                .WithBearerAuth(Session.AccessToken)
                .WithJsonBody(resultDictionary)
                .Accepts(MediaType.ApplicationJson)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
                    var result = response.TryParse();
                    callback.Try(result);
                }
            );
        }
    }
}

