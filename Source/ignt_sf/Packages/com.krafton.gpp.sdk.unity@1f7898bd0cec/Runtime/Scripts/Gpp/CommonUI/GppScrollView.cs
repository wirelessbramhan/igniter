using UnityEngine;
using UnityEngine.UI;

namespace Gpp.CommonUI
{
    public class GppScrollView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform content;

        [SerializeField]
        private ScrollRect scrollRect;

        [SerializeField]
        private float minHeight = 200f;

        [SerializeField]
        private float maxHeight = 700f;

        private float currentContentSize;

        private void Update()
        {
            float contentHeight = content.GetComponent<RectTransform>().sizeDelta.y;
            if (!(contentHeight > 0) || !(contentHeight > currentContentSize))
            {
                return;
            }
            float newHeight = Mathf.Clamp(contentHeight, minHeight, maxHeight);
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, newHeight);
            if (scrollRect.verticalScrollbar != null)
            {
                scrollRect.verticalScrollbar.enabled = newHeight > maxHeight;
            }

            currentContentSize = newHeight;
        }
    }
}