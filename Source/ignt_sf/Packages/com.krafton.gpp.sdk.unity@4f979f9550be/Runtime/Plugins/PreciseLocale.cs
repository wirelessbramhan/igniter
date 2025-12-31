#if UNITY_IOS && !IOS_SIMULATOR
#define IOS_BUILD
#elif UNITY_GAMECORE_SCARLETT || UNITY_PS5
using System.Globalization;
#endif

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Globalization;
using Gpp.Language;

public class PreciseLocale : System.Object
{
	private interface PlatformBridge
	{
		string GetRegion();

		string GetLanguage();

		string GetLanguageID();

		string GetCurrencyCode();

		string GetCurrencySymbol();
	}

	private class EditorBridge : PlatformBridge
	{
		public string GetRegion()
		{
			return "US";
		}

		public string GetLanguage()
		{
			return "en";
		}

		public string GetLanguageID()
		{
			return "en_US";
		}

		public string GetCurrencyCode()
		{
			return "USD";
		}

		public string GetCurrencySymbol()
		{
			return "$";
		}
	}

	private static PlatformBridge _platform;

	private static PlatformBridge platform
	{
		get
		{
			if (_platform == null)
			{
#if UNITY_ANDROID && !UNITY_EDITOR
				_platform = new PreciseLocaleAndroid();

#elif IOS_BUILD && !UNITY_EDITOR
				_platform = new PreciseLocaleiOS();

#elif UNITY_STANDALONE_OSX && !UNITY_EDITOR
				_platform = new PreciseLocaleOSX();

#elif UNITY_STANDALONE_WIN && !UNITY_EDITOR
				_platform = new PreciseLocaleWindows();

#elif UNITY_GAMECORE_SCARLETT && !UNITY_EDITOR
				_platform = new PreciseLocaleXboxScarlett();
#elif UNITY_PS5 && !UNITY_EDITOR
				_platform = new PreciseLocalePS5();
#else
                _platform = new EditorBridge();
#endif
            }
			return _platform;
		}
	}

	public static string GetRegion()
	{
		return platform.GetRegion();
	}

	public static string GetLanguageID()
	{
		return platform.GetLanguageID();
	}

	public static string GetLanguage()
	{
		return platform.GetLanguage();
	}

	public static string GetCurrencyCode()
	{
		return platform.GetCurrencyCode();
	}

	public static string GetCurrencySymbol()
	{
		return platform.GetCurrencySymbol();
	}

#if UNITY_ANDROID && !UNITY_EDITOR
	private class PreciseLocaleAndroid: PlatformBridge {
		//private static AndroidJavaClass _preciseLocale = new AndroidJavaClass("com.kokosoft.preciselocale.PreciseLocale");
		private static AndroidJavaClass _preciseLocale = new AndroidJavaClass("com.krafton.commonlibrary.LocaleHelper");
        private static AndroidJavaObject _currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");

		public string GetRegion() {
			return _preciseLocale.CallStatic<string>("getRegion", _currentActivity);                                 
		}

		public string GetLanguage() {
			return _preciseLocale.CallStatic<string>("getLanguage", _currentActivity);
		}

		public string GetLanguageID() {
			return _preciseLocale.CallStatic<string>("getLanguageID", _currentActivity);
		}

		public string GetCurrencyCode() {
			return _preciseLocale.CallStatic<string>("getCurrencyCode", _currentActivity);
		}

		public string GetCurrencySymbol() {
			return _preciseLocale.CallStatic<string>("getCurrencySymbol", _currentActivity);
		}

	}
#endif

#if IOS_BUILD && !UNITY_EDITOR
	private class PreciseLocaleiOS: PlatformBridge {

		[DllImport ("__Internal")]
		private static extern string _getRegion();

		[DllImport ("__Internal")]
		private static extern string _getLanguageID();

		[DllImport ("__Internal")]
		private static extern string _getLanguage();

		[DllImport ("__Internal")]
		private static extern string _getCurrencyCode();

		[DllImport ("__Internal")]
		private static extern string _getCurrencySymbol();

		public string GetRegion() {
			return _getRegion();
		}

		public string GetLanguage() {
			return _getLanguage();
		}

		public string GetLanguageID() {
			return _getLanguageID();
		}

