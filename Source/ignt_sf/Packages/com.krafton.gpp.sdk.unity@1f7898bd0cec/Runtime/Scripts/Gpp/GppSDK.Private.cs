using System;
using System.Collections.Generic;
using System.Linq;
using Gpp.Api;
using Gpp.Api.Basic;
using Gpp.Api.Catalog;
using Gpp.Api.DCS;
using Gpp.Api.Legal;
using Gpp.Api.Payment;
using Gpp.Api.Platform;
using Gpp.Api.Push;
using Gpp.Api.Social;
using Gpp.Api.Telemetry;
using Gpp.Auth;
using Gpp.CommonUI;
using Gpp.CommonUI.Modal;
using Gpp.CommonUI.Toast;
using Gpp.Config;
using Gpp.Config.Extensions;
using Gpp.Constants;
using Gpp.Core;
using Gpp.Extension;
using Gpp.Extensions.EpicGames;
using Gpp.Extensions.FirebasePush;
using Gpp.Extensions.GooglePlayGames;
using Gpp.Extensions.IAP;
using Gpp.Extensions.IAP.Models;
using Gpp.Extensions.Mac;
using Gpp.Extensions.Steam;
using Gpp.Localization;
using Gpp.Log;
using Gpp.Models;
using Gpp.Network;
using Gpp.Surveys;
using Gpp.Telemetry;
using Gpp.Utils;
using UnityEngine;
using static Gpp.Models.DcsModels;
using Object = UnityEngine.Object;

namespace Gpp
{
    public sealed partial class GppSDK
    {
        private static readonly Lazy<GppSDK> LazySdk = new(() => new GppSDK());

        private static GppSDK Instance => LazySdk.Value;

        private bool _isInitialized;
        private bool _isLoginInProgress;

        private GppOptions _options;
        private LoginSession _session;
        private GppConfig _config;
        private GppHttpClient _httpClient;
        private CoroutineRunner _coroutineRunner;
        private TelemetryManager _telemetryManager;
        private IamApi _iamApi;
        private BasicApi _basicApi;
        private PlatformApi _platformApi;
        private LegalApi _legalApi;
        private TelemetryApi _telemetryApi;
        private GdprApi _gdprApi;
        private Lobby _lobby;
        private PushApi _pushApi;
        private DcsApi _dcsApi;
        private SocialApi _socialApi;
        private KCNApi _kcnApi;
        private CatalogApi _catalogApi;
        private PaymentApi _paymentApi;

        private GppInputController _inputController;

        private ResultCallback<GppUser> _onLoginCallback;
        private IGppEventListener _eventListener;

        private GppSDK()
        {
            _inputController = new GppInputController();
        }

        internal static CoroutineRunner GetCoroutineRunner()
        {
            if (Instance._coroutineRunner != null)
            {
                return Instance._coroutineRunner;
            }

            CreateGppSdkObject();
            return Instance._coroutineRunner;
        }

        internal static TelemetryManager GetTelemetryManager()
        {
            if (Instance._telemetryManager != null)
            {
                return Instance._telemetryManager;
            }

            CreateGppSdkObject();
            return Instance._telemetryManager;
        }

        internal static GppHttpClient GetHttpClient()
        {
            if (Instance._httpClient != null) return Instance._httpClient;
            CreateGppHttpClient();
            return Instance._httpClient;
        }

        internal static GppConfig GetConfig()
        {
            if (Instance._config != null)
                return Instance._config;
            Instance._config = GppConfigSO.GetActiveConfigFromFile();
            return Instance._config;
        }

        internal static void CreateGppSdkObject()
        {
            var sdk = GameObject.Find("GppSDK");
            if (sdk is null)
            {
                sdk = new GameObject("GppSDK");
                sdk.AddComponent<GppSyncContext>();
                CoroutineRunner gppRunner = sdk.AddComponent<CoroutineRunner>();
                Instance._coroutineRunner = gppRunner;
                TelemetryManager telemetryManager = sdk.AddComponent<TelemetryManager>();
                Instance._telemetryManager = telemetryManager;
                Object.DontDestroyOnLoad(sdk);
                return;
            }

            Instance._coroutineRunner = sdk.GetComponent<CoroutineRunner>();
            Instance._telemetryManager = sdk.GetComponent<TelemetryManager>();
        }

