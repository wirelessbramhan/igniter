using System;
using System.Collections.Generic;
using Gpp.LanguageConverter;

namespace Gpp.Language
{
    internal static class GppLanguageConverter
    {
        private static bool IsGppLanguageConverterInitialized()
        {
            // GppDefaultLanguage가 설정되어있으면 GppSupportedLanguageList도 초기화된 것이라고 보면 된다. //
            if (GppDefaultLanguage != null && GppDefaultLanguage.IsValid())
                return true;

            return false;
        }

        public static bool InitGppLanguageConverter()
        {
            if (IsGppLanguageConverterInitialized())
            {
                // 초기화를 중복으로 실행하여 행여라도 Update한 데이터가 리셋되는 것을 방지한다. //
                return true;
            }

            if (!GppUpdateSupportedLanguage(GppLanguageData.GppSupportedLanguageData))
            {
                return false;
            }

            Dictionary<string, string[]> LanguageMappingMap = new Dictionary<string, string[]>();
            foreach (FGppLanguageMapping LanguageMappingElem in GppLanguageData.GppLanguageMapping)
            {
                LanguageMappingMap[LanguageMappingElem.Region] = LanguageMappingElem.Languages;
            }

            if (!GppUpdateLanguageMapping(LanguageMappingMap))
            {
                return false;
            }

            if (!GppUpdateDefaultLanguage(GPP_DEFAULT_LANGUAGE))
            {
                return false;
            }

            return true;
        }

        private static bool GppIsSupportedLanguage(FGppBcp47DataEx gppBcp47Data)
        {
            if (gppBcp47Data == null || !gppBcp47Data.IsValid())
                return false;

            if (GppSupportedLanguageMap == null)
                return false;

            if (!GppSupportedLanguageMap.ContainsKey(gppBcp47Data.GetLanguageCode()))
                return false;

            foreach (FGppBcp47DataEx SupportedLanguage in GppSupportedLanguageMap[gppBcp47Data.GetLanguageCode()])
            {
                if (gppBcp47Data.IsEqual(SupportedLanguage))
                    return true;
            }

            return false;
        }

        // Update 함수를 호출하려면 그 전에 InitGppLanguageConverter()를 호출하여 초기값을 설정해 놓아야한다. //
        // 지원 언어의 포맷은 BCP 47 규격의 2-4-2 혹은 2-4-3 문자열이어야한다. //
        // 만약 기존의 리스트에서 지원하던 언어가 새로 적용하는 SupportedLanguageList에서 빠져있다면 기본 언어 설정 및 언어 맵핑 설정이 꼬일 수 있다. //
        // 기존에 지원하던 언어가 빠져있고, 해당 언어가 기본 언어 혹은 언어 맵핑에 들어있으면 해당 데이터들도 같이 업데이트해줘야한다. //
        public static bool GppUpdateSupportedLanguage(string[] SupportedLanguageList)
        {
            if (SupportedLanguageList == null)
                return false;

            Dictionary<string, List<FGppBcp47DataEx>> NewGppSupportedLanguageMap = new Dictionary<string, List<FGppBcp47DataEx>>();

            foreach (string SupportedLanguage in SupportedLanguageList)
            {
                FGppBcp47DataEx gppBcp47Data = FGppBcp47Converter<FGppBcp47DataEx>.ParseFromBcp47String(SupportedLanguage);
                if (gppBcp47Data == null || !gppBcp47Data.IsValid())
                {
                    // 지원 언어 목록에 하나라도 포맷에 맞지 않는 문자열이 있으면 오류 처리한다. //
                    return false;
                }

                if (NewGppSupportedLanguageMap.ContainsKey(gppBcp47Data.GetLanguageCode()))
                {
                    NewGppSupportedLanguageMap[gppBcp47Data.GetLanguageCode()].Add(gppBcp47Data);
                }
                else
                {
                    List<FGppBcp47DataEx> SupportedLanguageArray = new List<FGppBcp47DataEx>();
                    SupportedLanguageArray.Add(gppBcp47Data);
                    NewGppSupportedLanguageMap.Add(gppBcp47Data.GetLanguageCode(), SupportedLanguageArray);
                }
            }

            GppSupportedLanguageMap = NewGppSupportedLanguageMap;

            return true;
        }

