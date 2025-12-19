using System.Collections;
using System.Collections.Generic;
using Gpp.Core;
using Gpp.Models;
using Gpp.Network;

using Result = Gpp.Core.Result;

namespace Gpp.Api.Platform
{
    internal partial class PlatformApi
    {
        private const string PlatformOwnerShipSyncUrl = "/public/namespaces/{namespace}/users/{userId}/steam/ownership/sync";
        private const string PlatformAppleOwnerShipSyncUrl = "/public/namespaces/{namespace}/users/{userId}/store/apple/ownership/sync";
        private const string PlatformEpicOwnerShipSyncUrl = "/public/namespaces/{namespace}/users/{userId}/epic/ownership/sync";

        private IEnumerator RequestOwnerShipSync(string appId, string steamId, ResultCallback<long> callback)
        {
            Dictionary<string, string> body = new Dictionary<string, string>()
            {
                ["appId"] = appId,
                ["steamId"] = steamId
            };

            var request = HttpRequestBuilder
                .CreatePut(ServiceUrl + PlatformOwnerShipSyncUrl)
                .WithPathParam("namespace", Namespace)
                .WithPathParam("userId", Session.UserId)
                .WithBearerAuth(Session.AccessToken)
                .Accepts(MediaType.ApplicationJson)
                .WithJsonBody(body)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
                    Result result = response.TryParse();

                    if (result.IsError)
                    {
                        callback.Try<long>(Result<long>.CreateError(result.Error));
                    }
                    else
                    {
                        callback.Try<long>(Result<long>.CreateOk(response.Code));
                    }
                }
            );
        }

        private IEnumerator RequestAppleOwnerShipSync(string bundleId, string appleUserId, string jwsRepresentation, ResultCallback<long> callback)
        {
            var body = new Dictionary<string, string>()
            {
                ["bundleId"] = bundleId,
                ["appleUserId"] = appleUserId,
                ["jwsRepresentation"] = jwsRepresentation
            };

            var request = HttpRequestBuilder
                .CreatePost(ServiceUrl + PlatformAppleOwnerShipSyncUrl)
                .WithPathParam("namespace", Namespace)
                .WithPathParam("userId", Session.UserId)
                .WithBearerAuth(Session.AccessToken)
                .Accepts(MediaType.ApplicationJson)
                .WithJsonBody(body)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
                    var result = response.TryParse();
                    callback.Try<long>(result.IsError
                        ? Result<long>.CreateError(result.Error)
                        : Result<long>.CreateOk(response.Code));

                }
            );
        }

        private IEnumerator RequestOwnerShipSync(string epicUserId, ResultCallback<EpicRefreshEntitlement> callback)
        {
            Dictionary<string, string> body = new Dictionary<string, string>()
            {
                ["epicUserId"] = epicUserId,
            };

            var request = HttpRequestBuilder
                .CreatePut(ServiceUrl + PlatformEpicOwnerShipSyncUrl)
                .WithPathParam("namespace", Namespace)
                .WithPathParam("userId", Session.UserId)
                .WithBearerAuth(Session.AccessToken)
                .Accepts(MediaType.ApplicationJson)
                .WithJsonBody(body)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
                    var result = response.TryParseJson<EpicRefreshEntitlement>();
                    callback.Try(result);
                }
            );
        }
    }
}

