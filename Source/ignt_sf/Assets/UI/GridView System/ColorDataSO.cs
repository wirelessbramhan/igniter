using System.Collections.Generic;
using UnityEngine;

namespace ignt.sports.cricket.UI
{
    public enum ColorType
    {
        primary,
        secondary,
        tertiary,
        quaternary,
        regular
    }

    [System.Serializable]
    public class ColorData
    {
        [HideInInspector]
        public string Name;
        public Color Color;
        public ColorType ColorType;
    }

    [CreateAssetMenu(menuName = "UI/GridView/Color Data", fileName = "ColorData_New", order = 3)]
    public class ColorDataSO : ScriptableObject
    {
        public List<ColorData> AllColors;
    }
}
