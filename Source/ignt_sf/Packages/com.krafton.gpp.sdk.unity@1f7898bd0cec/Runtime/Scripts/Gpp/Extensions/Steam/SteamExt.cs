using Gpp.Utils;

namespace Gpp.Extensions.Steam
{
    internal class SteamExt : GppExtension<IGppSteam, SteamExt>
    {
        protected override string TargetClassPath()
        {
            return "Gpp.Extensions.Steam.GppSteam";
        }
        
        protected override string PackageName()
        {
            return "com.krafton.gpp.sdk.unity.steam";
        }
        
        protected override bool IsSupportPlatform()
        {
            return PlatformUtil.IsPc();
        }
    }
}