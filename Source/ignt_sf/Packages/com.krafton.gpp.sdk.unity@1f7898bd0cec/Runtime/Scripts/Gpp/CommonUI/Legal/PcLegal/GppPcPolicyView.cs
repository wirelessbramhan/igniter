using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Gpp.Utils;
using Gpp.Models;
using Gpp.Telemetry;

namespace Gpp.CommonUI.Legal.PcLegal
{
    public class GppPcPolicyView : MonoBehaviour
    {
        [SerializeField]
        private Toggle checkbox;

        [SerializeField]
        private TextMeshProUGUI title;

        [SerializeField]
        private Button detailButton;

        [SerializeField]
        private Button agreeButton;

        private void Awake()
        {
            if (checkbox != null)
            {
                checkbox.isOn = false;
            }
        }

        public void Init(GppLegalModel policy, Action<bool, GppLegalModel> onCheckedPolicy, Action<GppLegalModel> onViewDetail)
        {
            SetTitle(policy.PolicyTitle);
            checkbox.onValueChanged.AddListener(isOn => { onCheckedPolicy(isOn, policy); });
            if (string.IsNullOrEmpty(policy.WebUrl))
            {
                detailButton.gameObject.GetComponent<CanvasGroup>().alpha = 0f;
                detailButton.gameObject.GetComponent<CanvasGroup>().interactable = false;
            }
            else
            {
                detailButton.onClick.AddListener(() =>
                {
                    onViewDetail?.Invoke(policy);
                });
            }
            agreeButton.onClick.AddListener(() =>
            {
                onCheckedPolicy(!checkbox.isOn, policy);
                checkbox.SetIsOnWithoutNotify(!checkbox.isOn);
            });

            checkbox.isOn = policy.IsAccepted;

            FixComponentSize();
            SetTitleSize();
        }

        public bool IsChecked()
        {
            return checkbox != null && checkbox.isOn;
        }

        public void Check(bool isOn)
        {
            checkbox.isOn = isOn;
        }

        private void SetTitle(string title)
        {
            this.title.text = title;
            if (GppUtil.IsRtlLang())
            {
                this.title.horizontalAlignment = HorizontalAlignmentOptions.Right;
                this.title.lineSpacing = -70f;
            }
        }

        private void FixComponentSize()
        {
            var detailButtonRectTransform = detailButton.GetComponent<RectTransform>();
            var agreeButtonRectTransform = agreeButton.GetComponent<RectTransform>();
            var buttonsRectTransform = detailButton.transform.parent.GetComponent<RectTransform>();

            detailButtonRectTransform.ForceUpdateRectTransforms();
            LayoutRebuilder.ForceRebuildLayoutImmediate(detailButtonRectTransform);
            agreeButtonRectTransform.ForceUpdateRectTransforms();
            LayoutRebuilder.ForceRebuildLayoutImmediate(agreeButtonRectTransform);
            buttonsRectTransform.ForceUpdateRectTransforms();
            LayoutRebuilder.ForceRebuildLayoutImmediate(buttonsRectTransform);
        }

        private void SetTitleSize()
        {
            var checkboxSize = checkbox.GetComponent<RectTransform>().sizeDelta.x;
            var buttonsSize = detailButton.transform.parent.GetComponent<RectTransform>().sizeDelta.x;
            var parentRecttransform = transform.GetComponent<RectTransform>();
            var parentSize = parentRecttransform.sizeDelta;

            Vector2 preferredSize = new Vector2(parentSize.x - buttonsSize - checkboxSize - 135, parentSize.y);
            title.GetComponent<RectTransform>().sizeDelta = preferredSize;

            parentRecttransform.ForceUpdateRectTransforms();
            LayoutRebuilder.ForceRebuildLayoutImmediate(parentRecttransform);
        }
    }
}