using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Gpp.Editor
{
    public class GppEditor : EditorWindow
    {
        [MenuItem("GppSDK/Utils/Open Local SDK Data", false, 0)]
        public static void OpenLocalSdkData()
        {
            if (Directory.Exists(Application.persistentDataPath))
            {
                EditorUtility.RevealInFinder(Application.persistentDataPath);
            }
            else
            {
                GppEditorUtil.ShowDialog($"Directory does not exist.\n{Application.persistentDataPath}");
            }
        }

        [MenuItem("GppSDK/Utils/Clear Local SDK Data", false, 1)]
        public static void ClearLocalSdkData()
        {
            PlayerPrefs.DeleteAll();
            var dir = new DirectoryInfo(Application.persistentDataPath);
            try
            {
                dir.Delete(true);
                GppEditorUtil.ShowDialog("App Data deleted successfully");
            }
            catch (Exception)
            {
                GppEditorUtil.ShowDialog("Please check for any files in AppData that are currently in use.");
            }
        }
    }
}