        // 지역코드는 ISO 3166-1 alpha-2 혹은 M49의 Area값이어야하고 맵핑된 언어는 BCP 47규격의 2-4-2 혹은 2-4-3 문자열이어야한다. //
        public static bool GppUpdateLanguageMapping(Dictionary<string, string[]> LanguageMappingMap)
        {
            if (LanguageMappingMap == null)
                return false;

            Dictionary<string, List<FGppBcp47DataEx>> newGppLanguageMappingMap = new Dictionary<string, List<FGppBcp47DataEx>>();

            foreach (KeyValuePair<string, string[]> Elem in LanguageMappingMap)
            {
                string RegionCode = FGppBcp47DataEx.GetValidRegionCode(Elem.Key);
                if (String.IsNullOrEmpty(RegionCode))
                {
                    // 언어 맵핑 목록에 하나라도 잘못된 문자열이 있으면 오류 처리한다. //
                    return false;
                }

                List<FGppBcp47DataEx> LanguageArray = new List<FGppBcp47DataEx>();
                foreach (string Language in Elem.Value)
                {
                    FGppBcp47DataEx gppBcp47Data = FGppBcp47Converter<FGppBcp47DataEx>.ParseFromBcp47String(Language);
                    if (gppBcp47Data == null || !gppBcp47Data.IsValid())
                    {
                        // 언어 맵핑 목록에 하나라도 포맷에 맞지 않는 문자열이 있으면 오류 처리한다. //
                        return false;
                    }

                    LanguageArray.Add(gppBcp47Data);
                }

                newGppLanguageMappingMap.Add(RegionCode, LanguageArray);
            }

            GppLanguageMappingMap = newGppLanguageMappingMap;

            return true;
        }

        // 기본 언어는 지원 언어에 포함되어있어야한다. //
        public static bool GppUpdateDefaultLanguage(string DefaultLanguage)
        {
            FGppBcp47DataEx gppBcp47Data = FGppBcp47Converter<FGppBcp47DataEx>.ParseFromBcp47String(DefaultLanguage);
            if (!GppIsSupportedLanguage(gppBcp47Data))
            {
                // 기본 언어가 지원하지 않는 언어라면 오류 처리한다. //
                return false;
            }

            GppDefaultLanguage = gppBcp47Data;

            return true;
        }

        /*
        * example json
        * 
        * {
        *	"supported_language_list" : [
        *		"en",
        *		"en-US",
        *		...
        *	]
        * }
        *
        * {
        *	"language_mapping_list" : {
        *		"CN" : ["zh-Hans", ...],
        *		"TW" : ["zh-Hant", ...],
        *		...
        *	}
        * }
        *
        * example json으로 된 값을 서버에서 받으면 파싱하여 Update 함수를 호출한다.
        * 변경 내용에 지원 언어가 포함되어있는 경우에는 GppUpdateSupportedLanguage()를 가장 먼저 호출한다.
        */

        // Update 없이 초기 설정값만을 사용하는 경우에는 굳이 InitGppLanguageConverter()를 호출하지 않아도 된다. //
        // 초기화되어있지 않다면 자동으로 초기 설정값으로 초기화한다. //
        public static string GppGetLanguage(int SceLanguageCode)
        {
            return GppGetLanguageFromGPPBcp47Data(FGppBcp47Converter<FGppBcp47DataEx>.ParseFromSceLanguageCode(SceLanguageCode));
        }

        public static string GppGetLanguage(string LanguageString)
        {
            return GppGetLanguageFromGPPBcp47Data(FGppBcp47Converter<FGppBcp47DataEx>.ParseFromLanguageString(LanguageString));
        }

        public static string GppGetLanguage(string LanguageCode, string RegionCode)
        {
            return GppGetLanguageFromGPPBcp47Data(FGppBcp47Converter<FGppBcp47DataEx>.ParseFromLanguageAndRegionCode(LanguageCode, RegionCode));
        }

        // 이하는 테스트용 함수들이므로 테스트 용도로만 사용하도록 한다. //
        public static string GppGetBcp47String(int SceLanguageCode)
        {
            return FGppBcp47Converter<FGppBcp47DataEx>.ParseFromSceLanguageCode(SceLanguageCode)?.ToString();
        }

