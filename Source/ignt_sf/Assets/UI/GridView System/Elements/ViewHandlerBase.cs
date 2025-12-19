using UnityEngine;

namespace com.krafton.fantasysports.UI
{
    public abstract class ViewHandlerBase : MonoBehaviour
    {
        void OnValidate()
        {
            InitHandler();
        }

        public virtual void InitHandler()
        {
            Setup();
        }

        protected abstract void Setup();
    }
}
