using System;
using System.Collections.Generic;
using System.Linq;
using Gpp.Language;

namespace Gpp.LanguageConverter
{
    interface IGppBcp47Data
    {
        public bool ParseFromSceLanguageCode(int SceLanguageCode);
        public bool ParseFromLanguageString(string LanguageString);
        public bool ParseFromLanguageAndRegionCode(string LanguageCode, string RegionCode);
        public bool ParseFromBcp47String(string Bcp47String);
    }

    class FGppBcp47Converter<T> where T : class, IGppBcp47Data, new()
    {
        public static T ParseFromSceLanguageCode(int SceLanguageCode)
        {
            T ResData = new T();
            if (ResData.ParseFromSceLanguageCode(SceLanguageCode))
            {
                return ResData;
            }

            return null;
        }

        public static T ParseFromLanguageString(string LanguageString)
        {
            T ResData = new T();
            if (ResData.ParseFromLanguageString(LanguageString))
            {
                return ResData;
            }

            return null;
        }

        public static T ParseFromLanguageAndRegionCode(string LanguageCode, string RegionCode)
        {
            T ResData = new T();
            if (ResData.ParseFromLanguageAndRegionCode(LanguageCode, RegionCode))
            {
                return ResData;
            }

            return null;
        }

        public static T ParseFromBcp47String(string Bcp47String)
        {
            T ResData = new T();
            if (ResData.ParseFromBcp47String(Bcp47String))
            {
                return ResData;
            }

            return null;
        }
    }

    class FGppBcp47Data : IGppBcp47Data
    {
        public FGppBcp47Data()
        {
            Iso639 = null;
            Iso15924 = null;
            Iso3166 = null;
            M49 = null;
        }

        public override string ToString()
        {
            if (Iso639 == null)
            {
                return null;
            }

            string Bcp47String = Iso639?.Alpha2;
            if (!String.IsNullOrEmpty(Iso15924))
            {
                Bcp47String += "-" + Iso15924;
            }

            if (Iso3166 != null)
            {
                Bcp47String += "-" + Iso3166?.Alpha2;
            }
            else if (!String.IsNullOrEmpty(M49))
            {
                Bcp47String += "-" + M49;
            }

            return Bcp47String;
        }

        public string GetLanguageCode()
        {
            if (Iso639 == null)
            {
                return null;
            }

            return Iso639?.Alpha2;
        }

        public string GetScriptCode()
        {
            if (String.IsNullOrEmpty(Iso15924))
            {
                return null;
            }

            return Iso15924;
        }

        public string GetCountryCode()
        {
            if (Iso3166 == null)
            {
                return null;
            }

            return Iso3166?.Alpha2;
        }

        public string GetAreaCode()
        {
            if (String.IsNullOrEmpty(M49))
            {
                return null;
            }

            return M49;
        }

        public bool ParseFromSceLanguageCode(int SceLanguageCode)
        {
            if (SceLanguageCode < 0 || SceLanguageCode >= GppLanguageData.GppSceLangParamMapping.Length)
            {
                return false;
            }

            return ParseFromBcp47String(GppLanguageData.GppSceLangParamMapping[SceLanguageCode]);
        }

