using System;
using System.Collections;
using Gpp.Extension;
using Gpp.Localization;
using Gpp.Models;
using Gpp.Core;
using Gpp.Telemetry;
using Gpp.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


namespace Gpp.CommonUI.Console
{
    public class GppConsoleCheckEligibilityUI : GppConsoleUI, IGppConsoleCheckEligibilityUI
    {
        [SerializeField]
        private RawImage qrImage;
        [SerializeField]
        private TextMeshProUGUI description1;
        [SerializeField]
        private TextMeshProUGUI description2;
        [SerializeField]
        private TextMeshProUGUI description3;
        [SerializeField]
        private TextMeshProUGUI description4;
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

        private CheckEligibilityResult _checkEligibilityResult;
        private Action<GameObject> _onCancelCallback;
        private Action<GameObject> _onCheckCallback;
        private Action<GameObject> _expiredCallback;
        private DateTime expiryTime;
        private Coroutine counterCoroutine;

        public void SetCheckEligibilityData(CheckEligibilityResult checkEligibilityResult)
        {
            _checkEligibilityResult = checkEligibilityResult;
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

        private void InitComponents()
        {
            Canvas.ForceUpdateCanvases();
            qrImage.texture = GppUtil.CreateQrCodeImage(_checkEligibilityResult.RedirectUri, 180, 180);
            description1.text = $"{LocalizationKey.AdditionalAccInfoRequired.Localise()}";
            description2.text = $"{LocalizationKey.ScanQrClickButtonTryAgain.Localise()}";
            description3.text = $"{LocalizationKey.QrButtonValidFor10Mins.Localise()}";
            description4.text = $"{LocalizationKey.IfExipredCloseTabTryAgain.Localise()}";
            cancelText.text = LocalizationKey.SurveyClose.Localise();
            openBrowserText.text = LocalizationKey.AdditionalAccInfoConfirm.Localise();

            remainTimeTitle.text = LocalizationKey.MaintenanceRemainingTime.Localise();
            remainTime.text = ConvertSecondsToString(_checkEligibilityResult.ExpiresIn);
            expiryTime = DateTime.Now.AddSeconds(_checkEligibilityResult.ExpiresIn);
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
            GppUI.ShowCheckEligibility(_checkEligibilityResult, _onCheckCallback, _onCancelCallback, _expiredCallback);
            Destroy(gameObject);
        }
    }
}