        internal static void CreateGppHttpClient()
        {
            Instance._httpClient ??= new GppHttpClient();
            Instance._httpClient.SetCredentials(Instance._config.ClientId, "");
            Instance._httpClient.SetBaseUri(new Uri(Instance._config.BaseUrl));            
        }

        internal static void CreateGppAPIs()
        {
            Instance._session = new LoginSession
            {
                LanguageCode = Instance._options.LanguageCode,
                AnalyticsId = GppTokenProvider.CreateGuestUserId()
            };
            Instance._iamApi = new IamApi();
            Instance._basicApi = new BasicApi();
            Instance._platformApi = new PlatformApi();
            Instance._legalApi = new LegalApi();
            Instance._telemetryApi = new TelemetryApi();
            Instance._gdprApi = new GdprApi();
            Instance._lobby = new Lobby(new WebSocket());
            Instance._pushApi = new PushApi();
            Instance._dcsApi = new DcsApi();
            Instance._socialApi = new SocialApi();
            Instance._kcnApi = new KCNApi();
            Instance._catalogApi = new CatalogApi();
            Instance._paymentApi = new PaymentApi();
        }

        private static void SetGppOptions(GppOptions options)
        {
            if (options == null)
            {
                Instance._options = GppOptions.GetDefault();
                return;
            }

            Instance._options = options;
        }

        private static void SetConfig(GppConfig config, Action<bool, string> completeCallback)
        {
            if (config is not null)
            {
                Instance._config = config;
                completeCallback?.Invoke(true, "SetConfig: success");
                return;
            }
            if (Instance._config is null)
            {
                GppConfigSO.LoadFromFileAsync(configFile =>
                {
                    try
                    {
                        if (configFile is null)
                        {
                            completeCallback?.Invoke(false, "'GppConfig.asset' isn't found in the Project/Assets/Resources/GppSDK directory");
                            return;
                        }
                        Instance._config = configFile.ActiveConfig;
                        completeCallback?.Invoke(true, "using default GppConfig from file.");
                    }
                    catch (Exception ex)
                    {
                        completeCallback?.Invoke(false, $"Exception occurred while loading GppConfig from file. {ex.Message}");
                        return;
                    }
                });
            }
            else
            {
                completeCallback?.Invoke(true, "using already set GppConfig.");
            }
        }

        private static void InitPlatform(Action onSuccess)
        {
            if (PlatformUtil.IsMobileNotEditor())
            {
                InitMobilePlatform(onSuccess);
            }
            else if (PlatformUtil.IsPc())
            {
                InitPcPlatform();
                onSuccess?.Invoke();
            }
            else if (PlatformUtil.IsConsole())
            {
#if UNITY_PS5
                InitConsolePlatform();
#endif
                onSuccess?.Invoke();
            }
            else
            {
                onSuccess?.Invoke();
            }
        }

        private static void AddGppListeners(IGppEventListener listener)
        {
            Instance._eventListener = listener;
        }

        internal static LoginSession GetSession()
        {
            return Instance._session;
        }

        internal static IamApi GetIamApi()
        {
            return Instance._iamApi;
        }

        internal static BasicApi GetBasicApi()
        {
            return Instance._basicApi;
        }

        internal static PlatformApi GetPlatformApi()
        {
            return Instance._platformApi;
        }

        internal static Lobby GetLobby()
        {
            return Instance._lobby;
        }

        internal static LegalApi GetLegalApi()
        {
            return Instance._legalApi;
        }

        internal static TelemetryApi GetTelemetryApi()
        {
            return Instance._telemetryApi;
        }

