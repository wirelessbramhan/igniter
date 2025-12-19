using System;
using Gpp.Telemetry;
using Newtonsoft.Json;

[Serializable]
public class GppConfig
{
    [JsonProperty("stage")]
    public string Stage;

    [JsonProperty("namespace")]
    public string Namespace;

    [JsonProperty("client_id")]
    public string ClientId;

    [JsonProperty("base_url")]
    public string BaseUrl;

    [JsonProperty("enable_debug_log")]
    public bool EnableDebugLog;

    [JsonProperty("telemetry_interval_seconds")]
    public int TelemetryIntervalSeconds = TelemetryManager.TELEMETRY_INTERVAL_DEFAULT_SECONDS;

    [JsonProperty("enable_login_another_account")]
    public bool EnableLoginAnotherAccount = true;

    [JsonProperty("extensions")]
    public Extensions Extensions;

    [JsonProperty("enable_game_server")]
    public bool EnableGameServer = false;

    [JsonProperty("enable_ownership_sync")]
    public bool EnableOwnershipSync = true;
}