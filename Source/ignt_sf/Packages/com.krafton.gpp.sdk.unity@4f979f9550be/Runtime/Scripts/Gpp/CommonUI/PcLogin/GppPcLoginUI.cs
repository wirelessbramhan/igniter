using System;
using System.Collections;
using System.Collections.Generic;
using Gpp.Constants;
using Gpp.Extension;
using Gpp.Localization;
using Gpp.Models;
using Gpp.Telemetry;
using Gpp.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gpp.CommonUI.Login
{
    internal class GppPcLoginUI : GppCommonPcUI, IGppPcLoginUI
    {
        [SerializeField]
        private TextMeshProUGUI description;

        [SerializeField]
        private TextMeshProUGUI subDescription;

        [SerializeField]
        private TextMeshProUGUI remainTimeTitle;

        [SerializeField]
        private TextMeshProUGUI remainTime;

        [SerializeField]
        private RawImage qrImage;

        [SerializeField]
        private Button linkButton;

        [SerializeField]
        private TextMeshProUGUI linkText;

        [SerializeField]
        private Button closeButton;

        [SerializeField]
        private Button loginButton;

        [SerializeField]
        private TextMeshProUGUI loginButtonText;

        [SerializeField]
        private RectTransform[] contentSizeFitTransform;

        private PcConsoleAuthResult uiData;

        private Action<GameObject> onCloseCallback;

        private Coroutine counterCoroutine;

        private DateTime expiryTime;

        private Action<GameObject> expiredCallback;

        public void RefreshLoginUIData(PcConsoleAuthResult data)
        {
            uiData = data;
            InitComponents();
        }

        public void SetOnClickCloseListener(Action<GameObject> onClose)
        {
            onCloseCallback = onClose;
        }

        public void SetOnExpireListener(Action<GameObject> callback)
        {
            expiredCallback = callback;
        }

        private void InitComponents()
        {
            if (uiData == null)
            {
                return;
            }

            string platformName = GppSDK.GetSession().AttemptingLoginPlatform.ToString();
            if (GppSDK.GetSession().AttemptingLoginPlatform == PlatformType.AppleMac)
                platformName = "Apple";
            else if (GppSDK.GetSession().AttemptingLoginPlatform == PlatformType.live)
                platformName = "XBOX";


            var descriptionDic = new Dictionary<string, string>
            {
                { "{n}",  platformName}
            };

            if (uiData.ErrorCode == 10163)
            {
                descriptionDic.Add("{m}", $"<color=#ff0000>{uiData.KTag}</color>");
                description.text = LocalizationKey.ThisPlatformAccHasLinkedKid.Localise(null, descriptionDic);
                subDescription.text = $"{LocalizationKey.ScanQrForSimpleRelink.Localise()} {LocalizationKey.CodeValidFor10Mins.Localise()} {LocalizationKey.ProceedWithLinkingAfterSeeingCode.Localise()}";
                loginButtonText.text = LocalizationKey.ConnectDirectlyOnThisDevice.Localise();
            }
            else
            {
                loginButtonText.text = LocalizationKey.ConnectDirectlyOnThisDevice.Localise();
                description.text = LocalizationKey.KidMustExistToContinue.Localise(null, descriptionDic);
                subDescription.text = $"{LocalizationKey.ScanQrForSimpleLogin.Localise()} {LocalizationKey.CodeValidFor10Mins.Localise()} {LocalizationKey.IfExipredCloseTabTryAgain.Localise()}";
            }
            if (linkButton != null)
            {
                linkButton.onClick.RemoveAllListeners();
                linkButton.onClick.AddListener(() =>
                {
                    Application.OpenURL(uiData.VerificationUri);
                });
            }
            if (linkText != null)
                linkText.text = uiData.VerificationUri;
            qrImage.texture = GppUtil.CreateQrCodeImage(uiData.RedirectUri, 100, 100);
            loginButton.onClick.RemoveAllListeners();
            loginButton.onClick.AddListener(() =>
            {
                GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.LoginQrManualOpen);
                GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.ExposeBrowser);
                Application.OpenURL(uiData.RedirectUri);
            });
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(() =>
            {
                onCloseCallback?.Invoke(gameObject);
            });
            remainTimeTitle.text = LocalizationKey.MaintenanceRemainingTime.Localise();
            remainTime.text = ConvertSecondsToString(uiData.ExpiresIn);
            expiryTime = DateTime.Now.AddSeconds(uiData.ExpiresIn);
            remainTime.text = ConvertSecondsToString((long)(expiryTime - DateTime.Now).TotalSeconds);
            counterCoroutine = StartCoroutine(CountRemainTime());

            for (int i = 0; i < contentSizeFitTransform.Length; i++)
            {
                RefreshLayout(contentSizeFitTransform[i]);
            }

            RefreshLayout(remainTimeTitle.transform.parent.GetComponent<RectTransform>());
            RefreshLayout(remainTimeTitle.rectTransform);
            RefreshLayout(remainTime.rectTransform);
        }

        private void OnDestroy()
        {
            if (counterCoroutine != null)
            {
                StopCoroutine(counterCoroutine);
            }
        }

        private IEnumerator CountRemainTime()
        {
            while (true)
            {
                var remainingTime = (long)(expiryTime - DateTime.Now).TotalSeconds;

                if (remainingTime <= 0)
                {
                    expiredCallback?.Invoke(gameObject);
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
            return UIType.ConsoleLogin;
        }

        protected override void UpdateController(InputControllerType prevController, InputControllerType currentController)
        {
            if (GppSDK.GetInputController().IsGamePad())
            {
                GppUI.ShowPcLogin(uiData, onCloseCallback, expiredCallback);
                Destroy(gameObject);
            }
        }
    }
}