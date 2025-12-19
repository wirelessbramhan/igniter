using com.krafton.fantasysports.UI;
using UnityEditor;
using UnityEngine;

namespace com.krafton.fantasysports
{
    [CustomEditor(typeof(ElementDataSO))]
    public class ElementDataSOEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //Draws default Inspector
            base.OnInspectorGUI();

            //Reference to script instance
            ElementDataSO dataSO = (ElementDataSO)target;

            //Simple button to debug
            if (GUILayout.Button("Save to Dict"))
            {
                dataSO.RaiseConfig();

                EditorUtility.SetDirty(target);
            }

            //Simple button to visualise loaded Dict
            if (GUILayout.Button("Show Dict"))
            {
                dataSO.ShowDict();

                EditorUtility.SetDirty(target);
            }
        }
    }
}
