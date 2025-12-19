using UnityEngine;

namespace com.krafton.fantasysports.UI
{
    [CreateAssetMenu(menuName = "UI/GridView/Grid Data", fileName = "GridData_New", order = 0)]
    public class GridDataSO : ScriptableObject
    {
        public float Spacing, InnerSpacing;
        public RectOffset Padding;

        public Vector2 GridDimensions;
    }

}