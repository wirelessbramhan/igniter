using Gpp;
using Gpp.Auth;
using Gpp.Constants;
using Gpp.Core;
using ignt.sports.cricket.core;
using UnityEngine;

/// <summary>
/// GPP 로그인 메소드 입니다.
/// </summary>
namespace ignt.sports.cricket.network
{
    [CreateAssetMenu(fileName = "GoogleSignIn", menuName = "NetworkManager/Services/Login/GoogleSignIn")]
    public class GoogleSignInSO : AuthenticationSO
    {
        private GppUser userinfo;


        public override void Authenticate()
        {

            InitiateGoogleLogin();
        }

        

        private void InitiateGoogleLogin()
        {
            Debug.Log("Google Login Start");
            GppSDK.Login(result =>
            {
                if (!result.IsError)
                    OnLoginSuccess(result.Value);
                else
                    OnLoginFailure(result.Error);
            }, PlatformType.Google);
            Debug.Log("Google Login End");
        }

        private void OnLoginSuccess(GppUser userInfo)
        {
            userinfo = userInfo;
            Debug.Log($"UserId : {userInfo.UserId}");
            AuthData.UserId = userInfo.UserId;

            Debug.Log($"AccessToken : {userInfo.AccessToken}");
            AuthData.AccessToken = userInfo.AccessToken;

            Debug.Log($"DeviceId : {userInfo.DeviceId}");
            AuthData.DeviceId = userInfo.DeviceId;

            Debug.Log($"KraftonTag : {userInfo.KraftonTag}");
            AuthData.KraftonTag = userInfo.KraftonTag;

            Debug.Log($"CountryCode : {userInfo.CountryCode}");
            AuthData.CountryCode = userInfo.CountryCode;

            Debug.Log($"LanguageCode : {userInfo.LanguageCode}");
            AuthData.LanguageCode = userInfo.LanguageCode;

            Debug.Log($"PlatformType : {userInfo.PlatformType.ToString()}");
            AuthData.PlatformType = userInfo.PlatformType;

            Debug.Log($"PlatformUserId : {userInfo.PlatformUserId}");
            AuthData.PlatformUserId = userInfo.PlatformUserId;

            Debug.Log($"Push Enable : {userInfo.PushEnable}");
            AuthData.PushEnable = userInfo.PushEnable;

            Debug.Log($"NightPush Enable : {userInfo.NightPushEnable}");
            AuthData.NightPushEnable = userInfo.NightPushEnable;

            Debug.Log($"IsGuest : {userInfo.IsGuest}");
            AuthData.IsGuest = userInfo.IsGuest;

            Debug.Log($"GuestUserId : {userInfo.GuestUserId}");
            AuthData.GuestUserId = userInfo.GuestUserId;

            Debug.Log($"Server Time : {userInfo.ServerTime}");
            AuthData.ServerTime = userInfo.ServerTime;
            // 로그인에 성공했으므로 게임에 진입합니다.
            // Login is successful, proceed to enter the game.

            OnAuthSuccessInvoke();
        }

        private void OnLoginFailure(Error error)
        {
            userinfo = null;
            AuthData = null;
            Debug.Log($"Login Failed Code : {error.Code}");
            Debug.Log($"Login Failed Message : {error.Message}");
            OnAuthSuccessInvoke();
            // 로그인에 실패했으므로 유저에게 로그인 실패 팝업을 노출하고 재로그인을 유도합니다.
            // After successful login, set the User Property information for Google Analytics.
        }
        

        


    }
}