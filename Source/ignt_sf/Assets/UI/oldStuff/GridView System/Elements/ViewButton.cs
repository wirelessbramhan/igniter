using ignt.sports.cricket.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.krafton.fantasysports.UI
{
    public class ViewButton : ViewElementBase
    {
        
        [SerializeField]
        protected Button btn;

        protected override void Configure()
        {
            throw new System.NotImplementedException();
        }

        // protected override void Configure(ElementData data)
        // {
        //     btnImage.sprite = data.IconData.IconShape;
        //     btnImage.color = data.IconData.IconColor;

        //     btnText.text = data.FontData.TextData;
        //     btnText.faceColor = data.FontData.FontColor;
        //     btnText.fontSize = data.FontData.FontSize;
        //     btnText.characterSpacing = data.FontData.CharacterSpacing;

        //     btn.colors = btnColors;
        // }

        // protected override bool Setup()
        // {
        //     if (!btn)
        //     {
        //         Debug.Log("btn not bound to " + gameObject.name, this);
        //     }

        //     btnColors.normalColor = Color.white;
        //     btnColors.highlightedColor = Color.white;
        //     btnColors.pressedColor = Color.grey;
        //     btnColors.selectedColor = Color.grey;
        //     btnColors.disabledColor = Color.grey;

        //     return base.Setup();
        // }
    }
}
