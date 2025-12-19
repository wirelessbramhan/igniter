// Language Mapping

namespace Gpp.Language
{
    readonly struct FGppLanguageMapping
    {
        public readonly string Region;
        public readonly string[] Languages;

        public FGppLanguageMapping(string Region, string[] Languages)
        {
            this.Region = Region;
            this.Languages = Languages;
        }
    }

    partial class GppLanguageData
    {
        public static readonly FGppLanguageMapping[] GppLanguageMapping =
        {
            new FGppLanguageMapping("419", new[] { "es-MX", }),
            new FGppLanguageMapping("AS", new[] { "en-US", }),
            new FGppLanguageMapping("GU", new[] { "en-US", }),
            new FGppLanguageMapping("MP", new[] { "en-US", }),
            new FGppLanguageMapping("PR", new[] { "en-US", }),
            new FGppLanguageMapping("UM", new[] { "en-US", }),
            new FGppLanguageMapping("VI", new[] { "en-US", }),
            new FGppLanguageMapping("CN", new[] { "zh-Hans", }),
            new FGppLanguageMapping("SG", new[] { "zh-Hans", }),
            new FGppLanguageMapping("HK", new[] { "zh-Hant", }),
            new FGppLanguageMapping("MO", new[] { "zh-Hant", }),
            new FGppLanguageMapping("TW", new[] { "zh-Hant", }),
        };
    }
}