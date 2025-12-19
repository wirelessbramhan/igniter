namespace Gpp.Editor
{
    internal enum PackagePlatform
    {
        Mobile,
        PC,
        Console
    }

    internal enum GppPackageType
    {
        IAP,
        GoogleAnalytics,
        FirebasePush,
        Braze,
        GooglePlayGames,
        Steam,
        EpicGames,
        GoogleSignIn,
        XboxPc,
        PS5
    }

    internal abstract class GppPackage
    {
        public const string BaseUrl = "https://github.krafton.com/gpp/gpp-unity-sdk.git";
        public abstract string DisplayName { get; }
        public abstract string PackageName { get; }
        public abstract string Description { get; }
        public abstract PackagePlatform SupportPlatform { get; }
        public string Url => $"{BaseUrl}?path={PackageName}";
    }

    internal class IAP : GppPackage
    {
        public override string DisplayName => "Mobile In-App Purchases";
        public override string PackageName => "com.krafton.gpp.sdk.unity.iap";
        public override string Description => "The package for GPP mobile in-app purchases includes payment modules for Google Play Store, Galaxy Store, and App Store.";
        public override PackagePlatform SupportPlatform => PackagePlatform.Mobile;
    }

    internal class GoogleAnalytics : GppPackage
    {
        public override string DisplayName => "Google Analytics";
        public override string PackageName => "com.krafton.gpp.sdk.unity.ga";
        public override string Description => "This package includes the Google Analytics feature of Firebase.";
        public override PackagePlatform SupportPlatform => PackagePlatform.Mobile;
    }

    internal class FirebasePush : GppPackage
    {
        public override string DisplayName => "Firebase Push";
        public override string PackageName => "com.krafton.gpp.sdk.unity.push.firebase";
        public override string Description => "This package includes the Messaging feature of Firebase. Push notifications can be used on Android and iOS.";
        public override PackagePlatform SupportPlatform => PackagePlatform.Mobile;
    }

    internal class Braze : GppPackage
    {
        public override string DisplayName => "Braze";
        public override string PackageName => "com.krafton.gpp.sdk.unity.braze";
        public override string Description => "This package includes the feature of Braze. Includes push notifications(Android, IOS), in-app messages, and more.";
        public override PackagePlatform SupportPlatform => PackagePlatform.Mobile;
    }

    internal class GooglePlayGames : GppPackage
    {
        public override string DisplayName => "Google Play Games";
        public override string PackageName => "com.krafton.gpp.sdk.unity.gpg";
        public override string Description => "This package includes Google Play Games authentication.";
        public override PackagePlatform SupportPlatform => PackagePlatform.Mobile;
    }

    internal class Steam : GppPackage
    {
        public override string DisplayName => "Steam";
        public override string PackageName => "com.krafton.gpp.sdk.unity.steam";
        public override string Description => "This package includes Steam authentication.";
        public override PackagePlatform SupportPlatform => PackagePlatform.PC;
    }

    internal class GoogleSignIn : GppPackage
    {
        public override string DisplayName => "Google SignIn";
        public override string PackageName => "com.krafton.gpp.sdk.unity.googlesignin";
        public override string Description => "This package includes Google SignIn.";
        public override PackagePlatform SupportPlatform => PackagePlatform.Mobile;
    }

    internal class XboxPc : GppPackage
    {
        public override string DisplayName => "Xbox Pc";
        public override string PackageName => "com.krafton.gpp.sdk.unity.xbox.pc";
        public override string Description => "This package includes Xbox Pc.";
        public override PackagePlatform SupportPlatform => PackagePlatform.PC;
    }

    internal class XboxConsole : GppPackage
    {
        public override string DisplayName => "Xbox Console";
        public override string PackageName => "com.krafton.gpp.sdk.unity.xbox";
        public override string Description => "This package includes Xbox Console.";
        public override PackagePlatform SupportPlatform => PackagePlatform.Console;
    }
    internal class Ps5 : GppPackage
    {
        public override string DisplayName => "PS5";
        public override string PackageName => "com.krafton.gpp.sdk.unity.ps5";
        public override string Description => "This package includes PS5.";
        public override PackagePlatform SupportPlatform => PackagePlatform.Console;
    }
    internal class EpicGames : GppPackage
    {
        public override string DisplayName => "EpicGames";
        public override string PackageName => "com.krafton.gpp.sdk.unity.epic";
        public override string Description => "This package includes EpicGames.";
        public override PackagePlatform SupportPlatform => PackagePlatform.PC;
    }
    internal class MacAppstore : GppPackage
    {
        public override string DisplayName => "MacAppstore";
        public override string PackageName => "com.krafton.gpp.sdk.unity.mac";
        public override string Description => "This package includes Mac Appstore.";
        public override PackagePlatform SupportPlatform => PackagePlatform.PC;
    }
}