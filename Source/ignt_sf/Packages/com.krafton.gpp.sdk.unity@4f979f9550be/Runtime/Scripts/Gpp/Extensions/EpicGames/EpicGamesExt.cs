using Gpp.Utils;

namespace Gpp.Extensions.EpicGames
{
    public class EpicGamesExt : GppExtension<IGppEpicGames, EpicGamesExt>
    {
        protected override string TargetClassPath()
        {
            return "Gpp.Extensions.EpicGames.GppEpic";
        }

        protected override string PackageName()
        {
            return "com.krafton.gpp.sdk.unity.epic";
        }

        protected override bool IsSupportPlatform()
        {
            return PlatformUtil.IsPc();
        }
    }
}