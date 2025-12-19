using Gpp.Extension;
using Gpp.Log;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gpp.CommonUI
{
    public class GppTranslator : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI text;

        [SerializeField]
        internal string key = "";

        private void OnEnable()
        {
            if (!GppSDK.IsInitialized)
            {
                GppLog.LogWarning("GppSDK is not initialized. Can not translate.");
                return;
            }

            if (text == null)
            {
                text = GetComponent<TextMeshProUGUI>();
            }

            if (text == null)
            {
                text = GetComponentInChildren<TextMeshProUGUI>();
            }

            if (text == null)
            {
                GppLog.LogWarning("TextMeshProUGUI component does not found.");
                return;
            }

            if (key == "")
            {
                key = text.text;
            }

            text.text = key.Localise();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GppTranslator))]
    [CanEditMultipleObjects]
    public class GppTranslatorEditor : Editor
    {
        void OnSceneGUI()
        {
            GppTranslator translator = (GppTranslator)target;
            if (translator == null)
            {
                return;
            }

            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.green;

            Handles.color = Color.blue;
            Handles.Label(translator.transform.position + Vector3.up * 2, "Key: " + translator.key, style);
        }
    }

#endif
}