        public bool ParseFromLanguageString(string LanguageString)
        {
            List<FToken> TokenArray = GetTokenArray(LanguageString);
            if (TokenArray == null)
                return false;

            List<Tuple<int, FGppIso639>> Iso639MatchList = new List<Tuple<int, FGppIso639>>();
            List<Tuple<int, string>> Iso15924MatchList = new List<Tuple<int, string>>();
            List<Tuple<int, FGppIso3166>> Iso3166MatchList = new List<Tuple<int, FGppIso3166>>();
            List<Tuple<int, string>> M49MatchList = new List<Tuple<int, string>>();

            for (int Index = 0; Index < TokenArray.Count; Index++)
            {
                FToken Token = TokenArray[Index];
                string SteamBcp47 = FindSteamLanguageAndConvertToBcp47(Token);
                if (!String.IsNullOrEmpty(SteamBcp47))
                {
                    bool bParseResult = ParseFromBcp47String(SteamBcp47);
                    if (bParseResult)
                    {
                        return true;
                    }
                }

                FGppIso639? Iso639_ = FindFromIso639(Token);
                if (Iso639_ != null)
                {
                    Iso639MatchList.Add(Tuple.Create(Index, Iso639_.Value));
                }

                string Iso15924_ = FindFromIso15924(Token);
                if (!String.IsNullOrEmpty(Iso15924_))
                {
                    Iso15924MatchList.Add(Tuple.Create(Index, Iso15924_));
                }

                FGppIso3166? Iso3166_ = FindFromIso3166(Token);
                if (Iso3166_ != null)
                {
                    Iso3166MatchList.Add(Tuple.Create(Index, Iso3166_.Value));
                }

                string M49_ = FindFromM49(Token);
                if (!String.IsNullOrEmpty(M49_))
                {
                    M49MatchList.Add(Tuple.Create(Index, M49_));
                }
            }

            // 일치하는 언어코드가 하나도 없으면 나머지 값들에 일치하는 값이 있는지 여부와 상관없이 무시한다. //
            // 국가코드로 언어코드를 유추하는 것도 필요한가? //
            if (Iso639MatchList.Count == 0)
            {
                return false;
            }

            // 일치하는 국가코드가 있다면 혹시 언어코드와 중복으로 인식된 것인지 확인하고 제외한다. //
            if (Iso3166MatchList.Count > 0)
            {
                if (Iso3166MatchList[0].Item1 == Iso639MatchList[0].Item1)
                {
                    if (Iso639MatchList.Count == 1)
                    {
                        // 일치하는 언어코드가 하나 뿐인데 국가코드와 중복으로 인식되었다면 국가코드를 제외한다. //
                        Iso3166MatchList.RemoveAt(0);
                    }
                    else if (Iso3166MatchList.Count == 1)
                    {
                        // 일치하는 언어코드가 둘 이상이고 일치하는 국가코드가 하나 뿐인데 언어코드와 중복으로 인식되었다면 언어코드를 제외한다. //
                        Iso639MatchList.RemoveAt(0);
                    }
                    else
                    {
                        // 나머지 경우라면 BCP47규격의 언어코드-국가코드 순서를 우선해서 국가코드를 제외한다. //
                        Iso3166MatchList.RemoveAt(0);
                    }
                }
            }

            Iso639 = Iso639MatchList[0].Item2;
            if (Iso15924MatchList.Count > 0)
            {
                Iso15924 = Iso15924MatchList[0].Item2;
            }

            if (Iso3166MatchList.Count > 0)
            {
                Iso3166 = Iso3166MatchList[0].Item2;
            }

            if (M49MatchList.Count > 0)
            {
                M49 = M49MatchList[0].Item2;
            }

            return true;
        }

        public bool ParseFromLanguageAndRegionCode(string LanguageCode, string RegionCode)
        {
            return ParseFromLanguageString(LanguageCode + "-" + RegionCode);
        }

