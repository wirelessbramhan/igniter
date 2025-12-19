using System;
using System.Collections.Generic;
using Gpp.Log;
using Gpp.Utils;
using Newtonsoft.Json;

namespace Gpp.Localization
{
    public class LocalizationManager
    {
        private Dictionary<string, Dictionary<string, string>> _localizations;
        
        private static LocalizationManager Instance { get; } = new();

        public static void Init(Action completeCallback)
        {
            Instance.LoadLocalizationFromFile(completeCallback);
        }

        private void LoadLocalizationFromFile(Action completeCallback)
        {
            GppLog.Log("Load localization from local file...");

            GppSDK.GetCoroutineRunner().Run(
                ResUtil.LoadJsonFromFile("Localization/Localization", (jsonString) =>
                {
                    if (string.IsNullOrEmpty(jsonString))
                    {
                        GppLog.LogWarning("Cannot load localization file from local file system.");
                    }
                    else
                    {
                        GppLog.Log("Successfully loaded localization file from local file system.");
                        Instance._localizations = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonString);
                    }
                    
                    completeCallback?.Invoke();
                }));
        }

        public static string Localise(string key, string language, Dictionary<string, string> symbolsToReplace = null, bool isFallbackEnglish = true)
        {   
            if (Instance._localizations == null || Instance._localizations.ContainsKey(key) == false)
            {
                return $"{key}";
            }

            if (!Instance._localizations[key].ContainsKey(language))
            {
                if (!isFallbackEnglish) return $"{key}";
                language = "en";
            }

            string localised = Instance._localizations[key][language];

            if (symbolsToReplace != null)
            {
                foreach (var pair in symbolsToReplace)
                {
                    localised = localised.Replace(pair.Key, pair.Value);
                }
            }

            return localised;
        }

        public static string GetInAppMarketingText(bool isNight, bool agree)
        {
            return isNight switch
            {
                true when agree => LocalizationKey.NightTimeInAppMarketingConsentAgree,
                false when agree => LocalizationKey.InAppMarketingConsentAgree,
                true when !agree => LocalizationKey.NightTimeInAppMarketingConsentDisagree,
                _ => LocalizationKey.InAppMarketingConsentDisagree
            };
        }
    }
}