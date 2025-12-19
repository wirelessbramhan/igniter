namespace Gpp.Utils
{
    public class GppPrefsUtil
    {
        private static string GetOptionalVersionKey()
        {
            return $"optional_update_version_{GppSDK.GetConfig()?.ClientId ?? ""}";
        }
        
        public static void SaveOptionalUpdateVersion(string version)
        {
            PrefsUtil.SetString(GetOptionalVersionKey(), version);
        }

        public static void DeleteOptionalUpdateVersion()
        {
            PrefsUtil.DeleteKey(GetOptionalVersionKey());
        }

        public static string GetOptionalUpdateVersion()
        {
            return PrefsUtil.GetString(GetOptionalVersionKey());
        }

        public static bool AvailableOptionalUpdate(string version)
        {
            string savedVersion = GetOptionalUpdateVersion();
            return string.IsNullOrEmpty(savedVersion) || !version.Equals(savedVersion);
        }
    }
}