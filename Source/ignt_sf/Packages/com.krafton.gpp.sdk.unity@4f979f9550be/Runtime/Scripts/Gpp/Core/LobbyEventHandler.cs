using System;
using System.Collections.Generic;
using Gpp.CommonUI;
using Gpp.CommonUI.Toast;
using Gpp.Extension;
using Gpp.Localization;
using Gpp.Log;
using Gpp.Models;
using Gpp.Network;
using Newtonsoft.Json;
using UnityEngine;

namespace Gpp.Core
{
    internal static class LobbyEventHandler
    {
        internal static void LobbyDuplicatedSessionHandler(Result<DisconnectNotify> result)
        {
            GppSDK.GetEventListener()?.NotifyDuplicatedSession();
            GppSDK.GetSession().ClearSession(false);
        }

        internal static void LobbyDisconnectingHandler(Result<DisconnectNotify> result)
        {
            if (!result.Value.message.Contains("multiple session"))
            {
                return;
            }

            GppSDK.GetEventListener()?.NotifyDuplicatedSession();
            GppSDK.GetSession().ClearSession(false);
        }

        internal static void LobbyNotificationHandler(Result<Notification> result)
        {
            switch (result.Value.topic)
            {
                case "e-commerce/rewards":
                    {
                        GppUI.ShowToast(LocalizationKey.RewardReceivedNotificationTitle.Localise(), LocalizationKey.RewardReceivedNotificationContent.Localise());
                        break;
                    }
                case "e-commerce/codeRedeemed":
                    {
                        GppUI.ShowToast(LocalizationKey.CodeRedeemedNotificationTitle.Localise(), LocalizationKey.CodeRedeemedNotificationContent.Localise());
                        break;
                    }
                case "in-game-notification":
                    {
                        var payload = JsonConvert.DeserializeObject<InGameNotificationPayload>(result.Value.payload);
                        var position = Enum.Parse<GppToastPosition>(payload.displayPosition);
                        var displayDurationAnimSec = payload.displayDurationMillis / 1000f / 2f;
                        GppUI.ShowToast(payload.title, payload.message, position, displayDurationAnimSec);
                        break;
                    }
            }
        }

        internal static void LobbyAccountDeletionHandler(Result<DisconnectNotify> result)
        {
            GppSDK.GetEventListener()?.NotifyDeletedAccount();
        }
        
        internal static void LobbyRefreshSurveyHandler(Result<RefreshSurvey> result)
        {
            var isUserNotificationNeeded = false;
            
            if (string.IsNullOrEmpty(result.Value.payload) is false)
            {
                var data = result.Value.payload.ToObject<Dictionary<string, object>>();
                
                if (data.TryGetValue("refreshTarget", out var refreshTarget))
                {
                    // POPUP: PopupSurveyResult.PopupSurveys data has been updated.
                    // SURVEY: PopupSurveyResult.Surveys data has been updated.
                    
                    if (refreshTarget.ToString().Equals("SURVEY"))
                    {
                        isUserNotificationNeeded = true;
                    }
                }
            }
            
            GppSDK.GetSurveys(surveyResult =>
            {
                if (surveyResult.IsError is false && isUserNotificationNeeded)
                {
                    GppSDK.GetEventListener()?.NotifyRefreshSurvey(surveyResult.Value);
                }
            });

        }

        internal static void LobbyDisconnectedHandler(WsCloseCode code)
        {
            GppLog.Log($"Lobby Disconnected {code}");
            switch (code)
            {
                case WsCloseCode.TokenRevoked:
                case WsCloseCode.DisconnectedByServer:
                case WsCloseCode.ClosedByServer:
                case WsCloseCode.Normal:
                    GppLog.Log("Disconnected normally");
                    break;
                case WsCloseCode.DisconnectKeepExistingCode:
                    GppSDK.GetEventListener()?.NotifyDuplicatedSession();
                    GppSDK.GetSession().ClearSession(false);
                    break;
                case WsCloseCode.ProtocolError:
                    GppSDK.GetIamApi().RefreshAccessToken(result =>
                        {
                            if (result.IsError)
                            {
                                GppLog.Log("Failed to refresh token :" + result.Error.Message);
                                GppSDK.GetSession().ClearSession();
                                GppSDK.GetEventListener()?.NotifyBrokenToken();
                            }
                            else
                            {
                                Debug.Log("Succeed to refresh token when protocol error.");
                                GppSDK.GetLobby().RestoreLobby(true);
                            }
                        }
                    );
                    break;
                case WsCloseCode.Abnormal:
                case WsCloseCode.ServerError:
                case WsCloseCode.ServiceRestart:
                case WsCloseCode.TryAgainLater:
                case WsCloseCode.TlsHandshakeFailure:
                    break;
                default:
                    if (GppSDK.GetSession().IsLoggedIn())
                    {
                        GppSDK.GetLobby().Connect(true);
                    }
                    else
                    {
                        GppSDK.GetIamApi().RefreshAccessToken(result =>
                            {
                                if (result.IsError)
                                {
                                    GppLog.Log("Failed to refresh token when token revoked " + result.Error.Message);
                                    GppSDK.GetSession().ClearSession();
                                    GppSDK.GetEventListener()?.NotifyBrokenToken();
                                }
                                else
                                {
                                    Debug.Log("Succeed to refresh token when token revoked");
                                    GppSDK.GetLobby().RestoreLobby(true);
                                }
                            }
                        );
                    }

                    break;
            }
        }
    }
}