		public string GetCurrencyCode() {
			return _getCurrencyCode();
		}

		public string GetCurrencySymbol() {
			return _getCurrencySymbol();
		}

	}
#endif

#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
	private class PreciseLocaleOSX: PlatformBridge {

		[DllImport ("__Internal")]
		private static extern string gppCommon_getDeviceCountry();

		[DllImport ("__Internal")]
		private static extern string gppCommon_getDeviceLanguage();
		public string GetRegion()
        {
            return gppCommon_getDeviceCountry();
        }

        public string GetLanguage()
        {
            return gppCommon_getDeviceLanguage();
        }

        public string GetLanguageID()
        {
            return "en_US";
        }

        public string GetCurrencyCode()
        {
            return "USD";
        }

        public string GetCurrencySymbol()
        {
            return "$";
        }
	}
#endif

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
	private class PreciseLocaleWindows : PlatformBridge
    {
        public string GetLanguage()
        {
            return CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
        }

        public string GetLanguageID()
        {
            return GetLanguage() + "_" + GetRegion();
        }

        public string GetRegion()
        {
            return RegionInfo.CurrentRegion.TwoLetterISORegionName;
        }

        public string GetCurrencyCode()
        {
            return string.Empty;
        }

        public string GetCurrencySymbol()
        {
            return string.Empty;
        }
    }
#endif

#if UNITY_GAMECORE_SCARLETT && !UNITY_EDITOR
	private class PreciseLocaleXboxScarlett : PlatformBridge
	{
		private CultureInfo _currentCulture;

		public PreciseLocaleXboxScarlett()
		{
			_currentCulture = CultureInfo.CurrentCulture;
		}

		public string GetRegion()
		{
			try
			{
				RegionInfo regionInfo = new RegionInfo(_currentCulture.Name);
				return regionInfo.TwoLetterISORegionName;
			}
			catch
			{
				string[] parts = _currentCulture.Name.Split('-', '_');
				if (parts.Length > 1)
				{
					return parts[1].ToUpper();
				}
				return "US";
			}
		}

		public string GetLanguage()
		{
			try
			{
				return _currentCulture.TwoLetterISOLanguageName;
			}
			catch
			{
				return "en";
			}
		}

		public string GetLanguageID()
		{
			try
			{
				return _currentCulture.Name.Replace('-', '_');
			}
			catch
			{
				return "en_US";
			}
		}

		public string GetCurrencyCode()
		{
			try
			{
				RegionInfo regionInfo = new RegionInfo(_currentCulture.Name);
				return regionInfo.ISOCurrencySymbol;
			}
			catch
			{
				return "USD";
			}
		}

		public string GetCurrencySymbol()
		{
			try
			{
				return _currentCulture.NumberFormat.CurrencySymbol;
			}
			catch
			{
				return "$";
			}
		}
	}
#endif

#if UNITY_PS5
	private class PreciseLocalePS5 : PlatformBridge
	{
        private CultureInfo _currentCulture;

        public PreciseLocalePS5()
        {
            _currentCulture = CultureInfo.CurrentCulture;
        }

        public string GetRegion()
        {
            try
            {
                RegionInfo regionInfo = new RegionInfo(_currentCulture.Name);
                return regionInfo.TwoLetterISORegionName;
            }
            catch
            {
                string[] parts = _currentCulture.Name.Split('-', '_');
                if (parts.Length > 1)
                {
                    return parts[1].ToUpper();
                }
                return "US";
            }
        }

        public string GetLanguage()
        {
            try
            {
                return _currentCulture.TwoLetterISOLanguageName;
            }
            catch
            {
                return "en";
            }
        }

        public string GetLanguageID()
        {
            try
            {
                return _currentCulture.Name.Replace('-', '_');
            }
            catch
            {
                return "en_US";
            }
        }

        public string GetCurrencyCode()
        {
            try
            {
                RegionInfo regionInfo = new RegionInfo(_currentCulture.Name);
                return regionInfo.ISOCurrencySymbol;
            }
            catch
            {
                return "USD";
            }
        }

        public string GetCurrencySymbol()
        {
            try
            {
                return _currentCulture.NumberFormat.CurrencySymbol;
            }
            catch
            {
                return "$";
            }
        }
    }
#endif
}