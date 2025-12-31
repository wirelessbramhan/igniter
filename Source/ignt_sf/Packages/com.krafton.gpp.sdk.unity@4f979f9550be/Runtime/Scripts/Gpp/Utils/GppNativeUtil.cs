using System;
using Gpp.Log;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Gpp.Utils
{
    internal static class GppNativeUtil
    {
        public const string InstallerGooglePlay = "com.android.vending";
        public const string InstallerOneStoreSkt = "com.skt.skaf.A000Z00040";
        public const string InstallerOneStoreKt = "com.kt.olleh.storefront";
        public const string InstallerOneStoreLgu = "android.lgt.appstore";
        public const string InstallerOneStoreLguPlus = "com.lguplus.appstore";
        public const string InstallerGalaxyStore = "com.sec.android.app.samsungapps";

        public const string SourceGooglePlay = "GooglePlayStore";
        public const string SourceOneStore = "OneStore";
        public const string SourceGalaxyStore = "GalaxyStore";
        public const string SourceNotSupported = "NotSupported";
        public const string SourceNone = "None";
        public const string SourceIllegal = "illegal";

        private static string carrierName = string.Empty;

#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string gppCommon_getCarrierName();
        private static string GetNativeCarrierName()
        {
            return gppCommon_getCarrierName();
        }
#elif UNITY_ANDROID && !UNITY_EDITOR
        private static string GetNativeCarrierName()
        {
            string carrier = string.Empty;
            using AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var unityPlayerActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
            using (AndroidJavaClass androidJavaObject = new AndroidJavaClass("com.krafton.commonlibrary.GppCommon"))
            {
                carrier = androidJavaObject.CallStatic<string>("getCarrierInfo", unityPlayerActivity);
            }

            return carrier;
        }
#else
        private static string GetNativeCarrierName()
        {
            return null;
        }
#endif

        public static string GetCarrierName()
        {
            carrierName = GetNativeCarrierName();

            return carrierName;
        }

        public static string GetInstallSource()
        {
            if (Application.isEditor)
            {
                return SourceNone;
            }

            if (Application.platform == RuntimePlatform.Android)
            {
                try
                {
                    using AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    using AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                    using AndroidJavaObject packageManager = activity.Call<AndroidJavaObject>("getPackageManager");
                    string packageName = activity.Call<string>("getPackageName");
                    int sdkVersion = new AndroidJavaClass("android.os.Build$VERSION").GetStatic<int>("SDK_INT");
                    string installingPackageName;

                    if (sdkVersion < 30)
                    {
                        installingPackageName = packageManager.Call<string>("getInstallerPackageName", packageName) ?? "";
                    }
                    else
                    {
                        using AndroidJavaObject installSourceInfo = packageManager.Call<AndroidJavaObject>("getInstallSourceInfo", packageName);
                        installingPackageName = installSourceInfo.Call<string>("getInstallingPackageName");
                    }

                    if (string.IsNullOrEmpty(installingPackageName))
                    {
                        return SourceIllegal;
                    }

                    switch (installingPackageName)
                    {
                        case InstallerGooglePlay: return SourceGooglePlay;
                        case InstallerOneStoreSkt:
                        case InstallerOneStoreKt:
                        case InstallerOneStoreLgu:
                        case InstallerOneStoreLguPlus:
                            return SourceOneStore;
                        case InstallerGalaxyStore:
                            return SourceGalaxyStore;
                        default: return SourceIllegal;
                    }
                }
                catch (Exception)
                {
                    return SourceIllegal;
                }
            }

            return SourceNotSupported;
        }
    }
}
