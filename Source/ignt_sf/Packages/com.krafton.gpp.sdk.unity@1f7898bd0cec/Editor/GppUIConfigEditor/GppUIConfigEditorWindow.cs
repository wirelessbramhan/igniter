using System;
using System.IO;
using Gpp.CommonUI.GPGS;
using Gpp.CommonUI.Legal;
using Gpp.CommonUI.Legal.PcLegal;
using Gpp.CommonUI.Login;
using Gpp.CommonUI.Maintenance;
using Gpp.CommonUI.Modal;
using Gpp.CommonUI.PcAccountCheck;
using Gpp.CommonUI.PcAuthWaiting;
using Gpp.CommonUI.Toast;
using TMPro;
using UnityEditor;
using UnityEngine;

public class GppUIConfigEditorWindow : EditorWindow
{
    private const string ConfigFilePath = "Assets/Resources/GppSDK";
    private const string ConfigFileName = "GppUIConfig.asset";
    private static readonly string configFileFullPath = $"{ConfigFilePath}/{ConfigFileName}";

    private GppUIConfigSo configFile;

    [MenuItem("GppSDK/SDK UI Configuration")]
    public static void ShowWindow()
    {
        GppUIConfigEditorWindow window = GetWindow<GppUIConfigEditorWindow>("SDK UI Config Editor");
        window.Show();
    }

    private void OnEnable()
    {
        LoadOrCreateConfigFile();
    }

