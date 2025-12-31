using System.Collections;
using System.Collections.Generic;
using Gpp.Log;
using Gpp.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gpp.CommonUI
{
    public class GppPreferredSizeButton : MonoBehaviour
    {
        [Header("Padding")]
        [SerializeField]
        private int Left = 0;
        [SerializeField]
        private int Right = 0;
        [SerializeField]
        private int Top = 0;
        [SerializeField]
        private int Bottom = 0;

        [Header("Min size")]
        [SerializeField]
        private float MinWidth = 0;
        [SerializeField]
        private float MinHeight = 0;

        [Header("Max size")]
        [SerializeField]
        private float MaxWidth = 0;
        [SerializeField]
        private float MaxHeight = 0;

        private Button button;
        private RectTransform rectTransform;
        private TextMeshProUGUI buttonText;

        void Start()
        {
            buttonText = transform.GetComponentInChildren<TextMeshProUGUI>();

            if (buttonText == null)
            {
                GppLog.Log("buttonText is null");
                return;
            }

            button = GetComponent<Button>();

            if (button == null)
            {
                GppLog.Log("button is null");
                return;
            }

            rectTransform = GetComponent<RectTransform>();

            float preferredWidth = buttonText.preferredWidth + Left + Right;
            float preferredHeight = buttonText.preferredHeight + Top + Bottom;

            if (MinWidth != 0)
                preferredWidth = preferredWidth > MinWidth ? preferredWidth : MinWidth;

            if (MinHeight != 0)
                preferredHeight = preferredHeight > MinHeight ? preferredHeight : MinHeight;

            if (MaxWidth != 0)
                preferredWidth = preferredWidth < MaxWidth ? preferredWidth : MaxWidth;

            if (MaxHeight != 0)
                preferredHeight = preferredHeight < MaxHeight ? preferredHeight : MaxHeight;

            rectTransform.sizeDelta = new Vector2(preferredWidth, preferredHeight);
        }
    }
}