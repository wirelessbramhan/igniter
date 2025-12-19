using System.Collections;
using ignt.sports.cricket.core;
using UnityEngine;

namespace com.krafton.fantasysports.UI
{
    public enum SwitchType
    {
        pop,
        fade,
        wipeLeft,
        wipeRight
    }

    [RequireComponent(typeof(CanvasGroup), typeof(RectTransform))]
    public class PanelSwitchHandler : MonoBehaviour
    {
        [SerializeField] private SwitchType switchBehavior = SwitchType.fade;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField]
        private bool isSwitching = false, isHidden = false;
        [SerializeField] private AnimationCurve lerpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        private float lerpAmount = 0;
        [Header("listening to"), Space(2)]
        public BoolEventChannelSO FadeChannel;

        void OnEnable()
        {
            FadeChannel.OnEventRaised += SwitchScreen;
        }

        private void SwitchScreen(bool hide)
        {
            if (!isSwitching)
                {
                    isHidden = hide;
                    var panel = GetComponent<RectTransform>();

                    switch (switchBehavior)
                    {
                        case SwitchType.pop:
                            StartCoroutine(PopScreen(1, isHidden));
                            break;
                        case SwitchType.fade:
                            StartCoroutine(FadeScreen(1, canvasGroup, isHidden));
                            break;
                        case SwitchType.wipeLeft:
                        StartCoroutine(WipeScreen(2, panel, isHidden, true));
                            break;
                        case SwitchType.wipeRight:
                        StartCoroutine(WipeScreen(2, panel, isHidden, false));
                            break;
                    }

                    Debug.Log("switch called " + hide + " on " + gameObject.name);
                }

                else
                {
                    Debug.Log( gameObject.name + " switching already!");
                }
        }

        void OnDisable()
        {
            FadeChannel.OnEventRaised -= SwitchScreen;
        }

        #region Lerp functions

        private IEnumerator FadeScreen(float timeToComplete, CanvasGroup panel, bool hide)
        {
            isSwitching = true;

            float currentTime = 0;
            float target = 0;

            if (!hide)
            {
                target = 1;
            }

            while(currentTime <= timeToComplete)
            {
                currentTime += Time.deltaTime;
                float fraction = currentTime / timeToComplete;
                lerpAmount = Mathf.Lerp(lerpAmount, target, lerpCurve.Evaluate(fraction));

                panel.alpha = lerpAmount;

                yield return null;
            }

            isSwitching = false;
        }

        private IEnumerator WipeScreen(float timeToComplete, RectTransform target, bool hide, bool wipeLeft)
        {
            isSwitching = true;

            float currentTime = 0;
            Vector2 endPos = Vector2.zero;

            if (hide)
            {
                if (!wipeLeft)
                {
                    endPos = new Vector2(1080, 0);
                }

                else
                {
                    endPos = new Vector2(-1080, 0);
                }
            }

            while(target.anchoredPosition != endPos)
            {
                currentTime += Time.deltaTime;
                float fraction = currentTime / timeToComplete;

                target.anchoredPosition = Vector2.Lerp(target.anchoredPosition, endPos, lerpCurve.Evaluate(fraction));

                yield return null;
            }

            isSwitching = false;
        }

        private IEnumerator PopScreen(float timeToComplete, bool hide)
        {
            isSwitching = true;

            float currentTime = 0;
            Vector3 targetScale = new(0.01f, 0.01f, 1.0f);

            if (!hide)
            {
                targetScale = Vector3.one;
            }

            while(transform.localScale != targetScale)
            {
                currentTime += Time.deltaTime;
                float fraction = currentTime / timeToComplete;

                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, lerpCurve.Evaluate(fraction));

                yield return null;
            }

            isSwitching = false;
        }

        #endregion
    }
}
