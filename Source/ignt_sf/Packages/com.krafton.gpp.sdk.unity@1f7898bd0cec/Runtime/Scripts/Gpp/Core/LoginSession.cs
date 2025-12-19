using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Gpp.Constants;
using Gpp.Extension;
using Gpp.Extensions.FirebasePush;
using Gpp.Extensions.GooglePlayGames;
using Gpp.Language;
using Gpp.Log;
using Gpp.Models;
using Gpp.Network;
using Gpp.Telemetry;
using Gpp.Utils;
using UnityEngine;
using Random = System.Random;

namespace Gpp.Core
{
    internal class LoginSession
    {
        public enum LoginType
        {
            auto,
            manual,
            link
        }

        public enum LoginFlowType
        {
            none,
            headless,
            full_kid
        }

        private readonly GppHttpClient _httpClient;
        private readonly CoroutineRunner _coroutineRunner;
        private readonly TimeSpan _maxBackoffInterval = TimeSpan.FromDays(1);
        private const uint WaitExpiryDelay = 100;
        private DateTime _nextRefreshTime;
        private Coroutine _maintainAccessTokenCoroutine;
        private Coroutine _bearerAuthRejectedCoroutine;
        private LoginType _loginType = LoginType.auto;
        private LoginFlowType _loginFlowType = LoginFlowType.none;

        internal Action<string> RefreshTokenCallback;
        internal Action<string> LanguageChangedCallback;

        internal string AnalyticsId { get; set; }

        internal string AccessToken
        {
            get => cachedTokenData?.AccessToken;
            set => cachedTokenData.AccessToken = value;
        }
        internal string RefreshToken
        {
            get => cachedTokenData?.RefreshToken;
            set => cachedTokenData.RefreshToken = value;
        }
        internal string UserId => cachedTokenData?.UserId;
        internal bool IsComply => cachedTokenData?.IsComply ?? false;

        internal string KraftonTag => cachedTokenData?.KraftonTag;
        internal string PlatformId => cachedTokenData?.PlatformType;
        internal string PlatformUserId => cachedTokenData?.PlatformUserId;
        internal string GameServerId { get; set; } = null;
        internal string Email { get; set; }
        internal DcsModels.SdkConfigInfoValues RemoteSdkConfig { get; set; }
        internal string PlayerLocale => LanguageCode + "-" + CountryCode;
        internal TokenData cachedTokenData;
        private PlatformType attemptingLoginPlatform = PlatformType.None;
        internal PlatformType AttemptingLoginPlatform
        {
            get
            {
                if (attemptingLoginPlatform != PlatformType.None)
                {
                    return attemptingLoginPlatform;
                }
                if (PlatformUtil.IsPc())
                {
                    return PlatformType.Steam;
                }

                return PlatformUtil.IsAndroidOrEditor() ? PlatformType.Google : PlatformType.iOS;
            }
            set
            {
                attemptingLoginPlatform = value;
            }
        }

        internal bool AttemptingHeadlessLogin { get; set; } = false;
        internal bool IsFirstSession { get; set; } = true;

