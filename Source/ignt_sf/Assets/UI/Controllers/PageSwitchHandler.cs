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

        private void ToggleScreen(bool hide)
        {
            shouldHide = hide;
            Debug.Log("Fade raised " + hide + " for " + gameObject.name, this);
            WipeScreen(shouldHide);
        }

        private void WipeScreen(bool hide)
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

            //Debug.Log("Fade raised " + hide + " for " + gameObject.name, this);

        }
    }
}
