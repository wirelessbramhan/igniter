using System;
using Newtonsoft.Json;

[Serializable]
public class ExtensionConfigs
{
    [Serializable]
    public class GoogleAnalyticsConfig
    {
        [JsonProperty("project_id")]
        public string ProjectId;

        [JsonProperty("web_client_id")]
        public string WebClientId;

        [JsonProperty("api_key")]
        public string ApiKey;

        [JsonProperty("android_app_id")]
        public string AndroidAppId;

        [JsonProperty("ios_app_id")]
        public string IosAppId;
    }

    [Serializable]
    public class FirebasePushConfig
    {
        [JsonProperty("enable_auto_permission")]
        public bool EnableAutoRequestPermission;

        [JsonProperty("project_id")]
        public string ProjectId;

        [JsonProperty("sender_id")]
        public string SenderId;

        [JsonProperty("api_key")]
        public string ApiKey;

        [JsonProperty("android_app_id")]
        public string AndroidAppId;

        [JsonProperty("ios_app_id")]
        public string IosAppId;
    }

    [Serializable]
    public class GooglePlayGamesConfig
    {
        [JsonProperty("web_client_id")]
        public string WebClientId;
    }

    [Serializable]
    public class SteamConfig
    {
        [JsonProperty("steam_app_id")]
        public string SteamAppId;
    }

    [Serializable]
    public class EpicGamesConfig
    {
        [JsonProperty("epic_product_id")]
        public string EpicProductId;
        [JsonProperty("epic_sandbox_id")]
        public string EpicSandboxId;
        [JsonProperty("epic_deployment_id")]
        public string EpicDeploymentId;
        [JsonProperty("epic_client_id")]
        public string EpicClientId;
        [JsonProperty("epic_client_secret")]
        public string EpicClientSecret;
    }

    [Serializable]
    public class BrazeConfig
    {
        [JsonProperty("android_api_key")]
        public string AndroidApiKey;

        [JsonProperty("android_endpoint")]
        public string AndroidEndpoint;

        [JsonProperty("ios_api_key")]
        public string IosApiKey;

        [JsonProperty("ios_endpoint")]
        public string IosEndpoint;

        [JsonProperty("sender_id")]
        public string SenderId;
    }

    [Serializable]
    public class GoogleSignInConfig
    {
        [JsonProperty("server_client_id")]
        public string ServerClientId;
    }

    [Serializable]
    public class XboxConfig
    {
        [JsonProperty("scid")]
        public string Scid;
    }

    [Serializable]
    public class PS5Config
    {
        [JsonProperty("client_id")]
        public string ClientId;
    }
}