        private string languageCode = "";
        internal string LanguageCode
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    languageCode = value;
                }
            }
            get
            {
                if (!string.IsNullOrEmpty(languageCode)) return languageCode;
                SetLanguageCode(GetSupportSdkLanguage());
                return languageCode;
            }
        }

        public int AccessTokenExpireIn => cachedTokenData?.AccessTokenExpiresIn ?? 0;
        public int RefreshTokenExpireIn => cachedTokenData?.RefreshTokenExpiresIn ?? 0;

        private string countryCode = "";
        public string CountryCode
        {
            set
            {
                if (value != null)
                {
                    countryCode = value;
                }
            }
            get => countryCode;
        }

        internal LoginSession()
        {
            _httpClient = GppSDK.GetHttpClient();
            _coroutineRunner = GppSDK.GetCoroutineRunner();

            _httpClient.BearerAuthRejected += BearerAuthRejected;
            _httpClient.UnauthorizedOccured += UnauthorizedOccured;
        }

        public void SetLanguageCode(string language)
        {
            if (language.Contains("zhs"))
                language = "zh-Hans";

            if (language.Contains("zht"))
                language = "zh-Hant";

            string country = PreciseLocale.GetRegion();

            if (language == "es" && country == "MX")
            {
                language = "es-MX";
            }

            if (language == "pt" && country == "BR")
            {
                language = "pt-BR";
            }

            language = GppLanguageConverter.GppGetLanguage(language);
            languageCode = language.Replace('_', '-');
            if (IsLoggedIn())
            {
                GppSDK.GetBasicApi().UpdateUserProfile();
                LanguageChangedCallback?.Invoke(language);
            }
        }

        public string GetSystemCountry()
        {
            return PlatformUtil.IsMobileNotEditor() ? PreciseLocale.GetRegion() : GeoUtility.GetCountryCode();
        }

        private string GetSupportSdkLanguage()
        {
            return Application.systemLanguage switch
            {
                SystemLanguage.ChineseSimplified => "zh-Hans",
                SystemLanguage.ChineseTraditional => "zh-Hant",
                SystemLanguage.Indonesian => "id",
                _ => ConvertSystemLangToGppLang()
            };
        }

        private string ConvertSystemLangToGppLang()
        {
            string language = PreciseLocale.GetLanguage();
            string country = PreciseLocale.GetRegion();
            if (language == "es" && country == "MX")
            {
                language = "es-MX";
            }

            if (language == "pt" && country == "BR")
            {
                language = "pt-BR";
            }

            return GppLanguageConverter.GppGetLanguage(language);
        }

        public bool CompareWithPlayerLanguage(string languageCode)
        {
            languageCode = GppLanguageConverter.GppGetLanguage(languageCode);
            if (languageCode.Contains("-"))
            {
                languageCode = languageCode.Split('-')[0];
            }

            return this.languageCode.Equals(languageCode);
        }

        private IEnumerator MaintainSession()
        {
            _nextRefreshTime = ScheduleNormalRefresh(cachedTokenData.AccessTokenExpiresIn);
            TimeSpan refreshBackoff = TimeSpan.FromSeconds(10);

            while (cachedTokenData != null)
            {
                if (refreshBackoff >= _maxBackoffInterval)
                {
                    yield break;
                }

                if (cachedTokenData.AccessToken == null || DateTime.UtcNow < _nextRefreshTime)
                {
                    yield return new WaitForSeconds(WaitExpiryDelay / 1000f);
                    continue;
                }

                Result refreshResult = null;

                yield return GppSDK.GetIamApi().RequestRefreshAccessToken(result =>
                    {
                        GppSDK.SendGppTelemetry(GppClientLogModels.LogType.TokenRefresh);
                        refreshResult = result;
                    }
                );
                if (refreshResult.IsError)
                {
                    if (refreshResult.Error.Code == ErrorCode.BadRequest)
                    {
                        GppSyncContext.RunOnUnityThread(() =>
                        {
                            GppSDK.GetEventListener()?.NotifyBrokenToken();
                            ClearSession();
                        });
                    }

                    yield break;
                }

                refreshBackoff = CalculateBackoffInterval(refreshBackoff, new Random().Next(1, 60));

                _nextRefreshTime = ScheduleNormalRefresh(cachedTokenData.AccessTokenExpiresIn);
                GppSDK.GetLobby().Connect(true);
            }
        }

        private IEnumerator BearerAuthRejectRefreshToken(Action<string> callback)
        {
            Result refreshResult = null;

            yield return GppSDK.GetIamApi().RequestRefreshAccessToken(result => refreshResult = result);

            if (refreshResult.IsError)
            {
                ClearSession();
                GppSDK.GetEventListener()?.NotifyBrokenToken();
                yield break;
            }

            callback?.Invoke(cachedTokenData.AccessToken);
            if (_bearerAuthRejectedCoroutine == null) yield break;
            _coroutineRunner.Stop(_bearerAuthRejectedCoroutine);
            _bearerAuthRejectedCoroutine = null;
        }

        private void BearerAuthRejected(string accessToken, Action<string> callback)
        {
            if (accessToken != cachedTokenData?.AccessToken || _bearerAuthRejectedCoroutine != null || _maintainAccessTokenCoroutine == null)
            {
                return;
            }

            _coroutineRunner.Stop(_maintainAccessTokenCoroutine);
            _maintainAccessTokenCoroutine = null;
            _bearerAuthRejectedCoroutine = _coroutineRunner.Run(BearerAuthRejectRefreshToken(callback));
        }

        private void UnauthorizedOccured(string accessToken)
        {
            if (accessToken == cachedTokenData?.AccessToken)
            {
                ClearSession();
            }
        }

        private static DateTime ScheduleNormalRefresh(int expiresIn)
        {
            return DateTime.UtcNow + TimeSpan.FromSeconds((expiresIn - 1) * 0.8);
        }

        private static TimeSpan CalculateBackoffInterval(TimeSpan previousRefreshBackoff, int randomNum)
        {
            previousRefreshBackoff = TimeSpan.FromSeconds(previousRefreshBackoff.Seconds * 2);
            return previousRefreshBackoff + TimeSpan.FromSeconds(randomNum);
        }

        internal void SetSession(TokenData tokenData)
        {
            GppLog.Log($"SetSession {tokenData.ToPrettyJsonString()}");
            cachedTokenData = tokenData;

            _httpClient.SetImplicitBearerToken(cachedTokenData.AccessToken);
            _httpClient.SetImplicitPathParams(
                new Dictionary<string, string>
                {
                    { "namespace", cachedTokenData.Namespace }, { "userId", cachedTokenData.UserId }
                }
            );
            _httpClient.ClearCookies();
            GppTokenProvider.SaveLastLoginType(GppSDK.GetSession().AttemptingLoginPlatform);
            GppTokenProvider.SaveRefreshTokenData(tokenData);

            RefreshTokenCallback?.Invoke(tokenData.AccessToken);
            _maintainAccessTokenCoroutine ??= _coroutineRunner.Run(MaintainSession());
            GppSDK.GetTelemetryManager().StartTelemetryScheduler();
            if (GooglePlayGamesExt.CanUse() && GooglePlayGamesExt.Impl().IsPC())
            {
                return;
            }
            if (FirebasePushExt.CanUse())
            {
                FirebasePushExt.Impl().GetPushToken(token =>
                {
                    if (cachedTokenData != null)
                        GppSDK.GetPushApi().RegisterToken(token.Value);
                });
            }
        }

        internal void SetBearerAuthToken(TokenData tokenData)
        {
            cachedTokenData = tokenData;

            _httpClient.SetImplicitBearerToken(cachedTokenData.AccessToken);
            _httpClient.SetImplicitPathParams(
                new Dictionary<string, string>
                {
                    { "namespace", cachedTokenData.Namespace }, { "userId", cachedTokenData.UserId }
                }
            );
            _httpClient.ClearCookies();
            GppTokenProvider.SaveLastLoginType(GppSDK.GetSession().AttemptingLoginPlatform);
            GppTokenProvider.SaveRefreshTokenData(tokenData);
        }

        internal void ClearSession(bool isDeleteTokenData = true)
        {
            GppLog.Log($"ClearSession isDeleteTokenData : {isDeleteTokenData}");
            if (_maintainAccessTokenCoroutine != null)
            {
                _coroutineRunner.Stop(_maintainAccessTokenCoroutine);
                _maintainAccessTokenCoroutine = null;
            }

            if (isDeleteTokenData)
            {
                GppTokenProvider.DeleteTokenData();
            }

            ClearTasks();
        }

        private void ClearTasks()
        {
            IsFirstSession = false;
            GppSDK.GetLobby().Disconnect();

            if (GooglePlayGamesExt.CanUse() && AttemptingLoginPlatform != PlatformType.Guest)
            {
                GooglePlayGamesExt.Impl().SetGpgInfo(null);
            }

            _httpClient.SetImplicitBearerToken(null);
            _httpClient.ClearImplicitPathParams();
            cachedTokenData = null;
            AttemptingLoginPlatform = PlatformType.None;
        }

        internal bool IsLoggedIn()
        {
            return !string.IsNullOrEmpty(AccessToken) && !string.IsNullOrEmpty(UserId);
        }

        public LoginType GetLoginType()
        {
            return _loginType;
        }

        internal void SetLoginType(LoginType loginType)
        {
            _loginType = loginType;
        }

        public LoginFlowType GetLoginFlowType()
        {
            return _loginFlowType;
        }

        internal void SetLoginFlowType(LoginFlowType type)
        {
            _loginFlowType = type;
        }
    }
}