        public static string GppGetBcp47String(string LanguageString)
        {
            return FGppBcp47Converter<FGppBcp47DataEx>.ParseFromLanguageString(LanguageString)?.ToString();
        }

        public static string GppGetLanguageFromBcp47String(string gppBcp47String)
        {
            return GppGetLanguageFromGPPBcp47Data(FGppBcp47Converter<FGppBcp47DataEx>.ParseFromBcp47String(gppBcp47String));
        }

        private static string GppGetLanguageFromGPPBcp47Data(FGppBcp47DataEx gppBcp47Data)
        {
            if (!InitGppLanguageConverter())
            {
                return null;
            }

            if (gppBcp47Data == null || !gppBcp47Data.IsValid())
            {
                // 언어를 인식하지 못해서 유효한 데이터를 생성하지 못했다면 기본 언어를 반환한다. //
                return GppDefaultLanguage.ToString();
            }

            if (!GppSupportedLanguageMap.ContainsKey(gppBcp47Data.GetLanguageCode()))
            {
                // 지원 언어에 없는 언어가 들어왔으면 그냥 기본 언어를 반환한다. //
                return GppDefaultLanguage.ToString();
            }

            FGppBcp47DataEx MatchOneLanguage = null;
            FGppBcp47DataEx MatchTwoByRegionCodeLanguage = null;
            FGppBcp47DataEx MatchTwoByScriptCodeLanguage = null;
            FGppBcp47DataEx MatchThreeLanguage = null;

            List<FGppBcp47DataEx> SupportedLanguageArray = GppSupportedLanguageMap[gppBcp47Data.GetLanguageCode()];

            MatchOneLanguage = gppBcp47Data.FindMatchOneLanguage(SupportedLanguageArray);
            if (MatchOneLanguage != null && SupportedLanguageArray.Count == 1)
            {
                // LanguageCode가 일치하고 해당 지원 언어에 다른 지역코드 등의 설정이 없다면 바로 반환한다. //
                return MatchOneLanguage?.ToString();
            }

            MatchTwoByRegionCodeLanguage = gppBcp47Data.FindMatchTwoByRegionCodeLanguage(SupportedLanguageArray);
            MatchTwoByScriptCodeLanguage = gppBcp47Data.FindMatchTwoByScriptCodeLanguage(SupportedLanguageArray);
            MatchThreeLanguage = gppBcp47Data.FindMatchThreeLanguage(SupportedLanguageArray);

            // 정보가 가장 많이 일치하는 지원 언어를 반환한다. //
            // RegionCode의 일치보다 ScriptCode의 일치를 우전한다. //
            if (MatchThreeLanguage != null)
            {
                return MatchThreeLanguage?.ToString();
            }

            if (MatchTwoByScriptCodeLanguage != null)
            {
                return MatchTwoByScriptCodeLanguage?.ToString();
            }

            if (MatchTwoByRegionCodeLanguage != null)
            {
                return MatchTwoByRegionCodeLanguage?.ToString();
            }

            // LanguageCode 이외의 정보가 일치하는 지원 언어가 없다면 맵핑 정보를 찾는다. //
            FGppBcp47DataEx MappingLanguage = gppBcp47Data.FindRegionMappingLanguage(GppLanguageMappingMap);
            if (MappingLanguage != null)
            {
                MatchTwoByRegionCodeLanguage = MappingLanguage?.FindMatchTwoByRegionCodeLanguage(SupportedLanguageArray);
                MatchTwoByScriptCodeLanguage = MappingLanguage?.FindMatchTwoByScriptCodeLanguage(SupportedLanguageArray);
                MatchThreeLanguage = MappingLanguage?.FindMatchThreeLanguage(SupportedLanguageArray);

                if (MatchThreeLanguage != null)
                {
                    return MatchThreeLanguage?.ToString();
                }

                if (MatchTwoByScriptCodeLanguage != null)
                {
                    return MatchTwoByScriptCodeLanguage?.ToString();
                }

                if (MatchTwoByRegionCodeLanguage != null)
                {
                    return MatchTwoByRegionCodeLanguage?.ToString();
                }
            }

            // 맵핑 정보로도 일치하는 지원 언어를 찾지 못했다면 LanguageCode만이라도 일치하는 지원 언어를 반환한다.  //
            if (MatchOneLanguage != null)
            {
                return MatchOneLanguage?.ToString();
            }

            // LanguageCode가 일치하는 지원 언어는 있지만 지원 언어의 조건을 만족시키지 못한다면 기본 언어를 반환한다.  //
            // 지원 언어에 LanguageCode만 설정된 언어가 없을 경우에 이쪽으로 올 수 있다. //
            return GppDefaultLanguage.ToString();
        }

