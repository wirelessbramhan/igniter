using System.Collections.Generic;
using Gpp.Constants;
using Gpp.Localization;

namespace Gpp.Extension
{
    internal static class GppExtension
    {
        public static bool IsGuest(this PlatformType type)
        {
            return type is PlatformType.Device or PlatformType.Guest;
        }

        public static string Localise(this string key, string language = null, Dictionary<string, string> symbolsToReplace = null)
        {
            return LocalizationManager.Localise(key, language ?? GppSDK.GetSession().LanguageCode, symbolsToReplace);
        }

        public static string ConvertUpperStoreString(this StoreType storeType)
        {
            return storeType switch
            {
                StoreType.GooglePlayStore => "GOOGLE",
                StoreType.GalaxyStore => "GALAXY",
                StoreType.AppStore => "APPLE",
                StoreType.SteamStore => "STEAM",
                StoreType.EpicGamesStore => "EPIC",
                _ => "GOOGLE"
            };
        }
    }
}