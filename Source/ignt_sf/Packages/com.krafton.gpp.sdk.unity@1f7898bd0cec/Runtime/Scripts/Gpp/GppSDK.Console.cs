using System.Collections.Generic;
using Gpp.Api.Platform;
using Gpp.Auth;
using Gpp.Constants;
using Gpp.Core;
using Gpp.Extensions.EpicGames;
using Gpp.Extensions.Ps5;
using Gpp.Extensions.Steam;
using Gpp.Extensions.Xbox;
using Gpp.Log;
using Gpp.Models;
using Gpp.Telemetry;
using Gpp.Utils;
using UnityEngine;

namespace Gpp
{
    public sealed partial class GppSDK
    {
        private AuthWebSocket _authWebSocketConsole;

        internal static void SetAuthWebSocketConsole(AuthWebSocket webSocket)
        {
            Instance._authWebSocket?.Close();
            Instance._authWebSocket = webSocket;
        }

        internal static AuthWebSocket GetAuthWebSocketConsole()
        {
            return Instance._authWebSocket;
        }

        private static void InitConsolePlatform()
        {
            if (!PlatformUtil.IsConsole())
            {
                return;
            }
#if UNITY_PS5
            Ps5Ext.Impl().Init(result =>
            {
                GppLog.Log("PS5 Init : " + result);
            });
#endif
            Application.runInBackground = true;
        }

        private static void HandleConsoleAutoLogin(PlatformType platformType, bool isHeadless = false)
        {
            if (platformType is PlatformType.None or PlatformType.live or PlatformType.PS5)
            {
                if (XboxExt.CanUse() && platformType == PlatformType.live)
                {
                    XboxExt.Impl().Init(_ =>
                    {
                        if (XboxExt.Impl().CanUse())
                        {
                            GppAuth.Login(platformType, result =>
                            {
                                GppSyncContext.RunOnUnityThread(() =>
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
                                        
                                        if (result.Error.Code == ErrorCode.SteamServerError)
                                        {
                                            if (GppTokenProvider.TryGetRefreshTokenData(out RefreshTokenData tokenData))
                                            {
                                                GppAuth.ContinuePcLogin(platformType, tokenData.refresh_token, continueResult =>
                                                {
                                                    TransformAndCallbackLoginResult(continueResult);
                                                    return;
                                                });
                                            }
                                            else
                                            {
                                                TransformAndCallbackLoginResult(result);
                                            }
                                        }
                                        else
                                        {
                                            TransformAndCallbackLoginResult(result);
                                        }
                                    }
                                    else
                                    {
                                        TransformAndCallbackLoginResult(result);
                                    }
                                }
                                );
                            },
                                isHeadless
                            );
                        }
                        else
                        {
                            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.PlatformTokenAcquireFailed, new Dictionary<string, object>
                                {
                                    {"platform_id", "xbox"}
                                });
                            GppLog.Log("Xbox is offline. Try auto login with refresh token.");
                            GppAuth.AutoLogin((_) =>
                            {
                                GppLog.LogWarning("Xbox is offline and can not auto login.");
                                GetLoginCallback().TryError(ErrorCode.SteamOffline);
                            }
                            );
                        }
                    });
                }
                else if (Ps5Ext.CanUse() && platformType == PlatformType.PS5)
                {
                    Ps5Ext.Impl().Init(initResult =>
                    {
                        if (Ps5Ext.Impl().CanUse())
                        {
                            GppAuth.Login(platformType, result =>
                            {
                                GppSyncContext.RunOnUnityThread(() =>
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
                                        
                                        if (result.Error.Code == ErrorCode.SteamServerError)
                                        {
                                            if (GppTokenProvider.TryGetRefreshTokenData(out RefreshTokenData tokenData))
                                            {
                                                GppAuth.ContinuePcLogin(platformType, tokenData.refresh_token, continueResult =>
                                                {
                                                    TransformAndCallbackLoginResult(continueResult);
                                                    return;
                                                });
                                            }
                                            else
                                            {
                                                TransformAndCallbackLoginResult(result);
                                            }
                                        }
                                        else
                                        {
                                            TransformAndCallbackLoginResult(result);
                                        }
                                    }
                                    else
                                    {
                                        TransformAndCallbackLoginResult(result);
                                    }
                                }
                                );
                            },
                                isHeadless
                            );
                        }
                        else
                        {
                            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.PlatformTokenAcquireFailed, new Dictionary<string, object>
                                {
                                    {"platform_id", "ps5"}
                                });
                            GppLog.Log("PSN is offline. Try auto login with refresh token.");
                            GppAuth.AutoLogin((_) =>
                            {
                                GppLog.LogWarning("PSN is offline and can not auto login.");
                                GetLoginCallback().TryError(ErrorCode.SteamOffline);
                            }
                            );
                        }
                    });
                }
                else
                {
                    GppLog.LogWarning("PSN is not available.");
                    GetLoginCallback().TryError(ErrorCode.SteamNotAvailable);
                }
            }
            else
            {
                GppLog.LogWarning($"Not Supported platform type: {platformType.ToString()}");
                GetLoginCallback().TryError(ErrorCode.NotSupportPlatform);
            }
        }
    }
}