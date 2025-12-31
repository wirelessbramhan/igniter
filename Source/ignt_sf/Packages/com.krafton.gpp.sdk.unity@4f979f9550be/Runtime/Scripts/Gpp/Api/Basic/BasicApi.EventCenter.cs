using System.Collections;
using System.Collections.Generic;
using Gpp.Core;
using Gpp.Extension;
using Gpp.Models;
using Gpp.Network;
using UnityEngine;

namespace Gpp.Api.Basic
{
    internal partial class BasicApi
    {
        private const string BasicGetEventCenterUrl = "/v1/namespaces/{namespace}/event-center/event-id/{eventId}/url";

        private IEnumerator RequestGetEventCenterUrl(string eventId, ResultCallback<EventCenterUrl> callback, Dictionary<string, object> additionalData)
        {
            var request = HttpRequestBuilder.CreatePost(ServiceUrl + BasicGetEventCenterUrl)
                .WithPathParam("namespace", Namespace)
                .WithPathParam("eventId", eventId)
                .WithBearerAuth(Session.AccessToken)
                .WithContentType(MediaType.ApplicationJson)
                .WithBody($"{{ \"data\": {additionalData.ToJsonString()} }}")
                .GetResult();
            
            yield return HttpClient.SendRequest(request, (response, _) =>
            {
                var result = response.TryParseJson<EventCenterUrl>();
                callback.Try(result);
            });
        }
    }
}