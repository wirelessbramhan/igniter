using Gpp;
using UnityEngine;

namespace GppSample
{
    public partial class SampleAuth
    {
        #region Step 8. 본인 인증 확인

        /// <summary>
        /// 로그인된 유저가 본인 인증을 했는지 확인합니다. 만약 되어있지 않으면 진행하고 결과를 반환합니다. 또한, 연동으로 인해 계정 정보가 변동될 수 있습니다.
        /// Checks if the logged-in user has completed identity verification. If not, proceeds with verification and returns the result. Also, GPP User may change due to linking.
        /// </summary>
        public void CheckEligibility()
        {
            GppSDK.CheckEligibility(result =>
                {
                    if (!result.IsError)
                    {
                        Debug.Log($"Verified? : {result.Value.Verified}");
                        if (result.Value.IsError)
                        {
                            Debug.Log($"CheckEligibility Failed : {result.Value.ErrorMessage}");
                        }
                        else
                        {
                            Debug.Log($"CheckEligibility Success");
                        }

                        if (result.Value.User != null)
                        {
                            Debug.Log($"User has changed");
                            Debug.Log($"UserId : {result.Value.User.UserId}");
                            Debug.Log($"AccessToken : {result.Value.User.AccessToken}");
                            Debug.Log($"DeviceId : {result.Value.User.DeviceId}");
                            Debug.Log($"KraftonTag : {result.Value.User.KraftonTag}");
                            Debug.Log($"CountryCode : {result.Value.User.CountryCode}");
                            Debug.Log($"LanguageCode : {result.Value.User.LanguageCode}");
                            Debug.Log($"PlatformType : {result.Value.User.PlatformType.ToString()}");
                            Debug.Log($"PlatformUserId : {result.Value.User.PlatformUserId}");
                            Debug.Log($"Push Enable : {result.Value.User.PushEnable}");
                            Debug.Log($"NightPush Enable : {result.Value.User.NightPushEnable}");
                            Debug.Log($"IsGuest : {result.Value.User.IsGuest}");
                            Debug.Log($"GuestUserId : {result.Value.User.GuestUserId}");
                            Debug.Log($"Server Time : {result.Value.User.ServerTime}");
                        }
                    }
                    else
                    {
                        Debug.Log($"CheckEligibility Failed Code : {result.Error.Code}");
                        Debug.Log($"CheckEligibility Failed Message : {result.Error.Message}");
                    }
                }
            );
        }

        #endregion
    }
}