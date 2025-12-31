using System.Collections.Generic;
using Gpp.Api.Platform;
using Gpp.Auth;
using Gpp.Constants;
using Gpp.Core;
using Gpp.Extensions.EpicGames;
using Gpp.Extensions.Mac;
using Gpp.Extensions.Steam;
using Gpp.Extensions.Xbox;
using Gpp.Extensions.XboxPc;
using Gpp.Log;
using Gpp.Models;
using Gpp.Telemetry;
using Gpp.Utils;
using UnityEngine;

namespace Gpp
{
    public sealed partial class GppSDK
    {
        private AuthWebSocket _authWebSocket;

        internal static void SetAuthWebSocket(AuthWebSocket webSocket)
        {
            Instance._authWebSocket?.Close();
            Instance._authWebSocket = webSocket;
        }

        internal static AuthWebSocket GetAuthWebSocket()
        {
            return Instance._authWebSocket;
        }

        private static void InitPcPlatform()
        {
            if (!PlatformUtil.IsPc())
            {
                return;
            }

            Application.runInBackground = true;
        }

        private static void HandlePcAutoLogin(PlatformType platformType, bool isHeadless = false, bool isLinking = false)
        {
            GppTokenProvider.SaveLastLoginMethodType(isHeadless ? LoginMethodType.Headless : LoginMethodType.FullKid);
            if (platformType is PlatformType.None or PlatformType.Steam or PlatformType.EpicGames or PlatformType.AppleMac or PlatformType.live)
            {
                if (EpicGamesExt.CanUse() && platformType == PlatformType.EpicGames)
                {
                    EpicGamesExt.Impl().Init();
                    GppAuth.Login(PlatformType.EpicGames, result =>
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
                                    }
                                    TransformAndCallbackLoginResult(result);
                                }
                            );
                        },
                        isHeadless
                    );
                }
                else if (MacExt.CanUse() && platformType == PlatformType.AppleMac)
                {
                    MacExt.Impl().Init();

                    if (isLinking)
                    {
                        GppAuth.Login(PlatformType.AppleMac, result =>
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

                                TransformAndCallbackLoginResult(result);
                            }
                            else
                            {
                                TransformAndCallbackLoginResult(result);
                            }
                        }, isHeadless);
                    }
                    else
                    {
                        GppAuth.AutoLogin((_) =>
                        {
                            GppAuth.Login(PlatformType.AppleMac, result =>
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

                                    TransformAndCallbackLoginResult(result);
                                }
                                else
                                {
                                    TransformAndCallbackLoginResult(result);
                                }
                            }, isHeadless);
                        });
                    }
                }
                else if (XboxPcExt.CanUse() && platformType == PlatformType.live)
                {
                    XboxPcExt.Impl().Init(result =>
                    {
                        if (XboxPcExt.Impl().CanUse())
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
                                            }
                                        );
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
                else if (SteamExt.CanUse() && platformType == PlatformType.Steam)
                {
                    SteamExt.Impl().Init();
                    if (SteamExt.Impl().IsOnline())
                    {
                        GppAuth.Login(PlatformType.Steam, result =>
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
                                                }
                                            );

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
                                { "platform_id", "steam" }
                            });
                        GppLog.Log("Steam is offline. Try auto login with refresh token.");
                        GppAuth.AutoLogin((_) =>
                            {
                                GppLog.LogException("Steam is offline and can not auto login.");
                                GetLoginCallback().TryError(ErrorCode.SteamOffline);
                            }
                        );
                    }
                }
                else
                {
                    GetLoginCallback().TryError(ErrorCode.NotSupportIdP);
                }
            }
            else
            {
                GppLog.LogWarning($"Not Supported platform type: {platformType.ToString()}");
                GetLoginCallback().TryError(ErrorCode.NotSupportIdP);
            }
        }
    }
}