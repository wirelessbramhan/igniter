using System.Globalization;
using System.Text.RegularExpressions;
using static Gpp.Extension.StringExtension.StringCases;

namespace Gpp.Extension
{
    internal static class StringExtension
    {
        public enum StringCases
        {
            PascalCase,
            CamelCase,
            SnakeCase
        }

        public static string CapitalizeAllLetter(this string key)
        {
            TextInfo tInfo = new CultureInfo("en-US", false).TextInfo;
            return tInfo.ToTitleCase(key);
        }

        public static string CapitalizeFirstLetter(this string str)
        {
            return str[0].ToString().ToUpper() + str[1..].ToLower();
        }

        public static string AddSpaceToString(this string key, StringCases cases)
        {
            switch (cases)
            {
                case PascalCase:
                case CamelCase:
                    return Regex.Replace(key, "(\\B[A-Z])", " $1");
                case SnakeCase:
                    return Regex.Replace(key, "_", " ");
                default:
                    return key;
            }
        }
    }
}