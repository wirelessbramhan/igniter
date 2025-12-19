using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Gpp.CommonUI;
using Gpp.CommonUI.Legal;
using Gpp.CommonUI.Login;
using Gpp.CommonUI.Modal;
using Gpp.CommonUI.Toast;
using Gpp.Constants;
using Gpp.Core;
using Gpp.Extension;
using Gpp.Extensions.EpicGames;
using Gpp.Extensions.FirebasePush;
using Gpp.Extensions.Steam;
using Gpp.Localization;
using Gpp.Log;
using Gpp.Models;
using Gpp.Telemetry;
using Gpp.Utils;
using Gpp.WebView;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Timers;
using Gpp.Network;
using Gpp.Extensions.GoogleSignIn;
using Gpp.Extensions.Mac;
using Gpp.Extensions.Xbox;
using Gpp.Extensions.XboxPc;
using Gpp.Extensions.Ps5;
using Gpp.Surveys;

namespace Gpp.Auth
{
    internal static class GppAuth
    {
        internal static void AutoLogin(Action<Error> onFail)
        {
            GppSDK.GetSession().SetLoginType(LoginSession.LoginType.auto);
            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.TryAutoLogin, new Dictionary<string, object>
            {
                {"login_type", GppSDK.GetSession().GetLoginType().ToString()}
            });
            if (GppTokenProvider.TryGetRefreshTokenData(out RefreshTokenData tokenData))
            {
                GppTokenProvider.TryGetLastLoginType(out var lastLoginType);
                GppSDK.GetSession().AttemptingLoginPlatform = lastLoginType;
                GppLog.Log("A valid refresh-token exist. Attempt to auto-login.");

                if (PlatformUtil.IsMobile())
                {
                    GppSDK.GetIamApi().AutoLogin(lastLoginType, tokenData.refresh_token, autoLoginResult =>
                        {
                            var callback = InternalLoginCallback(AutoLoginResultCallback(onFail), lastLoginType);
                            if (autoLoginResult.IsError)
                                callback?.TryError(autoLoginResult.Error);
                            else
                            {
                                GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.AutoLoginSuccess,
                                    new Dictionary<string, object>
                                    {
                                        { "login_type", GppSDK.GetSession().GetLoginType().ToString() }
                                    });
                                callback?.Invoke(autoLoginResult);
                            }
                        }
                );
                }
                else
                {
                    LoginMethodType loginMethodType = GppTokenProvider.GetLastLoginMethodType();

                    if (loginMethodType == LoginMethodType.None)
                    {
                        onFail.Invoke(new Error(ErrorCode.KidLoginError));
                    }
                    else
                    {
                        GppSDK.GetIamApi().PcAutoLogin(lastLoginType, loginMethodType == LoginMethodType.Headless, tokenData.refresh_token, autoLoginResult =>
                        {
                            var callback = InternalLoginCallback(AutoLoginResultCallback(onFail), lastLoginType);
                            if (autoLoginResult.IsError)
                                callback?.TryError(autoLoginResult.Error);
                            else
                            {
                                GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.AutoLoginSuccess,
                                    new Dictionary<string, object>
                                    {
                                        { "login_type", GppSDK.GetSession().GetLoginType().ToString() }
                                    });
                                callback?.Invoke(autoLoginResult);
                            }
                        });
                    }
                }
            }
            else
            {
                onFail.Invoke(new Error(ErrorCode.SdkRefreshTokenNotFound));
            }
        }

        private static ResultCallback AutoLoginResultCallback(Action<Error> onFail)
        {
            return result =>
            {
                if (result.IsError)
                {
                    GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.AutoLoginFailed,
                        new Dictionary<string, object>
                        {
                            { "login_type", GppSDK.GetSession().GetLoginType().ToString() },
                            { "error_code", result.Error.Code.ToString() },
                            { "error_message", result.Error.Message }
                        });
                    if (result.Error.Code == ErrorCode.UnderMaintenance)
                    {
                        GppSDK.TransformAndCallbackLoginResult(result);
                        return;
                    }

                    GppLog.Log(
                        $"Unable to auto-login. Show the login select view: {result.Error.ToPrettyJsonString()}");
                    onFail.Invoke(result.Error);
                }
                else
                {
                    GppLog.Log("Auto login succeeded, pass the result.");
                    GppSDK.TransformAndCallbackLoginResult(result);
                }
            };
        }

        private static void GetDeviceToken(ResultCallback<string> callback)
        {
            callback.TryOk(GppTokenProvider.TryGetGuestUserId(out var guestUserId) ? guestUserId : GppTokenProvider.CreateGuestUserId());
        }

        private static void GetGuestToken(ResultCallback<string> callback, bool skipInfoUi = false)
        {
            if (GppTokenProvider.TryGetGuestUserId(out var guestUserId))
            {
                callback.TryOk(guestUserId);
            }
            else
            {
                if (skipInfoUi)
                {
                    callback.TryOk(GppTokenProvider.CreateGuestUserId());
                    return;
                }
                GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.ExposeGuestInfo);
                GppModalData model = new GppModalData.Builder()
                    .SetUIPriority(201)
                    .SetTitle(LocalizationKey.GuestSignInTitle.Localise())
                    .SetMessage(LocalizationKey.GuestSignInContent.Localise())
                    .SetPositiveButtonText(LocalizationKey.GuestSignInButtonText.Localise())
                    .SetPositiveAction(modal =>
                        {
                            callback.TryOk(GppTokenProvider.CreateGuestUserId());
                            Object.Destroy(modal);
                        }
                    )
                    .SetNegativeButtonText(LocalizationKey.GeneralCancel.Localise())
                    .SetNegativeAction(modal =>
                    {
                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.CloseGuestInfo);
                        callback.TryError(ErrorCode.SdkGuestPopupCancel);
                        Object.Destroy(modal);
                    })
                    .SetBackButtonAction(modal =>
                    {
                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.CloseGuestInfo);
                        callback.TryError(ErrorCode.SdkGuestPopupCancel);
                        Object.Destroy(modal);
                    })
                    .Build();

                GppUI.ShowModal(model);
            }
        }

        private static void AuthorizeKidPcConsole(ResultCallback<string> callback, string platformToken, PlatformType platformType = PlatformType.Steam, bool isHeadless = false)
        {
            if (!isHeadless)
                GppSDK.GetIamApi().AuthorizePcConsole(platformType, platformToken, result =>
                {
                    if (!result.IsError)
                    {
                        if (string.IsNullOrEmpty(result.Value.Code))
                        {
                            //GppSDK.GetSession().ClearSession(true);
                            //GppTokenProvider.DeleteUserId();
                            ConnectAuthWebSocket(platformType, result.Value.WebSocketUri, result.Value, platformToken, callback, isHeadless);
                        }
                        else
                        {
                            GppSDK.GetSession().Email = string.IsNullOrEmpty(result.Value.Email) ? result.Value.KTag : result.Value.Email;
                            callback.TryOk(result.Value.Code);
                        }
                    }
                    else
                    {
                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.PlatformServerOauthFailed);
                        callback.TryError(result.Error);
                    }
                });
            else
                GppSDK.GetIamApi().AuthorizePcConsoleHeadless(platformType, platformToken, result =>
                {
                    if (!result.IsError)
                    {
                        if (string.IsNullOrEmpty(result.Value.Code))
                        {
                            //GppSDK.GetSession().ClearSession(true);
                            //GppTokenProvider.DeleteUserId();
                            ConnectAuthWebSocket(platformType, result.Value.WebSocketUri, result.Value, platformToken, callback, isHeadless);
                        }
                        else
                        {
                            GppSDK.GetSession().Email = string.IsNullOrEmpty(result.Value.Email) ? result.Value.KTag : result.Value.Email;
                            callback.TryOk(result.Value.Code);
                        }
                    }
                    else
                    {
                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.PlatformServerOauthFailed);
                        callback.TryError(result.Error);
                    }
                });
        }

        private static void ConnectAuthWebSocket(PlatformType platformType, string webSocketUrl, PcConsoleAuthResult data, string platformToken, ResultCallback<string> callback, bool isHeadless = false)
        {
            if (string.IsNullOrEmpty(webSocketUrl))
            {
                callback.TryError(ErrorCode.AuthWebsocketConnectFailed);
                return;
            }

            var webSocket = new AuthWebSocket
            {
                onConnected = () =>
                {
                    GppSDK.GetSession().SetLoginType(LoginSession.LoginType.manual);
                    GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.ExposeLoginQr, new Dictionary<string, object>
                    {
                        {"kid_status", data.ErrorCode == 10163 ? "hidden" : "clear"}
                    });
                    GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.WebSockerConnect, new Dictionary<string, object>
                    {
                        {"status", "True"}
                    });
                    GppUI.ShowPcLogin(data, ui =>
                        {
                            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.LoginQrClose);
                            GppSDK.GetAuthWebSocket()?.Close();
                            callback.TryError(ErrorCode.UserCancelled);
                            Object.Destroy(ui);
                        }
                      , ui =>
                        {
                            GppSyncContext.RunOnUnityThread(() =>
                                {
                                    GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.WebQrLoginFailed);
                                    Object.Destroy(ui);
                                    GppSDK.GetAuthWebSocket()?.Close();
                                    callback.TryError(ErrorCode.AuthSessionExpired);
                                    // pc, console에서 qr refresh가 없어지고 만료 처리 시킴 [KOS-992]
                                    //{
                                    //    GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.RefreshQr);
                                    //    Object.Destroy(ui);
                                    //    GppSDK.GetAuthWebSocket()?.Close();
                                    //    AuthorizeKidPcConsole(callback, platformToken, platformType, isHeadless);
                                    //}
                                }
                            );
                        }
                    );
                },
                onConnectError = _ =>
                {
                    GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.WebSockerConnect, new Dictionary<string, object>
                    {
                        {"status", "False"}
                    });
                    callback.TryError(ErrorCode.AuthWebsocketConnectFailed);
                    GppSDK.GetAuthWebSocket()?.Close();
                },
                onReceive = result =>
                {
                    switch (result.Message)
                    {
                        case AuthMsgType.MSG_DEVICE_AUTH_GRANT_USER_CODE_CONSUMED:
                            {
                                if (GppSDK.GetInputController().IsGamePad())
                                    GppUI.DismissUI(UIType.ConsoleLogin);
                                else
                                    GppUI.DismissUI(UIType.PcLogin);
                                GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.ExposeWaitForLogin);
                                GppUI.ShowPcAuthWaiting(!string.IsNullOrEmpty(data.Code), ui =>
                                    {
                                        GppSyncContext.RunOnUnityThread(() =>
                                            {
                                                GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.WaitForLoginCancel);
                                                Object.Destroy(ui);
                                                GppSDK.GetAuthWebSocket()?.Close();
                                                AuthorizeKidPcConsole(callback, platformToken, platformType, isHeadless);
                                            }
                                        );
                                    }
                                );
                                break;
                            }
                        case AuthMsgType.MSG_DEVICE_AUTH_HAS_ISSUE:
                            {
                                GppUI.DismissUI(UIType.PcAuthWaiting);
                                GppSDK.GetAuthWebSocket()?.Close();
                                GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.WebQrLoginFailed);
                                switch (result.Code)
                                {
                                    case "100001":
                                        AuthorizeKidPcConsole(callback, platformToken, platformType, isHeadless);
                                        break;
                                    case "100002":
                                        callback.TryError(ErrorCode.KidLoginError, result.State);
                                        break;
                                }

                                break;
                            }
                        case AuthMsgType.MSG_DEVICE_AUTH_GRANT_COMPLETE:
                            {
                                GppSyncContext.RunOnUnityThread(() =>
                                    {
                                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.WebQrLoginComplete);
                                        if (GppSDK.GetInputController().IsGamePad())
                                            GppUI.DismissUI(UIType.ConsoleAuthWaiting);
                                        else
                                            GppUI.DismissUI(UIType.PcAuthWaiting);
                                        callback.TryOk(data.DeviceCode);
                                    }
                                );
                                break;
                            }
                        case AuthMsgType.MSG_USER_STATUS_HAS_ISSUE:
                            {
                                GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.WebQrLoginFailed);
                                break;
                            }
                        case AuthMsgType.MSG_USER_STATUS_RESOLVED:
                            {
                                break;
                            }
                    }
                }
            };

            GppSDK.SetAuthWebSocket(webSocket);
            webSocket.Connect(webSocketUrl);
        }

        private static void ConnectWebSocketForAccountCheck(PlatformType platformType, string webSocketUrl, ResultCallback callback, Action waitEvent, Action cancelEvent, bool continueLogin = true, bool isCheckEligibility = false)
        {
            if (string.IsNullOrEmpty(webSocketUrl))
            {
                GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.WebSockerConnect, new Dictionary<string, object>
                    {
                        {"status", "False"}
                    });
                callback.TryError(ErrorCode.AuthWebsocketConnectFailed);
                return;
            }

            var webSocket = new AuthWebSocket
            {
                onConnected = () =>
                {
                    GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.WebSockerConnect, new Dictionary<string, object>
                    {
                        {"status", "True"}
                    });
                },
                onConnectError = _ =>
                {
                    GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.WebSockerConnect, new Dictionary<string, object>
                    {
                        {"status", "False"}
                    });
                    callback.TryError(ErrorCode.AuthWebsocketConnectFailed);
                    GppSDK.GetAuthWebSocket()?.Close();
                },
                onReceive = result =>
                {
                    GppSyncContext.RunOnUnityThread(() =>
                    {
                        UIType accountCheckUIType = GppSDK.GetInputController().IsGamePad() ? UIType.ConsoleAccountCheck : UIType.PcAccountCheck;
                        UIType checkEligibilityType = GppSDK.GetInputController().IsGamePad() ? UIType.ConsoleCheckEligibility : UIType.PcCheckEligibility;
                        UIType waitingUIType = GppSDK.GetInputController().IsGamePad() ? UIType.ConsoleAuthWaiting : UIType.PcAuthWaiting;

                        switch (result.Message)
                        {
                            case AuthMsgType.MSG_DEVICE_AUTH_GRANT_USER_CODE_CONSUMED:
                            case AuthMsgType.MSG_USER_STATUS_CHECK_STARTED:
                                {
                                    GppUI.DismissUI(accountCheckUIType);
                                    GppUI.DismissUI(waitingUIType);
                                    GppUI.DismissUI(checkEligibilityType);

                                    GppSDK.GetLobby().Disconnect();
                                    waitEvent.Invoke();
                                    GppUI.ShowPcAuthWaiting(true, ui =>
                                    {
                                        cancelEvent.Invoke();
                                        Object.Destroy(ui);
                                        GppSDK.GetAuthWebSocket()?.Close();
                                        callback.TryError(ErrorCode.UserCancelled);
                                    });
                                    break;
                                }
                            case AuthMsgType.MSG_DEVICE_AUTH_HAS_ISSUE:
                            case AuthMsgType.MSG_USER_STATUS_HAS_ISSUE:
                                {
                                    GppUI.DismissUI(waitingUIType);
                                    GppSDK.GetAuthWebSocket()?.Close();
                                    GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.WebStatusCheckFailed);
                                    GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.WebQrLoginFailed);
                                    switch (result.Code)
                                    {
                                        case "100001":
                                            Login(platformType, callback, GppSDK.GetSession().AttemptingHeadlessLogin);
                                            break;
                                        case "100002":
                                            if (result.State == "age-restricted-access")
                                            {
                                                callback.TryError(ErrorCode.UserUnderage);
                                            }
                                            else
                                            {
                                                callback.TryError(ErrorCode.KidLoginError, result.State);
                                            }
                                            break;
                                        default:
                                            callback.TryError(ErrorCode.UserCancelled);
                                            break;
                                    }

                                    break;
                                }
                            case AuthMsgType.MSG_DEVICE_AUTH_GRANT_COMPLETE:
                                {
                                    if (isCheckEligibility)
                                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.EligibilityUpdateUserInfoTry, new Dictionary<string, object>
                                        {
                                            {"is_headless", GppSDK.GetSession().GetLoginFlowType() is LoginSession.LoginFlowType.headless }
                                        });
                                    else
                                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.WebQrLoginComplete);

                                    GppUI.DismissUI(accountCheckUIType);
                                    GppUI.DismissUI(waitingUIType);

                                    GppSDK.GetAuthWebSocket()?.Close();
                                    Uri uri = new Uri(webSocketUrl);
                                    NameValueCollection queryString = ParseQueryString(uri.Query);
                                    string deviceCode = queryString.Get("oidcDeviceCode") ?? "";
                                    if (!string.IsNullOrEmpty(deviceCode))
                                    {
                                        if (continueLogin)
                                        {
                                            if (isCheckEligibility)
                                            {
                                                callback += loginResult =>
                                                {
                                                    if (!loginResult.IsError)
                                                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.EligibilityUpdateUserInfoSuccess, new Dictionary<string, object>
                                                        {
                                                            {"is_headless", GppSDK.GetSession().GetLoginFlowType() is LoginSession.LoginFlowType.headless }
                                                        });
                                                    else
                                                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.EligibilityUpdateUserInfoFail, new Dictionary<string, object>
                                                        {
                                                            {"is_headless", GppSDK.GetSession().GetLoginFlowType() is LoginSession.LoginFlowType.headless }
                                                        });
                                                };
                                            }
                                            GppLogin(platformType, deviceCode, callback, false, GppSDK.GetSession().AttemptingHeadlessLogin);
                                        }
                                        else
                                        {
                                            callback.TryOk();
                                        }
                                    }
                                    else
                                    {
                                        callback.TryOk();
                                    }
                                    break;
                                }
                            case AuthMsgType.MSG_USER_STATUS_RESOLVED:
                                {
                                    GppUI.DismissUI(waitingUIType);
                                    GppSDK.GetAuthWebSocket()?.Close();
                                    GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.WebStatusCheckComplete);

                                    Uri uri = new Uri(webSocketUrl);
                                    NameValueCollection queryString = ParseQueryString(uri.Query);
                                    string deviceCode = queryString.Get("oidcDeviceCode") ?? "";
                                    if (!string.IsNullOrEmpty(deviceCode))
                                    {
                                        if (continueLogin)
                                        {
                                            GppLogin(platformType, deviceCode, callback, false, GppSDK.GetSession().AttemptingHeadlessLogin);
                                        }
                                        else
                                        {
                                            callback.TryOk();
                                        }
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(result.Code))
                                            callback.TryOk();
                                        else
                                            GppLogin(platformType, result.Code, callback, false, GppSDK.GetSession().AttemptingHeadlessLogin);
                                    }
                                    break;
                                }
                        }
                    });
                }
            };

            GppSDK.SetAuthWebSocket(webSocket);
            webSocket.Connect(webSocketUrl);
        }

        private static void AuthorizeKidMobile(ResultCallback<string> callback, PlatformType platformType = PlatformType.Krafton, bool isLinking = false)
        {
            if (isLinking)
                GppSDK.GetLobby().Disconnect();

            if (platformType == PlatformType.Google && GoogleSignInExt.CanUse())
            {
                GoogleSignInExt.Impl().Login((response) =>
                {
                    if (response.ResponseObject == null)
                    {
                        if (isLinking)
                            GppSDK.GetLobby().Reconnect();

                        callback.TryError(ErrorCode.IDPLoginCancelled);
                    }
                    else
                    {
                        callback.TryOk(response.ResponseObject.IdToken);
                    }
                });
            }
            else
            {
                GppSDK.GetIamApi().AuthorizeMobile(result =>
                {
                    if (result.IsError)
                    {
                        callback.TryError(result.Error.Code, result.Error.Message);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(result.Value.Code))
                        {
                            // need add useUnSupportBrowser config
                            OpenWebViewForLogin(result.Value.RedirectTo, true, callback, platformType, isLinking);
                        }
                        else
                        {
                            callback.TryOk(result.Value.Code);
                        }
                    }
                }
              , platformType, isLinking);
            }
        }

        private static void OpenWebViewForLogin(string url, bool useUnSupportBrowser, ResultCallback<string> callback, PlatformType platformType, bool isLinking = false)
        {
            OpenWebViewForAuth(true, url, useUnSupportBrowser, result =>
                {
                    if (result.IsError)
                    {
                        if (isLinking)
                            GppSDK.GetLobby().Reconnect();

                        callback.TryError(result.Error);
                    }
                    else
                    {
                        callback.TryOk(result.Value.Code);
                    }
                },
                platformType
            );
        }

        private static void OpenWebViewForAuth(bool useCustomTab, string url, bool useUnSupportBrowser, ResultCallback<WebViewAuthResult> callback, PlatformType platformType)
        {
            if (string.IsNullOrEmpty(url))
            {
                callback.TryError(ErrorCode.InvalidWebUrl);
                return;
            }

            if (!useUnSupportBrowser && !GppWebView.CanUseKidSupportBrowser())
            {
                GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.ExposeRequestChromePopup,
                    new Dictionary<string, object>
                    {
                        { "browser_name", GppWebView.GetDefaultBrowserPackageName() }
                    });
                GppUI.DismissUI(UIType.MobileLogin);
                GppModalData modalData = new GppModalData.Builder()
                    .SetTitle(LocalizationKey.GeneralAlert.Localise())
                    .SetMessage(LocalizationKey.NotSupportBrowser.Localise())
                    .SetPositiveButtonText(LocalizationKey.GeneralConfirm.Localise())
                    .SetPositiveAction(ui =>
                        {
                            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.CloseRequestChromePopup);
                            Object.Destroy(ui);
                            callback.TryError(ErrorCode.UnsupportedBrowser);
                        }
                    )
                    .Build();
                GppUI.ShowModal(modalData);
                return;
            }

            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.ExposeBrowser,
                new Dictionary<string, object>
                {
                    { "login_method", platformType.ToString().ToLower() }
                });
            GppWebView.OpenGppWebView(useCustomTab, url, "kraftonsdk", useUnSupportBrowser, webViewResponse =>
                {
                    if (webViewResponse.ResultCode == GppWebViewResultCode.RESULT_NOT_SUPPORT_BROWSER)
                    {
                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.ExposeRequestChromePopup,
                            new Dictionary<string, object>
                            {
                                { "browser_name", GppWebView.GetDefaultBrowserPackageName() }
                            });
                        GppUI.DismissUI(UIType.MobileLogin);
                        GppModalData modalData = new GppModalData.Builder()
                            .SetTitle(LocalizationKey.GeneralAlert.Localise())
                            .SetMessage(LocalizationKey.NotSupportBrowser.Localise())
                            .SetPositiveButtonText(LocalizationKey.GeneralConfirm.Localise())
                            .SetPositiveAction(ui =>
                                {
                                    GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.CloseRequestChromePopup);
                                    Object.Destroy(ui);
                                    callback.TryError(ErrorCode.UnsupportedBrowser);
                                }
                            )
                            .Build();
                        GppUI.ShowModal(modalData);
                        return;
                    }
                    GppLog.Log($"Received deep link uri: {webViewResponse.Response}");
                    JObject jsonObj = JObject.Parse(webViewResponse.Response);
                    string data = jsonObj["data"]?.ToString();
                    string browserPackageName = jsonObj["browserPackageName"]?.ToString() ?? "unknown";
                    string browserPackageVersion = jsonObj["browserVersion"]?.ToString() ?? "unknown";


                    if (webViewResponse.ResultCode == GppWebViewResultCode.RESULT_URL_OK)
                    {
                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.OpenBrowserComplete,
                            new Dictionary<string, object>
                            {
                                { "browser_version", browserPackageVersion},
                                { "browser_url", url},
                                { "browser_name", browserPackageName }
                            });
                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.ReceiveDeeplink);
                        var isValidScheme = !string.IsNullOrEmpty(data) && !string.IsNullOrWhiteSpace(data) &&
                                            data.Contains($"kraftonsdk://{GppSDK.GetConfig().Namespace.ToLower()}/auth?");
                        if (isValidScheme)
                        {
                            WebViewAuthResult loginResult = ParseUrl(data);
                            if (loginResult.IsError)
                            {
                                callback.TryError(ErrorCode.KidLoginError, loginResult.ErrorDescription);
                            }
                            else
                            {
                                callback.TryOk(loginResult);
                            }
                        }
                        else
                        {
                            callback.TryError(ErrorCode.KidLoginError, "Not supported scheme: " + data);
                        }
                    }
                    else if (webViewResponse.ResultCode == GppWebViewResultCode.RESULT_USER_DISMISS)
                    {
                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.OpenBrowserCancel);
                        callback.TryError(ErrorCode.KidLoginError, data);
                    }
                    else
                    {
                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.OpenBrowserFailed,
                            new Dictionary<string, object>
                            {
                                { "browser_version", browserPackageVersion },
                                { "browser_url", url },
                                { "browser_name", browserPackageName },
                                { "error_code", $"{(int)webViewResponse.ResultCode}" },
                                { "error_message", webViewResponse.Response }
                            });
                        callback.TryError(ErrorCode.KidLoginError, data);
                    }
                }
            );
        }

        private static NameValueCollection ParseQueryString(string query)
        {
            if (query.StartsWith("?"))
            {
                query = query.Substring(1);
            }

            var collection = new NameValueCollection();

            foreach (var part in query.Split('&'))
            {
                var pair = part.Split(new[] { '=' }, 2);
                var key = Uri.UnescapeDataString(pair[0]);
                var value = pair.Length > 1 ? Uri.UnescapeDataString(pair[1]) : string.Empty;

                collection.Add(key, value);
            }

            return collection;
        }

        private static WebViewAuthResult ParseUrl(string url)
        {
            var result = new WebViewAuthResult();
            var uri = new Uri(url);
            NameValueCollection queryString = ParseQueryString(uri.Query);
            result.Code = queryString.Get("code") ?? "";
            result.Error = queryString.Get("error") ?? "";
            result.ErrorDescription = queryString.Get("error_description") ?? "";
            return result;
        }

        private static void GetPlatformToken(PlatformType platformType, ResultCallback<string> callback, bool isHeadless = false)
        {
            if (platformType == PlatformType.Device)
            {
                GetDeviceToken(callback);
            }
            else if (PlatformUtil.IsPc())
            {
                if (platformType == PlatformType.Steam)
                {
                    SteamExt.Impl().GetAuthSessionTicket(steamTokenResult =>
                        {
                            if (!steamTokenResult.IsError)
                            {
                                GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.PlatformTokenAcquireSuccess, new Dictionary<string, object>
                                {
                                    {"platform_id", "steam"}
                                });
                                AuthorizeKidPcConsole(callback, steamTokenResult.Value, PlatformType.Steam, isHeadless);
                            }
                            else
                            {
                                GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.PlatformTokenAcquireFailed,
                                    new Dictionary<string, object>
                                    {
                                        { "platform_id", "steam" },
                                        { "error_code", $"{(int)steamTokenResult.Error.Code}" }
                                    });
                                callback.TryError(steamTokenResult.Error);
                            }
                        }
                    );
                }
                else if (platformType == PlatformType.EpicGames)
                {
                    EpicGamesExt.Impl().GetAuthSessionTicket(result =>
                    {
                        if (!result.IsError)
                        {
                            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.PlatformTokenAcquireSuccess, new Dictionary<string, object>
                            {
                                {"platform_id", "epicgames"}
                            });

                            AuthorizeKidPcConsole(callback, result.Value, PlatformType.EpicGames, isHeadless);
                        }
                        else
                        {
                            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.PlatformTokenAcquireFailed,
                                new Dictionary<string, object>
                                {
                                    { "platform_id", "epicgames" },
                                    { "error_code", $"{(int)result.Error.Code}" }
                                });

                            callback.TryError(result.Error);
                        }
                    });
                }
                else if (platformType == PlatformType.AppleMac)
                {
                    MacExt.Impl().Login(result =>
                    {
                        if (result.IsError)
                        {
                            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.PlatformTokenAcquireFailed,
                                new Dictionary<string, object>
                                {
                                    { "platform_id", "applemac" },
                                    { "error_code", $"{(int)result.Error.Code}" }
                                });

                            callback.TryError(result.Error);
                        }
                        else
                        {
                            var appleSignInResult = result.Value;
                            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.PlatformTokenAcquireSuccess, new Dictionary<string, object>
                                {
                                    {"platform_id", "applemac"}
                                });
                            AuthorizeKidPcConsole(callback, appleSignInResult.AuthorizationCode, platformType, isHeadless);
                        }
                    });
                }
                else if (platformType == PlatformType.live)
                {
                    XboxPcExt.Impl().GetToken(result =>
                    {
                        if (!result.IsError)
                        {
                            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.PlatformTokenAcquireSuccess, new Dictionary<string, object>
                            {
                                {"platform_id", "live"}
                            });

                            AuthorizeKidPcConsole(callback, result.Value, PlatformType.live, isHeadless);
                        }
                        else
                        {
                            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.PlatformTokenAcquireFailed,
                                new Dictionary<string, object>
                                {
                                    { "platform_id", "live" },
                                    { "error_code", $"{(int)result.Error.Code}" }
                                });

                            callback.TryError(result.Error);
                        }
                    });
                }
            }
            else if (PlatformUtil.IsConsole())
            {
                if (platformType == PlatformType.live)
                {
                    XboxExt.Impl().GetToken(result =>
                    {
                        if (!result.IsError)
                        {
                            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.PlatformTokenAcquireSuccess, new Dictionary<string, object>
                                {
                                {"platform_id", "live"}
                                });

                            AuthorizeKidPcConsole(callback, result.Value, PlatformType.live, isHeadless);
                        }
                        else
                        {
                            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.PlatformTokenAcquireFailed,
                                new Dictionary<string, object>
                                {
                                    { "platform_id", "live" },
                                    { "error_code", $"{(int)result.Error.Code}" }
                                });

                            callback.TryError(result.Error);
                        }
                    });
                }
                else if (platformType == PlatformType.PS5)
                {
                    Ps5Ext.Impl().GetToken(result =>
                    {
                        if (!result.IsError)
                        {
                            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.PlatformTokenAcquireSuccess, new Dictionary<string, object>
                                {
                                {"platform_id", "ps5"}
                                });

                            AuthorizeKidPcConsole(callback, result.Value, PlatformType.PS5, isHeadless);
                        }
                        else
                        {
                            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.PlatformTokenAcquireFailed,
                                new Dictionary<string, object>
                                {
                                    { "platform_id", "ps5" },
                                    { "error_code", $"{(int)result.Error.Code}" }
                                });

                            callback.TryError(result.Error);
                        }
                    });
                }
            }
            else
            {
                if (platformType == PlatformType.Guest)
                {
                    GetGuestToken(callback);
                }
                else
                {
                    AuthorizeKidMobile(callback, platformType);
                }
            }
        }

        internal static void Login(PlatformType platformType, ResultCallback callback, bool isHeadless = false)
        {
            GppSDK.GetSession().AttemptingLoginPlatform = platformType;
            GetPlatformToken(platformType, result =>
                {
                    if (result.IsError)
                    {
                        callback.TryError(result.Error);
                    }
                    else
                    {
                        GppLogin(platformType, result.Value, callback, false, isHeadless);
                    }
                },
                isHeadless
            );
        }

        internal static void LoginWithGuest(ResultCallback callback)
        {
            // 임의로 게스트를 지정한 로그인은 게스트 경고 UI를 표시하지 않습니다.
            GppSDK.GetSession().AttemptingLoginPlatform = PlatformType.Guest;
            GetGuestToken(result =>
            {
                if (result.IsError)
                {
                    callback.TryError(result.Error);
                }
                else
                {
                    GppLogin(PlatformType.Guest, result.Value, callback, false);
                }
            }, true);
        }

        internal static void LoginWithCustomProviderId(string credential, ResultCallback callback)
        {
            GppLogin(PlatformType.CustomProviderId, credential, callback);
        }

        internal static void GppLogin(PlatformType platformType, string authorizedCode, ResultCallback callback, bool fromWeb = false, bool isHeadless = false, string platformId = null)
        {
            GppSDK.GetSession().AttemptingLoginPlatform = platformType;
            if (!isHeadless)
                GppSDK.GetIamApi().GetToken(platformType, authorizedCode, InternalLoginCallback(callback, platformType), fromWeb, platformId);
            else
                GppSDK.GetIamApi().GetTokenHeadless(platformType, authorizedCode, InternalLoginCallback(callback, platformType), fromWeb, platformId);
        }

        internal static void GppLogin(PlatformType platformType, CustomProviderIdParam customProviderIdParam, ResultCallback callback, bool fromWeb = false, bool isHeadless = false)
        {
            GppSDK.GetSession().AttemptingLoginPlatform = platformType;
            if (!isHeadless)
                GppSDK.GetIamApi().GetToken(platformType, customProviderIdParam.credential, InternalLoginCallback(callback, platformType), fromWeb, customProviderIdParam.platformId);
            else
                GppSDK.GetIamApi().GetTokenHeadless(platformType, customProviderIdParam.credential, InternalLoginCallback(callback, platformType), fromWeb, customProviderIdParam.platformId);
        }

        internal static void GppLinkLogin(PlatformType platformType, string authorizedCode, ResultCallback callback, bool fromWeb = false)
        {
            GppSDK.GetSession().AttemptingLoginPlatform = platformType;
            if (GoogleSignInExt.CanUse() && platformType == PlatformType.Google)
            {
                GppLinkNativeLogin(platformType, authorizedCode, callback);
            }
            else
            {
                GppSDK.GetIamApi().GetToken(platformType, authorizedCode, InternalLoginCallback(callback, platformType, true), fromWeb);
            }
        }

        internal static void GppLinkNativeLogin(PlatformType platformType, string authorizedCode, ResultCallback callback)
        {
            GppSDK.GetIamApi().LinkAccount(platformType, authorizedCode, linkResult =>
            {
                if (linkResult.IsError)
                {
                    if (linkResult.Error.Code == ErrorCode.DuplicatedEmailFound)
                    {
                        GppModalData modalData = new GppModalData.Builder()
                            .SetTitle(LocalizationKey.LoginFailedTitle.Localise())
                            .SetMessage(LocalizationKey.LoginFailedCheckAccountStatus.Localise())
                            .SetPositiveButtonText(LocalizationKey.GeneralConfirm.Localise())
                            .SetPositiveAction(ui =>
                                {
                                    Object.Destroy(ui);
                                    callback.TryError(ErrorCode.DuplicatedEmailFound);
                                }
                            )
                            .Build();

                        GppUI.ShowModal(modalData);
                    }
                    else if (!string.IsNullOrEmpty(linkResult.Error.RedirectUri))
                    {
                        GppWebView.OpenGppWebView(false, linkResult.Error.RedirectUri, "kraftonsdk", false, webviewResult =>
                        {
                            if (webviewResult.ResultCode != GppWebViewResultCode.RESULT_URL_OK)
                            {
                                callback.TryError(linkResult.Error);
                            }
                            else
                            {
                                try
                                {
                                    string queryParamString = webviewResult.Response.Split('?')[1];
                                    var queryParams = queryParamString.Split('&');

                                    string code = queryParams.Where(query =>
                                    {
                                        return query.Contains("code");
                                    }).FirstOrDefault();
                                    code = code.Split('=')[1];

                                    GppLogin(PlatformType.Krafton, code, callback);
                                }
                                catch
                                {
                                    callback.TryError(linkResult.Error);
                                }
                            }
                        });
                    }
                    else
                    {
                        callback.TryError(linkResult.Error);
                    }
                }
                else
                {
                    callback.TryOk();
                }
            });
        }

        internal static void GppLoginAfterWeb(string authorizedCode, ResultCallback callback, bool isHeadless = false)
        {
            if (!isHeadless)
                GppSDK.GetIamApi().GetToken(PlatformType.Krafton, authorizedCode, InternalLoginCallback(callback, PlatformType.Krafton), true);
            else
                GppSDK.GetIamApi().GetTokenHeadless(PlatformType.Krafton, authorizedCode, InternalLoginCallback(callback, PlatformType.Krafton), true);
        }

        public static void ContinueLogin(ResultCallback callback)
        {
            PlatformType platformType = GppSDK.GetSession().AttemptingLoginPlatform;
            GppSDK.GetIamApi().AutoLogin(platformType, GppSDK.GetSession().RefreshToken, InternalLoginCallback(callback, platformType));
        }

        public static void ContinuePcLogin(PlatformType platformType, string refreshToken, ResultCallback callback)
        {
            GppTokenProvider.DeleteTokenData();
            GppSDK.GetIamApi().AutoLogin(platformType, refreshToken, InternalLoginCallback(callback, platformType));
        }

        private static ResultCallback InternalLoginCallback(ResultCallback callback, PlatformType platformType, bool isLinking = false)
        {
            return result =>
            {
                if (result.IsError)
                {
                    GppSDK.GetSession().Email = null;
                    ErrorCode errorCode = result.Error.Code;
                    string redirectUri = result.Error.RedirectUri;
                    string wsUri = result.Error.WsUri;

                    switch (errorCode)
                    {
                        case ErrorCode.AccountBan:
                            if (PlatformUtil.IsPc() || PlatformUtil.IsConsole())
                            {
                                GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.ExposeAccountStatus);
                                GppUI.ShowAccountCheck(redirectUri, result.Error.ExpiresIn, ui =>
                                    {
                                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.AccountStatusManualOpen);
                                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.ExposeBrowser);
                                        Application.OpenURL(redirectUri);
                                    }, ui =>
                                    {
                                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.AccountStatusClose);
                                        GppSyncContext.RunOnUnityThread(() =>
                                            {
                                                Object.Destroy(ui);
                                                GppSDK.GetSession().ClearSession();
                                                callback.TryError(ErrorCode.UserCancelled);
                                            }
                                        );
                                    }, ui =>
                                    {
                                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.WebQrLoginFailed);
                                        GppSyncContext.RunOnUnityThread(() =>
                                        {
                                            Object.Destroy(ui);
                                            GppSDK.GetSession().ClearSession();
                                            callback.TryError(ErrorCode.AuthSessionExpired);
                                        });
                                    }
                                );
                                ConnectWebSocketForAccountCheck(platformType, wsUri, callback, () =>
                                {
                                    GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.ExposeWaitForAccountCheck);
                                }, () =>
                                {
                                    GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.WaitForAccountCheckCancel);
                                });
                            }
                            else
                            {
                                // need add useUnSupportBrowser config
                                OpenWebViewForAuth(!(PlatformUtil.IsAndroid() && platformType == PlatformType.Google), redirectUri, true, _ =>
                                    {
                                        GppSDK.GetSession().ClearSession();
                                        callback.TryError(ErrorCode.UserBanned);
                                    },
                                    platformType
                                );
                            }

                            break;
                        case ErrorCode.GppAccountDeletion:
                            {
                                if (PlatformUtil.IsPc() || PlatformUtil.IsConsole())
                                {
                                    GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.ExposeAccountStatus);
                                    GppUI.ShowAccountCheck(redirectUri, result.Error.ExpiresIn, ui =>
                                    {
                                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.AccountStatusManualOpen);
                                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.ExposeBrowser);
                                        Application.OpenURL(redirectUri);
                                    }, ui =>
                                        {
                                            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.AccountStatusClose);
                                            GppSyncContext.RunOnUnityThread(() =>
                                                {
                                                    Object.Destroy(ui);
                                                    callback.TryError(ErrorCode.UserCancelled);
                                                }
                                            );
                                        }, ui =>
                                        {
                                            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.WebQrLoginFailed);
                                            GppSyncContext.RunOnUnityThread(() =>
                                            {
                                                Object.Destroy(ui);
                                                callback.TryError(ErrorCode.AuthSessionExpired);
                                            });
                                        }
                                    );
                                    ConnectWebSocketForAccountCheck(platformType, wsUri, callback, () =>
                                    {
                                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.ExposeWaitForAccountCheck);
                                    }, () =>
                                    {
                                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.WaitForAccountCheckCancel);
                                    });
                                }
                                else
                                {
                                    // need add useUnSupportBrowser config
                                    OpenWebViewForLogin(redirectUri, true, apAccountDeletionResult =>
                                        {
                                            if (apAccountDeletionResult.IsError)
                                            {
                                                callback.TryError(apAccountDeletionResult.Error);
                                            }
                                            else
                                            {
                                                GppLoginAfterWeb(apAccountDeletionResult.Value, callback);
                                            }
                                        },
                                        platformType
                                    );
                                }

                                break;
                            }
                        case ErrorCode.KraftonAccountDeletion:
                            {
                                if (PlatformUtil.IsPc() || PlatformUtil.IsConsole())
                                {
                                    GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.ExposeAccountStatus);
                                    GppUI.ShowAccountCheck(redirectUri, result.Error.ExpiresIn, ui =>
                                    {
                                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.AccountStatusManualOpen);
                                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.ExposeBrowser);
                                        Application.OpenURL(redirectUri);
                                    }, ui =>
                                        {
                                            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.AccountStatusClose);
                                            GppSyncContext.RunOnUnityThread(() =>
                                                {
                                                    Object.Destroy(ui);
                                                    callback.TryError(ErrorCode.UserCancelled);
                                                }
                                            );
                                        }, ui =>
                                        {
                                            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.WebQrLoginFailed);
                                            GppSyncContext.RunOnUnityThread(() =>
                                            {
                                                Object.Destroy(ui);
                                                callback.TryError(ErrorCode.AuthSessionExpired);
                                            });
                                        }
                                    );
                                    ConnectWebSocketForAccountCheck(platformType, wsUri, callback, () =>
                                    {
                                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.ExposeWaitForAccountCheck);
                                    }, () =>
                                    {
                                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.WaitForAccountCheckCancel);
                                    });
                                }
                                else
                                {
                                    // need add useUnSupportBrowser config
                                    OpenWebViewForLogin(redirectUri, true, accountDeletionResult =>
                                        {
                                            if (accountDeletionResult.IsError)
                                            {
                                                callback.TryError(accountDeletionResult.Error);
                                            }
                                            else
                                            {
                                                GppLoginAfterWeb(accountDeletionResult.Value, callback);
                                            }
                                        },
                                        platformType
                                    );
                                }

                                break;
                            }
                        case ErrorCode.UserUnderage:
                            {
                                if (PlatformUtil.IsPc() || PlatformUtil.IsConsole())
                                {
                                    GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.ExposeAccountStatus);
                                    GppUI.ShowAccountCheck(redirectUri, result.Error.ExpiresIn, ui =>
                                    {
                                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.AccountStatusManualOpen);
                                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.ExposeBrowser);
                                        Application.OpenURL(redirectUri);
                                    }, ui =>
                                        {
                                            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.AccountStatusClose);
                                            GppSyncContext.RunOnUnityThread(() =>
                                                {
                                                    Object.Destroy(ui);
                                                    callback.TryError(ErrorCode.UserCancelled);
                                                }
                                            );
                                        }, ui =>
                                        {
                                            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.WebQrLoginFailed);
                                            GppSyncContext.RunOnUnityThread(() =>
                                            {
                                                Object.Destroy(ui);
                                                callback.TryError(ErrorCode.AuthSessionExpired);
                                            });
                                        }
                                    );
                                    ConnectWebSocketForAccountCheck(platformType, wsUri, callback, () =>
                                    {
                                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.ExposeWaitForAccountCheck);
                                    }, () =>
                                    {
                                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.WaitForAccountCheckCancel);
                                    });
                                }
                                else
                                {
                                    // need add useUnSupportBrowser config
                                    OpenWebViewForLogin(redirectUri, true, underAgeResult =>
                                        {
                                            if (underAgeResult.IsError)
                                            {
                                                callback.TryError(underAgeResult.Error);
                                            }
                                            else
                                            {
                                                GppLoginAfterWeb(underAgeResult.Value, callback);
                                            }
                                        },
                                        platformType
                                    );
                                }

                                break;
                            }
                        case ErrorCode.KraftonAccountAlreadyMerged:
                            GppSDK.GetSession().ClearSession();
                            GppSDK.GetLoginCallback().TryError(result.Error);
                            break;
                        case ErrorCode.RepayRequired:
                            OpenRepayUI((result) =>
                            {
                                if (result.IsError)
                                {
                                    if (GppSDK.GetSession().AttemptingLoginPlatform == PlatformType.CustomProviderId)
                                        callback.TryError(ErrorCode.UserCancelled);
                                    else
                                        GppUI.ShowLogin(GppSDK.OnClickLoginButton, GppSDK.OnClickCloseButton);
                                }
                                else
                                {
                                    ContinueLogin(callback);
                                }
                            });
                            break;
                        case ErrorCode.KraftonAccountMerge:
                            result.Error.value = (result.Error.MessageVariables as IHttpResponse).BodyBytes.ToObject<MultipleGameUserResult>();
                            GppUI.ShowSelectMainGameUserId(result.Error.value as MultipleGameUserResult,
                                (param, ui) =>
                                {
                                    Object.Destroy(ui);
                                    SelectMainGameUserId(param, platformType);
                                },
                                ui =>
                                {
                                    Object.Destroy(ui);
                                    callback.TryError(ErrorCode.UserCancelled);
                                });
                            break;
                        case ErrorCode.DuplicatedEmailFound:
                            GppModalData modalData = new GppModalData.Builder()
                                .SetTitle(LocalizationKey.LoginFailedTitle.Localise())
                                .SetMessage(LocalizationKey.LoginFailedCheckAccountStatus.Localise())
                                .SetPositiveButtonText(LocalizationKey.GeneralConfirm.Localise())
                                .SetPositiveAction(ui =>
                                    {
                                        Object.Destroy(ui);
                                        callback.TryError(ErrorCode.DuplicatedEmailFound);
                                    }
                                )
                                .Build();

                            GppUI.ShowModal(modalData);
                            break;
                        default:
                            callback.TryError(result.Error);
                            break;
                    }
                }
                else
                {
                    if (HasUserCompliedTerms())
                    {
                        GppSDK.GetBasicApi().UpdateUserProfile();
                        GppSDK.CheckMaintenance(maintenanceResult =>
                            {
                                if (maintenanceResult.IsError)
                                {
                                    if (PlatformUtil.IsPc() || PlatformUtil.IsConsole())
                                    {
                                        if (GppTokenProvider.TryGetUserId(out string userId))
                                        {   
                                            if (GppTokenProvider.TryGetLastLoginPlatformUserId(out string platformUserId))
                                            {   
                                                if (userId != GppSDK.GetSession().UserId && platformUserId == GppSDK.GetSession().PlatformUserId)
                                                {
                                                    GppSDK.GetEventListener()?.NotifyMergedAccount(userId, GppSDK.GetSession().UserId);
                                                }
                                            }
                                        }
#if UNITY_STANDALONE
                                        GppSyncContext.AddTimerAction(60.0f, () =>
                                            GppSDK.OwnerShipSync(GppSDK.GetSession().AttemptingLoginPlatform));
#endif

                                        GppTokenProvider.SaveUserId(GppSDK.GetSession().UserId);
                                        GppTokenProvider.SaveLastLoginPlatformUserId(GppSDK.GetSession().PlatformUserId);
                                    }

                                    if (GppSDK.GetOptions().EnableGameServer)
                                    {
                                        SetGameServerId(GppSDK.GetSession().GameServerId, userResult =>
                                        {
                                            if (userResult.IsError)
                                            {
                                                GppSDK.GetSession().ClearSession(false);
                                                if (userResult.Error.Code == ErrorCode.KCNCodeNotExist) // 서버에서 kcn과 에러코드를 동일하게 사용중임
                                                {
                                                    callback.TryError(ErrorCode.GameServerNotFound);
                                                }
                                                else
                                                {
                                                    callback.TryError(userResult.Error);
                                                }
                                            }
                                            else
                                            {
                                                GppSDK.CheckGameServersMaintenanceInternal(gameServerMaintenances =>
                                                {
                                                    if (gameServerMaintenances.IsError)
                                                    {
                                                        GppSDK.GetSession().ClearSession(false);
                                                        callback.TryError(gameServerMaintenances.Error);
                                                    }
                                                    else
                                                    {
                                                        if (gameServerMaintenances.Value.WhiteList)
                                                        {
                                                            callback.Try(result);
                                                            GppSDK.GetLobby().Connect();
                                                            RequestSurveys();
                                                            return;
                                                        }

                                                        if (gameServerMaintenances.Value.Maintenances.Any(maintenance => maintenance.Name == GppSDK.GetSession().GameServerId))
                                                        {
                                                            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.GameServerIdValidate);
                                                            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.LoginComplete, new Dictionary<string, object>
                                                            {
                                                                { "login_type", GppSDK.GetSession().GetLoginType().ToString() }
                                                            });
                                                            var gameServerMaintenance = gameServerMaintenances.Value.Maintenances
                                                                .Where(maintenance =>
                                                                {
                                                                    return maintenance.Name == GppSDK.GetSession().GameServerId;
                                                                })
                                                                .First();

                                                            if (gameServerMaintenance.UnderMaintenance == false)
                                                            {
                                                                callback.Try(result);
                                                                GppSDK.GetLobby().Connect();
                                                                RequestSurveys();
                                                            }
                                                            else
                                                            {
                                                                GppUI.DismissUI(UIType.MobileLogin);
                                                                GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.ExposeGameServerMaintenance);
                                                                GppUI.ShowMaintenance(gameServerMaintenance.Maintenance, uiObject =>
                                                                {
                                                                    GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.GameServerMaintenanceConﬁrm);
                                                                    Object.Destroy(uiObject);
                                                                    GppSDK.GetSession().ClearSession(false);
                                                                    callback.TryError(ErrorCode.GameServerMaintenance);
                                                                });
                                                            }
                                                        }
                                                        else
                                                        {
                                                            GppSDK.GetSession().ClearSession(false);
                                                            callback.TryError(ErrorCode.GameServerNotFound);
                                                        }
                                                    }
                                                });
                                            }
                                        });
                                    }
                                    else
                                    {
                                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.LoginComplete, new Dictionary<string, object>
                                        {
                                            { "login_type", GppSDK.GetSession().GetLoginType().ToString() }
                                        });
                                        callback.Try(result);
                                        GppSDK.GetLobby().Connect();
                                        RequestSurveys();
                                    }
                                }
                                else
                                {
                                    GppUI.DismissUI(UIType.MobileLogin);
                                    GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.ExposeMaintenance, new Dictionary<string, object>
                                    {
                                        {"login_type", GppSDK.GetSession().GetLoginType().ToString()}
                                    });
                                    GppUI.ShowMaintenance(maintenanceResult.Value, uiObject =>
                                    {
                                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.MaintenanceConfirm, new Dictionary<string, object>
                                        {
                                            {"login_type", GppSDK.GetSession().GetLoginType().ToString()}
                                        });
                                        Object.Destroy(uiObject);
                                        GppSDK.GetSession().ClearSession(false);
                                        callback.TryError(ErrorCode.UnderMaintenance);
                                    }
                                );
                                }
                            }
                            );
                    }
                    else
                    {
                        StartTermsProcess(!isLinking && GppSDK.GetConfig().EnableLoginAnotherAccount, callback, isLinking);
                    }
                }
            };
        }

        private static void RequestSurveys()
        {
            GppSDK.GetSurveys(result =>
            {
                if (result.IsError)
                {
                    GppLog.Log($"GetSurveys failed. error: {result.Error.ToString()}");
                    return;
                }

                CheckLoginSurvey();
            });
        }

        private static void CheckLoginSurvey()
        {
            var loginSurvey = GppSurveyDataManager.GetLoginSurvey();
            if (loginSurvey is null)
            {
                return;
            }

            if (loginSurvey.Messages.TryGetValue(GppSDK.GetSession().LanguageCode, out var message))
            {
                GppUI.ShowSurvey(message.Title, message.Description, GppSurveyDataManager.GetToastPosition(loginSurvey), loginSurvey.PopupCondition.Duration);
            }
            else
            {
                if (loginSurvey.Messages.TryGetValue(loginSurvey.DefaultLanguage, out var surveyMessage))
                {
                    GppUI.ShowSurvey(surveyMessage.Title, surveyMessage.Description, GppSurveyDataManager.GetToastPosition(loginSurvey), loginSurvey.PopupCondition.Duration);
                }
                else
                {
                    GppLog.Log("There are no language sets matching the survey.");
                }
            }
        }

        private static void StartTermsProcess(bool isOpenLoginAnotherAccount, ResultCallback callback, bool isLinking)
        {
            GetLegalPoliciesAndOpenLegalUI(isOpenLoginAnotherAccount, (uiObject, isAllAccepted, policies) =>
                {
                    if (isAllAccepted)
                    {
                        if (uiObject == null)
                        {
                            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.LegalAgreementComplete, new Dictionary<string, object>
                            {
                                {"login_type", GppSDK.GetSession().GetLoginType().ToString()}
                            });
                            ContinueLogin(callback);
                        }
                        else
                        {
                            var acceptedPolicyList = ConvertUIModelsToAcceptedPolicyList(policies);
                            AcceptLegalRequest[] agreementRequests = acceptedPolicyList.ToArray();
                            GppSDK.GetLegalApi().BulkAcceptPolicyVersions(agreementRequests, response =>
                                {
                                    if (response.IsError)
                                    {
                                        callback.TryError(response.Error);
                                    }
                                    else
                                    {
                                        Object.Destroy(uiObject);
                                        if (PlatformUtil.IsMobile())
                                        {
                                            ShowPushToast(policies);
                                        }
                                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.LegalAgreementComplete, new Dictionary<string, object>
                                        {
                                            {"login_type", GppSDK.GetSession().GetLoginType().ToString()}
                                        });
                                        ContinueLogin(callback);
                                    }
                                }
                            );
                        }
                    }
                    else
                    {
                        callback.TryError(ErrorCode.SdkNotAcceptedLegal);
                    }
                }, callback, isLinking
            );
        }

        private static List<AcceptLegalRequest> ConvertUIModelsToAcceptedPolicyList(Dictionary<string, GppLegalModel> policies)
        {
            if (policies == null || policies.Count == 0)
            {
                return null;
            }

            List<AcceptLegalRequest> acceptLegalRequests = policies.Values.Select(model => new AcceptLegalRequest
            {
                localizedPolicyVersionId = model.LocalizedPolicyVersionId,
                policyId = model.PolicyId,
                policyVersionId = model.PolicyVersionId,
                isAccepted = model.IsAccepted
            }
            ).Where(request => !string.IsNullOrEmpty(request.localizedPolicyVersionId)).ToList();
            return acceptLegalRequests;
        }

        private static void ShowPushToast(Dictionary<string, GppLegalModel> policies)
        {
            if (policies == null || policies.Count == 0)
            {
                return;
            }

            List<GppLegalModel> pushPolicies = policies.Values.Where(model => model.PushNotificationIntegration != PushNotificationIntegration.NONE).ToList();
            if (pushPolicies.Count == 0)
            {
                return;
            }

            foreach (GppLegalModel policy in pushPolicies)
            {
                GppUI.ShowPushToast(policy.PushNotificationIntegration == PushNotificationIntegration.NIGHT_TIME_IN_APP_MARKETING_CONSENT, policy.IsAccepted);
            }
        }

        private static void GetLegalPoliciesAndOpenLegalUI(bool isOpenLoginAnotherAccount, Action<GameObject, bool, Dictionary<string, GppLegalModel>> onSubmit, ResultCallback loginCallback, bool isLinking = false)
        {
            GppSDK.GetLegalApi().GetLegalPoliciesByNamespaceAndCountry(result =>
                {
                    if (result.IsError)
                    {
                        loginCallback.TryError(ErrorCode.LoadTermsFailed);
                        return;
                    }

                    if (result.Value.Length == 0)
                    {
                        GppLog.Log("Legal is empty. Continue login...");
                        onSubmit(null, true, null);
                        return;
                    }

                    Dictionary<string, GppLegalModel> localizedPolicies = GenerateLocalizedLegalPolicies(result.Value);
                    GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.ExposeLegal, new Dictionary<string, object>
                    {
                        {"login_type", GppSDK.GetSession().GetLoginType().ToString()}
                    });
                    if (isOpenLoginAnotherAccount && PlatformUtil.IsMobile())
                        OpenLoginAnotherAccountLegalUI(localizedPolicies, onSubmit, loginCallback);
                    else
                        OpenLegalUI(localizedPolicies, onSubmit, loginCallback);
                }
            );
        }

        private static void OpenLegalUI(Dictionary<string, GppLegalModel> localizedPolicies, Action<GameObject, bool, Dictionary<string, GppLegalModel>> onSubmit, ResultCallback loginCallback)
        {
            if (PlatformUtil.IsPc() || PlatformUtil.IsConsole())
            {
                GppUI.ShowPcLegal(localizedPolicies, onSubmit, policy =>
                {
                    GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.ViewLegalDetails, new Dictionary<string, object>
                    {
                        {"login_type", GppSDK.GetSession().GetLoginType().ToString()}
                    });
                    GppSDK.OpenLegalWebView(policy.LocalizedPolicyVersionId);
                }, uiObject =>
                {
                    GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.LegalAgreementCancel, new Dictionary<string, object>
                    {
                        {"login_type", GppSDK.GetSession().GetLoginType().ToString()}
                    });
                    Object.Destroy(uiObject);
                    loginCallback.TryError(ErrorCode.UserCancelled);
                });
            }
            else
            {
                GppUI.DismissUI(UIType.MobileLogin);
                GppUI.ShowLegal(localizedPolicies, onSubmit, () =>
                {
                    GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.ViewLegalDetails, new Dictionary<string, object>
                    {
                        {"login_type", GppSDK.GetSession().GetLoginType().ToString()}
                    });
                });
            }
        }

        private static void OpenLoginAnotherAccountLegalUI(Dictionary<string, GppLegalModel> localizedPolicies, Action<GameObject, bool, Dictionary<string, GppLegalModel>> onSubmit, ResultCallback loginCallback)
        {
            GppUI.DismissUI(UIType.MobileLogin);
            GppUI.ShowLegalLoginAnotherAccount(localizedPolicies, onSubmit, (ui) =>
            {
                Object.Destroy(ui);

                //일단 언리얼과 동일하게 동작 서임님한테 체크 해볼것
                GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.LegalAccountSwitch);
                GppModalData modalData = new GppModalData.Builder()
                    .SetTitle(LocalizationKey.PlayWithOtherAccount.Localise())
                    .SetMessage(LocalizationKey.DataNotSavedWarning.Localise() + "\n" + LocalizationKey.ConfirmLogout.Localise())
                    .SetNegativeAction((ui) =>
                        {
                            Object.Destroy(ui);
                            GetLegalPoliciesAndOpenLegalUI(true, onSubmit, loginCallback);
                        })
                    .SetNegativeButtonText(LocalizationKey.GeneralCancel.Localise())
                    .SetPositiveAction((ui) =>
                        {
                            Object.Destroy(ui);
                            var attemptingPlatform = GppSDK.GetSession().AttemptingLoginPlatform;
                            GppSDK.GetSession().ClearSession(false);
                            if (attemptingPlatform == PlatformType.CustomProviderId)
                                loginCallback.TryError(ErrorCode.UserCancelled);
                            else
                                GppUI.ShowLogin(GppSDK.OnClickLoginButton, GppSDK.OnClickCloseButton);
                        })
                    .SetPositiveButtonText(LocalizationKey.PlayWithOtherAccount.Localise())
                    .SetBackButtonAction((ui) =>
                    {
                        Object.Destroy(ui);
                        GetLegalPoliciesAndOpenLegalUI(true, onSubmit, loginCallback);
                    })
                    .Build();

                GppUI.ShowModal(modalData);
            }, () =>
            {
                GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.ViewLegalDetails, new Dictionary<string, object>
                {
                    {"login_type", GppSDK.GetSession().GetLoginType().ToString()}
                });
            });
        }

        private static Dictionary<string, GppLegalModel> GenerateLocalizedLegalPolicies(IEnumerable<PublicPolicy> policies)
        {
            Dictionary<string, GppLegalModel> legalUIModels = new Dictionary<string, GppLegalModel>();
            foreach (var policy in policies)
            {
                LegalPolicyType policyType = policy.policyType switch
                {
                    "Ecommerce" => LegalPolicyType.ECOMMERCE_TYPE,
                    "Marketing Preference" => LegalPolicyType.MARKETING_PREFERENCE_TYPE,
                    _ => LegalPolicyType.LEGAL_DOCUMENT_TYPE
                };

                switch (policyType)
                {
                    case LegalPolicyType.LEGAL_DOCUMENT_TYPE when !policy.isMandatory || policy.isAccepted:
                    case LegalPolicyType.MARKETING_PREFERENCE_TYPE when policy.isAccepted:
                    case LegalPolicyType.ECOMMERCE_TYPE:
                        continue;
                }

                foreach (var version in policy.policyVersions)
                {
                    LocalizedPolicyVersionObject[] localizedPolicyVersions = version.localizedPolicyVersions;
                    if (localizedPolicyVersions == null) continue;

                    LocalizedPolicyVersionObject versionObject = null;
                    if (localizedPolicyVersions.Length == 1)
                    {
                        if (localizedPolicyVersions.First().status.Equals("active", StringComparison.OrdinalIgnoreCase))
                        {
                            versionObject = localizedPolicyVersions.First();
                        }
                    }
                    else
                    {
                        var activeVersionObjects = localizedPolicyVersions.Where(v => v.status.Equals("active", StringComparison.OrdinalIgnoreCase));
                        versionObject = activeVersionObjects.FirstOrDefault(v => v.localeCode.Equals(GppSDK.GetSession().PlayerLocale))
                                        ?? activeVersionObjects.FirstOrDefault(v => v.localeCode.Equals(GppSDK.GetSession().LanguageCode))
                                        ?? activeVersionObjects.FirstOrDefault(v => GppSDK.GetSession().CompareWithPlayerLanguage(v.localeCode))
                                        ?? activeVersionObjects.FirstOrDefault(v => v.isDefaultSelection);
                    }

                    GppLegalModel model = new GppLegalModel.Builder()
                        .SetIsMandatory(policy.isMandatory)
                        .SetPolicyTitle(policy.isMandatory, versionObject?.description)
                        .SetWebUrl(versionObject?.attachmentLocation)
                        .SetPushNotificationIntegration(policy.pushNotificationIntegration)
                        .SetLocalizedPolicyVersionId(versionObject?.id)
                        .SetPolicyVersionId(version.id)
                        .SetPolicyId(policy.id)
                        .SetIsAccepted(false)
                        .Build();

                    legalUIModels.Add(policy.id, model);
                }
            }

            return legalUIModels;
        }

        private static bool HasUserCompliedTerms()
        {
            return GppSDK.GetSession().IsComply;
        }

        internal static void Logout(ResultCallback callback)
        {
            GppSDK.GetIamApi().Logout(callback);
        }

        internal static void LinkKraftonId(PlatformType platformType, ResultCallback<GppUser> callback)
        {
            if (GppSDK.GetSession().cachedTokenData.IsFullKid)
            {
                callback.TryError(new Error(ErrorCode.AlreadyLinked));
                return;
            }

            AuthorizeKidMobile(tokenResult =>
                {
                    if (tokenResult.IsError)
                    {
                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.LinkFailed);
                        callback.TryError(tokenResult.Error);
                    }
                    else
                    {
                        GppLinkLogin(platformType, tokenResult.Value, loginResult =>
                            {
                                if (loginResult.IsError)
                                {
                                    GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.LinkFailed);
                                    callback.TryError(loginResult.Error);
                                }
                                else
                                {
                                    AfterLinkSuccess(callback);
                                }
                            }
                        );
                    }
                }, platformType, true
            );
        }

        private static void AfterLinkSuccess(ResultCallback<GppUser> callback)
        {
            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.LinkComplete);
            GppTokenProvider.DeleteGuestUserId();
            GppSDK.GetLobby().Reconnect();
            GppSDK.GetBasicApi().GetPushStatus(result =>
                {
                    if (!result.IsError)
                    {
                        GppSDK.GetSession().cachedTokenData.PushEnabled = result.Value.pushEnable;
                    }

                    GppSDK.GetBasicApi().GetNightPushStatus(nightResult =>
                        {
                            if (!nightResult.IsError)
                            {
                                GppSDK.GetSession().cachedTokenData.NightPushEnabled = nightResult.Value.pushNightEnable;
                            }

                            callback.TryOk(new GppUser(GppSDK.GetSession().cachedTokenData));
                        }
                    );
                }
            );
        }

        private static void AfterCheckEligibility(bool isError, string errorMessage, bool verified, string ageStatus, ResultCallback<GppCheckEligibilityResult> callback)
        {
            GppSDK.GetBasicApi().GetPushStatus(result =>
                {
                    if (!result.IsError)
                    {
                        GppSDK.GetSession().cachedTokenData.PushEnabled = result.Value.pushEnable;
                    }

                    GppSDK.GetBasicApi().GetNightPushStatus(nightResult =>
                        {
                            if (!nightResult.IsError)
                            {
                                GppSDK.GetSession().cachedTokenData.NightPushEnabled = nightResult.Value.pushNightEnable;
                            }

                            callback.TryOk(new GppCheckEligibilityResult(isError, errorMessage, verified, ageStatus));
                        }
                    );
                }
            );
        }

        internal static void DeleteAccount(ResultCallback callback)
        {
            GppSDK.GetLobby().Disconnect();
            GppSDK.GetGdprApi().DeleteAccount(GppSDK.GetSession().UserId, result =>
            {
                if (!result.IsError)
                {
                    GppTelemetry.SendDeleteAccount(GppClientLogModels.EntryStep.AccountDeletionRequestSuccess);
                    GppSDK.GetSession().ClearSession();
                    GppUI.ShowToast(LocalizationKey.AccountDeletionNotificationTitle.Localise(), LocalizationKey.AccountDeletionNotificationContent.Localise(), GppToastPosition.TOP);
                    callback.Try(result);
                }
                else
                {
                    GppTelemetry.SendDeleteAccount(GppClientLogModels.EntryStep.AccountDeletionRequestFailed,
                        new Dictionary<string, object>
                        {
                            { "error_code", $"{(int)result.Error.Code}" },
                            { "error_message", result.Error.Message }
                        });
                    GppSDK.GetLobby().Connect();
                    callback.TryError(result.Error);
                }
            });
        }

        internal static void DeleteAccount(PlatformType platformType, ResultCallback callback)
        {
            if (platformType != PlatformType.AppleMac)
            {
                callback.TryError(ErrorCode.NotSupportPlatform);
                return;
            }

            if (!MacExt.CanUse())
            {
                callback.TryError(ErrorCode.NotSupportPlatform);
                return;
            }

            MacExt.Impl().Login(appleSignInResult =>
            {
                if (appleSignInResult.IsError)
                {
                    GppTelemetry.SendDeleteAccount(GppClientLogModels.EntryStep.PlatformTokenAcquireFailed);
                    GppTelemetry.SendDeleteAccount(GppClientLogModels.EntryStep.AccountDeletionRequestFailed,
                        new Dictionary<string, object>
                        {
                            { "error_code", $"{(int)appleSignInResult.Error.Code}" },
                            { "error_message", appleSignInResult.Error.Message }
                        });
                    callback.TryError(appleSignInResult.Error);
                }
                else
                {
                    GppTelemetry.SendDeleteAccount(GppClientLogModels.EntryStep.PlatformTokenAcquireSuccess);
                    GppSDK.GetLobby().Disconnect();
                    GppSDK.GetGdprApi().DeleteAccount(platformType, appleSignInResult.Value.AuthorizationCode, result =>
                    {
                        if (!result.IsError)
                        {
                            GppTelemetry.SendDeleteAccount(GppClientLogModels.EntryStep.AccountDeletionRequestSuccess);
                            GppSDK.GetSession().ClearSession();
                            GppUI.ShowToast(LocalizationKey.AccountDeletionNotificationTitle.Localise(), LocalizationKey.AccountDeletionNotificationContent.Localise(), GppToastPosition.TOP);
                            callback.Try(result);
                        }
                        else
                        {
                            GppTelemetry.SendDeleteAccount(GppClientLogModels.EntryStep.AccountDeletionRequestFailed,
                                new Dictionary<string, object>
                                {
                                    { "error_code", $"{(int)result.Error.Code}" },
                                    { "error_message", result.Error.Message }
                                });
                            GppSDK.GetLobby().Connect();
                            callback.TryError(result.Error);
                        }
                    });
                }
            });
        }

        internal static void ForceDeleteAccount(ResultCallback callback)
        {
            GppSDK.GetIamApi().ForceDeleteAccount(result =>
            {
                if (GppSDK.GetSession().AttemptingLoginPlatform == PlatformType.Guest)
                {
                    GppTokenProvider.DeleteGuestUserId();
                }
                if (FirebasePushExt.CanUse())
                {
                    FirebasePushExt.Impl().DeletePushToken();
                }
                GppSDK.GetSession().ClearSession();
                callback.Try(result);
            });
        }

        public static void SetGameServerId(string gameServerId, ResultCallback callback)
        {
            GppSDK.GetIamApi().GetTokenWithGameServerId(gameServerId, result =>
            {
                if (!result.IsError)
                {
                    callback.Try(result);
                }
                else
                {
                    callback.TryError(result.Error);
                }
            });
        }

        public static void CheckEligibility(ResultCallback<GppCheckEligibilityResult> callback)
        {
            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.TryCheckEligibility, new Dictionary<string, object>
                                {
                                    {"is_headless", GppSDK.GetSession().GetLoginFlowType() is LoginSession.LoginFlowType.headless }
                                });
            GppSDK.GetIamApi().CheckEligibility(result =>
            {
                if (!result.IsError)
                {
                    if (!result.Value.Verified)
                    {
                        if (result.Value.AgeStatus == "UNDER")
                        {
                            GppSDK.GetLobby().Connect();
                            AfterCheckEligibility(true, result.Error?.Message, result.Value.Verified, result.Value.AgeStatus, callback);
                            return;
                        }
                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.ExposeEligibilityCheck, new Dictionary<string, object>
                                {
                                    {"is_headless", GppSDK.GetSession().GetLoginFlowType() is LoginSession.LoginFlowType.headless }
                                });
                        GppUI.ShowCheckEligibility(result.Value, ui =>
                            {
                                GppSyncContext.RunOnUnityThread(() =>
                                    {
                                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.EligibilityCheckManualOpen, new Dictionary<string, object>
                                        {
                                            {"is_headless", GppSDK.GetSession().GetLoginFlowType() is LoginSession.LoginFlowType.headless }
                                        });
                                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.ExposeBrowser);
                                        Application.OpenURL(result.Value.RedirectUri);
                                    }
                                );
                            }, ui =>
                            {
                                GppSyncContext.RunOnUnityThread(() =>
                                    {
                                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.EligibilityCheckClose, new Dictionary<string, object>
                                        {
                                            {"is_headless", GppSDK.GetSession().GetLoginFlowType() is LoginSession.LoginFlowType.headless }
                                        });
                                        Object.Destroy(ui);
                                        GppSDK.GetLobby().Connect();
                                        AfterCheckEligibility(true, result.Error?.Message, result.Value.Verified, result.Value.AgeStatus, callback);
                                    }
                                );
                            }, ui =>
                            {
                                GppSyncContext.RunOnUnityThread(() =>
                                {
                                    GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.WebQrLoginFailed);
                                    Object.Destroy(ui);
                                    GppSDK.GetLobby().Connect();
                                    AfterCheckEligibility(true, "Auth Session Expired.", result.Value.Verified, result.Value.AgeStatus, callback);
                                }
                                );
                            }
                        );
                        ConnectWebSocketForAccountCheck(GppSDK.GetSession().AttemptingLoginPlatform, result.Value.WebSocketUri, checkResult =>
                        {
                            if (!checkResult.IsError)
                            {
                                CheckEligibility(callback);
                            }
                            else
                            {
                                if (checkResult.Error.Code == ErrorCode.UserCancelled || checkResult.Error.Code == ErrorCode.UserUnderage)
                                {
                                    CheckEligibility(callback);
                                }
                                else
                                {
                                    GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.EligibilityCheckFailed, new Dictionary<string, object>
                                    {
                                        {"is_headless", GppSDK.GetSession().GetLoginFlowType() is LoginSession.LoginFlowType.headless }
                                    });
                                    callback.TryError(checkResult.Error);
                                }
                            }
                        }, () =>
                        {
                            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.ExposeWaitForEligibilityCheck, new Dictionary<string, object>
                                {
                                    {"is_headless", GppSDK.GetSession().GetLoginFlowType() is LoginSession.LoginFlowType.headless }
                                });
                        }, () =>
                        {
                            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.EligibilityCheckCancel, new Dictionary<string, object>
                                {
                                    {"is_headless", GppSDK.GetSession().GetLoginFlowType() is LoginSession.LoginFlowType.headless }
                                });
                        },
                        true, true);
                    }
                    else
                    {
                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.EligibilityCheckComplete, new Dictionary<string, object>
                            {
                                {"is_headless", GppSDK.GetSession().GetLoginFlowType() is LoginSession.LoginFlowType.headless }
                            });

                        GppSDK.GetLobby().Connect();
                        AfterCheckEligibility(result.IsError, result.Error?.Message, result.Value.Verified, result.Value.AgeStatus, callback);
                    }
                }
                else
                {
                    if (result.Error.Code == ErrorCode.KraftonAccountAlreadyMerged)
                    {
                        CheckEligibility(callback);
                    }
                    else
                    {
                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.EligibilityCheckFailed);
                        callback.TryError(result.Error);
                    }
                }
            });
        }

        public static void GetKidIdToken(ResultCallback<KidIdTokenResult> callback)
        {
            GppSDK.GetIamApi().GetKidIdToken(callback);
        }

        private static void SelectMainGameUserId(SelectMainGameUserIdParam param, PlatformType platformType)
        {
            GppSDK.GetIamApi().SelectMainGameUserId(param.accessToken, param.userId,
                param.isMain, InternalLoginCallback(
                    result =>
                    {
                        GppSDK.TransformAndCallbackLoginResult(result);
                    }, platformType));
        }

        private static void OpenRepayUI(ResultCallback callback)
        {
            GppSDK.GetPlatformApi().GetRestrictions((restrictionProducts) =>
            {
                if (restrictionProducts.IsError)
                {
                    callback.TryError(ErrorCode.RepayRequired);
                }
                else
                {
                    GppSDK.GetUnconsumedProducts(result =>
                    {
                        if (!result.IsError)
                        {
                            bool hasProduct = result.Value.Any(x => restrictionProducts.Value.Any(restriction => restriction.repaymentInfo.productId == x.ReceiptInfo.productId));
                            if (result.Value.Count == 0 || !hasProduct)
                            {
                                GppUI.ShowRepayRequired(restrictionProducts.Value,
                                    () =>
                                    {
                                        GppUI.DismissUI(UIType.RepayRequired);
                                        callback.TryOk();
                                    },
                                    (ui) =>
                                    {
                                        GppSDK.GetSession().ClearSession(false);
                                        GppUI.DismissUI(UIType.RepayRequired);
                                        callback.TryError(ErrorCode.RepayRequired);
                                    }
                                );
                            }
                            else
                            {
                                GppSDK.Restore(restoreResult =>
                                {
                                    if (restoreResult.IsError)
                                    {
                                        GppUI.ShowRepayRequired(restrictionProducts.Value,
                                        () =>
                                        {
                                            GppUI.DismissUI(UIType.RepayRequired);
                                            callback.TryOk();
                                        },
                                        (ui) =>
                                        {
                                            GppSDK.GetSession().ClearSession(false);
                                            GppUI.DismissUI(UIType.RepayRequired);
                                            callback.TryError(ErrorCode.RepayRequired);
                                        }
                                    );
                                    }
                                    else
                                    {
                                        OpenRepayUI(callback);
                                    }
                                });
                            }
                        }
                    });
                }
            });
        }
    }
}
