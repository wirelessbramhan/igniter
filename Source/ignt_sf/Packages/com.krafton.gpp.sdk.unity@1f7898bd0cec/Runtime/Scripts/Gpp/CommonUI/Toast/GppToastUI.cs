using System;
using System.Collections;
using Gpp.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace Gpp.CommonUI.Toast
{
    public class GppToastUI : GppCommonUI, IGppToastUI
    {
        private CanvasGroup _canvasGroup;
        private GppToastMessage _toastMessage;
        private Action _onClose;

        protected override void OnChangedOrientation(ScreenOrientation orientation)
        {
            base.OnChangedOrientation(orientation);
            InitComponents();
        }

        protected override void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void SetToastMessage(GppToastMessage toastMessage)
        {
            _toastMessage = toastMessage;
            InitComponents();
            FadeInAndOut(_toastMessage.AnimSec);
        }

        private void InitComponents()
        {
            if (PlatformUtil.IsConsole())
            {
                // 콘솔에서는 토스트 UI의 X 버튼을 표시안함
                var closeBtn = GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == "CloseButton");
                if (closeBtn != null)
                    closeBtn.gameObject.SetActive(false);
            }

            SetMessage();
            SetSubMessage();
        }

        private void LateUpdate()
        {
            SetPosition();
            SetSize();
        }

        public void SetOnClickCloseListener(Action onClose)
        {
            _onClose = onClose;
        }

        public void Close()
        {
            _onClose?.Invoke();
            Destroy(gameObject);
        }

        private void SetSize()
        {
            var maxWidth = Screen.width * 0.9f;
            var content = GppUtil.FindChildWithName(CurrentLayout, "Content");
            var contentRect = content.GetComponent<RectTransform>();
            var hlg = content.GetComponent<HorizontalLayoutGroup>();
            var body = GppUtil.FindChildWithName(CurrentLayout, "Body");
            var vlg = body.GetComponent<VerticalLayoutGroup>();
            if (GppUtil.IsRtlLang())
            {
                hlg.spacing = 40;
                if (GetOrientation() != ScreenOrientation.Portrait)
                {
                    vlg.childAlignment = TextAnchor.MiddleRight;   
                }
            }
            else
            {
                hlg.spacing = 200;
            }
            if (!(contentRect.rect.width > maxWidth))
            {
                return;
            }

            var le = content.GetComponent<LayoutElement>();
            le.preferredWidth = maxWidth;
            
        }

        private void SetPosition()
        {
            var background = GppUtil.FindChildWithName(CurrentLayout, "Background");
            var backgroundRect = background.GetComponent<RectTransform>();
            float contentSizeX = backgroundRect.sizeDelta.x;
            float contentSizeY = backgroundRect.sizeDelta.y;

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
                case GppToastPosition.TOP_LEFT:
                    backgroundRect.anchorMin = new Vector2(safeAreaAnchorMin.x, safeAreaAnchorMax.y);
                    backgroundRect.anchorMax = new Vector2(safeAreaAnchorMin.x, safeAreaAnchorMax.y);
                    backgroundRect.anchoredPosition = new Vector2(contentSizeX / 2 + padding, -contentSizeY / 2 - padding);
                    break;
                case GppToastPosition.TOP_RIGHT:
                    backgroundRect.anchorMin = new Vector2(safeAreaAnchorMax.x, safeAreaAnchorMax.y);
                    backgroundRect.anchorMax = new Vector2(safeAreaAnchorMax.x, safeAreaAnchorMax.y);
                    backgroundRect.anchoredPosition = new Vector2(-contentSizeX / 2 - padding, -contentSizeY / 2 - padding);
                    break;
                case GppToastPosition.BOTTOM_LEFT:
                    backgroundRect.anchorMin = new Vector2(safeAreaAnchorMin.x, safeAreaAnchorMin.y);
                    backgroundRect.anchorMax = new Vector2(safeAreaAnchorMin.x, safeAreaAnchorMin.y);
                    backgroundRect.anchoredPosition = new Vector2(contentSizeX / 2 + padding, contentSizeY / 2 + padding);
                    break;
                case GppToastPosition.BOTTOM_RIGHT:
                    backgroundRect.anchorMin = new Vector2(safeAreaAnchorMax.x, safeAreaAnchorMin.y);
                    backgroundRect.anchorMax = new Vector2(safeAreaAnchorMax.x, safeAreaAnchorMin.y);
                    backgroundRect.anchoredPosition = new Vector2(-contentSizeX / 2 - padding, contentSizeY / 2 + padding);
                    var vlg = background.GetComponent<VerticalLayoutGroup>();
                    vlg.padding.left = 0;
                    vlg.padding.right = 50;
                    vlg.padding.top = 0;
                    vlg.padding.bottom = 50;
                    vlg.childAlignment = TextAnchor.LowerRight;
                    break;
                default:
                    backgroundRect.anchorMin = new Vector2(0.5f, 0.5f);
                    backgroundRect.anchorMax = new Vector2(0.5f, 0.5f);
                    backgroundRect.anchoredPosition = Vector2.zero;
                    break;
            }
        }

        private void SetSubMessage()
        {
            var messageComponent = GppUtil.FindChildWithName<TextMeshProUGUI>(CurrentLayout, "MessageText");
            messageComponent.text = _toastMessage.Message;
        }

        private void SetMessage()
        {
            var subMessageComponent = GppUtil.FindChildWithName<TextMeshProUGUI>(CurrentLayout, "SubMessageText");
            if (string.IsNullOrEmpty(_toastMessage.SubMessage))
            {
                subMessageComponent.gameObject.SetActive(false);
                return;
            }
            subMessageComponent.gameObject.SetActive(true);
            subMessageComponent.text = _toastMessage.SubMessage;
        }

        public void FadeInAndOut(float duration)
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
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Clamp01(elapsedTime / duration);
                yield return null;
            }
        }

        private IEnumerator FadeOut(float duration)
        {
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Clamp01(1 - elapsedTime / duration);
                yield return null;
            }
        }
    }
}