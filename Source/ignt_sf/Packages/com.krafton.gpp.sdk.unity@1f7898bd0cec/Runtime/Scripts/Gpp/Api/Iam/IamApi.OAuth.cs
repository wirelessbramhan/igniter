using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gpp.Constants;
using Gpp.Core;
using Gpp.Extension;
using Gpp.Extensions.GooglePlayGames;
using Gpp.Extensions.GoogleSignIn;
using Gpp.Log;
using Gpp.Models;
using Gpp.Network;
using Gpp.Utils;
using UnityEngine;
using Gpp.WebSocketSharp;

namespace Gpp.Api
{
    internal partial class IamApi
    {
        private const string IamOAuthLoginUrl = "/v3/oauth/platforms/{platformId}/PUBGGA/token";
        private const string IamOAuthLoginKidByWebUrl = "/oauth2/authorize";
        private const string IamOAuthRefreshTokenUrl = "/v3/oauth/token";
        private const string IamOAuthLogoutUrl = "/v3/logout";
        private const string IamOAuthRevokeTokenUrl = "/v3/oauth/revoke/token";
        private const string IamOAuthMeUrl = "/oauth2/ns/{namespace}/p/{platformId}/me";
        private const string IamOAuthPcConsoleUrl = "/oauth2/ns/{namespace}/p/{platformId}/authorize";
        private const string IamOAuthPcConsoleTokenUrl = "/oauth2/ns/{namespace}/token";
        private const string IamOAuthGetIdTokenUrl = "/oauth2/ns/{namespace}/p/{platformId}/authenticate";
        private const string IamOAuthPcConsoleHeadlessUrl = "/oauth2/ns/{namespace}/p/{platformId}/headless/authorize";
        private const string IamOAuthPcConsoleTokenHeadlessUrl = "/oauth2/ns/{namespace}/headless/token";
        private const string IamOauthResolve = "/oauth2/ns/{namespace}/users/resolve";

        private IEnumerator RequestAutoLogin(PlatformType type, string refreshToken, ResultCallback callback)
        {
            return RequestMobileToken(type, refreshToken, callback, true);
        }

        private IEnumerator RequestMobileToken(PlatformType platformType, string token, ResultCallback callback, bool isAutoLogin = false, bool fromWeb = false, string platformId = null)
        {
            var request = HttpRequestBuilder.CreatePost(ServiceUrl + IamOAuthLoginUrl)
                .WithBasicAuth()
                .WithContentType(MediaType.ApplicationForm)
                .Accepts(MediaType.ApplicationJson)
                .WithQueryParam("kickExisting", "true")
                .WithQueryParam("antiDuplication", "false")
                .WithFormParam("redirect_uri", $"kraftonsdk://{GppSDK.GetConfig().Namespace.ToLower()}/auth");

            if (platformType != PlatformType.Google || !GoogleSignInExt.CanUse())
            {
                request
                    .WithFormParam("namespace", Namespace)
                    .WithFormParam("device_id", GppDeviceProvider.GetGppDeviceId())
                    .WithFormParam("store", GetStore());
            }

            if (isAutoLogin)
            {
                request
                    .WithPathParam("platformId", PlatformType.Krafton.ToString().ToLower())
                    .WithFormParam("grant_type", "refresh_token")
                    .WithFormParam("refresh_token", token);
            }
            else
            {
                if ((platformType != PlatformType.Google || !GoogleSignInExt.CanUse()) && platformType != PlatformType.AppleMac && platformType != PlatformType.CustomProviderId)
                {
                    request.WithPathParam("platformId", platformType.IsGuest() && !fromWeb ? platformType.ToString().ToLower() : PlatformType.Krafton.ToString().ToLower());
                }
                else if (platformType == PlatformType.CustomProviderId)
                {
                    request.WithPathParam("platformId", platformId.IsNullOrEmpty() ? GppSDK.GetConfig().Namespace.ToLower() : platformId?.ToLower());
                }
                else
                {
                    request.WithPathParam("platformId", platformType.ToString().ToLower());
                }

                request.WithFormParam("platform_token", token);
            }

            if (GooglePlayGamesExt.CanUse() && !platformType.IsGuest())
            {
                request.WithFormParam("gpgs_code", GooglePlayGamesExt.Impl().GetGpgInfo()?.AccessToken ?? "");
            }

            yield return HttpClient.SendRequest(request.GetResult(), (response, _) =>
                {
                    if (response == null)
                    {
                        callback.TryError(ErrorCode.ServiceUnavailable);
                        return;
                    }

                    var result = response.TryParseJson<TokenData>();

                    if (!result.IsError)
                    {
                        if (!Namespace.Equals(result.Value.Namespace))
                        {
                            callback.TryError(ErrorCode.BadRequest, $"Namespace mismatch. SDK: {Namespace} Token: {result.Value.Namespace}");
                            return;
                        }

                        if (platformType.IsGuest() && !isAutoLogin)
                        {
                            GppTokenProvider.SaveGuestUserId(token);
                        }

                        Session.SetSession(result.Value);
                        callback.TryOk();
                    }
                    else
                    {
                        if (result.Error.Code == ErrorCode.RepayRequired)
                        {
                            var tokenData = (result.Error.MessageVariables as IHttpResponse).BodyBytes.ToObject<TokenData>();
                            Session.SetBearerAuthToken(tokenData);
                        }
                        else if (result.Error.Code is ErrorCode.IAMNotRegisteredEmailUser)
                        {
                            callback.TryError(ErrorCode.NotRegisteredEmailUser);
                            return;
                        }
                        
                        callback.TryError(result.Error);
                    }
                }
            );
        }

