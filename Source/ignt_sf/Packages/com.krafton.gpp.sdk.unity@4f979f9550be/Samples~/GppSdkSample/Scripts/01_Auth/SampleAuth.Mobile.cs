using System;
using Gpp;
using Gpp.Constants;
using UnityEngine;

namespace GppSample
{
    public partial class SampleAuth
    {
        #region Step 3. 로그아웃

        /// <summary>
        /// GPP 로그아웃 메소드 입니다. 로그인된 유저가 Guest인 경우 Guest 정보가 사라지지 않습니다.
        /// GPP logout method. If the logged-in user is a guest, the guest information will not be removed.
        /// </summary>
        public void Logout()
        {
            GppSDK.Logout(result =>
                {
                    if (!result.IsError)
                    {
                        Debug.Log("Logout Success!");
                        // 로그아웃에 성공했으므로 다시 GppSDK.Login()을 호출하여 유저에게 로그인을 유도합니다.
                        // Logout successful, prompt the user to log in again by calling GppSDK.Login().
                    }
                    else
                    {
                        Debug.Log($"Logout Failed Code : {result.Error.Code}");
                        Debug.Log($"Logout Failed Message : {result.Error.Message}");
                        // 로그아웃에 실패했으므로 유저에게 로그아웃 실패 팝업을 노출하고 재시도를 유도합니다.
                        // Logout failed, display a logout failure popup to the user and suggest retrying.
                    }
                }
            );
        }

        #endregion

        #region Step 4. Krafton ID 연동

        /// <summary>
        /// Guest 계정을 Krafton ID 계정으로 연동하는 메소드입니다. Guest로 로그인된 상태에서만 호출되어야 합니다.
        /// Method to link a Guest account to a Krafton ID account. This should only be called when logged in as a Guest.
        /// </summary>
        public void LinkKraftonId(string platformType)
        {
            if (Enum.TryParse(platformType, out PlatformType type))
            {
                GppSDK.LinkKraftonId(type, result =>
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
                            // 계정 연동이 되었으므로 전달된 GPP 유저 정보를 이용하여 접속된 게임 유저 정보를 업데이트할 수 있습니다.
                            // Account linking successful, you can update the connected game user information with the provided GPP user details.
                        }
                        else
                        {
                            Debug.Log($"LinkKraftonId Failed Code : {result.Error.Code}");
                            Debug.Log($"LinkKraftonId Failed Message : {result.Error.Message}");
                            // 계정 연동에 실패했으므로 유저에게 실패 팝업을 노출하고 재시도를 유도합니다.
                            // Account linking failed, display a failure popup to the user and suggest retrying.
                        }
                    }
                );
            }
        }

        #endregion

        #region Step 5. 계정 삭제

        /// <summary>
        /// 게임 계정 삭제(탈퇴) 메소드 입니다.
        /// Game account deletion method.
        /// </summary>
        public void DeleteAccount()
        {
            // 계정 탈퇴를 진행하기 전에 GppSDK.GetAccountDeletionConfig()를 호출하여 유저에게 탈퇴 유예 기간 알림을 노출 할 수 있습니다.
            // Before proceeding with account deletion, you can call GppSDK.GetAccountDeletionConfig() to notify the user of the account deletion grace period.

            GppSDK.DeleteAccount(result =>
                {
                    if (!result.IsError)
                    {
                        Debug.Log("DeleteAccount Success!");
                        // 게임 계정 삭제가 되었으므로 게임을 종료하거나 재로그인을 시도해주세요.
                        // The game account has been deleted, so please exit the game or attempt to re-login.
                    }
                    else
                    {
                        Debug.Log($"DeleteAccount Failed Code : {result.Error.Code}");
                        Debug.Log($"DeleteAccount Failed Message : {result.Error.Message}");
                        // 게임 계정 삭제가 실패했으므로 유저에게 실패 팝업을 노출하고 재시도를 유도합니다.
                        // Account deletion failed, display a failure popup to the user and suggest retrying.
                    }
                }
            );
        }

        #endregion
    }
}