        internal static GdprApi GetGdprApi()
        {
            return Instance._gdprApi;
        }

        internal static PushApi GetPushApi()
        {
            return Instance._pushApi;
        }

        internal static DcsApi GetDcsApi()
        {
            return Instance._dcsApi;
        }

        internal static SocialApi GetSocialApi()
        {
            return Instance._socialApi;
        }

        internal static KCNApi GetKCNApi()
        {
            return Instance._kcnApi;
        }
        
        internal static CatalogApi GetCatalogApi()
        {
            return Instance._catalogApi;
        }
        
        internal static PaymentApi GetPaymentApi()
        {
            return Instance._paymentApi;
        }

        internal static GppOptions GetOptions()
        {
            return Instance._options;
        }

        internal static IGppEventListener GetEventListener()
        {
            return Instance._eventListener;
        }

        internal static void SetLoginCallback(ResultCallback<GppUser> callback)
        {
            Instance._onLoginCallback = callback;
        }

        internal static ResultCallback<GppUser> GetLoginCallback()
        {
            return Instance._onLoginCallback;
        }

        internal static GppInputController GetInputController()
        {
            return Instance._inputController;
        }

        private static GppModalData.Builder BuildUIModalData(AppUpdateInfo updateData)
        {
            return new GppModalData.Builder()
                .SetUIPriority(500)
                .SetTitle(LocalizationKey.AppUpdateTitle.Localise())
                .SetMessage(updateData.Message)
                .SetPositiveButtonText(LocalizationKey.AppUpdateTitle.Localise())
                .SetPositiveAction(_ =>
                    {
                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.UpdateConfirm);
                        string marketUrl = string.IsNullOrEmpty(updateData.Url) ? GppUtil.GetMarketUrl(GetOptions().Store) : updateData.Url;
                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.ExposeBrowser);
                        Application.OpenURL(marketUrl);
                    }
                );
        }

        private static bool ShouldOfferOptionalUpdate(AppUpdateInfo updateData)
        {
            return updateData.ShowOptionalUpdate && GppPrefsUtil.AvailableOptionalUpdate(updateData.LatestVersion);
        }

        internal static void CheckMaintenance(ResultCallback<Maintenance> callback)
        {
            if (!IsInitialized)
            {
                callback.TryError(ErrorCode.NotInitialized);
                return;
            }

            GetBasicApi().GetMaintenance(callback);
        }


        internal static void HandleDeviceAutoLogin()
        {
            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.TryLogin, new Dictionary<string, object>
            {
                {"login_method", PlatformType.Device.ToString().ToLower()}
            });
            GppAuth.Login(PlatformType.Device, result =>
            {
                if (result.IsError)
                {
                    GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.LoginFailed, new Dictionary<string, object>
                    {
                        {"login_type", GetSession().GetLoginType().ToString()},
                        {"error_code", result.Error.Code.ToString()},
                        {"error_message", result.Error.Message}
                    });
                }
                TransformAndCallbackLoginResult(result);
            });
        }

        internal static void HandleGuestLogin()
        {
            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.TryLogin, new Dictionary<string, object>
            {
                {"login_method", PlatformType.Guest.ToString().ToLower()}
            });
            GppAuth.LoginWithGuest(result =>
            {
                if (result.IsError)
                {
                    GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.LoginFailed, new Dictionary<string, object>
                    {
                        {"login_type", GetSession().GetLoginType().ToString()},
                        {"error_code", result.Error.Code.ToString()},
                        {"error_message", result.Error.Message}
                    });
                }
                TransformAndCallbackLoginResult(result);
            });
        }


        internal static void CheckGameServerMaintenance(string gameServerId, ResultCallback<Maintenance> callback)
        {
            if (!IsInitialized)
            {
                callback.TryError(ErrorCode.NotInitialized);
                return;
            }

            GetBasicApi().GetGameServerMaintenance(gameServerId, callback);
        }

        internal static void GpgLogin(string gpgToken)
        {
            GetIamApi().GetKidInfo(PlatformType.Gpgs, gpgToken, result =>
                {
                    if (!result.IsError)
                    {
                        KidInfoResult kidInfoResult = result.Value;
                        GppUI.ShowGpgPopup(kidInfoResult, (ui, isConfirm) =>
                            {
                                Object.Destroy(ui);
                                if (isConfirm)
                                {
                                    GppAuth.Login(PlatformType.Krafton, loginResult =>
                                        {
                                            TransformAndCallbackLoginResult(loginResult);
                                        }
                                    );
                                }
                                else
                                {
                                    GooglePlayGamesExt.Impl().SetGpgInfo(null);
                                    GppUI.ShowLogin(OnClickLoginButton, OnClickCloseButton);
                                }
                            }
                        );
                    }
                    else
                    {
                        GppUI.ShowLogin(OnClickLoginButton, OnClickCloseButton);
                    }
                }
            );
        }

        internal static void TransformAndCallbackLoginResult(Result loginResult, bool isShowLoginToast = true)
        {
            if (loginResult == null)
            {
                Debug.LogError("loginResult must not be null.");
                return;
            }

            if (loginResult.IsError)
            {
                Instance._onLoginCallback.TryError(loginResult.Error);
            }
            else
            {
                GetBasicApi().UpdateUserProfile();

                if (PlatformUtil.IsMobile())
                {
                    GetBasicApi().GetPushStatus(pushResult =>
                        {
                            if (!pushResult.IsError && GetSession().cachedTokenData != null)
                            {
                                GetSession().cachedTokenData.PushEnabled = pushResult.Value.pushEnable;
                            }

                            GetBasicApi().GetNightPushStatus(nightPushResult =>
                                {
                                    if (!nightPushResult.IsError && GetSession().cachedTokenData != null)
                                    {
                                        GetSession().cachedTokenData.NightPushEnabled = nightPushResult.Value.pushNightEnable;
                                    }

                                    Instance._onLoginCallback.TryOk(new GppUser(GetSession().cachedTokenData));

                                    if (MobileIapExt.CanUse())
                                    {
                                        MobileIapExt.Impl().Restore(restoreResult =>
                                        {
                                            GppLog.Log($"Restore Result: {restoreResult.ToPrettyJsonString()}");

                                            if (!restoreResult.IsError)
                                            {
                                                var unconsumedList =
                                                    restoreResult.Value.FindAll(item =>
                                                        !item.ProductId.EndsWith(".rew"));
                                                var externalPurchaseList =
                                                    restoreResult.Value.FindAll(item =>
                                                        item.ProductId.EndsWith(".rew"));
                                                GetEventListener()?.NotifyRestoredPurchase(unconsumedList);
                                                GetEventListener()?.NotifyExternalPurchase(externalPurchaseList);
                                            }
                                        });
                                    }
                                }
                            );
                        }
                    );
                }
                else
                {
                    if (isShowLoginToast && !string.IsNullOrEmpty(GetSession().cachedTokenData.KraftonTag))
                    {
                        //var dic = new Dictionary<string, string>
                        //{
                        //    {"{n}", $"<b>{GetSession().cachedTokenData.KraftonTag}</b>"}
                        //};
                        //string message = LocalizationKey.KidUsedForGamePlay.Localise(null, dic);
                        //GppUI.ShowToast(message, null, GppToastPosition.BOTTOM_RIGHT);
                    }

                    if (MacExt.CanUse() && GetSession().AttemptingLoginPlatform == PlatformType.AppleMac)
                    {
                        SetAppTransaction();
                    }
                    Instance._onLoginCallback.TryOk(new GppUser(GetSession().cachedTokenData));
                }
            }
        }

        internal static void OpenLegalWebView(string policyId, ResultCallback callback = null)
        {
            OpenLegalWebView(new List<string> { policyId }, callback);
        }

        internal static void OpenLegalWebView(List<string> policyIds, ResultCallback callback = null)
        {
            foreach (var url in policyIds.Select(policyId => $"{Instance._config.BaseUrl}/legal/{policyId}"))
            {
                OpenWebView(url);
                callback?.TryOk();
            }
        }

        internal static void OpenWebView(string url, ResultCallback callback = null)
        {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            var webViewData = WebView.GppWebViewData.RecommendedWebViewData();
            webViewData.url = url;
            webViewData.backgroundColor = WebView.GppWebViewBackgroundColor.WHITE;
            Log.GppLog.Log("OpenWebView Url : " + webViewData.url);
            WebView.GppWebView.OpenWebView(webViewData, response =>
            {
                Log.GppLog.Log("Webview Closed : " + response.Response);
            });
#else
            Application.OpenURL(url);
#endif
            callback?.TryOk();
        }

        internal static void ForceDeleteAccount(ResultCallback callback)
        {
            GppAuth.ForceDeleteAccount(callback.Try);
        }

        internal static void DisconnectSdk()
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            if (IsLoggedIn)
            {
                CheckExitSurvey();
            }
