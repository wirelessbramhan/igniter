using System;
using System.Diagnostics;
using Gpp.Constants;
using Gpp.Core;
using Gpp.Extension;
using Gpp.Models;

namespace Gpp.Auth
{
    public class GppUser
    {
        public readonly string UserId;

        public readonly PlatformType PlatformType;

        public readonly string DeviceId;

        public readonly bool IsGuest;

        public readonly bool IsFullKid;

        public readonly string KraftonTag;

        public readonly string PlatformUserId;

        public readonly bool PushEnable;

        public readonly bool NightPushEnable;

        public readonly string LanguageCode;

        public readonly string CountryCode;

        public readonly string GuestUserId;

        public readonly string AccessToken;

        public readonly string GameServerId;

        public readonly string ServerTime;

        public GppUser(TokenData userData)
        {
            if (userData == null)
            {
                return;
            }
            AccessToken = userData.AccessToken;
            GameServerId = userData.GameServerId;
            UserId = userData.UserId;
            ServerTime = userData.ServerTime;

            if (!string.IsNullOrEmpty(userData.PlatformType))
            {
                try
                {
                    PlatformType = Enum.Parse<PlatformType>(userData.PlatformType.CapitalizeFirstLetter(), true);
                }
                catch
                {
                    PlatformType = PlatformType.CustomProviderId;
                }
            }

            DeviceId = GppDeviceProvider.GetGppDeviceId();
            if (PlatformType.IsGuest())
            {
                IsGuest = true;
                GppTokenProvider.TryGetGuestUserId(out var guestUserId);
                GuestUserId = guestUserId ?? string.Empty;
            }
            else
            {
                GuestUserId = "";
            }

            PlatformUserId = userData.PlatformUserId;
            KraftonTag = userData.KraftonTag;
            CountryCode = GppSDK.GetSession().CountryCode;
            LanguageCode = GppSDK.GetSession().LanguageCode;
            PushEnable = userData.PushEnabled;
            NightPushEnable = userData.NightPushEnabled;
            IsFullKid = userData.IsFullKid;
        }
    }
}