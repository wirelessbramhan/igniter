using UnityEngine;

namespace com.krafton.fantasysports.UI
{
    public enum ElementType
    {
        icon,
        text,
        btn,
        layout,
        iconTextH,
        iconTextV,
        btnIcon,
        btnText,
        btnIconTextH,
        btnIconTextV,
    }
    
    [System.Serializable]
    public class ElementData
    {
        [HideInInspector]
        public string Name;
        public ElementType Type;
        public IconData IconData;
        public FontData FontData;

        public virtual void SetName(string prefix)
        {
            Name = prefix + "_" + Type.ToString();
        }
    }

    [System.Serializable]
    public class IconData
    {
        public Sprite IconShape;
        public Color IconColor, BGColor;
    }
}
