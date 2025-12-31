using Gpp.Constants;
using Gpp.Utils;
using UnityEngine;

namespace Gpp.Core
{
    public class GppOptions
    {
        public GppConfig Configuration { get; private set; }
        public string LanguageCode { get; private set; }
        public string CountryCode { get; private set; }
        public StoreType Store { get; private set; }
        public string AppVersion { get; private set; }
        public bool EnableGameServer { get; private set; }

        private GppOptions()
        {
        }

        public static GppOptions GetDefault()
        {
            return new GppOptions
            {
                Configuration = null,
                LanguageCode = null,
                CountryCode = null,
                Store = GetDefaultStore(),
                AppVersion = Application.version,
                EnableGameServer = GppConfigSO.GetActiveConfigFromFile()?.EnableGameServer ?? false
            };
        }

        private static StoreType GetDefaultStore()
        {
            if (PlatformUtil.IsWindow())
                return StoreType.SteamStore;
            if (PlatformUtil.IsAndroidOrEditor())
                return StoreType.GooglePlayStore;
            if (PlatformUtil.IsIOSorEditor())
                return StoreType.AppStore;
            if (PlatformUtil.IsMac())
                return StoreType.AppStore;
            return StoreType.None;
        }

        public class Builder
        {
            private GppConfig configuration;
            private string languageCode;
            private string countryCode;
            private StoreType store = StoreType.None;
            private string appVersion;
            private bool enableGameServer;

            public Builder SetConfiguration(GppConfig config)
            {
                configuration = config;
                return this;
            }

            public Builder SetLanguageCode(string code)
            {
                languageCode = code;
                return this;
            }

            public Builder SetCountryCode(string code)
            {
                countryCode = code;
                return this;
            }

            public Builder SetStore(StoreType storeType)
            {
                store = storeType;
                return this;
            }

            public Builder SetAppVersion(string version)
            {
                appVersion = version;
                return this;
            }

            public Builder SetEnableGameServer(bool enableGameServer)
            {
                this.enableGameServer = enableGameServer;
                return this;
            }

            public GppOptions Build()
            {
                var options = new GppOptions
                {
                    Configuration = configuration,
                    LanguageCode = languageCode,
                    CountryCode = countryCode,
                    Store = store == StoreType.None ? GetDefaultStore() : store,
                    AppVersion = string.IsNullOrEmpty(appVersion) ? Application.version : appVersion,
                    EnableGameServer = enableGameServer
                };
                return options;
            }
        }
    }
}