using Gpp.Utils;

namespace Gpp.Extensions.GooglePlayGames
{
    internal class GooglePlayGamesExt : GppExtension<IGppGooglePlayGames, GooglePlayGamesExt>
    {
        protected override string TargetClassPath()
        {
            return "Gpp.Extensions.GooglePlayGames.GppGooglePlayGames";
        }

        protected override string PackageName()
        {
            return "com.krafton.gpp.sdk.unity.gpg";
        }

        protected override bool IsSupportPlatform()
        {
            return PlatformUtil.IsAndroid();
        }
    }
}