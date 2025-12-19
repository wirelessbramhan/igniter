using System;

namespace Gpp.WebView
{
    public enum GppWebViewRotation
    {
        GPP_ROTATION_BEHIND,
        GPP_ROTATION_LANDSCAPE,
        GPP_ROTATION_PORTRAIT,
        GPP_ROTATION_ALL
    }

    public enum GppWebViewBackgroundColor
    {
        WHITE,
        BLACK
    }
    
    [Serializable]
    public class GppWebViewData
    {
        public string url;
        public string title;
        public string userAgent;
        public string[] schemes;
        public int marginTop;
        public int marginBottom;
        public int marginLeft;
        public int marginRight;
        public bool zoom;
        public bool canGoBack;
        public bool openLinkOutBrowser;
        public GppWebViewRotation rotation;
        public GppWebViewBackgroundColor backgroundColor;

        public static GppWebViewData RecommendedWebViewData()
        {
            var webViewData = new GppWebViewData
            {
                title = "KRAFTON",
                userAgent = "/Unity",
                marginTop = 0,
                marginBottom = 0,
                marginLeft = 0,
                marginRight = 0,
                zoom = true,
                canGoBack = true,
                openLinkOutBrowser = false,
                rotation = GppWebViewRotation.GPP_ROTATION_BEHIND,
                backgroundColor = GppWebViewBackgroundColor.BLACK
            };
            return webViewData;
        }
    }
}