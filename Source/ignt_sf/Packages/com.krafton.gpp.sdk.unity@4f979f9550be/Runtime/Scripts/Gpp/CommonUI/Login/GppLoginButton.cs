using Gpp.Constants;
using Gpp.Utils;
using UnityEngine;

namespace Gpp.CommonUI.Login
{
    public class GppLoginButton : MonoBehaviour
    {
        public PlatformType platformType;

        private void Awake()
        {
            GppCommonButton button = GetComponent<GppCommonButton>();
            GppLoginUI loginUI = GppUtil.FindParent<GppLoginUI>(gameObject);
            if (button != null && loginUI != null)
            {
                button.onClick.AddListener(() => { loginUI.OnClickLogin(platformType); });
            }
        }
    }
}