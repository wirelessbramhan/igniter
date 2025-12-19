using System;
using System.Collections;
using Gpp.CommonUI.Toast;
using Gpp.Extension;
using Gpp.Localization;
using Gpp.Models;
using Gpp.Surveys;
using Gpp.Telemetry;
using Gpp.Utils;
using TMPro;
using UnityEngine;

namespace Gpp.CommonUI
{
    public class GppSurveyUI : GppCommonUI, IGppToastUI
    {
        private CanvasGroup _canvasGroup;
        private GppToastMessage _toastMessage;
        private Action _onClose;

        #region IGppToastUI Interface
        protected override void OnChangedOrientation(ScreenOrientation orientation)
        {
            base.OnChangedOrientation(orientation);
            InitComponents();
        }
        public void SetToastMessage(GppToastMessage toastMessage)
        {
            GppTelemetry.SendSurvey(GppClientLogModels.SurveyEventName.EXPOSED, GppSurveyDataManager.GetSurveyMonkeyId());
            _toastMessage = toastMessage;
            InitComponents();
            FadeInAndOut(_toastMessage.AnimSec);
        }

        public void SetOnClickCloseListener(Action onClose)
        {
            _onClose = onClose;
        }
        #endregion

        #region UI Events
        public void OnClose()
        {
            if (GppSDK.IsLoggedIn is false)
            {
                Close();
                return;
            }

            GppTelemetry.SendSurvey(GppClientLogModels.SurveyEventName.CLOSED, GppSurveyDataManager.GetSurveyMonkeyId());
            Close();
        }

        public void OnParticipate()
        {
            if (GppSDK.IsLoggedIn is false)
            {
                Close();
                return;
            }

            GppTelemetry.SendSurvey(GppClientLogModels.SurveyEventName.CLICKED, GppSurveyDataManager.GetSurveyMonkeyId());
            Application.OpenURL(GppSurveyDataManager.GetSurveyLink());

            Close();
        }

        public void OnDoNotShowAgame()
        {
            if (GppSDK.IsLoggedIn is false)
            {
                Close();
                return;
            }

            GppTelemetry.SendSurvey(GppClientLogModels.SurveyEventName.DO_NOT_SHOW_CLICKED, GppSurveyDataManager.GetSurveyMonkeyId());
            GppSDK.GetSocialApi().SurveyDoNotShowAgain(result =>
            {
                Close();
            }, GppSurveyDataManager.GetSurveyId());
        }
        #endregion

        protected override void Awake()
        {
            base.Awake();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void LateUpdate()
        {
            SetPosition();
        }

        private void InitComponents()
        {
            SetTitle();
            SetMessage();
            LocalizeText();
        }

        private void SetTitle()
        {
            var titleText = GppUtil.FindChildWithName<TextMeshProUGUI>(CurrentLayout, "TitleText");
            titleText.text = _toastMessage.Message;
        }

        private void SetMessage()
        {
            var messageText = GppUtil.FindChildWithName<TextMeshProUGUI>(CurrentLayout, "MessageText");
            messageText.text = _toastMessage.SubMessage;
        }

        private void LocalizeText()
        {
            var participateText = GppUtil.FindChildWithName<TextMeshProUGUI>(CurrentLayout, "ParticipateText");
            var doNotShowAgainText = GppUtil.FindChildWithName<TextMeshProUGUI>(CurrentLayout, "DoNotShowAgainButton");
            var closeText = GppUtil.FindChildWithName<TextMeshProUGUI>(CurrentLayout, "CloseButton");

            participateText.text = LocalizationKey.SurveyParticipate.Localise();
            doNotShowAgainText.text = LocalizationKey.SurveyDoNotShowAgain.Localise();
            closeText.text = LocalizationKey.SurveyClose.Localise();
        }

        private void Close()
        {
            _onClose?.Invoke();
            Destroy(gameObject);
        }

        private void SetPosition()
        {
            var background = GppUtil.FindChildWithName(CurrentLayout, "Background");
            var backgroundRect = background.GetComponent<RectTransform>();
            var contentSizeY = backgroundRect.sizeDelta.y;

            const float padding = 50f;

            var safeArea = Screen.safeArea;
            var safeAreaAnchorMin = new Vector2(safeArea.xMin / Screen.width, safeArea.yMin / Screen.height);
            var safeAreaAnchorMax = new Vector2(safeArea.xMax / Screen.width, safeArea.yMax / Screen.height);

            switch (_toastMessage.Position)
            {
                case GppToastPosition.TOP:
                    backgroundRect.anchorMin = new Vector2(0.5f, safeAreaAnchorMax.y);
                    backgroundRect.anchorMax = new Vector2(0.5f, safeAreaAnchorMax.y);
                    backgroundRect.anchoredPosition = new Vector2(0, -contentSizeY / 2 - padding);
                    break;
                case GppToastPosition.BOTTOM:
                    backgroundRect.anchorMin = new Vector2(0.5f, safeAreaAnchorMin.y);
                    backgroundRect.anchorMax = new Vector2(0.5f, safeAreaAnchorMin.y);
                    backgroundRect.anchoredPosition = new Vector2(0, contentSizeY / 2 + padding);
                    break;
                case GppToastPosition.CENTER:
                    backgroundRect.anchorMin = new Vector2(0.5f, (safeAreaAnchorMin.y + safeAreaAnchorMax.y) / 2);
                    backgroundRect.anchorMax = new Vector2(0.5f, (safeAreaAnchorMin.y + safeAreaAnchorMax.y) / 2);
                    backgroundRect.anchoredPosition = Vector2.zero;
                    break;
                default:
                    backgroundRect.anchorMin = new Vector2(0.5f, 0.5f);
                    backgroundRect.anchorMax = new Vector2(0.5f, 0.5f);
                    backgroundRect.anchoredPosition = Vector2.zero;
                    break;
            }
        }

        private void FadeInAndOut(float duration)
        {
            StartCoroutine(FadeToast(duration));
        }

        private IEnumerator FadeToast(float duration)
        {
            yield return StartCoroutine(FadeIn(1));
            yield return new WaitForSeconds(duration);
            yield return StartCoroutine(FadeOut(1));
            Destroy(gameObject);
        }

        private IEnumerator FadeIn(float duration)
        {
            var elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Clamp01(elapsedTime / duration);
                yield return null;
            }
        }

        private IEnumerator FadeOut(float duration)
        {
            var elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Clamp01(1 - elapsedTime / duration);
                yield return null;
            }
        }
    }
}
