using Gpp;
using Gpp.Constants;
using UnityEngine;

namespace ignt.sports.cricket.network
{
    [CreateAssetMenu(fileName = "KOSAuthSO", menuName = "Scriptable Objects/KOSAuthSO")]
    public class KOSAuthSO : AuthenticationSO
    {
        public override void Authenticate()
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
                        // 로그인에 성공했으므로 게임에 진입합니다.
                        // Login is successful, proceed to enter the game.
                    }
                    else
                    {
                        Debug.Log($"Login Failed Code : {result.Error.Code}");
                        Debug.Log($"Login Failed Message : {result.Error.Message}");
                        // 로그인에 실패했으므로 유저에게 로그인 실패 팝업을 노출하고 재로그인을 유도합니다.
                        // Login failed, prompt the user with a login failure popup and suggest re-login.
                    }
                }, PlatformType.Device
            );
        }
    }
}
