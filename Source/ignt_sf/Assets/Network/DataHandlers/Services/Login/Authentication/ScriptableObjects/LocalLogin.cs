using System;
using Gpp.Constants;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ignt.sports.cricket.network
{
    [CreateAssetMenu(fileName = "LocalLogin", menuName = "NetworkManager/Services/Login/Localogin")]
    public class LocalLogin : AuthenticationSO
    {
        
        public override void Authenticate()
        {
            InitiateLocalLogin();
        }
        private void InitiateLocalLogin()
        {
            var data = Load<AuthData>();
            if (data != null)
            {
                AuthData = data as AuthData;
                return;
            }


            AuthData.UserId = Random.Range(1, 1000000000).ToString();
            Debug.Log($"UserId : {AuthData.UserId}");

            AuthData.AccessToken = Random.Range(1, 1000000000).ToString();
            Debug.Log($"AccessToken : {AuthData.AccessToken}");

            AuthData.DeviceId = Random.Range(1, 1000000000).ToString();
            Debug.Log($"DeviceId : {AuthData.DeviceId}");

            AuthData.KraftonTag = Random.Range(1, 1000000000).ToString();
            Debug.Log($"KraftonTag : {AuthData.KraftonTag}");

            AuthData.PlatformType = PlatformType.Editor;
            Debug.Log($"PlatformType : {AuthData.PlatformType}");

            AuthData.PlatformUserId = Random.Range(1, 1000000000).ToString();
            Debug.Log($"PlatformUserId : {AuthData.PlatformUserId}");

            AuthData.PushEnable = true;
            Debug.Log($"PushEnable : {AuthData.PushEnable}");

            AuthData.NightPushEnable = true;
            Debug.Log($"NightPushEnable : {AuthData.NightPushEnable}");

            AuthData.IsGuest = false;
            Debug.Log($"IsGuest : {AuthData.IsGuest}");

            AuthData.GuestUserId = "";
            Debug.Log($"GuestUserId : {AuthData.GuestUserId}");

            AuthData.ServerTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            Debug.Log($"ServerTime : {AuthData.ServerTime}");

            AuthData.CountryCode = "IN";
            Debug.Log($"CountryCode : {AuthData.CountryCode}");

            AuthData.LanguageCode = "en";
            Debug.Log($"LanguageCode : {AuthData.LanguageCode}");

            Save(AuthData);
            OnAuthSuccessInvoke();
        }
    }
}