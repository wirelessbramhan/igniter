using System.IO;
using UnityEditor;

namespace Gpp.Editor
{
    [InitializeOnLoad]
    public class SdkInitializer
    {
        private const string SOURCE_FILE_PATH = "Packages/com.krafton.gpp.sdk.unity/Editor/SdkInitializer/link.xml";
        private const string DEST_FILE_PATH = "Assets/Resources/GppSDK/link.xml";
        
        private static readonly string MarkerPath = Path.Combine("ProjectSettings", $"com.krafton.gpp.sdk.unity/install.{GppSDK.SdkVersion}.json");
        
        static SdkInitializer()
        {   
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged; 
            
            CreateDir(Path.GetDirectoryName(MarkerPath));
            
            if (File.Exists(MarkerPath))
            {
                return;
            }
            
            foreach (var oldMarker in Directory.GetFiles(Path.GetDirectoryName(MarkerPath)))
            {
                File.Delete(oldMarker);
            }
            
            EditorApplication.delayCall += () =>
            {
                File.WriteAllText(MarkerPath, $"version:{GppSDK.SdkVersion}");
                CopyFile();
            };
        }

        private static void CopyFile()
        {
            if (File.Exists(SOURCE_FILE_PATH) is false)
            {
                return;
            }
            
            if (File.Exists(DEST_FILE_PATH))
            {
                File.SetAttributes(DEST_FILE_PATH, FileAttributes.Normal);
                File.Delete(DEST_FILE_PATH); 
            }

            CreateDir(Path.GetDirectoryName(DEST_FILE_PATH));

            File.Copy(SOURCE_FILE_PATH, DEST_FILE_PATH);
        }

        private static void CreateDir(string dirPath)
        {
            if (Directory.Exists(dirPath) is false)
            {
                Directory.CreateDirectory(dirPath);
            }
        }
        
        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state is PlayModeStateChange.ExitingPlayMode)
            {
                ResetStaticVariables();
            }
        }

        private static void ResetStaticVariables()
        {
            GppSDK.DisconnectSdk();
        }
    }
}