#endif
            if (!IsInitialized)
                return;

            GppLog.Log("DisconnectSdk...");
            Instance._isInitialized = false;
            GetLobby()?.Disconnect();
            Instance._authWebSocket?.Close();
        }

        private static void CheckExitSurvey()
        {
            var exitSurvey = GppSurveyDataManager.GetGameExitSurvey();
            if (exitSurvey is null)
            {
                return;
            }

            Application.OpenURL(GppSurveyDataManager.GetSurveyLink());
        }

        internal static void SendGppTelemetry(string eventName, Dictionary<string, object> payload = null, ResultCallback callback = null)
        {
            GppSyncContext.RunOnUnityThread(() =>
            {
                eventName = eventName.ToLower();
                var lowerCaseDict = new Dictionary<string, object>();
                if (payload != null)
                {
                    lowerCaseDict = payload.ToDictionary(
                        item => item.Key.ToLower(),
                        item => item.Value);
                }
                var telemetryBody = KpiTelemetryBody.CreateDefault();
                if (telemetryBody is null)
                {
                    GppLog.LogWarning("telemetryBody is null");
                    return;
                }
                telemetryBody.LogType = eventName;
                telemetryBody.SetAdditionalFields(lowerCaseDict);

                if (IsLoggedIn && eventName != GppClientLogModels.LogType.UserEntry)
                    GetTelemetryManager().Send(TelemetryType.KpiTelemetry, telemetryBody, callback);
                else
                    GetTelemetryManager().Send(TelemetryType.KpiNoAuthTelemetry, telemetryBody, callback);
            });
        }

        internal static void SendImmediatelyGppTelemetry(string eventName, Dictionary<string, object> payload = null, ResultCallback callback = null)
        {
            GppSyncContext.RunOnUnityThread(() =>
            {
                eventName = eventName.ToLower();
                var lowerCaseDict = new Dictionary<string, object>();
                if (payload != null)
                {
                    lowerCaseDict = payload.ToDictionary(
                        item => item.Key.ToLower(),
                        item => item.Value);
                }
                KpiTelemetryBody telemetryBody = KpiTelemetryBody.CreateDefault();
                if (telemetryBody is null)
                {
                    GppLog.LogWarning("telemetryBody is null");
                    return;
                }
                telemetryBody.LogType = eventName;
                telemetryBody.SetAdditionalFields(lowerCaseDict);

                if (IsLoggedIn && eventName != GppClientLogModels.LogType.UserEntry)
                    GetTelemetryManager().Send(TelemetryType.KpiTelemetry, telemetryBody, callback, true);
                else
                    GetTelemetryManager().Send(TelemetryType.KpiNoAuthTelemetry, telemetryBody, callback, true);
            });
        }

        internal static void SendGAPurchaseEvent(string itemId, string transactionId, string price, string currency)
        {
            if (PlatformUtil.IsIOS() && GoogleAnalyticsExt.CanUse())
            {
                GoogleAnalyticsExt.Impl().SendPurchase(itemId, transactionId, price, currency);
            }
        }

        internal static void RepayPurchase(string productId, string restrictionId, string refundTransactionId, ResultCallback<IapPurchase> callback)
        {
            if (MobileIapExt.CanUse())
            {
                MobileIapExt.Impl().RepayPurchase(productId, restrictionId, refundTransactionId, callback);
            }
            else
            {
                callback.TryError(ErrorCode.NotSupportPlatform);
            }
        }

        internal static void OpenRepayCS(ResultCallback callback)
        {
            GetIamApi().ExternalLink((result) =>
            {
                if (result.IsError)
                {
                    callback.TryError(ErrorCode.LoadExternalUrlFailed, result.Error.Message);
                }
                else
                {
                    Application.OpenURL(result.Value.RedirectTo);
                    callback.TryOk();
                }
            }, "supportmain");
        }

