using Gpp;
using UnityEngine;
using UnityEngine.UI;

public class SampleButton : MonoBehaviour
{
    public enum SupportedPlatform
    {
        Common,
        Pc,
        Mobile,
        PcNotEditor,
        MobileNotEditor
    }
    
    public bool isInitButton;
    public bool isLoginButton;
    public SupportedPlatform supportedPlatform;

    private void Update()
    {
        switch (supportedPlatform)
        {
            case SupportedPlatform.Common:
            case SupportedPlatform.Pc when IsPc():
            case SupportedPlatform.MobileNotEditor when IsMobileNotEditor():
            case SupportedPlatform.Mobile when IsMobile():
                gameObject.SetActive(true);
                break;
            default: 
                gameObject.SetActive(false);
                return;
        }
        
        if (isInitButton)
        {
            SetActive(!GppSDK.IsInitialized);
        }
        else if (isLoginButton)
        {
            SetActive(GppSDK.IsInitialized && !GppSDK.IsLoggedIn);
        }
        else
        {
            SetActive(GppSDK.IsLoggedIn);
        }
    }

    private void SetActive(bool active)
    {
        GetComponent<Button>().interactable = active;
    }
    
    private static bool IsMobile()
    {
#if UNITY_ANDROID || UNITY_IOS
        return true;
#else
            return false;
#endif
    }

    private static bool IsPc()
    {
#if !UNITY_ANDROID && !UNITY_IOS
                return true;
#else
        return false;
#endif
    }

    private static bool IsMobileNotEditor()
    {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            return true;
#else
        return false;
#endif
    }
}