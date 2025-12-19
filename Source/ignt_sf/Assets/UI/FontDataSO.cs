using System.Collections.Generic;
using UnityEngine;

namespace com.krafton.fantasysports.UI
{
    public enum FontType
    {
        title,
        heading1,
        heading2,
        body,
        hud
    }
    [System.Serializable]
    public class FontData
    {
        [HideInInspector] public string Name;
        public FontType FontType;
        public Color FontColor;
        public float FontSize;
        public float CharacterSpacing;
    }

    [CreateAssetMenu(menuName = "UI/Data Holder/Font Data", fileName = "FontData_New", order = 4)]
    public class FontDataSO : ScriptableObject
    {
        public List<FontData> FontDatas;

        public FontData GetFontData(FontType fontType)
        {
            FontData newFont = null;

            foreach (var data in FontDatas)
            {
                if (fontType == data.FontType)
                {
                    newFont = data;
                }
            }

            return newFont;
        }
    }
}
