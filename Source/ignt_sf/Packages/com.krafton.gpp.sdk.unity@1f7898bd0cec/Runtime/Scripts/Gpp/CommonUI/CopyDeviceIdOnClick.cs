using Gpp.Log;
using UnityEngine;

namespace Gpp.CommonUI
{
    public class CopyDeviceIdOnClick : MonoBehaviour
    {
        public int clickThreshold = 10;             // 클릭할 횟수
        private int clickCount;                     // 현재 클릭 횟수
        private const float ClickTimeWindow = 1.0f; // 클릭을 카운트할 시간 제한 (초)
        private float lastClickTime;                // 마지막 클릭 시간

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            if (Time.time - lastClickTime <= ClickTimeWindow)
            {
                clickCount++;
            }
            else
            {
                clickCount = 1;
            }
            lastClickTime = Time.time;
            if (clickCount < clickThreshold) return;
            CopyDeviceIdToClipboard();
            clickCount = 0;
        }

        private static void CopyDeviceIdToClipboard()
        {
            if (!GppSDK.GetSession().RemoteSdkConfig.AllowCopyDeviceId)
            {
                return;
            } 
            string deviceId = SystemInfo.deviceUniqueIdentifier;
            GUIUtility.systemCopyBuffer = deviceId;
            GppLog.Log("Device ID copied to clipboard: " + deviceId);
            GppUI.ShowToast("Device ID copied to clipboard");
        }
    }
}