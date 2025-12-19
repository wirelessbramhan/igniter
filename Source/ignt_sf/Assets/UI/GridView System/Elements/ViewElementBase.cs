using System.Collections;
using ignt.sports.cricket.core;
using UnityEngine;

namespace com.krafton.fantasysports.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class ViewElementBase : MonoBehaviour
    {
        [SerializeField]
        protected RectTransform targetTransform;
        [SerializeField]
        protected CanvasGroup canvasGroup;
        [SerializeField]
        protected bool isLoaded = false, overwriteData = false;
        [SerializeField]
        protected ViewHandlerBase[] viewHandlers;

        void OnDisable()
        {
            StopAllCoroutines();
        }

        void OnValidate()
        {
            if (gameObject.activeInHierarchy && GetComponentInParent<Canvas>() != null)
            {
                Setup();
            }
        }

        [ContextMenu("init element")]
        private void Init()
        {
            Setup();
            Configure();
        }

        protected virtual void Setup()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            targetTransform = canvasGroup.GetComponent<RectTransform>();
            
            viewHandlers = GetComponentsInChildren<ViewHandlerBase>();
        }
        protected abstract void Configure();
    }
}
