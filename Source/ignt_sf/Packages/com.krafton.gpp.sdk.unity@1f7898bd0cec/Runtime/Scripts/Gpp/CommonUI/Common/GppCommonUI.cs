using UnityEngine;

namespace Gpp.CommonUI
{
    public abstract class GppCommonUI : MonoBehaviour
    {
        [Header("Common")]
        [SerializeField]
        protected GameObject portraitLayout;

        [SerializeField]
        protected GameObject landscapeLayout;

        protected ScreenOrientation lastOrientation;

        protected GameObject CurrentLayout => GetOrientation() == ScreenOrientation.Portrait ? portraitLayout : landscapeLayout;

        protected virtual void OnChangedOrientation(ScreenOrientation orientation)
        {
            portraitLayout.SetActive(orientation == ScreenOrientation.Portrait);
            landscapeLayout.SetActive(orientation == ScreenOrientation.LandscapeLeft);
        }

        protected virtual void Awake()
        {
            lastOrientation = GetOrientation();
        }

        protected virtual void Start()
        {
            ShowCurrentLayout();
        }

        private void ShowCurrentLayout()
        {
            portraitLayout.SetActive(GetOrientation() == ScreenOrientation.Portrait);
            landscapeLayout.SetActive(GetOrientation() == ScreenOrientation.LandscapeLeft);
        }

        protected virtual void Update()
        {
            ScreenOrientation orientation = GetOrientation();
            if (orientation == lastOrientation) return;
            lastOrientation = orientation;
            OnChangedOrientation(orientation);
        }

        protected virtual void OnDestroy()
        {
            portraitLayout = null;
            landscapeLayout = null;
        }

        protected ScreenOrientation GetOrientation()
        {
            return Screen.height > Screen.width ? ScreenOrientation.Portrait : ScreenOrientation.LandscapeLeft;
        }
    }
}