using System.Collections.Generic;
using Gpp;
using Gpp.Auth;
using Gpp.Constants;
using Gpp.Core;
using Gpp.Models;
using UnityEngine;

namespace GppSample
{
    public partial class SampleAuth : MonoBehaviour
    {
        #region Step 1. 로그인

        /// <summary>
        /// GPP 로그인 메소드 입니다. 만약 이전에 로그인 했다면 자동로그인 됩니다.
        /// GPP login method. If the user has logged in previously, automatic login is performed.
        /// </summary>
        public void Login()
        {
            GppSDK.AutoLogin(result =>
            {
                if (!result.IsError)
                {
                    OnLoginSuccess(result.Value);
                }
                else
                {
                    ManualLogin();
                }
            }, error =>
            {
                ManualLogin();
            });
        }
        private void ManualLogin()
        {
            PlatformType platformType;
#if UNITY_ANDROID
            platformType = PlatformType.Android;
#elif UNITY_IOS
            platformType = PlatformType.iOS;
#elif UNITY_STANDALONE_WIN
            platformType = PlatformType.Steam;
#else
            platformType = PlatformType.Device;
#endif
            
            GppSDK.Login(result =>
                {
                    if (!result.IsError)
                    {
                        OnLoginSuccess(result.Value);
                    }
                    else
                    {
                        OnLoginFailure(result.Error);
                    }
                }, platformType
            );
        }

        private void OnLoginSuccess(GppUser userInfo)
        {
            Debug.Log($"UserId : {userInfo.UserId}");
            Debug.Log($"AccessToken : {userInfo.AccessToken}");
            Debug.Log($"DeviceId : {userInfo.DeviceId}");
            Debug.Log($"KraftonTag : {userInfo.KraftonTag}");
            Debug.Log($"CountryCode : {userInfo.CountryCode}");
            Debug.Log($"LanguageCode : {userInfo.LanguageCode}");
            Debug.Log($"PlatformType : {userInfo.PlatformType.ToString()}");
            Debug.Log($"PlatformUserId : {userInfo.PlatformUserId}");
            Debug.Log($"Push Enable : {userInfo.PushEnable}");
            Debug.Log($"NightPush Enable : {userInfo.NightPushEnable}");
            Debug.Log($"IsGuest : {userInfo.IsGuest}");
            Debug.Log($"GuestUserId : {userInfo.GuestUserId}");
            Debug.Log($"Server Time : {userInfo.ServerTime}");
            // 로그인에 성공했으므로 게임에 진입합니다.
            // Login is successful, proceed to enter the game.
        }

        private void OnLoginFailure(Error error)
        {
            Debug.Log($"Login Failed Code : {error.Code}");
            Debug.Log($"Login Failed Message : {error.Message}");
            // 로그인에 실패했으므로 유저에게 로그인 실패 팝업을 노출하고 재로그인을 유도합니다.
            // After successful login, set the User Property information for Google Analytics.
        }

        #endregion

        #region Step 2. 로그인(Device)

        public void DeviceLogin()
        {
            GppSDK.Login(result =>
                {
                    if (!result.IsError)
                    {
                        Debug.Log($"UserId : {result.Value.UserId}");
                        Debug.Log($"AccessToken : {result.Value.AccessToken}");
                        Debug.Log($"DeviceId : {result.Value.DeviceId}");
                        Debug.Log($"KraftonTag : {result.Value.KraftonTag}");
                        Debug.Log($"CountryCode : {result.Value.CountryCode}");
                        Debug.Log($"LanguageCode : {result.Value.LanguageCode}");
                        Debug.Log($"PlatformType : {result.Value.PlatformType.ToString()}");
                        Debug.Log($"PlatformUserId : {result.Value.PlatformUserId}");
                        Debug.Log($"Push Enable : {result.Value.PushEnable}");
                        Debug.Log($"NightPush Enable : {result.Value.NightPushEnable}");
                        Debug.Log($"IsGuest : {result.Value.IsGuest}");
                        Debug.Log($"GuestUserId : {result.Value.GuestUserId}");
                    }
                    else
                    {
                        Debug.Log($"Login Failed Code : {result.Error.Code}");
                        Debug.Log($"Login Failed Message : {result.Error.Message}");
                    }
                }, PlatformType.Device
            );
        }

        #endregion

        #region Step 6. 게임 서버 점검 상태 확인

