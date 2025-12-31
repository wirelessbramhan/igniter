using UnityEditor;

namespace Gpp.Editor
{
    internal static class GppEditorUtil
    {
        public static void ShowDialog(string message)
        {
            EditorUtility.DisplayDialog("Notification", message, "OK");
        } 
    }
}