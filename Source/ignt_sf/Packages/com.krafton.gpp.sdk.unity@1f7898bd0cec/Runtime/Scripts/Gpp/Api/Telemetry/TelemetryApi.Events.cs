using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gpp.Core;
using Gpp.Log;
using Gpp.Models;
using Gpp.Network;
using Gpp.Utils;
using UnityEngine;

namespace Gpp.Api.Telemetry
{
    internal partial class TelemetryApi
    {
        private const string TelemetryGameUrl = "/v2/auth/namespaces/{namespace}/game-log";
        private const string TelemetryGameNoAuthUrl = "/v2/no-auth/namespaces/{namespace}/game-log";
        private const string TelemetryKpitUrl = "/v2/auth/namespaces/{namespace}/kpi-log";
        private const string TelemetryKpiNoAuthUrl = "/v2/no-auth/namespaces/{namespace}/kpi-log";

        internal IEnumerator RequestPostGameLog(List<GameTelemetryBody> events, ResultCallback callback = null)
        {
            events.ToList().ForEach(x => x.CalculateQueueTime());
            var telemetryEvent = events.Select(x => x.ToJObject());

            var request = HttpRequestBuilder
                .CreatePost(ServiceUrl + TelemetryGameUrl)
                .WithPathParam("namespace", Namespace)
                .WithJsonBody(telemetryEvent)
                .WithBearerAuth(Session.AccessToken)
                .Accepts(MediaType.ApplicationJson)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
            {
                var result = response.TryParse();
                callback?.Try(result);
            }
            );
        }

        internal IEnumerator RequestPostGameNoAuthLog(List<GameTelemetryBody> events, ResultCallback callback = null)
        {
            events.ToList().ForEach(x => x.CalculateQueueTime());
            var telemetryEvent = events.Select(x => x.ToJObject());

            var request = HttpRequestBuilder
                .CreatePost(ServiceUrl + TelemetryGameNoAuthUrl)
                .WithPathParam("namespace", Namespace)
                .WithJsonBody(telemetryEvent)
                .Accepts(MediaType.ApplicationJson)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
            {
                var result = response.TryParse();
                callback?.Try(result);
            }
            );
        }

        internal IEnumerator RequestPostKpiLog(List<KpiTelemetryBody> events, ResultCallback callback)
        {
            events.ToList().ForEach(x => x.CalculateQueueTime());
            var telemetryEvent = events.Select(x => x.ToJObject());

            var request = HttpRequestBuilder
                .CreatePost(ServiceUrl + TelemetryKpitUrl)
                .WithPathParam("namespace", Namespace)
                .WithJsonBody(telemetryEvent)
                .WithBearerAuth(Session.AccessToken)
                .Accepts(MediaType.ApplicationJson)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
            {
                var result = response.TryParse();
                callback?.Try(result);
            }
            );
        }

        internal IEnumerator RequestPostKpiNoAuthLog(List<KpiTelemetryBody> events, ResultCallback callback)
        {
            events.ToList().ForEach(x => x.CalculateQueueTime());
            var telemetryEvent = events.Select(x => x.ToJObject());

            var request = HttpRequestBuilder
                .CreatePost(ServiceUrl + TelemetryKpiNoAuthUrl)
                .WithPathParam("namespace", Namespace)
                .WithJsonBody(telemetryEvent)
                .Accepts(MediaType.ApplicationJson)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
            {
                var result = response.TryParse();
                callback?.Try(result);
            }
            );
        }
    }
}