        private IEnumerator RequestAuthorizeMobile(ResultCallback<AuthorizeResult> callback, PlatformType platformType = PlatformType.Krafton, bool isLinking = false)
        {
            var clientId = GppSDK.GetConfig().ClientId;
            var state = GppUtil.GenerateNonce();
            var codeChallenge = GppUtil.GenerateCodeChallenge(GppUtil.GenerateNonce());
            var request = HttpRequestBuilder.CreateGet(ServiceUrl + IamOAuthLoginKidByWebUrl)
                .WithQueryParam("platform_id", platformType.ToString().ToLower())
                .WithQueryParam("redirect_uri", $"kraftonsdk://{GppSDK.GetConfig().Namespace.ToLower()}/auth")
                .WithQueryParam("client_id", clientId)
                .WithQueryParam("response_type", "code")
                .WithQueryParam("code_challenge", codeChallenge)
                .WithQueryParam("code_challenge_method", "S256")
                .WithQueryParam("state", state)
                .WithQueryParam("ui_locales", GppSDK.GetSession().LanguageCode.ToLower())
                .WithQueryParam("store", GetStore())
                .Accepts(MediaType.ApplicationJson)
                .SetRedirect(0)
                .SetIgnoreRedirectError(true);

            if (GooglePlayGamesExt.CanUse())
            {
                request.WithQueryParam("gpgs_code", GooglePlayGamesExt.Impl().GetGpgInfo()?.AccessToken ?? "");
            }

            if (isLinking)
            {
                request.WithBearerAuth(Session.AccessToken);
            }
            else
            {
                request.WithBasicAuth();
            }

            yield return HttpClient.SendRequest(request.GetResult(), (response, _) =>
                {
                    if (response == null)
                    {
                        callback.TryError(ErrorCode.NetworkError);
                        return;
                    }

                    var result = response.TryParseJson<AuthorizeResult>();
                    callback.Try(result);
                }
            );
        }

        internal IEnumerator RequestRefreshAccessToken(ResultCallback callback)
        {
            var request = HttpRequestBuilder.CreatePost(ServiceUrl + IamOAuthRefreshTokenUrl)
                .WithBasicAuth()
                .WithContentType(MediaType.ApplicationForm)
                .Accepts(MediaType.ApplicationJson)
                .WithFormParam("client_id", GppSDK.GetConfig().ClientId)
                .WithFormParam("grant_type", "refresh_token")
                .WithFormParam("refresh_token", Session.RefreshToken)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
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

        private IEnumerator RequestLogout(ResultCallback callback)
        {
            if (!Session.IsLoggedIn())
            {
                callback.TryOk();
                yield break;
            }

            var request = HttpRequestBuilder.CreatePost(ServiceUrl + IamOAuthLogoutUrl)
                .WithBearerAuth(Session.AccessToken)
                .WithContentType(MediaType.ApplicationJson)
                .WithBody($"{{ \"refresh_token\": \"{Session.cachedTokenData.RefreshToken}\" }}")
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
                    if (response == null)
                    {
                        callback.TryError(ErrorCode.NetworkError);
                        return;
                    }

                    var result = response.TryParse();
                    callback.Try(result);
                }
            );
        }

