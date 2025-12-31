using Gpp.Utils;
using TMPro;
using UnityEngine;

namespace Gpp.CommonUI
{
    public class RtlLangChecker : MonoBehaviour
    {
        [SerializeField]
        private float modifiedLineSpacing = -70f;
        private void Start()
        {
            if (!GppSDK.IsInitialized || !GppUtil.IsRtlLang())
            {
                return;
            }

            var tmp = GetComponent<TextMeshProUGUI>();
            if (tmp == null)
            {
                return;
            }

            tmp.horizontalAlignment = HorizontalAlignmentOptions.Right;
            tmp.lineSpacing = modifiedLineSpacing;
        }
    }
}