        /// <summary>
        /// GPP에 등록된 게임 서버들의 점검 상태를 확인할 수 있습니다.
        /// This method checks the maintenance status of game servers registered in GPP.
        /// </summary>
        public void CheckGameServersMaintenance()
        {
            GppSDK.CheckGameServersMaintenance(result =>
            {
                if (!result.IsError)
                {
                    List<GameServerMaintenance> servers = result.Value.Maintenances;
                    foreach (var server in servers)
                    {
                        Debug.Log($"Name : {server.Name} UnderMaintenance? : {server.UnderMaintenance}");
                    }
                    // 게임 UI에 노출하여 유저가 게임 서버 상태를 확인하고 선택할 수 있게 제공합니다.
                    // Displays the game server statuses on the UI, allowing users to check and select a server.
                }
                else
                {
                    Debug.Log($"CheckGameServersMaintenance Failed Code : {result.Error.Code}");
                    Debug.Log($"CheckGameServersMaintenance Failed Message : {result.Error.Message}");
                    // 유저에게 실패 팝업을 노출하고 재시도를 유도합니다.
                    // Displays a failure popup to the user and prompts them to retry.
                }
            });
        }

        #endregion

        #region Step 7. 게임 서버 설정

        /// <summary>
        /// Gpp 토큰 정보에 GameServerId를 추가하고 토큰을 재발급 받습니다.
        /// Adds a GameServerId to the Gpp token information and reissues the token.
        /// </summary>
        public void SetGameServerId(string gameServerId)
        {
            GppSDK.SetGameServerId(gameServerId, result =>
                {
                    if (!result.IsError)
                    {
                        Debug.Log($"UserId : {result.Value.UserId}");
                        Debug.Log($"AccessToken : {result.Value.AccessToken}");
                        Debug.Log($"DeviceId : {result.Value.DeviceId}");
                        Debug.Log($"KraftonTag : {result.Value.KraftonTag}");
                        Debug.Log($"CountryCode : {result.Value.CountryCode}");
                        Debug.Log($"LanguageCode : {result.Value.LanguageCode}");
                        Debug.Log($"PlatformType : {result.Value.PlatformType.ToString()}");
                        Debug.Log($"PlatformUserId : {result.Value.PlatformUserId}");
                        Debug.Log($"Push Enable : {result.Value.PushEnable}");
                        Debug.Log($"NightPush Enable : {result.Value.NightPushEnable}");
                        Debug.Log($"IsGuest : {result.Value.IsGuest}");
                        Debug.Log($"GuestUserId : {result.Value.GuestUserId}");
                        Debug.Log($"GameServerId : {result.Value.GameServerId}");
                        // 전달 받은 유저 정보를 참고하여 게임 정보를 업데이트 합니다.
                        // Use the received user information to update the game data.
                    }
                    else
                    {
                        Debug.Log($"SetGameServerId Failed Code : {result.Error.Code}");
                        Debug.Log($"SetGameServerId Failed Message : {result.Error.Message}");
                        // 게임 서버 설정에 실패했기 때문에 유저에게 알림 팝업을 노출하고 재시도를 유도합니다.
                        // Notify the user of the failure with a popup and prompt them to retry.
                    }
                }
            );
        }

        #endregion

        public void CheckGppMaintenance()
        {
            GppSDK.CheckMaintenance(result =>
            {
                if (result.IsError)
                {
                    if (result.Error.Code == Gpp.Core.ErrorCode.NotFound)
                        Debug.Log("Gpp server not in maintenance");
                    else
                    {
                        Debug.Log($"CheckMaintenance Failed Code : {result.Error.Code}");
                        Debug.Log($"CheckMaintenance Failed Message : {result.Error.Message}");
                    }
                    // Gpp.Core.ErrorCode.NotFound (404) 일 경우 서버는 점검 중이지 않습니다.
                    // If Gpp.Core.ErrorCode.NotFound (404), the server not in maintenance.
                }
                else
                {
                    Debug.Log("Gpp server under maintenance");
                    Debug.Log($"maintenance title : {result.Value.Maintenance.Title}");
                    Debug.Log($"maintenance detail : {result.Value.Maintenance.Detail}");
                    Debug.Log($"maintenance current time : {result.Value.Maintenance.CurrentTime}");
                    Debug.Log($"maintenance start date : {result.Value.Maintenance.StartDate}");
                    Debug.Log($"maintenance end date : {result.Value.Maintenance.EndDate}");
                    Debug.Log($"maintenance display period type : {result.Value.Maintenance.DisplayPeriodType}");
                    Debug.Log($"maintenance display start time : {result.Value.Maintenance.DisplayStartDate}");
                    Debug.Log($"maintenance display end time : {result.Value.Maintenance.DisplayEndDate}");
                    Debug.Log($"maintenance external url : {result.Value.Maintenance.ExternalUrl}");

                    // 점검 확인 api가 성공하면 점검중입니다.
                    // If the CheckMaintenance is success, it is under maintenance.
                }
            });
        }
    }
}