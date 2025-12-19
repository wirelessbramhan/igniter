using System;
using Gpp.Models;
using Gpp.Telemetry;
using TMPro;
using UnityEngine;

namespace Gpp.WebView
{
    internal static class GppWebView
    {
        public delegate void GppWebViewDelegate(GppWebViewResponse response);

#if UNITY_IOS
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void openWebView(string jsonData, GppWebViewCallback.WebViewCallback handler);
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void openWebViewAuthService(string url, string callbackScheme, GppWebViewCallback.WebViewCallback handler);
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void openExternalBrowser(string url);
#elif UNITY_ANDROID
        private static AndroidJavaClass webViewClass = new("com.krafton.gppwebview.GppWebViewHandler");
        private static GppWebViewInterface gppWebViewInterface = new();
        private static AndroidJavaObject GetUnityPlayerActivity()
        {
            using AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            return unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }
#endif

        public static void OpenGppWebView(bool useCustomTab, string url, string callbackScheme, bool useUnSupportBrowser, GppWebViewDelegate callback = null)
        {
            if (useCustomTab)
            {
                OpenWebViewForAuth(url, callbackScheme, useCustomTab, callback);
            }
            else
            {
                OpenWebView(url, callbackScheme, callback);
            }
        }

        public static void OpenWebView(GppWebViewData webviewData, GppWebViewDelegate callback = null)
        {
            var webViewDataJson = JsonUtility.ToJson(webviewData);
#if UNITY_EDITOR
            Application.OpenURL(webviewData.url);
#elif UNITY_ANDROID
            gppWebViewInterface.webViewComplete = (resultCode, data) => {
                Core.GppSyncContext.RunOnUnityThread(() =>
                {
                    callback?.Invoke(new GppWebViewResponse(Enum.Parse<GppWebViewResultCode>(resultCode), data));
                });
            };
            webViewClass.CallStatic("openWebView", GetUnityPlayerActivity(), webViewDataJson, gppWebViewInterface);
#elif UNITY_IOS
            GppWebViewCallback.OnWebViewCallback = (resultCode, data) => {
                callback(new GppWebViewResponse(Enum.Parse<GppWebViewResultCode>(resultCode), data));
            };
            openWebView(webViewDataJson, GppWebViewCallback.OnWebViewCallbackReceived);
#endif
        }

        public static void OpenWebView(string url, string callbackScheme, GppWebViewDelegate callback = null)
        {
            GppWebViewData webviewData = GppWebViewData.RecommendedWebViewData();
            webviewData.url = url;
            webviewData.schemes = new string[] { callbackScheme };

            var webViewDataJson = JsonUtility.ToJson(webviewData);
#if UNITY_EDITOR
            Application.OpenURL(webviewData.url);
#elif UNITY_ANDROID
            gppWebViewInterface.webViewComplete = (resultCode, data) => {
                Core.GppSyncContext.RunOnUnityThread(() =>
                {
                    callback?.Invoke(new GppWebViewResponse(Enum.Parse<GppWebViewResultCode>(resultCode), data));
                });
            };
            webViewClass.CallStatic("openWebView", GetUnityPlayerActivity(), webViewDataJson, gppWebViewInterface);
#elif UNITY_IOS
            GppWebViewCallback.OnWebViewCallback = (resultCode, data) => {
                callback(new GppWebViewResponse(Enum.Parse<GppWebViewResultCode>(resultCode), data));
            };
            openWebView(webViewDataJson, GppWebViewCallback.OnWebViewCallbackReceived);
#endif
        }


        public static void OpenWebViewForAuth(string url, string callbackScheme, bool useUnSupportBrowser, GppWebViewDelegate callback = null)
        {
#if UNITY_EDITOR
            Application.OpenURL(url);
#elif UNITY_ANDROID
            gppWebViewInterface.webViewComplete = (resultCode, data) => {
                Core.GppSyncContext.RunOnUnityThread(() =>
                {
                    callback?.Invoke(new GppWebViewResponse(Enum.Parse<GppWebViewResultCode>(resultCode), data));
                });
            };
            webViewClass.CallStatic("openCustomTab", GetUnityPlayerActivity(), url, callbackScheme, useUnSupportBrowser, gppWebViewInterface);
#elif UNITY_IOS
            GppWebViewCallback.OnWebViewCallback = (resultCode, data) => {
                callback(new GppWebViewResponse(Enum.Parse<GppWebViewResultCode>(resultCode), data));
            };
            openWebViewAuthService(url, callbackScheme, GppWebViewCallback.OnWebViewCallbackReceived);
#endif
        }

        public static bool CanUseKidSupportBrowser()
        {
#if UNITY_EDITOR
            return true;
#elif UNITY_ANDROID
            return webViewClass.CallStatic<bool>("canUseKidSupportBrowser", GetUnityPlayerActivity());
#elif UNITY_IOS
            return true;
#else
            return true;
#endif
        }

        public static string GetDefaultBrowserPackageName()
        {
#if UNITY_EDITOR
            return "unknown";
#elif UNITY_ANDROID
            var name = webViewClass.CallStatic<string>("defaultBrowserPackageName", GetUnityPlayerActivity()) ?? "unknown";
            return name;
#elif UNITY_IOS
            return "com.apple.mobilesafari";
#else
            return "unknown";
#endif
        }
    }
}
