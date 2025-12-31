using System;
using Gpp.Models;
using Gpp.Telemetry;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gpp.CommonUI.Legal
{
    public class GppPolicyView : MonoBehaviour
    {
        [SerializeField]
        private Toggle checkbox;

        [SerializeField]
        private TextMeshProUGUI title;

        [SerializeField]
        private Button detailButton;

        private void Awake()
        {
            if (checkbox != null)
            {
                checkbox.isOn = false;
            }
        }

        public void Init(GppLegalModel policy, Action<bool, GppLegalModel> onCheckedPolicy, Action onViewDetail)
        {
            SetTitle(policy.PolicyTitle);
            checkbox.onValueChanged.AddListener(isOn => { onCheckedPolicy(isOn, policy); });
            if (string.IsNullOrEmpty(policy.WebUrl))
            {
                detailButton.gameObject.SetActive(false);
            }
            else
            {
                detailButton.onClick.AddListener(() => {
                    if(onViewDetail != null)
                        onViewDetail.Invoke();
                    GppSDK.OpenLegalWebView(policy.LocalizedPolicyVersionId);
                });
            }
        }

        public void Check(bool isCheck)
        {
            if (checkbox == null)
            {
                return;
            }

            checkbox.isOn = isCheck;
        }

        public bool IsChecked()
        {
            return checkbox != null && checkbox.isOn;
        }

        private void SetTitle(string title)
        {
            this.title.text = title;
        }
    }
}