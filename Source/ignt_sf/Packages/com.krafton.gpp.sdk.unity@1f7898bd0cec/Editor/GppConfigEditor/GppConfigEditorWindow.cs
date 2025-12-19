using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gpp.Editor;
using Gpp.Extensions.Braze;
using Gpp.Extensions.EpicGames;
using Gpp.Extensions.FirebasePush;
using Gpp.Extensions.GooglePlayGames;
using Gpp.Extensions.GoogleSignIn;
using Gpp.Extensions.Ps5;
using Gpp.Extensions.Steam;
using Gpp.Telemetry;
using Gpp.Extensions.XboxPc;
using Gpp.Utils;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class GppConfigEditorWindow : EditorWindow
{
    private const string ConfigFilePath = "Assets/Resources/GppSDK";
    private const string ConfigFileName = "GppConfig.asset";
    private static readonly string configFileFullPath = $"{ConfigFilePath}/{ConfigFileName}";
    private readonly string[] tabNames =
    {
#if GPP_DEV_MODE
        "Alpha",
        "QA",
#endif
        "Beta",
        "Cert",
        "Live"
    };
    private const string SteamAppIdFileName = "steam_appid.txt";

    private readonly Dictionary<GppPackageType, bool> installedPackages = new()
    {
        { GppPackageType.GoogleAnalytics, false },
        { GppPackageType.FirebasePush, false },
        { GppPackageType.GooglePlayGames, false },
        { GppPackageType.Braze, false },
        { GppPackageType.Steam, false },
        { GppPackageType.EpicGames, false },
        { GppPackageType.XboxPc, false },
        { GppPackageType.PS5, false }
    };

    private GppConfigSO configFile;
    private int selectedTab;
    private Vector2 scrollPos;

    [MenuItem("GppSDK/SDK Configuration")]
    public static void ShowWindow()
    {
        GppConfigEditorWindow window = GetWindow<GppConfigEditorWindow>("SDK Config Editor");
        window.Show();
    }

    private void OnEnable()
    {
        CheckExtensions();
        ResizeWindow();
        LoadOrCreateConfigFile();
    }

    private void ResizeWindow()
    {
        const float defaultXMinSize = 860f;
        const float defaultYMinSize = 360f;
        float resultMinSize = defaultYMinSize;
        if (installedPackages.Any(pair => pair.Value))
        {
            resultMinSize += 30f;
        }

        if (PlatformUtil.IsMobile())
        {
            if (installedPackages[GppPackageType.GoogleAnalytics])
            {
                resultMinSize += 190f;
            }
            if (installedPackages[GppPackageType.FirebasePush])
            {
                resultMinSize += 190f;
            }
            if (installedPackages[GppPackageType.GooglePlayGames])
            {
                resultMinSize += 90f;
            }
            if (installedPackages[GppPackageType.Braze])
            {
                resultMinSize += 90f;
            }
        }
        else if (PlatformUtil.IsConsole())
        {
            if (installedPackages[GppPackageType.PS5])
            {
                resultMinSize += 90f;
            }
        }
        else
        {
            if (installedPackages[GppPackageType.Steam])
            {
                resultMinSize += 90f;
            }
        }

        minSize = new Vector2(defaultXMinSize, resultMinSize);
    }

    private void CheckExtensions()
    {
        installedPackages[GppPackageType.GoogleAnalytics] = GoogleAnalyticsExt.IsPackageInstalled();
        installedPackages[GppPackageType.FirebasePush] = FirebasePushExt.IsPackageInstalled();
        installedPackages[GppPackageType.GooglePlayGames] = GooglePlayGamesExt.IsPackageInstalled();
        installedPackages[GppPackageType.Braze] = BrazeExt.IsPackageInstalled();
        installedPackages[GppPackageType.Steam] = SteamExt.IsPackageInstalled();
        installedPackages[GppPackageType.EpicGames] = EpicGamesExt.IsPackageInstalled();
        installedPackages[GppPackageType.GoogleSignIn] = GoogleSignInExt.IsPackageInstalled();
        installedPackages[GppPackageType.XboxPc] = XboxPcExt.IsPackageInstalled();
        installedPackages[GppPackageType.PS5] = Ps5Ext.IsPackageInstalled();
    }

    private void LoadOrCreateConfigFile()
    {
        configFile = AssetDatabase.LoadAssetAtPath<GppConfigSO>(configFileFullPath);

        if (configFile == null)
        {
            if (!Directory.Exists(ConfigFilePath))
            {
                Directory.CreateDirectory(ConfigFilePath);
            }

            configFile = CreateInstance<GppConfigSO>();

            configFile.stages = new List<GppConfig>
            {
#if GPP_DEV_MODE
                CreateStageConfig("alpha", "https://alpha.gpp.krafton.dev"),
                CreateStageConfig("qa", "https://qa.gpp.krafton.dev"),
#endif

                CreateStageConfig("beta", "https://beta.gpp.krafton.dev"),
                CreateStageConfig("cert", "https://cert.gpp.krafton.io"),
                CreateStageConfig("live", "https://gpp.krafton.io")
            };

            configFile.activeStage = "beta";
            AssetDatabase.CreateAsset(configFile, configFileFullPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    private void OnGUI()
    {
        GUIStyle paddingStyle = new GUIStyle { padding = new RectOffset(25, 25, 25, 25) };
        EditorGUILayout.BeginVertical(paddingStyle);

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 16,
            alignment = TextAnchor.MiddleLeft,
            normal = { textColor = new Color(255, 255, 224) }
        };

        GUIStyle subtitleStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 12,
            fontStyle = FontStyle.Bold
        };

        GUIStyle boxStyle = new GUIStyle("box") { padding = new RectOffset(20, 20, 20, 20) };

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("SDK Config Editor", titleStyle);
        if (GUILayout.Button("Reset", GUILayout.Width(100)))
        {
            ResetConfig();
        }

        if (GUILayout.Button("Import JSON", GUILayout.Width(100)))
        {
            ImportJson();
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (configFile == null)
        {
            EditorGUILayout.HelpBox("SdkConfig could not be loaded or created.", MessageType.Error);
            EditorGUILayout.EndVertical();
            return;
        }

        EditorGUILayout.BeginVertical(boxStyle);
        EditorGUILayout.LabelField("Stages", subtitleStyle);
        EditorGUILayout.Space();

        int changeTab = GUILayout.Toolbar(selectedTab, tabNames, GUILayout.Height(25));

        if (changeTab != selectedTab)
        {
            selectedTab = changeTab;
            GUI.FocusControl(null);
        }

        configFile.activeStage = tabNames[selectedTab].ToLower();
        EditorUtility.SetDirty(configFile);

        EditorGUILayout.Space(10);

        if (configFile.stages.Count > selectedTab)
        {
            DrawStageConfig(configFile.stages[selectedTab]);
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();
    }

    private void ResetConfig()
    {
        if (EditorUtility.DisplayDialog("Reset Configuration",
                "Are you sure you want to reset all configuration values (excluding Base URL)? This action cannot be undone.",
                "Yes", "No"))
        {
            foreach (var stage in configFile.stages)
            {
                stage.Namespace = string.Empty;
                stage.ClientId = string.Empty;
                stage.EnableDebugLog = false;
                stage.Extensions = new Extensions(); // Reset extensions to default values
            }

            EditorUtility.SetDirty(configFile);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Configuration has been reset successfully.");
        }
    }

    private void ImportJson()
    {
        string path = EditorUtility.OpenFilePanel("Import JSON", "", "json");
        if (!string.IsNullOrEmpty(path))
        {
            using StreamReader reader = new StreamReader(path);
            TempGppConfigFile tempConfig = JsonConvert.DeserializeObject<TempGppConfigFile>(reader.ReadToEnd());
            configFile.activeStage = tempConfig.ActiveStage;
            foreach (var tempStageConfig in tempConfig.Stages)
            {
                var matchingConfig = configFile.stages.FirstOrDefault(c => c.Stage.Equals(tempStageConfig.Stage, StringComparison.OrdinalIgnoreCase));
                if (matchingConfig == null) continue;
                matchingConfig.Namespace = tempStageConfig.Namespace;
                matchingConfig.ClientId = tempStageConfig.ClientId;
                matchingConfig.BaseUrl = tempStageConfig.BaseUrl;
                matchingConfig.EnableDebugLog = tempStageConfig.EnableDebugLog;
                matchingConfig.Extensions = tempStageConfig.Extensions;
            }

            EditorUtility.SetDirty(configFile);
        }
        else
        {
            EditorUtility.DisplayDialog("Error", "Please select the 'sdk_config.json' file.", "OK");
        }
    }

    private void DrawStageConfig(GppConfig gpp)
    {
        GUIStyle boxStyle = new GUIStyle("box") { padding = new RectOffset(15, 15, 15, 15), margin = new RectOffset(5, 5, 5, 5) };
        GUIStyle labelStyle = new GUIStyle(EditorStyles.label) { fontStyle = FontStyle.Bold };
        GUIStyle fieldStyle = new GUIStyle(EditorStyles.textField) { margin = new RectOffset(60, 0, 5, 5) };

        EditorGUILayout.BeginVertical(boxStyle);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Namespace", labelStyle, GUILayout.Width(100));
        gpp.Namespace = EditorGUILayout.TextField(gpp.Namespace, fieldStyle);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Client ID", labelStyle, GUILayout.Width(100));
        gpp.ClientId = EditorGUILayout.TextField(gpp.ClientId, fieldStyle);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Base URL", labelStyle, GUILayout.Width(100));
        GUI.enabled = false;
        gpp.BaseUrl = EditorGUILayout.TextField(gpp.BaseUrl, fieldStyle);
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
        gpp.EnableDebugLog = EditorGUILayout.Toggle(gpp.EnableDebugLog, GUILayout.Width(20));
        EditorGUILayout.LabelField("Enable Debug", labelStyle, GUILayout.Width(100));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(10);

        if (PlatformUtil.IsMobile())
        {
            EditorGUILayout.BeginHorizontal();
            gpp.EnableLoginAnotherAccount = EditorGUILayout.Toggle(gpp.EnableLoginAnotherAccount, GUILayout.Width(20));
            EditorGUILayout.LabelField("Show Login with a different account UI on the Terms", labelStyle, GUILayout.Width(450));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10);
        }

        EditorGUILayout.BeginHorizontal();
        gpp.EnableGameServer = EditorGUILayout.Toggle(gpp.EnableGameServer, GUILayout.Width(20));
        EditorGUILayout.LabelField("Enable KOS Game Server", labelStyle, GUILayout.Width(450));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(10);

        if (PlatformUtil.IsPc())
        {
            EditorGUILayout.BeginHorizontal();
            gpp.EnableOwnershipSync = EditorGUILayout.Toggle(gpp.EnableOwnershipSync, GUILayout.Width(20));
            EditorGUILayout.LabelField("Enable Ownership Sync", labelStyle, GUILayout.Width(450));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10);
        }

        if (installedPackages.Any(pair => pair.Value))
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Extensions", labelStyle);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(10);

            DrawExtensions(gpp.Extensions);
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawExtensions(Extensions extensions)
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);

        GUIStyle boxStyle = new GUIStyle("box") { padding = new RectOffset(15, 15, 15, 15) };
        GUIStyle labelStyle = new GUIStyle(EditorStyles.label) { fontStyle = FontStyle.Bold };
        GUIStyle fieldStyle = new GUIStyle(EditorStyles.textField) { margin = new RectOffset(0, 0, 5, 5) };

        if (PlatformUtil.IsMobile() && installedPackages[GppPackageType.GoogleAnalytics])
        {
            // Google Analytics
            EditorGUILayout.BeginVertical(boxStyle);
            EditorGUILayout.LabelField("Google Analytics", labelStyle);
            EditorGUILayout.Space(5);
            extensions.GoogleAnalytics.ProjectId = EditorGUILayout.TextField("Project ID", extensions.GoogleAnalytics.ProjectId, fieldStyle);
            extensions.GoogleAnalytics.WebClientId = EditorGUILayout.TextField("Web Client ID", extensions.GoogleAnalytics.WebClientId, fieldStyle);
            extensions.GoogleAnalytics.ApiKey = EditorGUILayout.TextField("API Key", extensions.GoogleAnalytics.ApiKey, fieldStyle);
            extensions.GoogleAnalytics.AndroidAppId = EditorGUILayout.TextField("Android App ID", extensions.GoogleAnalytics.AndroidAppId, fieldStyle);
            extensions.GoogleAnalytics.IosAppId = EditorGUILayout.TextField("iOS App ID", extensions.GoogleAnalytics.IosAppId, fieldStyle);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(10);
        }

        if (PlatformUtil.IsMobile() && installedPackages[GppPackageType.FirebasePush])
        {
            // Firebase Push
            EditorGUILayout.BeginVertical(boxStyle);
            EditorGUILayout.LabelField("Firebase Push", labelStyle);
            EditorGUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            extensions.FirebasePush.EnableAutoRequestPermission = EditorGUILayout.Toggle(extensions.FirebasePush.EnableAutoRequestPermission, GUILayout.Width(20));
            EditorGUILayout.LabelField("Auto Request Push Permission", labelStyle, GUILayout.Width(500));
            EditorGUILayout.EndHorizontal();
            extensions.FirebasePush.ProjectId = EditorGUILayout.TextField("Project ID", extensions.FirebasePush.ProjectId, fieldStyle);
            extensions.FirebasePush.SenderId = EditorGUILayout.TextField("Sender ID", extensions.FirebasePush.SenderId, fieldStyle);
            extensions.FirebasePush.ApiKey = EditorGUILayout.TextField("API Key", extensions.FirebasePush.ApiKey, fieldStyle);
            extensions.FirebasePush.AndroidAppId = EditorGUILayout.TextField("Android App ID", extensions.FirebasePush.AndroidAppId, fieldStyle);
            extensions.FirebasePush.IosAppId = EditorGUILayout.TextField("iOS App ID", extensions.FirebasePush.IosAppId, fieldStyle);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(10);
        }

        if (PlatformUtil.IsMobile() && installedPackages[GppPackageType.GooglePlayGames])
        {
            // Google Play Games
            EditorGUILayout.BeginVertical(boxStyle);
            EditorGUILayout.LabelField("Google Play Games", labelStyle);
            EditorGUILayout.Space(5);
            extensions.GooglePlayGames.WebClientId = EditorGUILayout.TextField("Web Client ID", extensions.GooglePlayGames.WebClientId, fieldStyle);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(10);
        }

        if (PlatformUtil.IsMobile() && installedPackages[GppPackageType.Braze])
        {
            // Braze
            EditorGUILayout.BeginVertical(boxStyle);
            EditorGUILayout.LabelField("Braze", labelStyle);
            EditorGUILayout.Space(5);
            extensions.Braze.AndroidApiKey = EditorGUILayout.TextField("Android API Key", extensions.Braze.AndroidApiKey, fieldStyle);
            extensions.Braze.AndroidEndpoint = EditorGUILayout.TextField("Android Endpoint", extensions.Braze.AndroidEndpoint, fieldStyle);
            extensions.Braze.IosApiKey = EditorGUILayout.TextField("iOS API Key", extensions.Braze.IosApiKey, fieldStyle);
            extensions.Braze.IosEndpoint = EditorGUILayout.TextField("iOS Endpoint", extensions.Braze.IosEndpoint, fieldStyle);
            extensions.Braze.SenderId = EditorGUILayout.TextField("Sender ID", extensions.Braze.SenderId, fieldStyle);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(10);
        }

        if (PlatformUtil.IsPc() && installedPackages[GppPackageType.Steam])
        {
            // Steam
            EditorGUILayout.BeginVertical(boxStyle);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Steam", labelStyle, GUILayout.ExpandWidth(false));
            GUILayout.FlexibleSpace();

            string steamButtonLabel = File.Exists(Path.Combine(Application.dataPath, "..", SteamAppIdFileName)) &&
                                      !string.IsNullOrEmpty(File.ReadAllText(Path.Combine(Application.dataPath, "..", SteamAppIdFileName)).Trim())
                ? "Update steam_appid.txt"
                : "Create steam_appid.txt";

            if (GUILayout.Button(steamButtonLabel, GUILayout.Width(150)))
            {
                CreateOrUpdateSteamAppIdFile(extensions.Steam.SteamAppId);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
            extensions.Steam.SteamAppId = EditorGUILayout.TextField("Steam App ID", extensions.Steam.SteamAppId, fieldStyle);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(10);
        }

        if (PlatformUtil.IsPc() && installedPackages[GppPackageType.EpicGames])
        {
            // Epic Games
            EditorGUILayout.BeginVertical(boxStyle);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("EpicGames", labelStyle, GUILayout.ExpandWidth(false));
            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
            extensions.EpicGames.EpicProductId = EditorGUILayout.TextField("Epic Product Id", extensions.EpicGames.EpicProductId, fieldStyle);
            extensions.EpicGames.EpicSandboxId = EditorGUILayout.TextField("Epic Sandbox Id", extensions.EpicGames.EpicSandboxId, fieldStyle);
            extensions.EpicGames.EpicDeploymentId = EditorGUILayout.TextField("Epic Deployment Id", extensions.EpicGames.EpicDeploymentId, fieldStyle);
            extensions.EpicGames.EpicClientId = EditorGUILayout.TextField("Epic Client Id", extensions.EpicGames.EpicClientId, fieldStyle);
            extensions.EpicGames.EpicClientSecret = EditorGUILayout.TextField("Epic Client Secret", extensions.EpicGames.EpicClientSecret, fieldStyle);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(10);
        }

        if (PlatformUtil.IsAndroidOrEditor() && installedPackages[GppPackageType.GoogleSignIn])
        {
            // Google Sign In
            EditorGUILayout.BeginVertical(boxStyle);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("GoogleSignIn", labelStyle, GUILayout.ExpandWidth(false));
            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
            extensions.GoogleSignIn.ServerClientId = EditorGUILayout.TextField("Google ServerClientId", extensions.GoogleSignIn.ServerClientId, fieldStyle);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(10);
        }

        if (PlatformUtil.IsPc() && installedPackages[GppPackageType.XboxPc])
        {
            //Xbox Pc
            EditorGUILayout.BeginVertical(boxStyle);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Xbox Pc", labelStyle, GUILayout.ExpandWidth(false));
            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
            extensions.Xbox.Scid = EditorGUILayout.TextField("Scid", extensions.Xbox.Scid, fieldStyle);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(10);
        }
        if (PlatformUtil.IsConsoleOrEditor() && installedPackages[GppPackageType.PS5])
        {
            // PS5
            EditorGUILayout.BeginVertical(boxStyle);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("PS5", labelStyle, GUILayout.ExpandWidth(false));
            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
            extensions.PS5.ClientId = EditorGUILayout.TextField("PS5 Client Id", extensions.PS5.ClientId, fieldStyle);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(10);
        }

        EditorGUILayout.EndScrollView();
    }

    private void CreateOrUpdateSteamAppIdFile(string appId)
    {
        if (string.IsNullOrEmpty(appId))
        {
            EditorUtility.DisplayDialog("Error", "Steam App ID cannot be empty.", "OK");
            return;
        }

        string filePath = Path.Combine(Application.dataPath, "..", SteamAppIdFileName);

        if (File.Exists(filePath))
        {
            Debug.Log("steam_appid.txt file exists. Updating the file with new App ID.");
        }
        else
        {
            Debug.Log("steam_appid.txt file does not exist. Creating a new file.");
        }

        File.WriteAllText(filePath, appId);
        Debug.Log($"AppID: {appId} has been written to steam_appid.txt file.");

        EditorUtility.RevealInFinder(filePath);
    }

    private void CreateSteamAppIdFile(string appId)
    {
        if (string.IsNullOrEmpty(appId))
        {
            return;
        }

        string filePath = Path.Combine(Application.dataPath, "..", SteamAppIdFileName);
        if (!File.Exists(filePath))
        {
            Debug.Log("steam_appid.txt file does not exist. Creating a new file.");
            File.Create(filePath).Dispose();
        }

        File.WriteAllText(filePath, appId);
        Debug.Log($"AppID: {appId} has been written to steam_appid.txt file.");

        EditorUtility.RevealInFinder(filePath);
    }

    private GppConfig CreateStageConfig(string stageName, string baseUrl)
    {
        GppConfig gppConfig = new GppConfig
        {
            Stage = stageName,
            Namespace = "",
            ClientId = "",
            BaseUrl = baseUrl,
            TelemetryIntervalSeconds = TelemetryManager.TELEMETRY_INTERVAL_DEFAULT_SECONDS,
            Extensions = null
        };

        return gppConfig;
    }
}