#if UNITY_STANDALONE

        private static bool NeedToCheckLocalConfig()
        {
            var remoteSdkConfig = GetSession()?.RemoteSdkConfig;

            if (remoteSdkConfig is null)
            {
                return true;
            }

            var useSyncOwnership = remoteSdkConfig.UseSyncOwnership;
            
            if (string.IsNullOrEmpty(useSyncOwnership))
            {
                return true;
            }

            return bool.TryParse(useSyncOwnership, out var value) is false;
        }
        
        internal static void OwnerShipSync(PlatformType platformType)
        {
            if (GetSession()?.IsLoggedIn() == false)
            {
                return;
            }

            switch (platformType)
            {
                case PlatformType.Steam:
                {
                    if (SteamExt.CanUse() is false)
                    {
                        return;
                    }

                    bool enableOwnershipSync;
                    
                    if (NeedToCheckLocalConfig())
                    {
                        enableOwnershipSync = GetConfig().EnableOwnershipSync;
                    }
                    else
                    {
                        bool.TryParse(GetSession().RemoteSdkConfig.UseSyncOwnership, out enableOwnershipSync);
                    }

                    if (enableOwnershipSync is false)
                    {
                        return;
                    }

                    if (string.IsNullOrEmpty(SteamExt.Impl().GetAppId()))
                    {
                        return;
                    }
                
                    GetPlatformApi().OwnerShipSync(SteamExt.Impl().GetAppId(), SteamExt.Impl().GetSteamUserId(), result =>
                    {
                        if (result.IsError)
                        {
                            return;
                        }
                        
                        if (result.Value is not 200)
                        {
                            return;
                        }
                        
                        GetDurableEntitlements(result =>
                        {
                            if (result.IsError)
                            {
                                return;
                            }
                            
                            var entitlements = new Entitlements
                            {
                                DurableEntitlements = result.Value?.DurableEntitlements
                            };
                                        
                            GetEventListener().NotifyOwnerShipUpdated(entitlements);
                        });
                    });
                    break;
                }
                case PlatformType.EpicGames:
                {
                    if (EpicGamesExt.CanUse() is false)
                    {
                        return;
                    }

                    if (string.IsNullOrEmpty(EpicGamesExt.Impl().GetEpicAccountId()))
                    {
                        return;
                    }

                    GetPlatformApi().EpicOwnerShipSync(EpicGamesExt.Impl().GetEpicAccountId(), result =>
                    {
                        if (result.IsError)
                        {
                            return;
                        }
                        
                        if (result.Value.isRefreshEntitlements is false)
                        {
                            return;
                        }
                        
                        GetDurableEntitlements(result =>
                        {
                            if (result.IsError)
                            {
                                return;
                            }
                            
                            GetConsumableEntitlements(consumableResult =>
                            {
                                if (consumableResult.IsError)
                                {
                                    return;
                                }
                                
                                var entitlements = new Entitlements
                                {
                                    DurableEntitlements = result.Value?.DurableEntitlements,
                                    ConsumableEntitlements = consumableResult.Value?.ConsumableEntitlements
                                };

                                GetEventListener().NotifyOwnerShipUpdated(entitlements);
                            });
                        });
                    });
                    
                    break;
                }
            }
        }
