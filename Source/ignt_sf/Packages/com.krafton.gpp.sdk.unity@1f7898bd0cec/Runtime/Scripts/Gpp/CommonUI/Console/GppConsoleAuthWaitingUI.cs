using System;
using System.Collections;
using Gpp.Extension;
using Gpp.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace Gpp.CommonUI.Console
{
    internal class GppConsoleAuthWaitingUI : GppConsoleUI, IGppConsoleAuthWaitingUI
    {
        [SerializeField]
        private TextMeshProUGUI subTitle;

        [SerializeField]
        private TextMeshProUGUI description1;

        [SerializeField]
        private TextMeshProUGUI description2;

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

        protected override void Start()
        {
            base.Start();
            InitComponent();
        }
        private void Update()
        {
            if (Gamepad.current != null)
            {
                if (Gamepad.current.buttonEast.wasPressedThisFrame) // B ��ư
                {
                    _onCloseCallback?.Invoke(gameObject);
                    _onCloseCallback = null;
                }
            }
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

        protected override UIType PcUIType()
        {
            return UIType.PcAuthWaiting;
        }

        protected override void ChangeToPcUI()
        {
            GppUI.ShowPcAuthWaiting(_isAccountCheck, _onCloseCallback);
            Destroy(gameObject);
        }
    }
}