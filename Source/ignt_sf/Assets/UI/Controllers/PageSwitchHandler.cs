using DG.Tweening;
using ignt.sports.cricket.core;
using UnityEngine;

namespace com.krafton.fantasysports.UI
{
    public class PageSwitchHandler : MonoBehaviour
    {
        [SerializeField] 
        private bool shouldHide = false;
        [SerializeField]
        private float hideDuration = 0.5f;
        public BoolEventChannelSO FadeChannel;

        void OnEnable()
        {
            FadeChannel.OnEventRaised += ToggleScreen;
        }

        void OnDisable()
        {
            FadeChannel.OnEventRaised -= ToggleScreen;
        }

        private void ShowScreen()
        {
            ToggleScreen(0.5f, false);
        }

        private void HideScreen()
        {
            ToggleScreen(0.5f, true);
        }

        private void ToggleScreen(float duration, bool hide)
        {
            if (TryGetComponent<RectTransform>(out var rect))
            {
                if (hide)
                {
                    rect.DOLocalMoveX(-1080, duration, true);
                }

                else
                {
                    rect.DOLocalMoveX(0, duration, true);
                }
            }
        }

        private void ToggleScreen(bool hide)
        {
            if (TryGetComponent<RectTransform>(out var rect))
            {
                if (hide)
                {
                    rect.DOLocalMoveX(-1080, hideDuration, true);
                }

                else
                {
                    rect.DOLocalMoveX(0, hideDuration, true);
                }
            }

            shouldHide = hide;
        }
        
    }
}
