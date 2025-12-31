using Gpp.Utils;

namespace Gpp.Extensions.Braze
{
    internal class BrazeExt : GppExtension<IGppBraze, BrazeExt>
    {
        protected override string TargetClassPath()
        {
            return "Gpp.Extensions.Braze.GppBraze";
        }

        protected override string PackageName()
        {
            return "com.krafton.gpp.sdk.unity.braze";
        }

        protected override bool IsSupportPlatform()
        {
            return PlatformUtil.IsMobile();
        }
    }
}