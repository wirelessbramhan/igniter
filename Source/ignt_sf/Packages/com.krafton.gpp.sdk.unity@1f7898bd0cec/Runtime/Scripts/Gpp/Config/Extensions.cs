using System;
using Newtonsoft.Json;

[Serializable]
public class Extensions
{
    [JsonProperty("google_analytics")]
    public ExtensionConfigs.GoogleAnalyticsConfig GoogleAnalytics;

    [JsonProperty("firebase_push")]
    public ExtensionConfigs.FirebasePushConfig FirebasePush;

    [JsonProperty("google_play_games")]
    public ExtensionConfigs.GooglePlayGamesConfig GooglePlayGames;

    [JsonProperty("braze")]
    public ExtensionConfigs.BrazeConfig Braze;

    [JsonProperty("steam")]
    public ExtensionConfigs.SteamConfig Steam;

    [JsonProperty("epicgames")]
    public ExtensionConfigs.EpicGamesConfig EpicGames;

    [JsonProperty("google_sign_in")]
    public ExtensionConfigs.GoogleSignInConfig GoogleSignIn;

    [JsonProperty("xbox")]
    public ExtensionConfigs.XboxConfig Xbox;
    
    [JsonProperty("ps5")]
    public ExtensionConfigs.PS5Config PS5;
}