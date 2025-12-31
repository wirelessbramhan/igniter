using Gpp.Utils;

namespace Gpp.Extensions.Mac
{
    internal class MacExt : GppExtension<IGppMac, MacExt>
    {
        protected override string TargetClassPath()
        {
            return "Gpp.Extensions.Mac.GppMac";
        }

        protected override string PackageName()
        {
            return "com.krafton.gpp.sdk.unity.mac";
        }

        protected override bool IsSupportPlatform()
        {
            return PlatformUtil.IsMac();
        }
    }
}