using System;
using Gpp.Auth;
using Gpp.Constants;
using UnityEngine;

namespace ignt.sports.cricket.network
{
    [System.Serializable]
    public class AuthData
    {
        public string UserId;

        public PlatformType PlatformType;

        public string DeviceId;

        public bool IsGuest;

        public bool IsFullKid;

        public string KraftonTag;

        public string PlatformUserId;

        public bool PushEnable;

        public bool NightPushEnable;

        public string LanguageCode;

        public string CountryCode;

        public string GuestUserId;

        public string AccessToken;

        public string GameServerId;

        public string ServerTime;
    }
}