using UnityEngine;

namespace Gpp.CommonUI
{
    public class SafeAreaResizer : MonoBehaviour
    {
        private void Awake()
        {
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
            var rect = GetComponent<RectTransform>();

            if (rect == null)
            {
                return;
            }

            var minAnchor = Screen.safeArea.min;
            var maxAnchor = Screen.safeArea.max;

            minAnchor.x /= Screen.width;
            minAnchor.y /= Screen.height;

            maxAnchor.x /= Screen.width;
            maxAnchor.y /= Screen.height;

            rect.anchorMin = minAnchor;
            rect.anchorMax = maxAnchor;
#endif
        }
    }
}