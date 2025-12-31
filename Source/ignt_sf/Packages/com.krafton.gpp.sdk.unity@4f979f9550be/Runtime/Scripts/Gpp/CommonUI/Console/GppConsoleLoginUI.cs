using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Gpp.Constants;
using Gpp.Extension;
using Gpp.Localization;
using Gpp.Models;
using Gpp.Utils;
using System.Collections.Generic;
using Gpp.Log;
using System.Collections;
using UnityEngine.InputSystem;
using Gpp.Telemetry;

namespace Gpp.CommonUI.Console
{
    public class GppConsoleLoginUI : GppConsoleUI, IGppConsoleLoginUI
    {
        [SerializeField]
        private TextMeshProUGUI description1;
        [SerializeField]
        private TextMeshProUGUI description2;
        [SerializeField]
        private TextMeshProUGUI description3;

        [SerializeField]
        private TextMeshProUGUI remainTimeTitle;

        [SerializeField]
        private TextMeshProUGUI remainTime;

        [SerializeField]
        private RawImage qrImage;

        [SerializeField]
        private TextMeshProUGUI cancelText;

        [SerializeField]
        private TextMeshProUGUI openBrowserText;

        [SerializeField]
        private RectTransform[] contentSizeFitTransform;

        private PcConsoleAuthResult uiData;

        private Action<GameObject> onCancelCallback;

        private Coroutine counterCoroutine;

        private DateTime expiryTime;

        private Action<GameObject> expiredCallback;

        private float inputCooldown = 0.2f;
        private float lastInputTime = 0f;

        void Update()
        {
            if (Time.time - lastInputTime > inputCooldown)
            {
                if (Gamepad.current != null)
                {
                    if (Gamepad.current.buttonSouth.wasPressedThisFrame) // A ��ư
                    {
                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.LoginQrManualOpen);
                        GppTelemetry.SendUserEntry(GppClientLogModels.EntryStep.ExposeBrowser);
                        Application.OpenURL(uiData.RedirectUri);
                    }
                    if (Gamepad.current.buttonEast.wasPressedThisFrame) // B ��ư
                    {
                        onCancelCallback?.Invoke(gameObject);
                        onCancelCallback = null;
                    }
                }
            }
        }

        public void RefreshLoginUIData(PcConsoleAuthResult data)
        {
            uiData = data;
            InitComponents();
        }

        public void SetOnClickCancelListener(Action<GameObject> onCancel)
        {
            onCancelCallback = onCancel;
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
            if (GppSDK.GetSession().AttemptingLoginPlatform == PlatformType.live)
                platformName = "XBOX";

            var descriptionDic = new Dictionary<string, string>
            {
                { "{n}", platformName }
            };
            if (uiData.ErrorCode == 10163)
            {
                descriptionDic.Add("{m}", $"<color=#ff0000>{uiData.KTag}</color>");
                description1.text = LocalizationKey.ThisPlatformAccHasLinkedKid.Localise(null, descriptionDic);
                description2.text = $"{LocalizationKey.ScanQrForSimpleLogin.Localise()} {LocalizationKey.CodeValidFor10Mins.Localise()}";
                description3.text = LocalizationKey.IfExipredCloseTabTryAgain.Localise();
            }
            else
            {
                description1.text = LocalizationKey.KidMustExistToContinue.Localise(null, descriptionDic);
                description2.text = $"{LocalizationKey.ScanQrForSimpleLogin.Localise()} {LocalizationKey.CodeValidFor10Mins.Localise()}";
                description3.text = LocalizationKey.IfExipredCloseTabTryAgain.Localise();
            }
            qrImage.texture = GppUtil.CreateQrCodeImage(uiData.RedirectUri, 250, 250);

            cancelText.text = LocalizationKey.GeneralCancel.Localise();
            openBrowserText.text = LocalizationKey.LinkInBrowser.Localise();

            remainTimeTitle.text = LocalizationKey.MaintenanceRemainingTime.Localise();
            remainTime.text = ConvertSecondsToString(uiData.ExpiresIn);
            expiryTime = DateTime.Now.AddSeconds(uiData.ExpiresIn);
            remainTime.text = ConvertSecondsToString((long)(expiryTime - DateTime.Now).TotalSeconds);
            counterCoroutine = StartCoroutine(CountRemainTime());

            for (int i = 0; i < contentSizeFitTransform.Length; i++)
            {
                RefreshLayout(contentSizeFitTransform[i]);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
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

        protected override UIType PcUIType()
        {
            return UIType.PcLogin;
        }

        protected override void ChangeToPcUI()
        {
            GppUI.ShowPcLogin(uiData, onCancelCallback, expiredCallback);
            Destroy(gameObject);
        }
    }
}