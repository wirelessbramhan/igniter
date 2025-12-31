using Gpp;
using UnityEngine;

namespace GppSample
{
    public class SampleAppIntegrity : MonoBehaviour
    {
        #region Step 1. 설치 마켓 정보 조회

        /// <summary>
        /// 앱의 무결성을 확인하기 위한 메소드입니다.
        /// Method to verify the integrity of the app.
        /// </summary>
        public void GetInstallSource()
        {
            string source = GppSDK.GetInstallSource();
            Debug.Log($"Install Source : {source}");
        }

        #endregion
    }
}