        private IEnumerator RequestRevokeToken(ResultCallback callback)
        {
            if (!Session.IsLoggedIn())
            {
                callback.TryOk();
                yield break;
            }

            var request = HttpRequestBuilder.CreatePost(ServiceUrl + IamOAuthRevokeTokenUrl)
                .WithBearerAuth(Session.AccessToken)
                .WithContentType(MediaType.ApplicationForm)
                .WithFormParam("token", Session.AccessToken)
                .Accepts(MediaType.ApplicationJson)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
                    var result = response.TryParse();
                    callback.Try(result);
                }
            );
        }

        private IEnumerator RequestKidInfo(PlatformType type, string platformToken, ResultCallback<KidInfoResult> callback)
        {
            var request = HttpRequestBuilder.CreatePost(ServiceUrl + IamOAuthMeUrl)
                .WithBasicAuth()
                .WithContentType(MediaType.ApplicationForm)
                .WithFormParam("code", platformToken)
                .WithPathParam("namespace", Namespace)
                .WithPathParam("platformId", type.ToString().ToLower())
                .Accepts(MediaType.ApplicationJson)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
                    var result = response.TryParseJson<KidInfoResult>();
                    callback.Try(result);
                }
            );
        }

        private IEnumerator RequestPcHeadlessAutoLogin(PlatformType platformType, string token, ResultCallback callback)
        {
            var codeChallenge = GppUtil.GenerateCodeChallenge(GppUtil.GenerateNonce());

            var request = HttpRequestBuilder.CreatePost(ServiceUrl + IamOAuthPcConsoleTokenHeadlessUrl)
                .WithBasicAuth()
                .WithContentType(MediaType.ApplicationForm)
                .WithPathParam("namespace", Namespace)
                .WithPathParam("platformId", platformType.ToString().ToLower())
                .WithFormParam("grant_type", "refresh_token")
                .WithFormParam("refresh_token", token)
                .WithFormParam("client_id", GppSDK.GetConfig().ClientId)
                .WithFormParam("code_challenge", codeChallenge)
                .WithFormParam("code_challenge_method", "S256")
                .WithFormParam("store", GetStore())
                .Accepts(MediaType.ApplicationJson)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
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

        private IEnumerator RequestPcFullKidAutoLogin(PlatformType platformType, string token, ResultCallback callback)
        {
            var codeChallenge = GppUtil.GenerateCodeChallenge(GppUtil.GenerateNonce());

            var request = HttpRequestBuilder.CreatePost(ServiceUrl + IamOAuthPcConsoleTokenUrl)
                .WithBasicAuth()
                .WithContentType(MediaType.ApplicationForm)
                .WithPathParam("namespace", Namespace)
                .WithPathParam("platformId", platformType.ToString().ToLower())
                .WithFormParam("grant_type", "refresh_token")
                .WithFormParam("refresh_token", token)
                .WithFormParam("client_id", GppSDK.GetConfig().ClientId)
                .WithFormParam("code_challenge", codeChallenge)
                .WithFormParam("code_challenge_method", "S256")
                .WithFormParam("store", GetStore())
                .Accepts(MediaType.ApplicationJson)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
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

        private IEnumerator RequestAuthorizePcConsole(PlatformType type, string platformToken, ResultCallback<PcConsoleAuthResult> callback)
        {
            var state = GppUtil.GenerateNonce();
            var codeChallenge = GppUtil.GenerateCodeChallenge(GppUtil.GenerateNonce());
            var request = HttpRequestBuilder.CreatePost(ServiceUrl + IamOAuthPcConsoleUrl)
                .WithBasicAuth()
                .WithContentType(MediaType.ApplicationForm)
                .WithPathParam("namespace", Namespace)
                .WithPathParam("platformId", type.ToString().ToLower())
                .WithFormParam("platform_token", platformToken)
                .WithFormParam("response_type", "code")
                .WithFormParam("client_id", GppSDK.GetConfig().ClientId)
                .WithFormParam("code_challenge", codeChallenge)
                .WithFormParam("code_challenge_method", "S256")
                .WithFormParam("state", state)
                .WithFormParam("store", GetStore())
                .Accepts(MediaType.ApplicationJson)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
            {
                var result = response.TryParseJson<PcConsoleAuthResult>();
                if (result.Value?.ExpiresIn == 0)
                {
                    result.Value.ExpiresIn = 600;
                }
                
                callback.Try(result);
            });
        }

        private IEnumerator RequestPcConsoleToken(string code, ResultCallback callback)
        {
            var request = HttpRequestBuilder.CreatePost(ServiceUrl + IamOAuthPcConsoleTokenUrl)
                .WithBasicAuth()
                .WithContentType(MediaType.ApplicationForm)
                .WithPathParam("namespace", Namespace)
                .WithFormParam("grant_type", "authorization_code")
                .WithFormParam("code", code)
                .WithFormParam("client_id", GppSDK.GetConfig().ClientId)
                .WithFormParam("store", GetStore())
                .Accepts(MediaType.ApplicationJson);

            var gameServerId = GppSDK.GetSession()?.cachedTokenData?.GameServerId;
            if (!string.IsNullOrEmpty(gameServerId))
                request = request.WithFormParam("game_server_id", gameServerId);

            yield return HttpClient.SendRequest(request.GetResult(), (response, _) =>
                {
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

        private IEnumerator RequestAuthorizePcConsoleHeadless(PlatformType type, string platformToken, ResultCallback<PcConsoleAuthResult> callback)
        {
            var state = GppUtil.GenerateNonce();
            var codeChallenge = GppUtil.GenerateCodeChallenge(GppUtil.GenerateNonce());
            var request = HttpRequestBuilder.CreatePost(ServiceUrl + IamOAuthPcConsoleHeadlessUrl)
                .WithBasicAuth()
                .WithContentType(MediaType.ApplicationForm)
                .WithPathParam("namespace", Namespace)
                .WithPathParam("platformId", type.ToString().ToLower())
                .WithFormParam("platform_token", platformToken)
                .WithFormParam("response_type", "code")
                .WithFormParam("client_id", GppSDK.GetConfig().ClientId)
                .WithFormParam("code_challenge", codeChallenge)
                .WithFormParam("code_challenge_method", "S256")
                .WithFormParam("state", state)
                .WithFormParam("store", GetStore())
                .Accepts(MediaType.ApplicationJson)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
            {
                var result = response.TryParseJson<PcConsoleAuthResult>();
                if (result.Value?.ExpiresIn == 0)
                {
                    result.Value.ExpiresIn = 600;
                }
                
                callback.Try(result);
            });
        }

        private IEnumerator RequestPcConsoleTokenHeadless(string code, ResultCallback callback)
        {
            var request = HttpRequestBuilder.CreatePost(ServiceUrl + IamOAuthPcConsoleTokenHeadlessUrl)
                .WithBasicAuth()
                .WithContentType(MediaType.ApplicationForm)
                .WithPathParam("namespace", Namespace)
                .WithFormParam("grant_type", "authorization_code")
                .WithFormParam("code", code)
                .WithFormParam("client_id", GppSDK.GetConfig().ClientId)
                .WithFormParam("store", GetStore())
                .Accepts(MediaType.ApplicationJson);

            var gameServerId = GppSDK.GetSession()?.cachedTokenData?.GameServerId;
            if (!string.IsNullOrEmpty(gameServerId))
                request = request.WithFormParam("game_server_id", gameServerId);

            yield return HttpClient.SendRequest(request.GetResult(), (response, _) =>
            {
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

        private IEnumerator RequestIdToken(ResultCallback<KidIdTokenResult> callback)
        {
            var request = HttpRequestBuilder.CreatePost(ServiceUrl + IamOAuthGetIdTokenUrl)
                .WithBearerAuth(Session.AccessToken)
                .WithContentType(MediaType.ApplicationForm)
                .WithPathParam("namespace", Namespace)
                .WithPathParam("platformId", PlatformType.Krafton.ToString().ToLower())
                .Accepts(MediaType.ApplicationJson)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
                    var result = response.TryParseJson<KidIdTokenResult>();
                    if (!result.IsError)
                    {
                        callback.Try(result);
                    }
                    else
                    {
                        callback.TryError(result.Error);
                    }
                }
            );
        }
        private IEnumerator RequestKidResolve(string token, string userId, bool isMain, ResultCallback callback)
        {
            Dictionary<string, object> body = new Dictionary<string, object>
            {
                { "is_main", isMain },
                { "user_id", userId }
            };

            var request = HttpRequestBuilder.CreatePost(ServiceUrl + IamOauthResolve)
                .WithBearerAuth(token)
                .WithBody(body.ToJsonString())
                .WithPathParam("namespace", Namespace)
                .WithContentType(MediaType.ApplicationJson)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
            {
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

        private static string GetStore()
        {
            return GppSDK.GetOptions().Store.ToString();
        }
    }
}