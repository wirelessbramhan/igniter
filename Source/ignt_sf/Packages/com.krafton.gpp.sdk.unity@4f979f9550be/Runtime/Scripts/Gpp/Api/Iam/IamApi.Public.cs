using System.Collections;
using Gpp.Constants;
using Gpp.Core;
using Gpp.Extension;
using Gpp.Extensions.GooglePlayGames;
using Gpp.Models;
using Gpp.Network;
using Gpp.Utils;
using UnityEngine;

namespace Gpp.Api
{
    internal partial class IamApi
    {
        private const string IamPublicUpdateTokenUrl = "/v3/public/ns/{namespace}/users/me/token";
        private const string IamPublicAgeVerification = "/v4/public/users/me/age-verification";
        private const string IamPublicArchiveUser = "/v4/public/users/me/archive";
        private const string IamOAuthLinkUrl = "/v3/public/namespaces/{namespace}/platforms/{platformId}/PUBGGA/link";

        private IEnumerator RequestUpdateTokenWithGameServerId(string gameServerId, ResultCallback callback)
        {
            var request = HttpRequestBuilder.CreatePost(ServiceUrl + IamPublicUpdateTokenUrl)
                .WithBearerAuth(Session.AccessToken)
                .WithContentType(MediaType.ApplicationForm)
                .WithFormParam("grant_type", "refresh_token")
                .WithFormParam("refresh_token", Session.RefreshToken)
                .WithFormParam("game_server_id", gameServerId)
                .Accepts(MediaType.ApplicationJson)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
                    if (response == null)
                    {
                        callback.TryError(ErrorCode.ServiceUnavailable);
                        return;
                    }

                    var result = response.TryParseJson<TokenData>();

                    if (!result.IsError)
                    {
                        Session.SetSession(result.Value);
                        callback.TryOk();
                    }
                    else
                    {
                        callback.TryError(result.Error);
                    }
                }
            );
        }

        private IEnumerator RequestCheckEligibility(ResultCallback<CheckEligibilityResult> callback)
        {
            var request = HttpRequestBuilder.CreateGet(ServiceUrl + IamPublicAgeVerification)
                .WithBearerAuth(Session.AccessToken)
                .Accepts(MediaType.ApplicationJson)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
                    var result = response.TryParseJson<CheckEligibilityResult>();

                    if (!result.IsError)
                    {
                        if (result.Value.ExpiresIn == 0)
                            result.Value.ExpiresIn = 600;
                    }

                    callback.Try(result);
                }
            );
        }

        private IEnumerator RequestArchive(ResultCallback callback)
        {
            var request = HttpRequestBuilder
                .CreatePatch(ServiceUrl + IamPublicArchiveUser)
                .WithBearerAuth(Session.AccessToken)
                .Accepts(MediaType.ApplicationJson)
                .SetRedirect(0)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
                    callback.Try(response.TryParse());
                }
            );
        }

        private IEnumerator RequestLinkAccount(PlatformType platformType, string token, ResultCallback callback)
        {
            var request = HttpRequestBuilder.CreatePost(ServiceUrl + IamOAuthLinkUrl)
                .WithBearerAuth()
                .WithContentType(MediaType.ApplicationForm)
                .Accepts(MediaType.ApplicationJson)
                .WithPathParam("platformId", platformType.ToString().ToLower())
                .WithPathParam("namespace", Namespace)
                .WithFormParam("redirect_uri", $"kraftonsdk://{GppSDK.GetConfig().Namespace.ToLower()}/auth")
                .WithFormParam("platform_token", token)
                .WithFormParam("verify_email", "true");

            var gameServerId = GppSDK.GetSession()?.cachedTokenData?.GameServerId;
            if (!string.IsNullOrEmpty(gameServerId))
                request = request.WithFormParam("game_server_id", gameServerId);

            yield return HttpClient.SendRequest(request.GetResult(), (response, _) =>
            {
                var result = response.TryParseJson<TokenData>();

                if (result.IsError)
                {
                    callback.TryError(result.Error);
                }
                else
                {
                    if (!Namespace.Equals(result.Value.Namespace))
                    {
                        callback.TryError(ErrorCode.BadRequest, $"Namespace mismatch. SDK: {Namespace} Token: {result.Value.Namespace}");
                        return;
                    }

                    Session.SetSession(result.Value);
                    callback.TryOk();
                }
            });
        }
    }
}