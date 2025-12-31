using Gpp;
using UnityEngine;

namespace GppSample
{
    public class SampleHelpdesk : MonoBehaviour
    {
        #region Step 1. HelpDesk 웹 페이지 열기
        
        /// <summary>
        /// 인앱 또는 외부 브라우저를 사용하여 HelpDesk를 열 수 있습니다.
        /// Opens the HelpDesk using either an in-app or external browser.
        /// </summary>
        public void OpenHelpDesk(bool isExternalBrowser)
        {
            GppSDK.OpenHelpDesk(result =>
                {
                    if (!result.IsError)
                    {
                        Debug.Log($"OpenHelpDeskWebView Success!");
                    }
                    else
                    {
                        Debug.Log($"OpenHelpDeskWebView Failed Code: {result.Error.Code}");
                        Debug.Log($"OpenHelpDeskWebView Failed Message: {result.Error.Message}");
                        // 유저에게 실패 팝업을 노출하고 재시도를 유도합니다.
                        // Display a failure popup to the user and prompt for a retry.
                    }
                }, isExternalBrowser
            );
        }

        public void OpenHelpDeskWebView()
        {
            GppSDK.OpenHelpDesk(result =>
                {
                    if (!result.IsError)
                    {
                        Debug.Log($"OpenHelpDeskWebView Success!");
                    }
                    else
                    {
                        Debug.Log($"OpenHelpDeskWebView Failed Code: {result.Error.Code}");
                        Debug.Log($"OpenHelpDeskWebView Failed Message: {result.Error.Message}");
                    }
                }
            );
        }

        public void OpenHelpDeskExternalBrowser()
        {
            GppSDK.OpenHelpDesk(result =>
                {
                    if (!result.IsError)
                    {
                        Debug.Log($"OpenHelpDeskExternalBrowser Success!");
                    }
                    else
                    {
                        Debug.Log($"OpenHelpDeskExternalBrowser Failed Code: {result.Error.Code}");
                        Debug.Log($"OpenHelpDeskExternalBrowser Failed Message: {result.Error.Message}");
                    }
                }, true
            );
        }

        #endregion
    }
}