using System;

namespace Gpp.Utils
{
    internal static class PlatformUtil
    {
        public static bool IsMobile()
        {
#if UNITY_ANDROID || UNITY_IOS
            return true;
#else
            return false;
#endif
        }

        public static bool IsPc()
        {
#if !UNITY_ANDROID && !UNITY_IOS && !UNITY_GAMECORE_SCARLETT && !UNITY_PS5
            return true;
#else
            return false;
#endif
        }

        public static bool IsMac()
        {
#if UNITY_STANDALONE_OSX
            return true;
#endif
            return false;
        }

        public static bool IsWindow()
        {
#if UNITY_STANDALONE_WIN
            return true;
#endif
            return false;
        }

        public static bool IsPcNotEditor()
        {
#if !UNITY_ANDROID && !UNITY_IOS && !UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }

        public static bool IsMobileNotEditor()
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }

        public static bool IsAndroidOrEditor()
        {
#if UNITY_ANDROID
            return true;
#else
            return false;
#endif
        }

        public static bool IsIOSorEditor()
        {
#if UNITY_IOS
            return true;
#else
            return false;
#endif
        }

        public static bool IsAndroid()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }

        public static bool IsIOS()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }

        public static bool IsConsole()
        {
#if (UNITY_GAMECORE_SCARLETT || UNITY_PS5) && !UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }

        public static bool IsSteamDeck()
        {
            return Environment.GetEnvironmentVariable("SteamDeck") == "1";
        }
        public static bool IsConsoleOrEditor()
        {
#if UNITY_GAMECORE_SCARLETT || UNITY_PS5
            return true;
#else
            return false;
#endif
        }


    }
}