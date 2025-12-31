using Gpp.Utils;

namespace Gpp.Extensions.IAP
{
    internal class MobileIapExt : GppExtension<IGppIap, MobileIapExt>
    {
        protected override string TargetClassPath()
        {
            return "Gpp.Extensions.MobileIap.GppMobileIap";
        }

        protected override string PackageName()
        {
            return "com.krafton.gpp.sdk.unity.iap";
        }

        protected override bool IsSupportPlatform()
        {
            return PlatformUtil.IsMobileNotEditor();
        }
    }
}