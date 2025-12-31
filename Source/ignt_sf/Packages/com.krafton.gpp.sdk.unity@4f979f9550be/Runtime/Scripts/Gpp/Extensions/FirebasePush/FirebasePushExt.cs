using Gpp.Utils;

namespace Gpp.Extensions.FirebasePush
{
    internal class FirebasePushExt : GppExtension<IGppFirebasePush, FirebasePushExt>
    {
        protected override string TargetClassPath()
        {
            return "Gpp.Extensions.FirebasePush.GppFirebasePush";
        }

        protected override string PackageName()
        {
            return "com.krafton.gpp.sdk.unity.push.firebase";
        }

        protected override bool IsSupportPlatform()
        {
            return PlatformUtil.IsMobileNotEditor();
        }
    }
}