using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.krafton.fantasysports.UI
{
    [RequireComponent(typeof(Button))]
    public class ButtonHandler : ViewHandler<Button>
    {
        [SerializeField]
        protected Image btnImage;
        [SerializeField]
        protected TextMeshProUGUI btnText;
        [SerializeField]
        protected ColorBlock btnColors;
        [SerializeField]
        protected Sprite btnIcon;
        [SerializeField]
        protected string textData;
        [SerializeField]
        protected float fontSize;
        [SerializeField]
        protected Color fontColor = Color.white, iconColor = Color.white;

        protected override void Setup()
        {
            base.Setup();
            btnImage = ViewComponent.image;
            btnText = ViewComponent.GetComponentInChildren<TextMeshProUGUI>();
        }
        protected override void Configure(Button component)
        {
            component.colors = btnColors;
            
            btnImage.sprite = btnIcon;
            btnImage.color = iconColor;
            
            btnText.text = textData;
            btnText.fontSize = fontSize;
            btnText.color = fontColor;
        }
    }
}
