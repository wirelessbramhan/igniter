using TMPro;
using UnityEngine;

namespace com.krafton.fantasysports.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextHandler : ViewHandler<TextMeshProUGUI>
    {
        [SerializeField] protected FontDataSO fontData;
        [SerializeField] protected FontType fontType;
        protected override void Configure(TextMeshProUGUI component)
        {
            if (fontData)
            {
                var data = fontData.GetFontData(fontType);

                component.fontSize = data.FontSize;
                component.characterSpacing = data.CharacterSpacing;
                component.font = fontData.FontAsset;
                component.UpdateFontAsset();
            }
        }
    }
}
