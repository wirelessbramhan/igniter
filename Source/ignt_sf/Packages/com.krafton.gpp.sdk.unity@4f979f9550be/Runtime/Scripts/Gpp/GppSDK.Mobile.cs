using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gpp.Auth;
using Gpp.CommonUI;
using Gpp.Core;
using Gpp.Extension;
using Gpp.Extensions.Braze;
using Gpp.Extensions.FirebasePush;
using Gpp.Extensions.GooglePlayGames;
using Gpp.Extensions.IAP.Models;
using Gpp.Extensions.IAP;
using Gpp.Log;
using Gpp.Models;
using Gpp.Constants;
using Gpp.Telemetry;
using Gpp.Utils;
using System.Diagnostics;

namespace Gpp
{
    public sealed partial class GppSDK
    {
        private static void StartMobileLoginFlow()
        {
            if (GooglePlayGamesExt.CanUse() && GetSession().IsFirstSession)
            {
                var gpgInfo = GooglePlayGamesExt.Impl().GetGpgInfo();
                if (gpgInfo == null)
                {
                    GppUI.ShowLogin(OnClickLoginButton, OnClickCloseButton);
                }
                else
                {
                    GpgLogin(gpgInfo.AccessToken);
                }
            }
            else
            {
                GppUI.ShowLogin(OnClickLoginButton, OnClickCloseButton);
            }
        }

        private static async void InitMobilePlatform(Action callback)
        {
            if (!PlatformUtil.IsMobileNotEditor())
            {
                return;
            }

            if (GetConfig().Extensions.FirebasePush.EnableAutoRequestPermission)
            {
                await InitFirebasePush();
            }
            await InitGoogleAnalytics();
            await InitMobileIap();
            await InitGooglePlayGameService();
            await InitBraze();

            // 일단 모바일만 KPS Init을 수행합니다.
            Kps.PaymentService.InitKps();

            callback.Invoke();
        }

        private static Task InitBraze()
        {
            var tcs = new TaskCompletionSource<bool>();
            if (GooglePlayGamesExt.CanUse() && GooglePlayGamesExt.Impl().IsPC())
            {
                GppLog.Log("Braze is disabled due to the Google Play Games PC emulator environment.");
                tcs.SetResult(true);
                return tcs.Task;
            }
            if (BrazeExt.CanUse())
            {
                BrazeExt.Impl().Init();
            }
            tcs.SetResult(true);
            return tcs.Task;
        }

        private static Task InitMobileIap()
        {
            var tcs = new TaskCompletionSource<bool>();
            if (MobileIapExt.CanUse())
            {
                MobileIapExt.Impl().Init(result =>
                {
                    GppLog.Log(result.ToPrettyJsonString());
                    tcs.SetResult(true);
                }, GetOptions().Store);
            }
            else
            {
                tcs.SetResult(true);
            }

            return tcs.Task;
        }

        private static Task InitGoogleAnalytics()
        {
            var tcs = new TaskCompletionSource<bool>();
            if (GoogleAnalyticsExt.CanUse())
            {
                GoogleAnalyticsExt.Impl().Init();
            }
            tcs.SetResult(true);
            return tcs.Task;
        }

        private static Task InitFirebasePush()
        {
            var tcs = new TaskCompletionSource<bool>();

            if (GooglePlayGamesExt.CanUse() && GooglePlayGamesExt.Impl().IsPC())
            {
                GppLog.Log("Push is disabled due to the Google Play Games PC emulator environment.");
                tcs.SetResult(true);
                return tcs.Task;
            }
            if (FirebasePushExt.CanUse())
            {
                FirebasePushExt.Impl().GetPermission(state =>
                    {
                        if (state.Value == PermissionState.Granted)
                        {
                            FirebasePushExt.Impl().GetPushToken(token =>
                            {
                                GppLog.Log($"GetPushToken : {token.ToPrettyJsonString()}");
                            });
                            tcs.SetResult(true);
                        }
                        else
                        {
                            GppLog.Log($"GetPushToken fail : {state.Value}");
                            tcs.SetResult(true);
                        }
                    }
                );
            }
            else
            {
                tcs.SetResult(true);
            }

            return tcs.Task;
        }

        private static Task InitGooglePlayGameService()
        {
            var tcs = new TaskCompletionSource<bool>();
            if (!GooglePlayGamesExt.CanUse())
            {
                tcs.SetResult(true);
                return tcs.Task;
            }

            var gpgClientId = GetConfig().Extensions.GooglePlayGames.WebClientId;
            GooglePlayGamesExt.Impl().Init(gpgClientId, initResult =>
            {
                GppLog.Log($"GPG Init Result: {initResult.ToPrettyJsonString()}");
                tcs.SetResult(true);
            });
            return tcs.Task;
        }

        private static void HandleMobileAutoLogin(PlatformType platformType = PlatformType.None, string credential = null)
        {
            if (PlatformUtil.IsMobileNotEditor())
            {
                if (platformType == PlatformType.CustomProviderId)
                {
                    GppAuth.GppLogin(platformType, credential, result =>
                    {
                        if (result.IsError)
                        {
                            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.LoginFailed,
                                new Dictionary<string, object>
                                {
                                    { "login_type", GetSession().GetLoginType().ToString() },
                                    { "error_code", $"{(int)result.Error.Code}" },
                                    { "error_message", result.Error.Message }
                                });
                        }
                        TransformAndCallbackLoginResult(result);
                    });
                }
                else
                {
                    StartMobileLoginFlow();
                }
            }
            else
            {
                GppUI.ShowLogin(OnClickLoginButton, OnClickCloseButton);
            }
        }

        private static void HandleCustomProviderIdLogin(LoginRequestParam requestParam)
        {
            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.TryLogin, new Dictionary<string, object>
                    {
                        {"login_method", "custom_provider"}
                    });
            GppAuth.GppLogin(requestParam.platformType, requestParam.customProviderIdParam, result =>
            {
                if (result.IsError)
                {
                    GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.LoginFailed,
                        new Dictionary<string, object>
                        {
                                    { "login_method", "custom_provider" },
                                    { "login_type", GetSession().GetLoginType().ToString() },
                                    { "error_code", $"{(int)result.Error.Code}" },
                                    { "error_message", result.Error.Message }
                        });
                }
                TransformAndCallbackLoginResult(result);
            });
        }
    }
}