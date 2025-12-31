using System;
using System.Collections.Generic;
using System.Linq;
using Gpp.Auth;
using Gpp.CommonUI;
using Gpp.CommonUI.Toast;
using Gpp.Constants;
using Gpp.Core;
using Gpp.Datadog;
using Gpp.Extension;
using Gpp.Extensions.FirebasePush;
using Gpp.Extensions.GooglePlayGames;
using Gpp.Extensions.IAP.Models;
using Gpp.Extensions.IAP;
using Gpp.Extensions.Steam;
using Gpp.Localization;
using Gpp.Log;
using Gpp.Models;
using Gpp.Telemetry;
using Gpp.Utils;
using UnityEngine;
using Object = UnityEngine.Object;
using Gpp.Extensions.Mac;
using Gpp.Extensions.Ps5;
using Gpp.Extensions.EpicGames;

namespace Gpp
{
    public sealed partial class GppSDK
    {
        public const string SdkVersion = "2.13.0";

        public static bool IsInitialized => Instance._isInitialized;
        public static bool IsLoginInProgress => Instance._isLoginInProgress;

        public static bool IsLoggedIn
        {
            get
            {
                if (!IsInitialized || GetSession() == null)
                {
                    return false;
                }

                return GetSession().IsLoggedIn();
            }
        }