#endif

        internal static void CheckGameServersMaintenanceInternal(ResultCallback<GameServerMaintenanceResult> callback)
        {
            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.GameServerIdExistence);
            GetBasicApi().GetGameServersMaintenance(callback);
        }

        private static void SetAppTransaction()
        {
            if (MacExt.CanUse())
            {
                MacExt.Impl().GetAppTransaction((result) =>
                {
                    if (result.IsError)
                    {
                        GppLog.Log(result.ToPrettyJsonString());
                        return;
                    }

                    SendAppleOwnerShipSync(result.Value);
                });
            }
        }

        private static void SendAppleOwnerShipSync(string jwsRepresentation)
        {
            GppSDK.GetPlatformApi().AppleOwnerShipSync(
                Application.identifier,
                GppSDK.GetSession().PlatformUserId,
                jwsRepresentation,
                result =>
                {
                    // There is no processing on the server response.
                });
        }

        private static void InternalLoginLogic(LoginRequestParam requestParam)
        {
            if (!IsInitialized)
            {
                GppLog.Log("GppSDK is not initialized.");
                requestParam.callback.TryError(ErrorCode.NotInitialized);
                return;
            }

            if (IsLoggedIn && requestParam.tryLink == false)
            {
                GppLog.Log("Already Logged in.");
                requestParam.callback.TryError(ErrorCode.AlreadyLoggedIn);
                return;
            }

            if (Instance._isLoginInProgress)
            {
                GppLog.LogWarning("Login is already in progress.");
                requestParam.callback.TryError(ErrorCode.AlreadyLoginProgress);
                return;
            }
            
            Instance._isLoginInProgress = true;
            var originCallback = requestParam.callback;
            void WrappedCallback(Result<GppUser> result)
            {
                try
                {
                    originCallback.Try(result);
                }
                finally
                {
                    Instance._isLoginInProgress = false;
                }
            }
            requestParam.callback = WrappedCallback;

            GetSession().AttemptingHeadlessLogin = requestParam.isHeadless;

            if (!PlatformUtil.IsMobile())
                GetSession().SetLoginFlowType(requestParam.isHeadless ? LoginSession.LoginFlowType.headless : LoginSession.LoginFlowType.full_kid);

            SetLoginCallback(requestParam.callback);

            GetDcsApi().GetSdkConfig(configResult =>
            {
                try
                {
                    if (configResult.IsError && configResult.Error.Code == ErrorCode.NetworkError)
                    {
                        GetSession().CountryCode = "";
                    }
                    else
                    {
                        var countryCode = GetOptions().CountryCode ?? configResult.Value.Value.CountryCode;
                        GetSession().CountryCode = countryCode;
                        GetSession().RemoteSdkConfig = configResult.Value.Value;
                    }
                }
                catch (Exception)
                {
                    GetSession().CountryCode = "";
                }
                finally
                {
                    if (requestParam.platformType == PlatformType.Device)
                    {
                        HandleDeviceAutoLogin();
                    }
                    else if (PlatformUtil.IsMobile())
                    {
                        if (requestParam.platformType == PlatformType.CustomProviderId)
                            HandleCustomProviderIdLogin(requestParam);
                        else if (requestParam.platformType == PlatformType.Guest)
                            HandleGuestLogin();
                        else
                            HandleMobileAutoLogin();
                    }
                    else if (PlatformUtil.IsConsole())
                    {
                        HandleConsoleAutoLogin(requestParam.platformType, requestParam.isHeadless);
                    }
                    else
                    {
                        HandlePcAutoLogin(requestParam.platformType, requestParam.isHeadless, requestParam.tryLink);
                    }
                }
            }
            );
        }
    }
}