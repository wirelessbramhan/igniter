using UnityEngine;

namespace com.krafton.fantasysports.UI
{
    public abstract class ViewHandler<T> : ViewHandlerBase
    {
        [SerializeField] protected T ViewComponent;
        public override void InitHandler()
        {
            Setup();

            if (ViewComponent != null)
            {
                Configure(ViewComponent);
            }
        }

        protected override void Setup()
        {
            ViewComponent = GetComponent<T>();
        }
        protected abstract void Configure(T component);

        [ContextMenu("Init Element")]
        protected void ForceInit()
        {
            if (ViewComponent != null)
            {
                Configure(ViewComponent);
            }
        }
    }
}
