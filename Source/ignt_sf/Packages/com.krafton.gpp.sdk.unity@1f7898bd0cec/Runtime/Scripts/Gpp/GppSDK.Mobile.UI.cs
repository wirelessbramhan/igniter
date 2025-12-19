using System.Collections.Generic;
using Gpp.Auth;
using Gpp.CommonUI;
using Gpp.Constants;
using Gpp.Core;
using Gpp.Models;
using Gpp.Telemetry;
using UnityEngine;

namespace Gpp
{
    public sealed partial class GppSDK
    {
        internal static void OnClickLoginButton(GameObject ui, PlatformType type)
        {
            GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.TryLogin, new Dictionary<string, object>
            {
                {"login_method", type.ToString().ToLower()}
            });
            Object.Destroy(ui);
            GppAuth.Login(type, result =>
            {
                if (result.IsError)
                {
                    if (result.Error.Code == ErrorCode.SdkGuestPopupCancel)
                    {
                        GppUI.ShowLogin(OnClickLoginButton, OnClickCloseButton);
                        return;
                    }

                    GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.LoginFailed, new Dictionary<string, object>
                    {
                        { "login_type", GetSession().GetLoginType().ToString() },
                        { "error_code", $"{(int)result.Error.Code}" },
                        { "error_message", result.Error.Message }
                    });
                }
                TransformAndCallbackLoginResult(result);
            });
        }

        internal static void OnClickCloseButton(GameObject ui)
        {
            Object.Destroy(ui);
            TransformAndCallbackLoginResult(Result.CreateError(ErrorCode.SelectLoginCancelled));
        }
    }
}
