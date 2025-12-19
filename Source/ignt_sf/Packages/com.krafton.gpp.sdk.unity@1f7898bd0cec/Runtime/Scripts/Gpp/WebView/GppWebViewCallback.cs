#if UNITY_IOS
using System;
using AOT;

namespace Gpp.WebView
{
    internal class GppWebViewCallback
    {
        internal delegate void WebViewCallback(string resultCode, string message);
        
        internal static WebViewCallback OnWebViewCallback;
        
        [MonoPInvokeCallback(typeof(WebViewCallback))]
        internal static void OnWebViewCallbackReceived(string resultCode, string message)
        {
            OnWebViewCallback?.Invoke(resultCode, message);
        }
    }
}
#endif