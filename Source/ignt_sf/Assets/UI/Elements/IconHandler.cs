using UnityEngine;
using UnityEngine.UI;

namespace com.krafton.fantasysports.UI
{
    [RequireComponent(typeof(Image))]
    public class IconHandler : ViewHandler<Image>
    {
        [SerializeField]
        protected Sprite iconShape;
        [SerializeField]
        protected Color iconColor;
        protected override void Configure(Image component)
        {
            component.sprite = iconShape;
            component.color = iconColor;
        }
    }
}