        public static void Initialize(ResultCallback callback, IGppEventListener listener = null, GppOptions options = null)
        {   
            try
            {
                LocalizationManager.Init(() =>
                {
                    try
                    {
                        SetGppOptions(options);
                        SetConfig(GetOptions().Configuration, (isSuccess, message) =>
                        {
                            if (isSuccess is false)
                            {
#if UNITY_EDITOR
                                GppLog.LogWarning("Gpp Initialize Failed. GppConfig is not found. Please check the [GppSDK->SDK Configuration menu] to create GppConfig file.");
#endif
                                callback.TryError(ErrorCode.GPPConfigNotSet, $"Gpp Initialize Failed. {message}");
                                return;
                            }
                            CreateGppHttpClient();
                            CreateGppSdkObject();
                            CreateGppAPIs();
                            AddGppListeners(listener);
#if UNITY_EDITOR
                            // The default value of telemetryIdNo is 0.
                            // In UnityEditor, the app_init log is sent as 0 because the client_gpp_device_info log is not sent.
                            // app_init should always be sent as 1, so change it to 1.
                            KpiTelemetryBody.telemetryIdNo = 1;
#else
                            GppTelemetry.SendDeviceInfo(GppDeviceUtil.GetHardwareInfo());
#endif
                            InitPlatform(() =>
                            {
                                Instance._isInitialized = true;
                                Instance._isLoginInProgress = false;
                                GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.AppInit);
                                if (GetOptions().EnableGameServer)
                                    GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.GameServerStatusOn);
                                else
                                    GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.GameServerStatusOff);
                                callback.TryOk();
                            });
                        });
                    }
                    catch (Exception ex)
                    {
                        var remoteLog = new GppClientRemoteLog()
                        {
                            ErrorCode = ((int)ErrorCode.GPPSDKInitFailed).ToString(),
                            Log = ex.Message,
                            StackTrace = ex.StackTrace
                        };
                        DatadogManager.ReportLog(remoteLog);
                            
                        callback.TryError(ErrorCode.GPPSDKInitFailed, $"error : {ex.Message}");                 
                    }
                });
            }
            catch (Exception ex)
            {
                var remoteLog = new GppClientRemoteLog()
                {
                    ErrorCode = ((int)ErrorCode.GPPSDKInitFailed).ToString(),
                    Log = ex.Message,
                    StackTrace = ex.StackTrace
                };
                DatadogManager.ReportLog(remoteLog);
                    
                callback.TryError(ErrorCode.GPPSDKInitFailed, $"error : {ex.Message}");
            }
        }

        public static void Login(ResultCallback<GppUser> callback, PlatformType platformType = PlatformType.None, bool isHeadless = false)
        {
            var requestData = new LoginRequestParam
            {
                callback = callback,
                platformType = platformType,
                isHeadless = isHeadless
            };
            Login(requestData);
        }

        public static void Login(LoginRequestParam requestParam)
        {
            GetSession().SetLoginType(LoginSession.LoginType.manual);
            InternalLoginLogic(requestParam);
        }

        public static void LoginWithCustomProviderId(ResultCallback<GppUser> callback, string credential, string platformId = null)
        {
            var customProviderIdParam = new CustomProviderIdParam()
            {
                credential = credential,
                platformId = platformId
            };
            var requestParam = new LoginRequestParam
            {
                callback = callback,
                platformType = PlatformType.CustomProviderId,
                customProviderIdParam = customProviderIdParam
            };
            Login(requestParam);
        }

        public static void LoginWithCustomProviderId(ResultCallback<GppUser> callback, CustomProviderIdParam customProviderIdParam)
        {
            var requestParam = new LoginRequestParam
            {
                callback = callback,
                platformType = PlatformType.CustomProviderId,
                customProviderIdParam = customProviderIdParam
            };
            Login(requestParam);
        }

        public static void Logout(ResultCallback callback)
        {
            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.Logout);
            GppAuth.Logout(result =>
                {
                    if (!result.IsError)
                    {
                        if (FirebasePushExt.CanUse())
                        {
                            FirebasePushExt.Impl().DeletePushToken();
                        }
                        GetSession().ClearSession();
                        GppUI.ShowToast(LocalizationKey.NotificationLoggedOutTitle.Localise(), LocalizationKey.NotificationLoggedOutContent.Localise(), GppToastPosition.TOP);
                        callback?.TryOk();
                    }
                    else
                    {
                        callback?.TryError(result.Error);
                    }
                }
            );
        }

        public static void GetKidIdToken(ResultCallback<KidIdTokenResult> callback)
        {
            if (!IsInitialized)
            {
                callback.TryError(ErrorCode.NotInitialized);
                return;
            }
            if (!IsLoggedIn)
            {
                callback.TryError(ErrorCode.NotLoggedIn);
                return;
            }
            GppAuth.GetKidIdToken(callback);
        }

        public static void GetKIDUserInfoWebURL(ResultCallback<KIDUserInfoWebURLData> callback, KIDUserInfoUri userInfoUri = null)
        {
            GppAuth.GetKIDUserInfoWebURL(userInfoUri, callback);
        }

        public static void CheckAppUpdate(ResultCallback<DcsModels.AppUpdateInfoResult> callback, bool withUI = true)
        {
            if (!IsInitialized)
            {
                callback.TryError(ErrorCode.NotInitialized);
                return;
            }

            GetDcsApi().CheckAppUpdate(result =>
            {
                if (result.IsError)
                {
                    callback.TryError(result.Error);
                    return;
                }

                var appUpdateResultValue = result.Value;
                DcsModels.AppUpdateInfoResult updateResult = new DcsModels.AppUpdateInfoResult
                {
                    Mandatory = appUpdateResultValue.Mandatory,
                    Message = appUpdateResultValue.Message,
                    Url = appUpdateResultValue.Url,
                    UpdateAvailable = appUpdateResultValue.UpdateAvailable,
                    LatestVersion = appUpdateResultValue.LatestVersion,
                    ShowOptionalUpdate = appUpdateResultValue.ShowOptionalUpdate
                };

                // Gpp Common UI를 사용하지 않기 때문에 결과 리턴
                if (!withUI)
                {
                    callback.TryOk(updateResult);
                    return;
                }

                // Gpp Common UI를 사용하는 경우
                // Update 사용 불가 상태이기 때문에 바로 리턴
                if (!result.Value.UpdateAvailable)
                {
                    updateResult.UserActionType = DcsModels.AppUpdateUserAction.None;
                    callback.TryOk(updateResult);
                    return;
                }

                var uiData = BuildUIModalData(result.Value);
                if (result.Value.Mandatory)
                {
                    uiData.SetNegativeButtonText(LocalizationKey.GeneralExit.Localise());
                    uiData.SetNegativeAction(modal =>
                        {
                            Object.Destroy(modal);
                            updateResult.UserActionType = DcsModels.AppUpdateUserAction.MandatoryCancel;
                            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.UpdateClose);
                            callback.TryOk(updateResult);
                        }
                    );
                    GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.ExposeUpdateMandatory);
                    GppUI.ShowModal(uiData.Build());
                }
                else
                {
                    if (ShouldOfferOptionalUpdate(result.Value))
                    {
                        uiData.SetNegativeButtonText(LocalizationKey.GeneralLater.Localise());
                        uiData.SetNegativeAction(modal =>
                            {
                                Object.Destroy(modal);
                                updateResult.UserActionType = DcsModels.AppUpdateUserAction.OptionalLater;
                                GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.UpdateLater);
                                GppPrefsUtil.SaveOptionalUpdateVersion(updateResult.LatestVersion);
                                callback.TryOk(updateResult);
                            }
                        );
                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.ExposeUpdateOptional);
                        GppUI.ShowModal(uiData.Build());
                    }
                    else
                    {
                        updateResult.UserActionType = DcsModels.AppUpdateUserAction.None;
                        callback.TryOk(updateResult);
                    }
                }
            });
        }

        public static void SetGameServerId(string gameServerId, ResultCallback<GppUser> callback)
        {
            CheckGameServersMaintenanceInternal(gamesServerMaintenanceResult =>
            {
                if (gamesServerMaintenanceResult.IsError)
                {
                    callback.TryError(gamesServerMaintenanceResult.Error);
                    return;
                }

                if (gamesServerMaintenanceResult.Value.WhiteList)
                {
                    SetLoginCallback(callback);
                    GppAuth.SetGameServerId(gameServerId, result =>
                    {
                        if (result.IsError)
                        {
                            callback.TryError(result.Error);
                            return;
                        }
                        SetGameServerIdBeforeLogin(gameServerId);
                        TransformAndCallbackLoginResult(result, false);
                    });
                    return;
                }

                if (gamesServerMaintenanceResult.Value.Maintenances.Any(maintenance =>
                        maintenance.Name == gameServerId))
                {
                    GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.GameServerIdValidate);
                    var gameServerMaintenance = gamesServerMaintenanceResult.Value.Maintenances
                        .Where(maintenance =>
                        {
                            return maintenance.Name == gameServerId;
                        })
                        .First();

                    if (gameServerMaintenance.UnderMaintenance == false)
                    {
                        SetLoginCallback(callback);
                        GppAuth.SetGameServerId(gameServerId, result =>
                        {
                            if (result.IsError)
                            {
                                callback.TryError(result.Error);
                                return;
                            }
                            SetGameServerIdBeforeLogin(gameServerId);
                            TransformAndCallbackLoginResult(result, false);
                        });
                    }
                    else
                    {
                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.ExposeGameServerMaintenance);
                        GppUI.ShowMaintenance(gameServerMaintenance.Maintenance, uiObject =>
                        {
                            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.GameServerMaintenanceConﬁrm);
                            Object.Destroy(uiObject);
                            callback.TryError(ErrorCode.GameServerMaintenance);
                        });
                    }
                }
                else
                {
                    callback.TryError(ErrorCode.GameServerNotFound);
                }
            });
        }

        public static void CheckGameServersMaintenance(ResultCallback<GameServerMaintenanceResult> callback)
        {
            GetBasicApi().GetGameServersMaintenance(callback);
        }

        public static void CheckMaintenance(ResultCallback<MaintenanceResult> callback)
        {
            if (!IsInitialized)
            {
                callback.TryError(ErrorCode.NotInitialized);
                return;
            }

            GetBasicApi().GetMaintenance(result =>
            {
                var maintenanceResult = new MaintenanceResult();
                if (result.IsError)
                {
                    if (result.Error.Code == ErrorCode.NotFound)
                    {
                        // 404 점검 아님
                        maintenanceResult.IsMaintenance = false;
                        callback.TryOk(maintenanceResult);
                    }
                    else
                    {
                        // 기타 에러
                        callback.TryError(result.Error);
                    }
                }
                else
                {
                    // 점검 있음
                    maintenanceResult.IsMaintenance = true;
                    maintenanceResult.Maintenance = result.Value;
                    callback.TryOk(maintenanceResult);
                }
            });
        }

        public static void LinkKraftonId(PlatformType platformType, ResultCallback<GppUser> callback)
        {
            if (GetSession() is null)
            {
                callback.TryError(ErrorCode.NotLoggedIn);
                return;
            }
            if (GetSession().cachedTokenData.IsFullKid)
            {
                callback.TryError(ErrorCode.AlreadyLinked);
                return;
            }

            GetSession().SetLoginType(LoginSession.LoginType.link);
            if (PlatformUtil.IsPc() || PlatformUtil.IsConsole())
            {
                GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.KidLinkTry, new Dictionary<string, object>
                {
                    {"login_type", GetSession().GetLoginType().ToString()},
                    {"login_method", platformType.ToString().ToLower()}
                });
                var requestParam = new LoginRequestParam();
                requestParam.platformType = platformType;
                requestParam.callback = callback;
                requestParam.isHeadless = false;
                requestParam.tryLink = true;
                InternalLoginLogic(requestParam);
            }
            else
            {
                GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.KidLinkTry, new Dictionary<string, object>
                {
                    {"login_type", GetSession().GetLoginType().ToString()},
                    {"login_method", platformType.ToString().ToLower()}
                });
                if (platformType == PlatformType.None)
                {
                    GppUI.ShowLink((linkUI, type) =>
                    {
                        GppUI.DismissUI(UIType.MobileLogin);
                        GppAuth.LinkKraftonId(type, callback);
                    }, linkUI =>
                    {
                        GppUI.DismissUI(UIType.MobileLogin);
                        callback.TryError(ErrorCode.UserCancelled);
                    });
                }
                else
                {
                    GppAuth.LinkKraftonId(platformType, callback);
                }
            }
        }

        public static void DeleteAccount(ResultCallback callback)
        {
            GppTelemetry.SendDeleteAccount(GppClientLogModels.EntryStep.TryAccountDeletionRequest);
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
            GppAuth.DeleteAccount(PlatformType.AppleMac, callback.Try);       
#else
            GppAuth.DeleteAccount(callback.Try);
#endif
        }

        public static void SendGameTelemetryImmediately(string eventName, Dictionary<string, object> payload = null, ResultCallback callback = null)
        {
            SendGameTelemetry(eventName, payload, callback, true);
        }

        public static void SendGameTelemetry(string eventName, Dictionary<string, object> payload = null, ResultCallback callback = null, bool immediately = false)
        {
            GppSyncContext.RunOnUnityThread(() =>
            {
                GameTelemetryBody telemetryBody = GameTelemetryBody.CreateDefault();
                if (telemetryBody is null)
                {
                    GppLog.LogWarning("telemetryBody is null");
                    return;
                }
                telemetryBody.LogType = eventName;
                telemetryBody.Payload = payload;
                GetTelemetryManager().Send(TelemetryType.GameTelemetry, telemetryBody, callback, immediately);
            });
        }

        /// <summary>
        /// Immediately sends telemetry that remains in the GPP SDK's Telemetry job queue
        /// <example>
        /// See the examples below.
        /// <code>
        /// public static void SampleFlushTelemetry()
        /// {
        ///     GppSDK.FlushTelemetry();
        /// }
        /// </code>
        /// </example>
        /// </summary>
        public static void FlushTelemetry()
        {
            GetTelemetryManager().Flush();
        }

        public static void SendGameNoAuthTelemetryImmediately(string eventName, Dictionary<string, object> payload = null, ResultCallback callback = null)
        {
            SendGameNoAuthTelemetry(eventName, payload, callback, true);
        }

        public static void SendGameNoAuthTelemetry(string eventName, Dictionary<string, object> payload = null, ResultCallback callback = null, bool immediately = false)
        {
            GppSyncContext.RunOnUnityThread(() =>
            {
                GameTelemetryBody telemetryBody = GameTelemetryBody.CreateDefault();
                if (telemetryBody is null)
                {
                    GppLog.LogWarning("telemetryBody is null");
                    return;
                }
                telemetryBody.LogType = eventName;
                telemetryBody.Payload = payload;
                GetTelemetryManager().Send(TelemetryType.GameNoAuthTelemetry, telemetryBody, callback, immediately);
            });
        }

        public static bool GetNightPushStatus()
        {
            return GetSession().cachedTokenData.PushEnabled && GetSession().cachedTokenData.NightPushEnabled;
        }

        public static void SetNightPushStatus(bool isEnable, Action<bool> callback)
        {
            if (!GetSession().cachedTokenData.PushEnabled)
            {
                callback?.Invoke(false);
                return;
            }
            GetBasicApi().SetNightPushStatus(isEnable, result =>
                {
                    if (result.IsError)
                    {
                        callback?.Invoke(isEnable);
                    }
                    else
                    {
                        GppUI.ShowPushToast(true, isEnable);
                        GetSession().cachedTokenData.NightPushEnabled = result.Value.pushNightEnable;
                        callback?.Invoke(result.Value.pushNightEnable);
                    }
                }
            );
        }

        public static bool GetPushStatus()
        {
            return GetSession().cachedTokenData.PushEnabled;
        }

        public static void SetPushStatus(bool isEnable, Action<bool> callback)
        {
            if (GetSession().cachedTokenData.PushEnabled == isEnable)
            {
                callback?.Invoke(isEnable);
                return;
            }
            GetBasicApi().SetPushStatus(isEnable, result =>
                {
                    if (result.IsError)
                    {
                        callback?.Invoke(isEnable);
                    }
                    else
                    {
                        GetSession().cachedTokenData.PushEnabled = result.Value.pushEnable;
                        GppUI.ShowPushToast(false, isEnable);
                        callback?.Invoke(result.Value.pushEnable);
                    }
                }
            );
            if (!isEnable && GetSession().cachedTokenData.NightPushEnabled)
            {
                GetBasicApi().SetNightPushStatus(false, result =>
                    {
                        if (!result.IsError)
                        {
                            GetSession().cachedTokenData.NightPushEnabled = result.Value.pushNightEnable;
                        }
                    }
                );
            }

        }

        public static void SetLanguageCode(string language)
        {
            GetSession().SetLanguageCode(language);
        }

        public static string GetLanguageCode()
        {
            return GetSession().LanguageCode;
        }

        public static void GetOffers(ResultCallback<CatalogOfferResponse> callback)
        {
            Kps.CatalogService.GetOffers(callback);
        }

        public static void PaymentHistory(ResultCallback callback)
        {
            Kps.PaymentService.PaymentHistory(callback);
        }

        public static void Checkout(string offerId, ResultCallback<CheckoutResponse> callback)
        {         
            Kps.PaymentService.Checkout(offerId, callback);
        }

        public static void GetProducts(List<string> productIds, ResultCallback<List<Extensions.IAP.Models.IapProduct>> callback)
        {
            if (MobileIapExt.CanUse())
            {
                MobileIapExt.Impl().GetProducts(productIds, callback);                
            }
            else
            {
                callback.TryError(ErrorCode.NotSupportPlatform);
            }
        }

        public static void GetProducts(ResultCallback<List<Extensions.IAP.Models.IapProduct>> callback)
        {
            if (PlatformUtil.IsMobileNotEditor())
            {
                if (MobileIapExt.CanUse())
                {
                    MobileIapExt.Impl().GetProducts(callback);
                }
                else
                {
                    callback.TryError(ErrorCode.NotSupportPlatform);
                }
            }
            else if (PlatformUtil.IsPc())
            {
                if (GetSession().AttemptingLoginPlatform == PlatformType.Steam)
                {
                    SteamExt.Impl().GetProducts(callback);
                }
                else if (GetSession().AttemptingLoginPlatform == PlatformType.EpicGames)
                {
                    EpicGamesExt.Impl().GetProducts(callback);
                }
                else
                {
                    callback.TryError(ErrorCode.NotSupportPlatform);
                }
            }
            else
            {
                callback.TryError(ErrorCode.NotSupportPlatform);
            }
        }

        public static void Purchase(string productId, ResultCallback<IapPurchase> callback)
        {
            if (PlatformUtil.IsMobileNotEditor())
            {
                if (MobileIapExt.CanUse())
                {
                    MobileIapExt.Impl().Purchase(productId, callback);
                }
                else
                {
                    callback.TryError(ErrorCode.NotSupportPlatform);
                }
            }
            else if (PlatformUtil.IsPc())
            {
                if (GetSession().AttemptingLoginPlatform == PlatformType.Steam)
                {
                    SteamExt.Impl().Purchase(productId, callback);
                }
                else if (GetSession().AttemptingLoginPlatform == PlatformType.EpicGames)
                {
                    EpicGamesExt.Impl().Purchase(productId, callback);
                }
                else
                {
                    callback.TryError(ErrorCode.NotSupportPlatform);
                }
            }
            else
            {
                callback.TryError(ErrorCode.NotSupportPlatform);
            }
        }

        public static void Purchase(string productId, ResultCallback<IapPurchase> callback, IapTestMode mode)
        {
            if (PlatformUtil.IsMobileNotEditor())
            {
                if (MobileIapExt.CanUse())
                {
                    MobileIapExt.Impl().Purchase(productId, callback, mode);
                }
                else
                {
                    callback.TryError(ErrorCode.NotSupportPlatform);
                }
            }
            else if (PlatformUtil.IsPc())
            {
                if (GetSession().AttemptingLoginPlatform == PlatformType.Steam)
                {
                    SteamExt.Impl().Purchase(productId, callback);
                }
                else if (GetSession().AttemptingLoginPlatform == PlatformType.EpicGames)
                {
                    EpicGamesExt.Impl().Purchase(productId, callback);
                }
                else
                {
                    callback.TryError(ErrorCode.NotSupportPlatform);
                }
            }
            else
            {
                callback.TryError(ErrorCode.NotSupportPlatform);
            }
        }

        public static void Restore(List<StoreReceipt> receiptList, ResultCallback<List<IapPurchase>> callback)
        {
            if (MobileIapExt.CanUse())
            {
                MobileIapExt.Impl().Restore(receiptList, callback);
            }
            else
            {
                callback.TryError(ErrorCode.NotSupportPlatform);
            }
        }

        public static void Restore(ResultCallback<List<IapPurchase>> callback)
        {
            if (MobileIapExt.CanUse())
            {
                MobileIapExt.Impl().Restore(callback);
            }
            else
            {
                callback.TryError(ErrorCode.NotSupportPlatform);
            }
        }

        public static void OpenLegalWithTag(LegalPolicyType legalPolicyType, string tag, ResultCallback callback)
        {
            if (string.IsNullOrEmpty(tag))
            {
                callback.TryError(ErrorCode.EmptyTermsTag);
                return;
            }

            GetLegalApi().GetLegalPoliciesByNamespaceAndCountryWithTags(legalPolicyType, new[] { tag }, result =>
                {
                    if (result.IsError)
                    {
                        callback.TryError(result.Error.Code, result.Error.Message);
                        return;
                    }

                    var policies = result.Value;
                    if (policies == null || policies.Length == 0)
                    {
                        callback.TryError(ErrorCode.MissingRequestedTerms);
                        return;
                    }

                    var policyIdList = new List<string>();

                    foreach (var policy in policies)
                    {
                        foreach (var version in policy.policyVersions)
                        {
                            LocalizedPolicyVersionObject[] localizedPolicyVersions = version.localizedPolicyVersions;
                            if (localizedPolicyVersions == null) continue;

                            LocalizedPolicyVersionObject versionObject = null;
                            if (localizedPolicyVersions.Length == 1)
                            {
                                if (localizedPolicyVersions.First().status.Equals("active", StringComparison.OrdinalIgnoreCase))
                                {
                                    policyIdList.Add(localizedPolicyVersions.First().id);
                                }
                            }
                            else
                            {
                                var activeVersionObjects = localizedPolicyVersions.Where(v => v.status.Equals("active", StringComparison.OrdinalIgnoreCase));
                                versionObject = activeVersionObjects.FirstOrDefault(v => v.localeCode.Equals(GppSDK.GetSession().PlayerLocale))
                                                ?? activeVersionObjects.FirstOrDefault(v => v.localeCode.Equals(GppSDK.GetSession().LanguageCode))
                                                ?? activeVersionObjects.FirstOrDefault(v => GppSDK.GetSession().CompareWithPlayerLanguage(v.localeCode))
                                                ?? activeVersionObjects.FirstOrDefault(v => v.isDefaultSelection);
                                policyIdList.Add(versionObject.id);
                            }
                        }
                    }

                    if (policyIdList.Count == 0)
                    {
                        callback.TryError(ErrorCode.MissingRequestedTerms);
                    }
                    else
                    {
                        OpenLegalWebView(policyIdList, callback);
                    }
                }
            );
        }

        public static void OpenHelpDesk(ResultCallback callback, bool isExternalBrowser = false)
        {
            GetIamApi().ExternalLink(result =>
                {
                    if (result.IsError)
                    {
                        callback.TryError(ErrorCode.LoadExternalUrlFailed, result.Error.Message);
                    }
                    else
                    {
                        if (isExternalBrowser)
                        {
                            Application.OpenURL(result.Value.RedirectTo);
                            callback.TryOk();
                        }
                        else
                        {
                            OpenWebView(result.Value.RedirectTo, callback.Try);
                        }
                    }
                }
            );
        }

        public static void GetSurveys(ResultCallback<List<Survey>> callback)
        {
            if (!IsInitialized)
            {
                callback.TryError(ErrorCode.NotInitialized);
                return;
            }

            if (!IsLoggedIn)
            {
                callback.TryError(ErrorCode.NotLoggedIn);
                return;
            }

            GetSocialApi().GetSurveysByUser(callback);
        }

        public static void GetSurveyByEventKey(string eventKey, ResultCallback<SurveyEventResult> callback)
        {
            if (!IsInitialized)
            {
                callback.TryError(ErrorCode.NotInitialized);
                return;
            }

            if (!IsLoggedIn)
            {
                callback.TryError(ErrorCode.NotLoggedIn);
                return;
            }

            GetSocialApi().GetSurveysByEventName(eventKey, callback);
        }

        public static void SendGoogleAnalyticsEvent(string eventName, Dictionary<string, object> payload)
        {
            if (GoogleAnalyticsExt.CanUse())
            {
                GoogleAnalyticsExt.Impl().SendEvent(eventName, payload);
            }
        }

        public static void CheckEligibility(ResultCallback<GppCheckEligibilityResult> callback)
        {
            GppAuth.CheckEligibility(callback);
        }

        public static string GetDeviceId(bool useStoredId = false)
        {
            return GppDeviceProvider.GetGppDeviceId(useStoredId);
        }

        public static void GetAccountDeletionConfig(ResultCallback<AccountDeletionConfig> callback)
        {
            GetBasicApi().GetAccountDeletionConfig(callback);
        }

        public static void GetEventCenterUrl(string eventId, ResultCallback<EventCenterUrl> callback, Dictionary<string, object> additionalData = null)
        {
            GetBasicApi().GetEventCenterUrl(eventId, callback, additionalData);
        }

        public static string GetInstallSource()
        {
            return GppNativeUtil.GetInstallSource();
        }

        public static void GetLastCreatorCode(ResultCallback<CreatorCodeResult> callback)
        {
            GppTelemetry.SendKcnSubmitProcess(GppClientLogModels.KcnEventCode.TryGetCode);
            GetKCNApi().GetLastCreatorCode(result =>
            {
#if STEAMWORKS_NET
                if (!result.IsError)
                {
                    GppTelemetry.SendKcnSubmitProcess(GppClientLogModels.KcnEventCode.TryGetCodeSuccess);
                    var creatorCodeResult = new CreatorCodeResult();

                    if (result.Value == null)
                    {
                        callback.TryOk(creatorCodeResult);
                        return;
                    }

                    creatorCodeResult.Code = result.Value.Code ?? null;
                    creatorCodeResult.IsExpired = result.Value.ExpiredAfter <= 0 && DateTime.Parse(result.Value.ExpireAt).Year != 1970;
                    creatorCodeResult.ExpireAfter = result.Value.ExpiredAfter;
                    creatorCodeResult.ExpireAt = DateTime.Now.AddSeconds(result.Value.ExpiredAfter);

                    if (result.Value.AssociatedPurchases == null)
                    {
                        callback.TryOk(creatorCodeResult);
                        return;
                    }

                    SteamExt.Impl().GetUserDLCAppIdList(dlcAppIdList =>
                    {
                        if (!dlcAppIdList.IsError)
                        {
                            GppTelemetry.SendKcnSubmitProcess(GppClientLogModels.KcnEventCode.AppIdLoadSuccess);
                            var appIdList = dlcAppIdList.Value ?? new List<string>();
                            if (string.IsNullOrEmpty(SteamExt.Impl().GetAppId()))
                            {
                                callback.TryError(ErrorCode.SteamGetDLCListFailed, "SteamAppId is empty");
                                return;
                            }
                            appIdList.Add(SteamExt.Impl().GetAppId());
                            if (appIdList != null)
                            {
                                foreach (var appId in appIdList)
                                {
                                    if (!result.Value.AssociatedPurchases.Contains(appId))
                                    {
                                        callback.TryOk(creatorCodeResult);
                                        return;
                                    }
                                }
                                callback.TryError(creatorCodeResult, ErrorCode.KCNNoCodeToInput);
                            }
                            else
                                callback.TryOk(creatorCodeResult);
                        }
                        else
                        {
                            GppTelemetry.SendKcnSubmitProcess(GppClientLogModels.KcnEventCode.AppIdLoadFail,
                                new Dictionary<string, object>
                                {
                                    { "error_code", $"{(int)dlcAppIdList.Error.Code}" },
                                });
                            callback.TryError(dlcAppIdList.Error);
                        }
                    });
                }
                else
                {
                    GppTelemetry.SendKcnSubmitProcess(GppClientLogModels.KcnEventCode.TryGetCodeFail,
                        new Dictionary<string, object>
                        {
                            { "error_code", $"{(int)result.Error.Code}" },
                        });
                    if (result.Value != null)
                    {
                        var creatorCodeResult = new CreatorCodeResult();
                        creatorCodeResult.Code = result.Value.Code ?? null;
                        callback.TryError(creatorCodeResult, result.Error.Code);
                        return;
                    }
                    else
                        callback.TryError(result.Error);
                    return;
                }
#else
                if (!result.IsError)
                {
                    GppTelemetry.SendKcnSubmitProcess(GppClientLogModels.KcnEventCode.TryGetCodeSuccess);
                    var creatorCodeResult = new CreatorCodeResult();
                    if (result.Value == null)
                    {
                        callback.TryOk(creatorCodeResult);
                        return;
                    }
                    creatorCodeResult.Code = result.Value.Code ?? null;
                    creatorCodeResult.IsExpired = result.Value.ExpiredAfter <= 0 && DateTime.Parse(result.Value.ExpireAt).Year != 1970;
                    creatorCodeResult.ExpireAfter = result.Value.ExpiredAfter;
                    creatorCodeResult.ExpireAt = DateTime.Now.AddSeconds(result.Value.ExpiredAfter);
                    callback.TryOk(creatorCodeResult);
                }
                else
                {
                    GppTelemetry.SendKcnSubmitProcess(GppClientLogModels.KcnEventCode.TryGetCodeFail,
                        new Dictionary<string, object>
                        {
                            { "error_code", $"{(int)result.Error.Code}" },
                        });
                    if (result.Value != null)
                    {
                        var creatorCodeResult = new CreatorCodeResult();
                        creatorCodeResult.Code = result.Value.Code ?? null;
                        callback.TryError(creatorCodeResult, result.Error.Code);
                    }
                    else
                        callback.TryError(result.Error);
                }
#endif
            });

        }

        public static void SetCreatorCode(string code, ResultCallback<CreatorCodeResult> callback)
        {
            if (string.IsNullOrEmpty(code))
            {
                callback.TryError(ErrorCode.EmptyKcnCode);
                return;
            }

            GppTelemetry.SendKcnSubmitProcess(GppClientLogModels.KcnEventCode.TrySetCode, new Dictionary<string, object>
            {
                { "creator_code", code }
            });

            GetKCNApi().GetLastCreatorCode(getCodeResult =>
            {
                if (getCodeResult.IsError)
                {
                    callback.TryError(getCodeResult.Error);
                    return;
                }

                if (getCodeResult.Value.ExpiredAfter > 0)
                {
                    callback.TryError(ErrorCode.KCNCodeAlreadySubmitted);
                    return;
                }

                if (SteamExt.CanUse())
                {
                    SteamExt.Impl().GetUserDLCAppIdList(dlcAppIdList =>
                    {
                        if (!dlcAppIdList.IsError)
                        {
                            GppTelemetry.SendKcnSubmitProcess(GppClientLogModels.KcnEventCode.AppIdLoadSuccess, new Dictionary<string, object>
                            {
                            { "creator_code", code }
                            });
                            var appIdList = dlcAppIdList.Value ?? new List<string>();
                            appIdList.Add(SteamExt.Impl().GetAppId());
                            if (string.IsNullOrEmpty(SteamExt.Impl().GetAppId()))
                            {
                                callback.TryError(ErrorCode.SteamGetDLCListFailed, "SteamAppId is empty");
                                return;
                            }
                            GetKCNApi().SetCreatorCode(code, appIdList.ToArray(), result =>
                            {
                                if (result.IsError)
                                {
                                    GppTelemetry.SendKcnSubmitProcess(GppClientLogModels.KcnEventCode.TrySetCodeFail,
                                        new Dictionary<string, object>
                                        {
                                            { "creator_code", code },
                                            { "error_code", $"{(int)result.Error.Code}" },
                                        });
                                    if (result.Value != null)
                                    {
                                        var creatorCodeResult = new CreatorCodeResult();
                                        creatorCodeResult.Code = result.Value.Code ?? null;
                                        callback.TryError(creatorCodeResult, result.Error.Code);
                                    }
                                    else
                                        callback.TryError(result.Error);
                                }
                                else
                                {
                                    GppTelemetry.SendKcnSubmitProcess(GppClientLogModels.KcnEventCode.TrySetCodeSuccess, new Dictionary<string, object>
                                    {
                                    { "creator_code", code }
                                    });
                                    var creatorCodeResult = new CreatorCodeResult();
                                    creatorCodeResult.Code = result.Value.Code ?? null;
                                    creatorCodeResult.IsExpired = result.Value.ExpiredAfter <= 0 && DateTime.Parse(result.Value.ExpireAt).Year != 1970;
                                    creatorCodeResult.ExpireAfter = result.Value.ExpiredAfter;
                                    creatorCodeResult.ExpireAt = DateTime.Now.AddSeconds(result.Value.ExpiredAfter);
                                    callback.TryOk(creatorCodeResult);
                                }
                            });
                        }
                        else
                        {
                            GppTelemetry.SendKcnSubmitProcess(GppClientLogModels.KcnEventCode.AppIdLoadFail,
                                new Dictionary<string, object>
                                {
                                    { "error_code", $"{(int)dlcAppIdList.Error.Code}" },
                                });
                            callback.TryError(dlcAppIdList.Error);
                        }
                    });
                }
                else if (MacExt.CanUse())
                {
                    string appId = MacExt.Impl().GetAppStoreAppId();

                    GppTelemetry.SendKcnSubmitProcess(GppClientLogModels.KcnEventCode.AppIdLoadSuccess, new Dictionary<string, object>
                    {
                        { "creator_code", code }
                    });

                    GetKCNApi().SetCreatorCode(code, new string[] { appId }, result =>
                    {
                        if (result.IsError)
                        {
                            GppTelemetry.SendKcnSubmitProcess(GppClientLogModels.KcnEventCode.TrySetCodeFail,
                                new Dictionary<string, object>
                                {
                                    { "creator_code", code },
                                    { "error_code", $"{(int)result.Error.Code}" }
                                });
                            if (result.Value != null)
                            {
                                var creatorCodeResult = new CreatorCodeResult();
                                creatorCodeResult.Code = result.Value.Code ?? null;

                                callback.TryError(creatorCodeResult, result.Error.Code);
                            }
                            else
                                callback.TryError(result.Error);
                        }
                        else
                        {
                            GppTelemetry.SendKcnSubmitProcess(GppClientLogModels.KcnEventCode.TrySetCodeSuccess, new Dictionary<string, object>
                                    {
                                    { "creator_code", code }
                                    });
                            var creatorCodeResult = new CreatorCodeResult();
                            creatorCodeResult.Code = result.Value.Code ?? null;
                            creatorCodeResult.IsExpired = result.Value.ExpiredAfter <= 0 && DateTime.Parse(result.Value.ExpireAt).Year != 1970;
                            creatorCodeResult.ExpireAfter = result.Value.ExpiredAfter;
                            creatorCodeResult.ExpireAt = DateTime.Now.AddSeconds(result.Value.ExpiredAfter);
                            callback.TryOk(creatorCodeResult);
                        }
                    });
                }
                else
                {
                    GetKCNApi().SetCreatorCode(code, new string[] { Application.identifier }, result =>
                    {
                        if (result.IsError)
                        {
                            if (result.Value != null)
                            {
                                var creatorCodeResult = new CreatorCodeResult();
                                creatorCodeResult.Code = result.Value.Code ?? null;
                                callback.TryError(creatorCodeResult, result.Error.Code);
                            }
                            else
                                callback.TryError(result.Error);
                        }
                        else
                        {
                            var creatorCodeResult = new CreatorCodeResult();
                            creatorCodeResult.Code = result.Value.Code ?? null;
                            creatorCodeResult.IsExpired = result.Value.ExpiredAfter <= 0 && DateTime.Parse(result.Value.ExpireAt).Year != 1970;
                            creatorCodeResult.ExpireAfter = result.Value.ExpiredAfter;
                            creatorCodeResult.ExpireAt = DateTime.Now.AddSeconds(result.Value.ExpiredAfter);
                            callback.TryOk(creatorCodeResult);
                        }
                    });
                }
            });
        }

        public static void GetDurableEntitlements(ResultCallback<Entitlements> callback)
        {
            GetPlatformApi().GetEntitlements(callback, true);
        }

        public static void GetConsumableEntitlements(ResultCallback<Entitlements> callback)
        {
            GetPlatformApi().GetEntitlements(callback, false);
        }

        public static void GetSteamAuthSessionTicket(ResultCallback<string> callback)
        {
            if (!SteamExt.CanUse())
            {
                GppLog.Log("SteamSDK is not available.");
                callback.TryError("", ErrorCode.SteamNotAvailable);
                return;
            }
            SteamExt.Impl().GetAuthSessionTicket(callback);
        }

        public static string GetSteamLatestAuthSessionTicket()
        {
            if (!SteamExt.CanUse())
            {
                GppLog.Log("SteamSDK is not available.");
                return "";
            }
            return SteamExt.Impl().GetLatestAuthSessionTicket();
        }

        public static void GetPublicDCSInfo(string key, DcsDataAccess dataAccess, ResultCallback<PublicDCSInfo> callback)
        {
            GetDcsApi().GetPublicDCSInfo(key, dataAccess, callback);
        }

        public static void AutoLogin(ResultCallback<GppUser> callback, Action<Error> onTokenError)
        {
            if (Instance._isLoginInProgress)
            {
                callback.TryError(ErrorCode.AlreadyLoginProgress);
                return;
            }
            Instance._isLoginInProgress = true;

            SetLoginCallback(result =>
            {
                try
                {
                    callback.Invoke(result);
                }
                finally
                {
                    Instance._isLoginInProgress = false;
                }
            });

            GetDcsApi().GetSdkConfig((configResult) =>
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
                    GppAuth.AutoLogin(error =>
                    {
                        Instance._isLoginInProgress = false;
                        onTokenError?.Invoke(error);
                    });
                }
            });
        }

        public static void SetGAUserProperty(string key, string value)
        {
            if (GoogleAnalyticsExt.CanUse())
            {
                GoogleAnalyticsExt.Impl().SetUserProperty(key, value);
            }
        }

        public static void SetGameServerIdBeforeLogin(string gameServerId)
        {
            GetSession().GameServerId = gameServerId;
        }

        public static void DeleteKCNCreatorCode(ResultCallback callback)
        {
            GetKCNApi().DeleteCreatorCode(callback);
        }

        public static void GetPushPermissionGranted(ResultCallback<PermissionState> callback)
        {
            if (FirebasePushExt.CanUse())
            {
                FirebasePushExt.Impl().GetPushPermissionGranted(callback);
            }
        }

        public static void OpenNotificationSetting()
        {
            if (FirebasePushExt.CanUse())
            {
                FirebasePushExt.Impl().OpenNotificationSetting();
            }
        }


        public static void RequestPushPermission(ResultCallback<PermissionState> callback)
        {
            if (FirebasePushExt.CanUse())
            {
                FirebasePushExt.Impl().GetPermission(callback);
            }
        }

        public static void SetInputController(InputControllerType controllerType)
        {
            GetInputController().SetControllerType(controllerType);
        }

        public static InputControllerType GetSDKInputController()
        {
            return GetInputController().GetControllerType();
        }

        public static void ConsumeAll(ResultCallback callback)
        {
            if (MobileIapExt.CanUse())
            {
                MobileIapExt.Impl().ConsumeAll(callback);
            }
            else
            {
                callback.TryError(ErrorCode.NotSupportPlatform);
            }
        }

        public static void GetUnconsumedProducts(ResultCallback<List<IapUnconsumedProduct>> callback)
        {
            if (MobileIapExt.CanUse())
            {
                MobileIapExt.Impl().GetUnconsumedProducts(callback);
            }
            else
            {
                callback.TryError(ErrorCode.NotSupportPlatform);
            }
        }
    }
}