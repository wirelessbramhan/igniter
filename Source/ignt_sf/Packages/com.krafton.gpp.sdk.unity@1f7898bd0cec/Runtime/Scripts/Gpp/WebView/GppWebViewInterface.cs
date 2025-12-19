#if UNITY_ANDROID
using UnityEngine;
using System;

namespace Gpp.WebView
{
    public class GppWebViewInterface : AndroidJavaProxy
    {
        public GppWebViewInterface() : base("com.krafton.gppwebview.GppWebViewComplete"){}

        internal Action<string, string> webViewComplete;

        public void webViewCompleteReceived(string resultCode, string data)
        {
            webViewComplete?.Invoke(resultCode, data);
        }

    }
}
#endif