using Gpp.Utils;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Gpp.CommonUI
{
    public class GppCommonButton : Button
    {
        [Header("Text and Icon Color")]
        [SerializeField]
        private Color normalTextColor;

        [SerializeField]
        private Color hoveredTextColor;

        [SerializeField]
        private Color disabledTextColor;

        [SerializeField]
        private Color disabledIconColor = new Color(255, 255, 255);

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);

            if (transition == Transition.None)
                return;

            TextMeshProUGUI text = GetComponentInChildren<TextMeshProUGUI>();
            Image iconImage = GppUtil.FindChildWithName(gameObject, "IconImage")?.GetComponent<Image>();
            switch (state)
            {
                case SelectionState.Normal:
                case SelectionState.Selected:
                    {
                        if (text != null)
                            text.color = normalTextColor;
                        break;
                    }
                case SelectionState.Disabled:
                    {
                        if (text != null)
                            text.color = disabledTextColor;
                        if (iconImage != null)
                        {
                            iconImage.color = disabledIconColor;
                        }

                        break;
                    }
                default:
                    {
                        if (text != null)
                            text.color = hoveredTextColor;
                        break;
                    }
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GppCommonButton))]
    public class GppButtonInspector : Editor
    {
    }
#endif
}