        private const string GPP_DEFAULT_LANGUAGE = "en";
        private static Dictionary<string, List<FGppBcp47DataEx>> GppSupportedLanguageMap; // key : language code
        private static Dictionary<string, List<FGppBcp47DataEx>> GppLanguageMappingMap;   // key : region code
        private static FGppBcp47DataEx GppDefaultLanguage;
    }

    partial struct FGppIso639
    {
        public static bool operator ==(FGppIso639 Val1, FGppIso639 Val2)
        {
            if (String.Equals(Val1.Alpha2, Val2.Alpha2, StringComparison.Ordinal))
            {
                return true;
            }

            return false;
        }

        public static bool operator !=(FGppIso639 Val1, FGppIso639 Val2)
        {
            if (String.Equals(Val1.Alpha2, Val2.Alpha2, StringComparison.Ordinal))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(Object Obj)
        {
            if (GetType() != Obj.GetType())
            {
                return false;
            }

            if (String.Equals(Alpha2, ((FGppIso639)Obj).Alpha2, StringComparison.Ordinal))
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }

    partial struct FGppIso3166
    {
        public static bool operator ==(FGppIso3166 Val1, FGppIso3166 Val2)
        {
            if (String.Equals(Val1.Alpha2, Val2.Alpha2, StringComparison.Ordinal))
            {
                return true;
            }

            return false;
        }

        public static bool operator !=(FGppIso3166 Val1, FGppIso3166 Val2)
        {
            if (String.Equals(Val1.Alpha2, Val2.Alpha2, StringComparison.Ordinal))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(Object Obj)
        {
            if (GetType() != Obj.GetType())
            {
                return false;
            }

            if (String.Equals(Alpha2, ((FGppIso3166)Obj).Alpha2, StringComparison.Ordinal))
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }

    class FGppBcp47DataEx : FGppBcp47Data
    {
        // 지원하는 언어를 찾는데 사용하는 함수들 추가. //

        public bool IsEqual(FGppBcp47DataEx gppBcp47Data)
        {
            if (GetIso639() != gppBcp47Data.GetIso639())
            {
                return false;
            }

            if (!String.Equals(GetIso15924(), gppBcp47Data.GetIso15924(), StringComparison.Ordinal))
            {
                return false;
            }

            if (GetIso3166() != gppBcp47Data.GetIso3166())
            {
                return false;
            }

            if (!String.Equals(GetM49(), gppBcp47Data.GetM49(), StringComparison.Ordinal))
            {
                return false;
            }

            return true;
        }

        // SupportedLanguage에 언어코드만 존재하고 이것이 일치할 경우. //
        public bool CheckMatchOne(FGppBcp47DataEx SupportedLanguage)
        {
            if (!IsValid() || SupportedLanguage == null || !SupportedLanguage.IsValid())
            {
                return false;
            }

            if (!String.IsNullOrEmpty(SupportedLanguage.GetIso15924()) ||
                SupportedLanguage.GetIso3166() != null ||
                !String.IsNullOrEmpty(SupportedLanguage.GetM49()))
            {
                return false;
            }

            if (GetIso639() == SupportedLanguage.GetIso639())
            {
                return true;
            }

            return false;
        }

        public FGppBcp47DataEx FindMatchOneLanguage(List<FGppBcp47DataEx> SupportedLanguageArray)
        {
            if (SupportedLanguageArray == null)
                return null;

            foreach (FGppBcp47DataEx SupportedLanguage in SupportedLanguageArray)
            {
                if (CheckMatchOne(SupportedLanguage))
                {
                    return SupportedLanguage;
                }
            }

            return null;
        }

        // SupportedLanguage에 언어코드와 지역코드만 존재하고 이 2개가 모두 일치할 경우. //
        // Iso3166이나 M49 중 하나라도 있드면 지역코드가 있는 것으로 간주한다. //
        public bool CheckMatchTwoByRegionCode(FGppBcp47DataEx SupportedLanguage)
        {
            if (!IsValid() || SupportedLanguage == null || !SupportedLanguage.IsValid())
            {
                return false;
            }

            if (!String.IsNullOrEmpty(SupportedLanguage.GetIso15924()))
            {
                return false;
            }

            if (GetIso639() == SupportedLanguage.GetIso639())
            {
                if (GetIso3166() != null && SupportedLanguage.GetIso3166() != null)
                {
                    // SupportedLanguage에 CountryCode(Iso3166)가 있다면 CountryCode가 일치해야한다. //
                    if (GetIso3166() == SupportedLanguage.GetIso3166())
                    {
                        return true;
                    }
                }
                else if (!String.IsNullOrEmpty(SupportedLanguage.GetM49()))
                {
                    if (!String.IsNullOrEmpty(GetM49()))
                    {
                        if (String.Equals(GetM49(), SupportedLanguage.GetM49(), StringComparison.Ordinal))
                        {
                            return true;
                        }
                    }
                    else if (GetIso3166() != null)
                    {
                        // SupportedLanguage에 AreaCode(M49)가 있고 현재 언어의 지역이 해당 Area에 포함될 경우 일치하는 것으로 간주한다. //
                        if (String.Equals(SupportedLanguage.GetM49(), GetIso3166()?.IntermediateRegionCode, StringComparison.Ordinal) ||
                            String.Equals(SupportedLanguage.GetM49(), GetIso3166()?.SubRegionCode, StringComparison.Ordinal) ||
                            String.Equals(SupportedLanguage.GetM49(), GetIso3166()?.RegionCode, StringComparison.Ordinal))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        public FGppBcp47DataEx FindMatchTwoByRegionCodeLanguage(List<FGppBcp47DataEx> SupportedLanguageArray)
        {
            if (SupportedLanguageArray == null)
                return null;

            foreach (FGppBcp47DataEx SupportedLanguage in SupportedLanguageArray)
            {
                if (CheckMatchTwoByRegionCode(SupportedLanguage))
                {
                    return SupportedLanguage;
                }
            }

            return null;
        }

        // SupportedLanguage에 언어코드와 스크립트코드만 존재하고 이 2개가 모두 일치할 경우. //
        public bool CheckMatchTwoByScriptCode(FGppBcp47DataEx SupportedLanguage)
        {
            if (!IsValid() || SupportedLanguage == null || !SupportedLanguage.IsValid())
            {
                return false;
            }

            if (String.IsNullOrEmpty(SupportedLanguage.GetIso15924()) ||
                SupportedLanguage.GetIso3166() != null ||
                !String.IsNullOrEmpty(SupportedLanguage.GetM49()))
            {
                return false;
            }

            if (GetIso639() == SupportedLanguage.GetIso639() &&
                String.Equals(GetIso15924(), SupportedLanguage.GetIso15924(), StringComparison.Ordinal))
                return true;

            return false;
        }

        public FGppBcp47DataEx FindMatchTwoByScriptCodeLanguage(List<FGppBcp47DataEx> SupportedLanguageArray)
        {
            if (SupportedLanguageArray == null)
                return null;

            foreach (FGppBcp47DataEx SupportedLanguage in SupportedLanguageArray)
            {
                if (CheckMatchTwoByScriptCode(SupportedLanguage))
                {
                    return SupportedLanguage;
                }
            }

            return null;
        }

        // SupportedLanguage에 언어코드, 스크립트코드, 지역코드 모든 값이 존재하고 이것이 전부 일치할 경우. //
        public bool CheckMatchThree(FGppBcp47DataEx SupportedLanguage)
        {
            if (!IsValid() || SupportedLanguage == null || !SupportedLanguage.IsValid())
            {
                return false;
            }

            if (GetIso639() != SupportedLanguage.GetIso639())
            {
                return false;
            }

            if (String.IsNullOrEmpty(SupportedLanguage.GetIso15924()) ||
                !String.Equals(GetIso15924(), SupportedLanguage.GetIso15924(), StringComparison.Ordinal))
            {
                return false;
            }

            if (SupportedLanguage.GetIso3166() != null &&
                GetIso3166() == SupportedLanguage.GetIso3166())
            {
                return true;
            }

            if (!String.IsNullOrEmpty(SupportedLanguage.GetM49()) &&
                String.Equals(GetM49(), SupportedLanguage.GetM49(), StringComparison.Ordinal))
            {
                return true;
            }

            return false;
        }

        public FGppBcp47DataEx FindMatchThreeLanguage(List<FGppBcp47DataEx> SupportedLanguageArray)
        {
            if (SupportedLanguageArray == null)
                return null;

            foreach (FGppBcp47DataEx SupportedLanguage in SupportedLanguageArray)
            {
                if (CheckMatchThree(SupportedLanguage))
                {
                    return SupportedLanguage;
                }
            }

            return null;
        }

        public FGppBcp47DataEx FindRegionMappingLanguage(Dictionary<string, List<FGppBcp47DataEx>> LanguageMappingMap)
        {
            if (LanguageMappingMap == null)
                return null;

            if (GetIso3166() != null)
            {
                if (LanguageMappingMap.ContainsKey(GetIso3166()?.Alpha2))
                {
                    List<FGppBcp47DataEx> LanguageArray = LanguageMappingMap[GetIso3166()?.Alpha2];
                    foreach (FGppBcp47DataEx Language in LanguageArray)
                    {
                        if (GetIso639() == Language.GetIso639())
                        {
                            return Language;
                        }
                    }
                }

                if (LanguageMappingMap.ContainsKey(GetIso3166()?.IntermediateRegionCode))
                {
                    List<FGppBcp47DataEx> LanguageArray = LanguageMappingMap[GetIso3166()?.IntermediateRegionCode];
                    foreach (FGppBcp47DataEx Language in LanguageArray)
                    {
                        if (GetIso639() == Language.GetIso639())
                        {
                            return Language;
                        }
                    }
                }

                if (LanguageMappingMap.ContainsKey(GetIso3166()?.SubRegionCode))
                {
                    List<FGppBcp47DataEx> LanguageArray = LanguageMappingMap[GetIso3166()?.SubRegionCode];
                    foreach (FGppBcp47DataEx Language in LanguageArray)
                    {
                        if (GetIso639() == Language.GetIso639())
                        {
                            return Language;
                        }
                    }
                }

                if (LanguageMappingMap.ContainsKey(GetIso3166()?.RegionCode))
                {
                    List<FGppBcp47DataEx> LanguageArray = LanguageMappingMap[GetIso3166()?.RegionCode];
                    foreach (FGppBcp47DataEx Language in LanguageArray)
                    {
                        if (GetIso639() == Language.GetIso639())
                        {
                            return Language;
                        }
                    }
                }
            }
            else if (!String.IsNullOrEmpty(GetM49()))
            {
                if (LanguageMappingMap.ContainsKey(GetM49()))
                {
                    List<FGppBcp47DataEx> LanguageArray = LanguageMappingMap[GetM49()];
                    foreach (FGppBcp47DataEx Language in LanguageArray)
                    {
                        if (GetIso639() == Language.GetIso639())
                        {
                            return Language;
                        }
                    }
                }
            }

            return null;
        }

        public static string GetValidRegionCode(string RegionCode)
        {
            if (String.IsNullOrEmpty(RegionCode))
                return null;

            switch (RegionCode.Length)
            {
                case 2:
                {
                    FToken Token = new FToken { Str = RegionCode, Type = EStringType.EST_Alphabet };
                    FGppIso3166? Iso3166 = FindFromIso3166(Token);
                    if (Iso3166 != null)
                    {
                        return Iso3166?.Alpha2;
                    }

                    break;
                }
                case 3:
                {
                    FToken Token = new FToken { Str = RegionCode, Type = EStringType.EST_Number };
                    return FindFromM49(Token);
                }
            }

            return null;
        }
    }
}