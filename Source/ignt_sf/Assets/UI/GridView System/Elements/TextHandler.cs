using TMPro;
using UnityEngine;

namespace com.krafton.fantasysports.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextHandler : ViewHandler<TextMeshProUGUI>
    {
        [SerializeField] protected string textData;
        [SerializeField] protected Color textColor = Color.white;
        [SerializeField] protected float fontSize;
        protected override void Configure(TextMeshProUGUI component)
        {
            component.text = textData;
            component.color = textColor;
            component.fontSize = fontSize;
        }
    }
}
