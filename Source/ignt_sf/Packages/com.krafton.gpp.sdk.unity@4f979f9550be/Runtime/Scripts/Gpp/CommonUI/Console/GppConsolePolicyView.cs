using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Gpp.Utils;
using UnityEngine.EventSystems;
using System.Collections;
using Gpp.Log;

namespace Gpp.CommonUI.Legal.Console
{
    public class GppConsolePolicyView : MonoBehaviour, IGppConsoleUIFocusable
    {
        [SerializeField]
        public Toggle checkbox;

        [SerializeField]
        private TextMeshProUGUI title;

        private bool isCheck = false;
        private Action<bool, GppLegalModel> onCheckedPolicy;
        private Action<GppLegalModel, RectTransform> onFocus;
        private Action onDefocus;
        private Action<GppLegalModel> onShowDetail;
        private GppLegalModel policy;
        [SerializeField]
        private RectTransform[] contentSizeFitTransform;

        private void Awake()
        {
            if (checkbox != null)
            {
                checkbox.isOn = false;
            }
        }

        public void Init(GppLegalModel policy, Action<bool, GppLegalModel> onCheckedPolicy, Action<GppLegalModel, RectTransform> onFocus, Action onDefocus, Action<GppLegalModel> onShowDetail)
        {
            SetTitle(policy.PolicyTitle);
            this.policy = policy;
            this.onFocus =  onFocus;
            this.onDefocus = onDefocus;
            this.onCheckedPolicy = onCheckedPolicy;
            this.onShowDetail = onShowDetail;

            for (int i = 0; i < contentSizeFitTransform.Length; i++)
            {
                RefreshLayout(contentSizeFitTransform[i]);
            }
        }

        public bool IsChecked()
        {
            return isCheck;
        }

        public void Check(bool isOn)
        {
            isCheck = isOn;
            checkbox.isOn = isCheck;
        }
        public void OnFocus()
        {
            try
            {
                EventSystem.current.SetSelectedGameObject(checkbox.gameObject);
                onFocus?.Invoke(policy, checkbox.transform.parent.parent.GetComponent<RectTransform>());
            }
            catch (Exception e)
            {
                GppLog.LogWarning(e);
            }
        }

        public void OnDefocus()
        {
            onDefocus?.Invoke();
        }

        public void OnAPressed()
        {
            Check(!IsChecked());
            onCheckedPolicy?.Invoke(IsChecked(), policy);
        }

        public void OnYPressed()
        {
            onShowDetail?.Invoke(policy);
        }

        private void SetTitle(string title)
        {
            this.title.text = title;
            if(GppUtil.IsRtlLang())
            {
                this.title.horizontalAlignment = HorizontalAlignmentOptions.Right;
                this.title.lineSpacing = -70f;
            }
        }

        private void RefreshLayout(RectTransform target)
        {
            StartCoroutine(ForceRebuildNextFrame(target));
        }

        private IEnumerator ForceRebuildNextFrame(RectTransform target)
        {
            yield return null;
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(target);
        }
    }
}