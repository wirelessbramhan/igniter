using System;
using System.Collections;
using Gpp.Constants;
using Gpp.Extension;
using Gpp.Localization;
using Gpp.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gpp.CommonUI.PcAccountCheck
{
    public class GppPcAccountCheckUI : GppCommonPcUI, IGppPcAccountCheckUI
    {
        [SerializeField]
        private Button cancelButton;

        [SerializeField]
        private RawImage qrImage;

        [SerializeField]
        private Button accountCheckButton;

        [SerializeField]
        private TextMeshProUGUI description;

        [SerializeField]
        private TextMeshProUGUI remainTimeTitle;

        [SerializeField]
        private TextMeshProUGUI remainTime;

        [SerializeField]
        private RectTransform[] contentSizeFitTransform;

        private string _redirectUri;
        private int _expiresIn;
        private Action<GameObject> _onCancelCallback;
        private Action<GameObject> _onCheckCallback;
        private Action<GameObject> _expiredCallback;
        private DateTime expiryTime;
        private Coroutine counterCoroutine;

        public void SetRedirectUri(string redirectUrl, int expiresIn)
        {
            _redirectUri = redirectUrl;
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
            qrImage.texture = GppUtil.CreateQrCodeImage(_redirectUri, 92, 92);
            description.text = $"{LocalizationKey.ScanQrForHealUpAndTryAgain.Localise()} {LocalizationKey.CodeValidFor10Mins.Localise()} {LocalizationKey.IfExipredCloseTabTryAgain.Localise()}";

            accountCheckButton.onClick.AddListener(() =>
            {
                _onCheckCallback?.Invoke(gameObject);
            });

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

        protected override UIType ConsoleUIType()
        {
            return UIType.ConsoleAccountCheck;
        }

        protected override void UpdateController(InputControllerType prevController, InputControllerType currentController)
        {
            if (GppSDK.GetInputController().IsGamePad())
            {
                GppUI.ShowAccountCheck(_redirectUri, _expiresIn, _onCheckCallback, _onCancelCallback, _expiredCallback);
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