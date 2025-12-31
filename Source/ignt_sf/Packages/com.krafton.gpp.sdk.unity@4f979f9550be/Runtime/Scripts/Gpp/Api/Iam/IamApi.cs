using Gpp.Constants;
using Gpp.Core;
using Gpp.Models;
using Gpp.Utils;

namespace Gpp.Api
{
    internal partial class IamApi : GppApi
    {
        protected override string GetServiceName()
        {
            return "iam";
        }

        internal void AutoLogin(PlatformType type, string refreshToken, ResultCallback callback)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                callback.TryError(ErrorCode.SdkRefreshTokenNotFound);
                return;
            }

            Run(RequestAutoLogin(type, refreshToken, callback));
        }

        internal void RefreshAccessToken(ResultCallback callback)
        {
            Run(RequestRefreshAccessToken(callback));
        }

        internal void Logout(ResultCallback callback)
        {
            Run(RequestLogout(callback));
        }

        internal void RevokeToken(ResultCallback callback)
        {
            Run(RequestRevokeToken(callback));
        }

        internal void GetToken(PlatformType platformType, string authorizedCode, ResultCallback callback, bool fromWeb = false, string platformId = null)
        {
            if (string.IsNullOrEmpty(authorizedCode))
            {
                callback.TryError(ErrorCode.SdkPlatformTokenNotFound, "AuthorizedCode must not be null or empty.");
                return;
            }
            Run((PlatformUtil.IsPc() || PlatformUtil.IsConsole()) && platformType != PlatformType.Device ? RequestPcConsoleToken(authorizedCode, callback) : RequestMobileToken(platformType, authorizedCode, callback, false, fromWeb, platformId));
        }

        internal void GetTokenHeadless(PlatformType platformType, string authorizedCode, ResultCallback callback, bool fromWeb = false, string platformId = null)
        {
            if (string.IsNullOrEmpty(authorizedCode))
            {
                callback.TryError(ErrorCode.SdkPlatformTokenNotFound, "AuthorizedCode must not be null or empty.");
                return;
            }
            Run((PlatformUtil.IsPc() || PlatformUtil.IsConsole()) && platformType != PlatformType.Device ? RequestPcConsoleTokenHeadless(authorizedCode, callback) : RequestMobileToken(platformType, authorizedCode, callback, false, fromWeb, platformId));
        }

        internal void AuthorizeMobile(ResultCallback<AuthorizeResult> callback, PlatformType platformType = PlatformType.Krafton, bool isLinking = false)
        {
            Run(RequestAuthorizeMobile(callback, platformType, isLinking));
        }

        internal void ExternalLink(ResultCallback<ExternalLink> callback, string keyword = "helpdesk")
        {
            Run(RequestExternalLink(keyword, callback));
        }

        internal void GetKidInfo(PlatformType type, string platformToken, ResultCallback<KidInfoResult> callback)
        {
            Run(RequestKidInfo(type, platformToken, callback));
        }

        internal void GetTokenWithGameServerId(string gameServerId, ResultCallback callback)
        {
            Run(RequestUpdateTokenWithGameServerId(gameServerId, callback));
        }

        internal void AuthorizePcConsole(PlatformType platformType, string platformToken, ResultCallback<PcConsoleAuthResult> callback)
        {
            if (string.IsNullOrEmpty(platformToken))
            {
                callback.TryError(ErrorCode.SdkPlatformTokenNotFound, "PlatformToken must not be null or empty.");
                return;
            }

            Run(RequestAuthorizePcConsole(platformType, platformToken, callback));
        }

        internal void AuthorizePcConsoleHeadless(PlatformType platformType, string platformToken, ResultCallback<PcConsoleAuthResult> callback)
        {
            if (string.IsNullOrEmpty(platformToken))
            {
                callback.TryError(ErrorCode.SdkPlatformTokenNotFound, "PlatformToken must not be null or empty.");
                return;
            }

            Run(RequestAuthorizePcConsoleHeadless(platformType, platformToken, callback));
        }

        internal void CheckEligibility(ResultCallback<CheckEligibilityResult> callback)
        {
            Run(RequestCheckEligibility(callback));
        }

        internal void ForceDeleteAccount(ResultCallback callback)
        {
            Run(RequestArchive(callback));
        }

        internal void GetKidIdToken(ResultCallback<KidIdTokenResult> callback)
        {
            Run(RequestIdToken(callback));
        }

        internal void SelectMainGameUserId(string token, string userId, bool isMain, ResultCallback callback)
        {
            Run(RequestKidResolve(token, userId, isMain, callback));
        }

        internal void LinkAccount(PlatformType platformType, string token, ResultCallback callback)
        {
            Run(RequestLinkAccount(platformType, token, callback));
        }

        internal void PcAutoLogin(PlatformType platformType, bool isHeadless, string token, ResultCallback callback)
        {
            Run(isHeadless ? RequestPcHeadlessAutoLogin(platformType, token, callback) : RequestPcFullKidAutoLogin(platformType, token, callback));
        }

        internal void GetKIDUserInfoWebURL(KIDUserInfoUri userInfoUri, ResultCallback<KIDUserInfoWebURLData> callback)
        {
            Run(RequestKIDUserInfoWebURL(userInfoUri, callback));
        }
    }
}