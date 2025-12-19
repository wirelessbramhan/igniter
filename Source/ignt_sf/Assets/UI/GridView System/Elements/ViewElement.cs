using UnityEngine;

namespace com.krafton.fantasysports.UI
{
    public class ViewElement : ViewElementBase
    {
        protected override void Configure()
        {
            for (int i = 0; i < viewHandlers.Length; i++)
            {
                viewHandlers[i].InitHandler();
            }
        }
    }
}