    private void LoadOrCreateConfigFile()
    {
        configFile = AssetDatabase.LoadAssetAtPath<GppUIConfigSo>(configFileFullPath);

        if (configFile == null)
        {
            if (!Directory.Exists(ConfigFilePath))
            {
                Directory.CreateDirectory(ConfigFilePath);
            }

            configFile = CreateInstance<GppUIConfigSo>();
            AssetDatabase.CreateAsset(configFile, configFileFullPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    private void OnGUI()
    {
        GUIStyle paddingStyle = new GUIStyle { padding = new RectOffset(25, 25, 25, 25) };
        EditorGUILayout.BeginVertical(paddingStyle);
        EditorGUILayout.Space(10);

        if (configFile == null)
        {
            EditorGUILayout.HelpBox("Configuration file not found.", MessageType.Error);
            if (GUILayout.Button("Create Configuration File"))
            {
                LoadOrCreateConfigFile();
            }

            EditorGUILayout.EndVertical();
            return;
        }

        // Title 영역
        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 16,
            alignment = TextAnchor.MiddleLeft,
            normal = { textColor = new Color(255, 255, 224) }
        };

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Gpp UI Configuration", titleStyle, GUILayout.Width(250));

        // Reset 버튼을 오른쪽 끝에 배치
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Reset", GUILayout.Width(80), GUILayout.Height(20)))
        {
            ResetConfigValues();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.Space(10);
        DrawFontSection();

        EditorGUILayout.Space(10);
        DrawCommonSection();

        EditorGUILayout.Space(10);
        DrawMobileSection();

        EditorGUILayout.Space(10);
        DrawPcSection();

        EditorGUILayout.Space(10);
        EditorGUILayout.EndVertical();

        minSize = new Vector2(600, 730);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(configFile);
            AssetDatabase.SaveAssets();
        }
    }

// Reset 동작을 처리하는 메서드
    private void ResetConfigValues()
    {
        if (configFile == null)
        {
            Debug.LogWarning("Configuration file is not loaded. Cannot reset values.");
            return;
        }

        // Reset 각 필드의 값을 기본값(null 또는 필요한 초기값)으로 설정
        configFile.font = null;

        configFile.maintenanceUI = null;
        configFile.modalUI = null;
        configFile.toastUI = null;

        configFile.mobileLoginUI = null;
        configFile.mobileLegalUI = null;
        configFile.mobileGpgPopupUI = null;

        configFile.pcLoginUI = null;
        configFile.pcLegalUI = null;
        configFile.pcAccountCheckUI = null;
        configFile.pcAuthWaitingUI = null;

        Debug.Log("Configuration values have been reset to default.");
        EditorUtility.SetDirty(configFile);
        AssetDatabase.SaveAssets();
    }

    private void DrawFontSection()
    {
        GUIStyle boxStyle = new GUIStyle("box") { padding = new RectOffset(10, 10, 5, 5) };
        EditorGUILayout.BeginVertical(boxStyle);
        EditorGUILayout.Space(10);

        GUIStyle boldLabelStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 12,
            alignment = TextAnchor.MiddleLeft
        };

        EditorGUILayout.LabelField("Font Setting (Only GppCommonUI)", boldLabelStyle, GUILayout.Width(250));
        EditorGUILayout.Space(10);

        configFile.font = (TMP_FontAsset)EditorGUILayout.ObjectField("Font", configFile.font, typeof(TMP_FontAsset), false);

        EditorGUILayout.Space(10);
        EditorGUILayout.EndVertical();
    }

    private void DrawCommonSection()
    {
        GUIStyle boxStyle = new GUIStyle("box") { padding = new RectOffset(10, 10, 5, 5) };
        EditorGUILayout.BeginVertical(boxStyle);
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Common", EditorStyles.boldLabel, GUILayout.Width(250));
        EditorGUILayout.Space(10);

        configFile.maintenanceUI = DrawValidatedObjectField("Maintenance UI", configFile.maintenanceUI, typeof(GameObject), typeof(IGppMaintenanceUI), "The IGppMaintenanceUI interface must be included in the Root Component.");
        EditorGUILayout.Space(5);

        configFile.modalUI = DrawValidatedObjectField("Modal UI", configFile.modalUI, typeof(GameObject), typeof(IGppModalUI), "The IGppModalUI interface must be included in the Root Component.");
        EditorGUILayout.Space(5);

        configFile.toastUI = DrawValidatedObjectField("Toast UI", configFile.toastUI, typeof(GameObject), typeof(IGppToastUI), "The IGppToastUI interface must be included in the Root Component.");
        EditorGUILayout.Space(10);

        EditorGUILayout.EndVertical();
    }

    private void DrawMobileSection()
    {
        GUIStyle boxStyle = new GUIStyle("box") { padding = new RectOffset(10, 10, 5, 5) };
        EditorGUILayout.BeginVertical(boxStyle);
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Mobile", EditorStyles.boldLabel, GUILayout.Width(250));
        EditorGUILayout.Space(10);

        configFile.mobileLoginUI = DrawValidatedObjectField("Login UI", configFile.mobileLoginUI, typeof(GameObject), typeof(IGppLoginUI), "The IGppLoginUI interface must be included in the Root Component.");
        EditorGUILayout.Space(5);

        configFile.mobileLegalUI = DrawValidatedObjectField("Legal UI", configFile.mobileLegalUI, typeof(GameObject), typeof(IGppLegalUI), "The IGppLegalUI interface must be included in the Root Component.");
        EditorGUILayout.Space(5);

        // configFile.mobileJapanPaymentUI = DrawValidatedObjectField("Japan Payment UI", configFile.mobileJapanPaymentUI, typeof(GameObject), typeof(IGppJapanPaymentUI), "The IGppJapanPaymentUI interface must be included in the Root Component.");
        // EditorGUILayout.Space(5);

        configFile.mobileGpgPopupUI = DrawValidatedObjectField("GPG Popup UI", configFile.mobileGpgPopupUI, typeof(GameObject), typeof(IGppGPGSPopupUI), "The IGppGPGSPopupUI interface must be included in the Root Component.");
        EditorGUILayout.Space(10);

        EditorGUILayout.EndVertical();
    }

    private void DrawPcSection()
    {
        GUIStyle boxStyle = new GUIStyle("box") { padding = new RectOffset(10, 10, 5, 5) };
        EditorGUILayout.BeginVertical(boxStyle);
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("PC", EditorStyles.boldLabel, GUILayout.Width(250));
        EditorGUILayout.Space(10);

        configFile.pcLoginUI = DrawValidatedObjectField("Login UI", configFile.pcLoginUI, typeof(GameObject), typeof(IGppPcLoginUI), "The IGppPcLoginUI interface must be included in the Root Component.");
        EditorGUILayout.Space(5);

        configFile.pcLegalUI = DrawValidatedObjectField("Legal UI", configFile.pcLegalUI, typeof(GameObject), typeof(IGppPcLegalUI), "The IGppPcLegalUI interface must be included in the Root Component.");
        EditorGUILayout.Space(5);

        configFile.pcAccountCheckUI = DrawValidatedObjectField("Account Check UI", configFile.pcAccountCheckUI, typeof(GameObject), typeof(IGppPcAccountCheckUI), "The IGppPcAccountCheckUI interface must be included in the Root Component.");
        EditorGUILayout.Space(5);

        configFile.pcAuthWaitingUI = DrawValidatedObjectField("Auth Waiting UI", configFile.pcAuthWaitingUI, typeof(GameObject), typeof(IGppPcAuthWaitingUI), "The IGppPcAuthWaitingUI interface must be included in the Root Component.");
        EditorGUILayout.Space(10);

        EditorGUILayout.EndVertical();
    }

    private static GameObject DrawValidatedObjectField(string label, GameObject currentObject, Type objectType, Type requiredInterface, string tooltip)
    {
        using var check = new EditorGUI.ChangeCheckScope();

        GUIContent content = new GUIContent(label, tooltip);

        GameObject selectedObject = (GameObject)EditorGUILayout.ObjectField(content, currentObject, objectType, false);

        if (!check.changed)
        {
            return currentObject;
        }

        if (selectedObject == null || HasInterface(selectedObject, requiredInterface))
        {
            if (selectedObject != null)
            {
                Debug.Log($"This GameObject can be used because it implements the {requiredInterface.Name} interface.");
            }

            return selectedObject;
        }

        return currentObject;
    }

    private static bool HasInterface(GameObject obj, Type interfaceType)
    {
        var components = obj.GetComponents<Component>();
        foreach (var component in components)
        {
            if (interfaceType.IsInstanceOfType(component))
            {
                return true;
            }
        }

        return false;
    }
}