using Gpp.Utils;

namespace Gpp.Extensions.Xbox
{
    internal class XboxExt : GppExtension<IGppXbox, XboxExt>
    {
        protected override string TargetClassPath()
        {
            return "Gpp.Extensions.Xbox.GppXbox";
        }

        protected override string PackageName()
        {
            return "com.krafton.gpp.sdk.unity.xbox";
        }

        protected override bool IsSupportPlatform()
        {
            return PlatformUtil.IsConsole();
        }
    }
}
