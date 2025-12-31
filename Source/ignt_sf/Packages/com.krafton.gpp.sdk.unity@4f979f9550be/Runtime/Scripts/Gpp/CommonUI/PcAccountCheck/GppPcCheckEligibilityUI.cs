using System;
using System.Collections;
using Gpp.Constants;
using Gpp.Extension;
using Gpp.Localization;
using Gpp.Models;
using Gpp.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gpp.CommonUI.PcCheckEligibility
{
    public class GppPcCheckEligibilityUI : GppCommonPcUI, IGppPcCheckEligibilityUI
    {
        [SerializeField]
        private Button cancelButton;

        [SerializeField]
        private RawImage qrImage;

        [SerializeField]
        private Button accountCheckButton;

        [SerializeField]
        private TextMeshProUGUI remainTimeTitle;

        [SerializeField]
        private TextMeshProUGUI remainTime;

        [SerializeField]
        private RectTransform[] contentSizeFitTransform;

        private CheckEligibilityResult _checkEligibilityResult;
        private Action<GameObject> _onCheckCallback;
        private Action<GameObject> _onCancelCallback;
        private Action<GameObject> _expiredCallback;
        private Coroutine counterCoroutine;
        private DateTime expiryTime;

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

        private void Start()
        {
            InitComponents();
        }

        private void InitComponents()
        {
            cancelButton.onClick.AddListener(() =>
            {
                _onCancelCallback?.Invoke(gameObject);
            });
            Canvas.ForceUpdateCanvases();
            qrImage.texture = GppUtil.CreateQrCodeImage(_checkEligibilityResult.RedirectUri, 114, 114);

            accountCheckButton.onClick.AddListener(() =>
            {
                _onCheckCallback?.Invoke(gameObject);
            });

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

        protected override UIType ConsoleUIType()
        {
            return UIType.ConsoleAccountCheck;
        }

        protected override void UpdateController(InputControllerType prevController, InputControllerType currentController)
        {
            if (GppSDK.GetInputController().IsGamePad())
            {
                GppUI.ShowCheckEligibility(_checkEligibilityResult, _onCheckCallback, _onCancelCallback, _expiredCallback);
                Destroy(gameObject);
            }
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
    }
}