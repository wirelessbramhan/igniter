using System;
using System.Collections;
using Gpp.Constants;
using Gpp.Extension;
using Gpp.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gpp.CommonUI.PcAuthWaiting
{
    internal class GppPcAuthWaitingUI : GppCommonPcUI, IGppPcAuthWaitingUI
    {
        [SerializeField]
        private TextMeshProUGUI subTitle;

        [SerializeField]
        private TextMeshProUGUI description1;

        [SerializeField]
        private TextMeshProUGUI description2;

        [SerializeField]
        private Button cancelButton;

        [SerializeField]
        private RectTransform[] contentSizeFitTransform;

        private bool _isAccountCheck;
        private Action<GameObject> _onCloseCallback;

        public void SetIsAccountCheck(bool isAccountCheck)
        {
            _isAccountCheck = isAccountCheck;
        }

        public void SetOnClickCancel(Action<GameObject> onCancel)
        {
            _onCloseCallback = onCancel;
        }

        private void Start()
        {
            InitComponent();
        }

        private void InitComponent()
        {
            if (_isAccountCheck)
            {
                subTitle.text = LocalizationKey.AdditionalAccInfoRequired.Localise();
                description1.text = LocalizationKey.ReturnAfterRecoverAccInBrowser.Localise();
                description2.text = LocalizationKey.PageRefreshOnceAccRecovered.Localise();
            }
            else
            {
                subTitle.text = LocalizationKey.LoginTitle.Localise();
                description1.text = LocalizationKey.ReturnAfterLoginInBrowser.Localise();
                description2.text = LocalizationKey.PageRefreshOnceAccLoggedIn.Localise();
            }

            subTitle.gameObject.SetActive(false);
            description1.gameObject.SetActive(false);
            description2.gameObject.SetActive(false);

            subTitle.gameObject.SetActive(true);
            description1.gameObject.SetActive(true);
            description2.gameObject.SetActive(true);

            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(() =>
            {
                _onCloseCallback?.Invoke(gameObject);
            });

            for (int i = 0; i < contentSizeFitTransform.Length; i++)
            {
                RefreshLayout(contentSizeFitTransform[i]);
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

        protected override UIType ConsoleUIType()
        {
            return UIType.ConsoleAuthWaiting;
        }

        protected override void UpdateController(InputControllerType prevController, InputControllerType currentController)
        {
            if (GppSDK.GetInputController().IsGamePad())
            {
                GppUI.ShowPcAuthWaiting(_isAccountCheck, _onCloseCallback);
                Destroy(gameObject);
            }
        }
    }
}