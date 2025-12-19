using UnityEngine;
using UnityEngine.UI;

namespace com.krafton.fantasysports.UI
{
    [RequireComponent(typeof(LayoutGroup))]
    public class ViewLayoutGroup : ViewHandler<HorizontalOrVerticalLayoutGroup>
    {
        [SerializeField]
        protected LayoutGroup layoutGroup;
        public GridDataSO gridData;

        protected override void Configure(HorizontalOrVerticalLayoutGroup component)
        {
            if (gridData)
            {
                component.padding = gridData.Padding;
                component.spacing = gridData.Spacing;
            }

            component.childAlignment = TextAnchor.MiddleCenter;

            component.childControlHeight = true;
            component.childControlWidth = true;
            component.childForceExpandHeight = true;
            component.childForceExpandWidth = true;
        }
    }
}