        public bool ParseFromBcp47String(string Bcp47String)
        {
            string[] Bcp47SubtagArray = Bcp47StringToBcp47SubtagArray(Bcp47String);
            if (Bcp47SubtagArray == null || Bcp47SubtagArray.Length < 1 || Bcp47SubtagArray.Length > 3)
            {
                return false;
            }

            string LanguageCode = Bcp47SubtagArray[0];
            string ScriptCode = null;
            string CountryCode = null;
            string AreaCode = null;

            if (Bcp47SubtagArray.Length > 1)
            {
                switch (Bcp47SubtagArray[1].Length)
                {
                    case 2:
                    {
                        CountryCode = Bcp47SubtagArray[1];
                        break;
                    }
                    case 3:
                    {
                        AreaCode = Bcp47SubtagArray[1];
                        break;
                    }
                    case 4:
                    {
                        ScriptCode = Bcp47SubtagArray[1];
                        break;
                    }
                    default:
                    {
                        return false;
                    }
                }
            }

            if (Bcp47SubtagArray.Length > 2)
            {
                if (ScriptCode == null || CountryCode != null || AreaCode != null)
                {
                    return false;
                }

                switch (Bcp47SubtagArray[2].Length)
                {
                    case 2:
                    {
                        CountryCode = Bcp47SubtagArray[2];
                        break;
                    }
                    case 3:
                    {
                        AreaCode = Bcp47SubtagArray[2];
                        break;
                    }
                    default:
                    {
                        return false;
                    }
                }
            }

            FGppIso639? Iso639_ = null;
            string Iso15924_ = null;
            FGppIso3166? Iso3166_ = null;
            string M49_ = null;

            if (!String.IsNullOrEmpty(LanguageCode))
            {
                FToken Token = new FToken { Str = LanguageCode, Type = EStringType.EST_Alphabet };
                Iso639_ = FindFromIso639(Token);
                if (Iso639_ == null)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            if (!String.IsNullOrEmpty(ScriptCode))
            {
                FToken Token = new FToken { Str = ScriptCode, Type = EStringType.EST_Alphabet };
                Iso15924_ = FindFromIso15924(Token);
                if (String.IsNullOrEmpty(Iso15924_))
                {
                    return false;
                }
            }

            if (!String.IsNullOrEmpty(CountryCode))
            {
                FToken Token = new FToken { Str = CountryCode, Type = EStringType.EST_Alphabet };
                Iso3166_ = FindFromIso3166(Token);
                if (Iso3166_ == null)
                {
                    return false;
                }
            }

            if (!String.IsNullOrEmpty(AreaCode))
            {
                FToken Token = new FToken { Str = AreaCode, Type = EStringType.EST_Number };
                M49_ = FindFromM49(Token);
                if (String.IsNullOrEmpty(M49_))
                {
                    return false;
                }
            }

            Iso639 = Iso639_;
            Iso15924 = Iso15924_;
            Iso3166 = Iso3166_;
            M49 = M49_;

            return true;
        }

        public bool IsValid()
        {
            if (Iso639 == null)
            {
                return false;
            }

            return true;
        }

        protected static string[] Bcp47StringToBcp47SubtagArray(string GPPBcp47String)
        {
            if (String.IsNullOrEmpty(GPPBcp47String))
                return null;

            return GPPBcp47String.Split('-');
        }

        private FGppIso639? Iso639;
        private string Iso15924;
        private FGppIso3166? Iso3166;
        private string M49;

        protected FGppIso639? GetIso639()
        {
            return Iso639;
        }

        protected string GetIso15924()
        {
            return Iso15924;
        }

        protected FGppIso3166? GetIso3166()
        {
            return Iso3166;
        }

        protected string GetM49()
        {
            return M49;
        }

        protected enum EStringType
        {
            EST_NotDefined = 0,
            EST_Number,
            EST_Alphabet,
            EST_Other,
        }

        protected struct FToken
        {
            public string Str;
            public EStringType Type;
        }

        protected static EStringType CheckTokenCharType(char Ch)
        {
            if ('0' <= Ch && Ch <= '9')
                return EStringType.EST_Number;

            if ('A' <= Ch && Ch <= 'Z')
                return EStringType.EST_Alphabet;

            if ('a' <= Ch && Ch <= 'z')
                return EStringType.EST_Alphabet;

            return EStringType.EST_Other;
        }

        protected static EStringType CheckTokenStringType(char[] Str, ref int Pos)
        {
            EStringType Type = EStringType.EST_NotDefined;
            if (Pos < 0 || Pos >= Str.Length)
            {
                return Type;
            }

            for (; Pos < Str.Length; ++Pos)
            {
                EStringType CharType = CheckTokenCharType(Str[Pos]);
                if (Type == EStringType.EST_NotDefined)
                {
                    Type = CharType;
                }
                else if (Type != CharType)
                {
                    return Type;
                }
            }

            return Type;
        }

        protected static List<FToken> GetTokenArray(string Str)
        {
            if (String.IsNullOrEmpty(Str))
                return null;

            char[] StrArray = Str.ToCharArray();
            List<FToken> TokenArray = new List<FToken>();
            int PrevPos = 0,
                Pos = 0;
            for (EStringType Type = CheckTokenStringType(StrArray, ref Pos);
                 Type != EStringType.EST_NotDefined;
                 PrevPos = Pos, Type = CheckTokenStringType(StrArray, ref Pos))
            {
                if (Type == EStringType.EST_Other) // 숫자 혹은 영문자만으로 이루어진 문자열 이외에는 무시한다. //
                {
                    continue;
                }

                int Len = Pos - PrevPos;

                FToken Token = new FToken();
                Token.Str = new string(Str.Skip(PrevPos).Take(Len).ToArray());
                Token.Type = Type;

                TokenArray.Add(Token);
            }

            return TokenArray;
        }

        protected static FGppIso639? FindFromIso639(FToken Token)
        {
            if (Token.Str.Length != 2 && Token.Str.Length != 3)
            {
                return null;
            }

            // Find from ISO 639
            foreach (FGppIso639 Iso639Elem in GppLanguageData.GPPIso639)
            {
                if (Token.Str.Length == 2)
                {
                    if (String.Equals(Token.Str, Iso639Elem.Alpha2, StringComparison.OrdinalIgnoreCase))
                    {
                        return Iso639Elem;
                    }
                }
                else
                {
                    if (String.Equals(Token.Str, Iso639Elem.Alpha3, StringComparison.OrdinalIgnoreCase) ||
                        String.Equals(Token.Str, Iso639Elem.Alpha3B, StringComparison.OrdinalIgnoreCase))
                    {
                        return Iso639Elem;
                    }
                }
            }

            // Find from deprecated ISO 639
            foreach (FGPPDeprecatedIso639 DeprecatedElem in GppLanguageData.GPPDeprecatedIso639)
            {
                if (String.Equals(Token.Str, DeprecatedElem.Deprecated, StringComparison.OrdinalIgnoreCase))
                {
                    // Find from ISO 639
                    foreach (FGppIso639 Iso639Elem in GppLanguageData.GPPIso639)
                    {
                        if (String.Equals(DeprecatedElem.Current, Iso639Elem.Alpha2, StringComparison.OrdinalIgnoreCase))
                        {
                            return Iso639Elem;
                        }
                    }

                    return null;
                }
            }

            return null;
        }

        protected static string FindFromIso15924(FToken Token)
        {
            if (Token.Str.Length != 4 || Token.Type != EStringType.EST_Alphabet)
            {
                return null;
            }

            // Find from ISO 15924
            foreach (string Iso15924Elem in GppLanguageData.GPPIso15924)
            {
                if (String.Equals(Token.Str, Iso15924Elem, StringComparison.OrdinalIgnoreCase))
                {
                    return Iso15924Elem;
                }
            }

            return null;
        }

        protected static FGppIso3166? FindFromIso3166(FToken Token)
        {
            if (Token.Str.Length != 2 && Token.Str.Length != 3)
            {
                return null;
            }

            // Find from ISO 3166
            if (Token.Type == EStringType.EST_Alphabet)
            {
                foreach (FGppIso3166 Iso3166Elem in GppLanguageData.GPPIso3166)
                {
                    if (Token.Str.Length == 2)
                    {
                        if (String.Equals(Token.Str, Iso3166Elem.Alpha2, StringComparison.OrdinalIgnoreCase))
                        {
                            return Iso3166Elem;
                        }
                    }
                    else
                    {
                        if (String.Equals(Token.Str, Iso3166Elem.Alpha3, StringComparison.OrdinalIgnoreCase))
                        {
                            return Iso3166Elem;
                        }
                    }
                }
            }
            else
            {
                if (Token.Str.Length == 3)
                {
                    int TokenInteger = Int32.Parse(Token.Str);

                    foreach (FGppIso3166 Iso3166Elem in GppLanguageData.GPPIso3166)
                    {
                        if (TokenInteger == Int32.Parse(Iso3166Elem.CountryCode))
                        {
                            return Iso3166Elem;
                        }
                    }
                }
            }

            return null;
        }

        protected static string FindFromM49(FToken Token)
        {
            if (Token.Str.Length != 3 || Token.Type != EStringType.EST_Number)
            {
                return null;
            }

            // Find from M49
            int TokenInteger = Int32.Parse(Token.Str);
            foreach (string M49Elem in GppLanguageData.GPPM49)
            {
                if (TokenInteger == Int32.Parse(M49Elem))
                {
                    return M49Elem;
                }
            }

            return null;
        }

        protected static string FindSteamLanguageAndConvertToBcp47(FToken Token)
        {
            if (Token.Type != EStringType.EST_Alphabet)
            {
                return null;
            }

            // Find from steam language
            foreach (FGppSteamLanguage SteamLanguageElem in GppLanguageData.GppSteamLanguage)
            {
                if (String.Equals(Token.Str, SteamLanguageElem.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return SteamLanguageElem.Bcp47;
                }
            }

            return null;
        }
    }
}