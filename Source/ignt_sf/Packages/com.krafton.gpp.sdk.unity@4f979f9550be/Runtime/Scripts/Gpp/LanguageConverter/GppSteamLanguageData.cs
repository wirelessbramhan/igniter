// Steam Language

namespace Gpp.Language
{
    readonly struct FGppSteamLanguage
    {
        public readonly string Code;
        public readonly string Bcp47;

        public FGppSteamLanguage(string Code, string Bcp47)
        {
            this.Code = Code;
            this.Bcp47 = Bcp47;
        }
    }

    partial class GppLanguageData
    {
        public static readonly FGppSteamLanguage[] GppSteamLanguage =
        {
            new FGppSteamLanguage("arabic", "ar"),
            new FGppSteamLanguage("bulgarian", "bg"),
            new FGppSteamLanguage("schinese", "zh-Hans"),
            new FGppSteamLanguage("tchinese", "zh-Hant"),
            new FGppSteamLanguage("czech", "cs"),
            new FGppSteamLanguage("danish", "da"),
            new FGppSteamLanguage("dutch", "nl"),
            new FGppSteamLanguage("english", "en"),
            new FGppSteamLanguage("finnish", "fi"),
            new FGppSteamLanguage("french", "fr"),
            new FGppSteamLanguage("german", "de"),
            new FGppSteamLanguage("greek", "el"),
            new FGppSteamLanguage("hungarian", "hu"),
            new FGppSteamLanguage("italian", "it"),
            new FGppSteamLanguage("japanese", "ja"),
            new FGppSteamLanguage("koreana", "ko"),
            new FGppSteamLanguage("norwegian", "no"),
            new FGppSteamLanguage("polish", "pl"),
            new FGppSteamLanguage("portuguese", "pt"),
            new FGppSteamLanguage("brazilian", "pt-BR"),
            new FGppSteamLanguage("romanian", "ro"),
            new FGppSteamLanguage("russian", "ru"),
            new FGppSteamLanguage("spanish", "es"),
            new FGppSteamLanguage("latam", "es-419"),
            new FGppSteamLanguage("swedish", "sv"),
            new FGppSteamLanguage("thai", "th"),
            new FGppSteamLanguage("turkish", "tr"),
            new FGppSteamLanguage("ukrainian", "uk"),
            new FGppSteamLanguage("vietnamese", "vi"),
        };
    }
}