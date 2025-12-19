using System;
using System.Collections.Generic;
using System.Linq;
using Gpp.CommonUI.Console;
using Gpp.CommonUI.GPGS;
using Gpp.CommonUI.GppRepay;
using Gpp.CommonUI.JapanPayment;
using Gpp.CommonUI.Legal;
using Gpp.CommonUI.Legal.Console;
using Gpp.CommonUI.Legal.PcLegal;
using Gpp.CommonUI.Login;
using Gpp.CommonUI.Maintenance;
using Gpp.CommonUI.Modal;
using Gpp.CommonUI.PcAccountCheck;
using Gpp.CommonUI.PcAuthWaiting;
using Gpp.CommonUI.Toast;
using Gpp.CommonUI.PcCheckEligibility;
using Gpp.Constants;
using Gpp.Core;
using Gpp.Localization;
using Gpp.Models;
using Gpp.Telemetry;
using Gpp.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Gpp.CommonUI
{
    internal static class GppUI
    {
        private static readonly Dictionary<UIType, GppUIProperty> GppPrefabs = new()
        {
            { UIType.MobileLogin, new GppUIProperty("GppCommonUI/Mobile/GppLoginUI") },
            { UIType.MobileLegal, new GppUIProperty("GppCommonUI/Mobile/GppLegalUI") },
            { UIType.Maintenance, new GppUIProperty("GppCommonUI/Common/GppMaintenanceUI") },
            { UIType.Modal, new GppUIProperty("GppCommonUI/Common/GppModalUI", true) },
            { UIType.Toast, new GppUIProperty("GppCommonUI/Common/GppToastUI", true) },
            { UIType.Survey, new GppUIProperty("GppCommonUI/Mobile/GppSurveyUI", true) },
            { UIType.PcSurvey, new GppUIProperty("GppCommonUI/Pc/GppPcSurveyUI", true) },
            { UIType.MobileJapanPayment, new GppUIProperty("GppCommonUI/Mobile/GppJapanPaymentUI") },
            { UIType.MobileGPGSPopup, new GppUIProperty("GppCommonUI/Mobile/GppGPGSPopupUI") },
            { UIType.PcLogin, new GppUIProperty("GppCommonUI/Pc/GppPcLoginUI") },
            { UIType.PcAuthWaiting, new GppUIProperty("GppCommonUI/Pc/GppPcAuthWaitingUI") },
            { UIType.PcAccountCheck, new GppUIProperty("GppCommonUI/Pc/GppPcAccountCheckUI") },
            { UIType.PcLegal, new GppUIProperty("GppCommonUI/Pc/GppPcLegalUI") },
            { UIType.ConsoleLogin, new GppUIProperty("GppCommonUI/Console/GppConsoleLoginUI") },
            { UIType.ConsoleAuthWaiting, new GppUIProperty("GppCommonUI/Console/GppConsoleAuthWaitingUI") },
            { UIType.ConsoleAccountCheck, new GppUIProperty("GppCommonUI/Console/GppConsoleAccountCheckUI") },
            { UIType.ConsoleLegal, new GppUIProperty("GppCommonUI/Console/GppConsoleLegalUI") },
            { UIType.ConsoleMaintenance, new GppUIProperty("GppCommonUI/Console/GppConsoleMaintenanceUI") },
            { UIType.RepayRequired, new GppUIProperty("GppCommonUI/Common/GppRepayUI") },
            { UIType.SelectMainGameUserId, new GppUIProperty("GppCommonUI/Common/GppSelectMainGameUserIdUI") },
            { UIType.PcCheckEligibility, new GppUIProperty("GppCommonUI/Pc/GppPcCheckEligibilityUI") },
            { UIType.PcMaintenance, new GppUIProperty("GppCommonUI/Pc/GppPcMaintenanceUI") },
            { UIType.ConsoleCheckEligibility, new GppUIProperty("GppCommonUI/Console/GppConsoleCheckEligibilityUI") },
        };

        public static void ShowLogin(Action<GameObject, PlatformType> onLogin, Action<GameObject> onClose)
        {
            GppSDK.GetSession().SetLoginType(LoginSession.LoginType.manual);
            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.ExposeLogin);
            ShowUI(UIType.MobileLogin, ui =>
                {
                    var uiComponent = ui.GetComponent<IGppLoginUI>();
                    uiComponent.SetOnClickLoginListener(onLogin);
                    uiComponent.SetOnClickCloseListener(onClose);
                }
            );
        }

        public static void ShowLink(Action<GameObject, PlatformType> onLink, Action<GameObject> onClose)
        {
            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.TryLink);
            ShowUI(UIType.MobileLogin, ui =>
                {
                    var uiComponent = ui.GetComponent<IGppLoginUI>();
                    uiComponent.SetOnClickLoginListener(onLink);
                    uiComponent.SetOnClickCloseListener(onClose);
                    var unUsedPlatformTypes = new PlatformType[] { PlatformType.Discord, PlatformType.Guest };
                    uiComponent.SetUnusedPlatformType(unUsedPlatformTypes);
                }
            );
        }

        public static void ShowLegal(Dictionary<string, GppLegalModel> policies, Action<GameObject, bool, Dictionary<string, GppLegalModel>> onSubmit, Action onViewDetail = null)
        {
            ShowUI(UIType.MobileLegal, ui =>
                {
                    var uiComponent = ui.GetComponent<IGppLegalUI>();
                    uiComponent.SetPolicies(policies, onViewDetail);
                    uiComponent.SetSubmitDelegate(onSubmit);
                }
            );
        }

        public static void ShowLegalLoginAnotherAccount(Dictionary<string, GppLegalModel> policies, Action<GameObject, bool, Dictionary<string, GppLegalModel>> onSubmit, Action<GameObject> onLoginAnotherAccount, Action onViewDetail = null)
        {
            ShowUI(UIType.MobileLegal, ui =>
                {
                    var uiComponent = ui.GetComponent<IGppLegalUI>();
                    uiComponent.SetPolicies(policies, onViewDetail);
                    uiComponent.SetSubmitDelegate(onSubmit);
                    uiComponent.SetLoginAnotherAccountDelegate(onLoginAnotherAccount);
                }
            );
        }

        public static void ShowPcLegal(Dictionary<string, GppLegalModel> policies, Action<GameObject, bool, Dictionary<string, GppLegalModel>> onSubmit, Action<GppLegalModel> onViewDetail, Action<GameObject> onClose)
        {
            if (GppSDK.GetInputController().IsGamePad())
            {
                ShowUI(UIType.ConsoleLegal, ui =>
                {
                    var uiComponent = ui.GetComponent<IGppConsoleLegalUI>();
                    uiComponent.SetPolicies(policies, onViewDetail);
                    uiComponent.SetSubmitDelegate(onSubmit);
                    uiComponent.SetCloseDelegate(onClose);
                });
            }
            else
            {
                ShowUI(UIType.PcLegal, ui =>
                {
                    var uiComponent = ui.GetComponent<IGppPcLegalUI>();
                    uiComponent.SetPolicies(policies, onViewDetail);
                    uiComponent.SetSubmitDelegate(onSubmit);
                    uiComponent.SetCloseDelegate(onClose);
                });
            }
        }

        public static void ShowMaintenance(Models.Maintenance maintenance, Action<GameObject> onConfirm)
        {
            if (GppSDK.GetInputController().IsGamePad())
            {
                ShowUI(UIType.ConsoleMaintenance, ui =>
                {
                    var uiComponent = ui.GetComponent<IGppConsoleMaintenanceUI>();
                    uiComponent.SetMaintenanceData(maintenance);
                    uiComponent.SetOnClickConfirmListener(onConfirm);
                });
            }
            else if (PlatformUtil.IsPc())
            {
                ShowUI(UIType.PcMaintenance, ui =>
                {
                    var uiComponent = ui.GetComponent<IGppPcMaintenanceUI>();
                    uiComponent.SetMaintenanceData(maintenance);
                    uiComponent.SetOnClickConfirmListener(onConfirm);
                });
            }
            else
            {
                ShowUI(UIType.Maintenance, ui =>
                {
                    var uiComponent = ui.GetComponent<IGppMaintenanceUI>();
                    uiComponent.SetMaintenanceData(maintenance);
                    uiComponent.SetOnClickConfirmListener(onConfirm);
                });
            }
        }

        public static void ShowJapanPayment(ViolationRule rule, Action<GameObject, string> onConfirm, Action<GameObject> onCancel)
        {
            ShowUI(UIType.MobileJapanPayment, ui =>
                {
                    var uiComponent = ui.GetComponent<IGppJapanPaymentUI>();
                    uiComponent.SetViolationRule(rule);
                    uiComponent.SetOnClickConfirmListener(onConfirm);
                    uiComponent.SetOnClickCancelListener(onCancel);
                }
            );
        }

        public static void ShowModal(GppModalData modalData)
        {
            ShowUI(UIType.Modal, ui =>
                {
                    var uiComponent = ui.GetComponent<IGppModalUI>();
                    uiComponent.SetModalData(modalData);
                }
            );
        }

        public static void ShowToast(GppToastMessage message)
        {
            GppSyncContext.RunOnUnityThread(() =>
                {
                    if (!GppUtil.TryFindGameObjectWithName("GppToastManager", out var toastManagerObj))
                    {
                        toastManagerObj = new GameObject("GppToastManager");
                        toastManagerObj.AddComponent<GppToastManager>();
                    }

                    var toastManager = toastManagerObj.GetComponent<GppToastManager>();
                    toastManager.ShowToast(message);
                }
            );
        }

        public static void ShowToast(string message, string subMessage = null, GppToastPosition position = GppToastPosition.BOTTOM, float displaySec = 2.0f)
        {
            var toastMessage = new GppToastMessage(message, subMessage, position, GetUIPrefab(UIType.Toast), displaySec);
            ShowToast(toastMessage);
        }

        public static void ShowSurvey(string title, string message, GppToastPosition position, float displaySec)
        {
            var surveyMessage = new GppToastMessage(title, message, position, GetUIPrefab(PlatformUtil.IsPc()?UIType.PcSurvey:UIType.Survey), displaySec);
            ShowToast(surveyMessage);
        }
        
        public static void ShowGpgPopup(KidInfoResult kidInfoResult, Action<GameObject, bool> onClick)
        {
            ShowUI(UIType.MobileGPGSPopup, ui =>
                {
                    var uiComponent = ui.GetComponent<IGppGPGSPopupUI>();
                    uiComponent.SetKidInfo(kidInfoResult);
                    uiComponent.SetOnClickListener(onClick);
                }
            );
        }

        public static void ShowPcLogin(PcConsoleAuthResult pcConsoleAuthResult, Action<GameObject> onClose, Action<GameObject> onExpired)
        {
            if (!GppSDK.GetInputController().IsGamePad())
                ShowUI(UIType.PcLogin, ui =>
                {
                    var uiComponent = ui.GetComponent<IGppPcLoginUI>();
                    uiComponent.SetOnClickCloseListener(onClose);
                    uiComponent.SetOnExpireListener(onExpired);
                    uiComponent.RefreshLoginUIData(pcConsoleAuthResult);
                });
            else
            {
                ShowUI(UIType.ConsoleLogin, ui =>
                {
                    var uiComponent = ui.GetComponent<IGppConsoleLoginUI>();
                    uiComponent.SetOnClickCancelListener(onClose);
                    uiComponent.SetOnExpireListener(onExpired);
                    uiComponent.RefreshLoginUIData(pcConsoleAuthResult);
                });
            }
        }

        public static void ShowPcAuthWaiting(bool isAccountCheck, Action<GameObject> onCancel)
        {
            if (!GppSDK.GetInputController().IsGamePad())
                ShowUI(UIType.PcAuthWaiting, ui =>
                {
                    var uiComponent = ui.GetComponent<IGppPcAuthWaitingUI>();
                    uiComponent.SetOnClickCancel(onCancel);
                    uiComponent.SetIsAccountCheck(isAccountCheck);
                });
            else
            {
                ShowUI(UIType.ConsoleAuthWaiting, ui =>
                {
                    var uiComponent = ui.GetComponent<IGppConsoleAuthWaitingUI>();
                    uiComponent.SetOnClickCancel(onCancel);
                    uiComponent.SetIsAccountCheck(isAccountCheck);
                });
            }
        }

        public static void ShowAccountCheck(string redirectUri, int expiresIn, Action<GameObject> onCheck, Action<GameObject> onCancel, Action<GameObject> onExpire)
        {
            if (!GppSDK.GetInputController().IsGamePad())
            {
                ShowUI(UIType.PcAccountCheck, ui =>
                {
                    var uiComponent = ui.GetComponent<IGppPcAccountCheckUI>();
                    uiComponent.SetOnClickCheck(onCheck);
                    uiComponent.SetOnClickCancel(onCancel);
                    uiComponent.SetOnExpireListener(onExpire);
                    uiComponent.SetRedirectUri(redirectUri, expiresIn);
                });
            }
            else
            {
                ShowUI(UIType.ConsoleAccountCheck, ui =>
                {
                    var uiComponent = ui.GetComponent<IGppConsoleAccountCheckUI>();
                    uiComponent.SetOnClickCheck(onCheck);
                    uiComponent.SetOnClickCancel(onCancel);
                    uiComponent.SetOnExpireListener(onExpire);
                    uiComponent.SetRedirectUri(redirectUri, expiresIn);
                });
            }
        }

        public static void ShowCheckEligibility(CheckEligibilityResult checkEligibilityResult, Action<GameObject> onCheck, Action<GameObject> onCancel, Action<GameObject> onExpire)
        {
            if (!GppSDK.GetInputController().IsGamePad())
            {
                ShowUI(UIType.PcCheckEligibility, ui =>
                {
                    var uiComponent = ui.GetComponent<IGppPcCheckEligibilityUI>();
                    uiComponent.SetOnClickCheck(onCheck);
                    uiComponent.SetOnClickCancel(onCancel);
                    uiComponent.SetOnExpireListener(onExpire);
                    uiComponent.SetCheckEligibilityData(checkEligibilityResult);
                });
            }
            else
            {
                ShowUI(UIType.ConsoleCheckEligibility, ui =>
                {
                    var uiComponent = ui.GetComponent<IGppConsoleCheckEligibilityUI>();
                    uiComponent.SetOnClickCheck(onCheck);
                    uiComponent.SetOnClickCancel(onCancel);
                    uiComponent.SetOnExpireListener(onExpire);
                    uiComponent.SetCheckEligibilityData(checkEligibilityResult);
                });
            }
        }

        public static void ShowRepayRequired(RefundRestriction[] refundRestrictions, Action onSuccess, Action<GameObject> onFail)
        {
            List<string> sameStoreProductIds = refundRestrictions
                .Where(x =>
                {
                    var storeType = GppSDK.GetOptions().Store;
                    if (x.repaymentInfo.store == "GOOGLE")
                        return storeType == StoreType.GooglePlayStore;
                    else if (x.repaymentInfo.store == "APPLE")
                        return storeType == StoreType.AppStore;
                    else if (x.repaymentInfo.store == "GALAXY")
                        return storeType == StoreType.GalaxyStore;
                    return false;
                })
                .Select(x => x.repaymentInfo.productId).ToList();

            GppSDK.GetProducts(sameStoreProductIds, result =>
            {
                ShowUI(UIType.RepayRequired, ui =>
                {
                    var uiComponent = ui.GetComponent<IGppRepay>();
                    uiComponent.SetFailRepayCallback(onFail);
                    uiComponent.SetSuccessRepayCallback(onSuccess);
                    uiComponent.SetRestrictionProducts(refundRestrictions, result.Value, result.IsError);
                });
            });
        }

        public static void ShowPushToast(bool isNight, bool isAccepted)
        {
            string agreeTimeText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").Replace('-', '/');
            string localizationKey = LocalizationManager.GetInAppMarketingText(isNight, isAccepted);
            string toastMessage = $"{LocalizationManager.Localise(localizationKey, GppSDK.GetSession().LanguageCode)} {agreeTimeText}";
            ShowToast(toastMessage);
        }

        public static void ShowSelectMainGameUserId(MultipleGameUserResult result, Action<SelectMainGameUserIdParam, GameObject> onSelect, Action<GameObject> onCancel)
        {
            ShowUI(UIType.SelectMainGameUserId, ui =>
            {
                var uiComponent = ui.GetComponent<IGppSelectMainGameUserIdUI>();
                uiComponent.Init(result, onSelect, onCancel);
            });
        }

        public static void DismissUI(UIType uiType)
        {
            GppSyncContext.RunOnUnityThread(() =>
                {
                    var prefab = GetUIPrefab(uiType);
                    var gameObject = GameObject.Find(prefab.name);
                    if (gameObject != null)
                    {
                        Object.Destroy(gameObject);
                    }
                }
            );
        }

        private static GameObject GetUIPrefab(UIType type)
        {
            var customUIPrefab = GppUIConfigSo.GetCustomUIPrefab(type);
            if (customUIPrefab != null)
            {
                return customUIPrefab;
            }

            var prefab = Resources.Load<GameObject>(GppPrefabs[type].PrefabPath);
            if (prefab == null)
            {
                throw new Exception($"Can't find prefab : {type}.");
            }

            return prefab;
        }

        private static GameObject InstantiateUIPrefab(UIType type)
        {
            var prefab = GetUIPrefab(type);
            var gameObject = GameObject.Find(prefab.name);
            if (gameObject != null && !GppPrefabs[type].AllowMultipleInstance)
            {
                return null;
            }

            gameObject = Object.Instantiate(prefab);
            gameObject.name = prefab.name;
            return gameObject;
        }

        private static void ShowUI(UIType type, Action<GameObject> setupUI)
        {
            GppSyncContext.RunOnUnityThread(() =>
                {
                    var customUIPrefab = GppUIConfigSo.GetCustomUIPrefab(type);
                    if (customUIPrefab != null)
                    {
                        GameObject gameObject = Object.Instantiate(customUIPrefab);
                        gameObject.name = customUIPrefab.name;
                        setupUI(gameObject);
                        return;
                    }
                    var ui = InstantiateUIPrefab(type);
                    if (ui != null)
                    {
                        setupUI(ui);
                    }
                }
            );
        }
    }
}