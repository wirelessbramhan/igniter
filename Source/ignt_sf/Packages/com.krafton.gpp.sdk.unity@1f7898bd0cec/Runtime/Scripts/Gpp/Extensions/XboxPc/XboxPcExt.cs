using Gpp.Utils;

namespace Gpp.Extensions.XboxPc
{
    internal class XboxPcExt : GppExtension<IGppXboxPc, XboxPcExt>
    {
        protected override string TargetClassPath()
        {
            return "Gpp.Extensions.XboxPc.GppXboxPc";
        }

        protected override string PackageName()
        {
            return "com.krafton.gpp.sdk.unity.xbox.pc";
        }

        protected override bool IsSupportPlatform()
        {
            return PlatformUtil.IsPc();
        }
    }
}