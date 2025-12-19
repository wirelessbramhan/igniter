using Gpp.Extensions.GoogleAnalytics;
using Gpp.Utils;

namespace Gpp.Extensions.GooglePlayGames
{
    internal class GoogleAnalyticsExt : GppExtension<IGppGoogleAnalytics, GoogleAnalyticsExt>
    {
        protected override string TargetClassPath()
        {
            return "Gpp.Extensions.GoogleAnalytics.GppGoogleAnalytics";
        }

        protected override string PackageName()
        {
            return "com.krafton.gpp.sdk.unity.ga";
        }

        protected override bool IsSupportPlatform()
        {
            return PlatformUtil.IsMobileNotEditor();
        }
    }
}