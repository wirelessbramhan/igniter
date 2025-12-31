using System.Collections;
using System.Collections.Generic;
using Gpp.Core;
using Gpp.Log;
using Gpp.Models;
using Gpp.Network;

namespace Gpp.Api.Basic
{
    internal partial class BasicApi
    {
        private const string BasicGetMaintenanceUrl = "/v2/public/namespaces/{namespace}/clients/{clientId}/maintenance";
        private const string BasicGetGameServersMaintenanceUrl = "/v2/public/namespaces/{namespace}/clients/{clientId}/maintenance/game-servers";

        private IEnumerator RequestGetMaintenance(ResultCallback<Maintenance> callback)
        {
            var request = HttpRequestBuilder
                .CreateGet(ServiceUrl + BasicGetMaintenanceUrl)
                .WithPathParam("namespace", Namespace)
                .WithPathParam("clientId", GppSDK.GetConfig().ClientId)
                .Accepts(MediaType.ApplicationJson)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
            {
                var result = response.TryParseJson<Maintenance>();
                callback.Try(result);
            });
        }

        private IEnumerator RequestGetGameServerMaintenance(string gameServerId, ResultCallback<Maintenance> callback)
        {
            var queries = new Dictionary<string, string> { { "gameServerId", gameServerId } };
            var request = HttpRequestBuilder
                .CreateGet(ServiceUrl + BasicGetMaintenanceUrl)
                .WithPathParam("namespace", Namespace)
                .WithPathParam("clientId", GppSDK.GetConfig().ClientId)
                .WithQueries(queries)
                .Accepts(MediaType.ApplicationJson)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
            {
                var result = response.TryParseJson<Maintenance>();
                callback.Try(result);
            });
        }

        private IEnumerator RequestGetGameServersMaintenance(ResultCallback<GameServerMaintenanceResult> callback)
        {
            var request = HttpRequestBuilder
                .CreateGet(ServiceUrl + BasicGetGameServersMaintenanceUrl)
                .WithPathParam("namespace", Namespace)
                .WithPathParam("clientId", GppSDK.GetConfig().ClientId)
                .Accepts(MediaType.ApplicationJson)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, error) =>
            {
                var result = response.TryParseJson<GameServerMaintenanceResult>();
                callback.Try(result);
            });
        }
    }
}