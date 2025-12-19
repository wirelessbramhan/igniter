using Gpp.Utils;

namespace Gpp.Extensions.Ps5
{
    internal class Ps5Ext : GppExtension<IGppPs5, Ps5Ext>
    {
        protected override string TargetClassPath()
        {
            return "Gpp.Extensions.Ps5.GppPs5";
        }

        protected override bool IsSupportPlatform()
        {
#if UNITY_PS5 && !UNITY_EDITOR
            return true;
#endif
            return false;
        }

        protected override string PackageName()
        {
            return "com.krafton.gpp.sdk.unity.ps5";
        }
    }
}
