using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.krafton.fantasysports.UI
{
    public class PreviewPlayerHandler : MonoBehaviour
    {
        public Image PlayerIcon;
        public TextMeshProUGUI PlayerName;
        public RectTransform CaptChip, ViceCaptChip;

        public void SetData(Image icon, string playerName, bool isCapt, bool isViceCapt)
        {
            PlayerIcon = icon;
            PlayerName.text = playerName;
            CaptChip.gameObject.SetActive(isCapt);
            ViceCaptChip.gameObject.SetActive(isViceCapt);
        }

        public void SetData(string playerName, bool isCapt, bool isViceCapt)
        {
            PlayerName.text = playerName;
            CaptChip.gameObject.SetActive(isCapt);
            ViceCaptChip.gameObject.SetActive(isViceCapt);
        }
    }
}
