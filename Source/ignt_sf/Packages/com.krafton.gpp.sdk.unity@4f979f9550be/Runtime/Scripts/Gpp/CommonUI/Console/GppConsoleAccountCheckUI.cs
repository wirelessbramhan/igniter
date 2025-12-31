using System;
using System.Collections;
using Gpp.Extension;
using Gpp.Localization;
using Gpp.Core;
using Gpp.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace Gpp.CommonUI.Console
{
    public class GppConsoleAccountCheckUI : GppConsoleUI, IGppConsoleAccountCheckUI
    {
        [SerializeField]
        private RawImage qrImage;
        [SerializeField]
        private TextMeshProUGUI description;
        [SerializeField]
        private TextMeshProUGUI subDescription;
        [SerializeField]
        private TextMeshProUGUI remainTimeTitle;
        [SerializeField]
        private TextMeshProUGUI remainTime;
        [SerializeField]
        private TextMeshProUGUI cancelText;
        [SerializeField]
        private TextMeshProUGUI openBrowserText;
        [SerializeField]
        private RectTransform[] contentSizeFitTransform;

        private string _redirectUri;
        private int _expiresIn;
        private Action<GameObject> _onCancelCallback;
        private Action<GameObject> _onCheckCallback;
        private Action<GameObject> _expiredCallback;
        private DateTime expiryTime;
        private Coroutine counterCoroutine;

        private void Update()
        {
            if (Gamepad.current != null)
            {
                if (Gamepad.current.buttonEast.wasPressedThisFrame) // B ��ư
                {
                    GppSyncContext.RunOnUnityThread(() =>
                    {
                        _onCancelCallback?.Invoke(gameObject);
                        _onCancelCallback = null;
                    });
                }

                if (Gamepad.current.buttonSouth.wasPressedThisFrame) // A ��ư
                {
                    _onCheckCallback?.Invoke(gameObject);
                    _onCheckCallback = null;
                }
            }
        }

        public void SetRedirectUri(string redirectUri, int expiresIn)
        {
            _redirectUri = redirectUri;
            _expiresIn = expiresIn;
        }

        public void SetOnClickCheck(Action<GameObject> onCheck)
        {
            _onCheckCallback = onCheck;
        }

        public void SetOnClickCancel(Action<GameObject> onCancel)
        {
            _onCancelCallback = onCancel;
        }

        public void SetOnExpireListener(Action<GameObject> callback)
        {
            _expiredCallback = callback;
        }

        protected override void Start()
        {
            base.Start();
            InitComponents();
        }

        private void InitComponents()
        {
            Canvas.ForceUpdateCanvases();
            qrImage.texture = GppUtil.CreateQrCodeImage(_redirectUri, 180, 180);
            description.text = $"{LocalizationKey.AdditionalAccInfoRequired.Localise()}";
            subDescription.text = $"{LocalizationKey.ScanQrForHealUpAndTryAgain.Localise()} {LocalizationKey.CodeValidFor10Mins.Localise()} {LocalizationKey.IfExipredCloseTabTryAgain.Localise()}";
            cancelText.text = LocalizationKey.GeneralCancel.Localise();
            openBrowserText.text = LocalizationKey.LinkInBrowser.Localise();

            remainTimeTitle.text = LocalizationKey.MaintenanceRemainingTime.Localise();
            remainTime.text = ConvertSecondsToString(_expiresIn);
            expiryTime = DateTime.Now.AddSeconds(_expiresIn);
            remainTime.text = ConvertSecondsToString((long)(expiryTime - DateTime.Now).TotalSeconds);
            counterCoroutine = StartCoroutine(CountRemainTime());

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
        private IEnumerator CountRemainTime()
        {
            while (true)
            {
                var remainingTime = (long)(expiryTime - DateTime.Now).TotalSeconds;

                if (remainingTime <= 0)
                {
                    _expiredCallback?.Invoke(gameObject);
                    yield break;
                }

                remainTime.text = ConvertSecondsToString(remainingTime);

                yield return new WaitForSeconds(1);
            }
        }

        private static string ConvertSecondsToString(long seconds)
        {
            const int secondsPerMinute = 60;
            const int secondsPerHour = 3600;

            long minutes = seconds % secondsPerHour / secondsPerMinute;
            long secs = seconds % secondsPerMinute;

            string result = "";
            result += $"{minutes:D2} : ";
            result += $"{secs:D2}";

            return result.Trim();
        }

        protected override UIType PcUIType()
        {
            return UIType.PcAccountCheck;
        }

        protected override void ChangeToPcUI()
        {
            GppUI.ShowAccountCheck(_redirectUri, _expiresIn, _onCheckCallback, _onCancelCallback, _expiredCallback);
            Destroy(gameObject);
        }
    }
}