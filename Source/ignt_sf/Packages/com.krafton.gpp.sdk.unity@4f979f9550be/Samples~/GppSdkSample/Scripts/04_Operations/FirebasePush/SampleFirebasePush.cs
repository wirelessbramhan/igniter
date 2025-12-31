using Gpp;
using UnityEngine;

namespace GppSample
{
    public class SampleFirebasePush : MonoBehaviour
    {
        #region Step 1. 일반 푸쉬 상태 조회
        
        /// <summary>
        /// 유저의 일반 푸쉬 상태를 조회합니다.
        /// Retrieves the user's general push notification status.
        /// </summary>
        public void GetPushStatus()
        {
            bool status = GppSDK.GetPushStatus();
            Debug.Log($"GetPushStatus IsEnabled: {status}");
            // 유저의 푸쉬 상태를 업데이트 합니다.
            // Update the user's push notification status.
        }
        
        #endregion

        #region Step 2. 일반 푸쉬 상태 설정
        
        /// <summary>
        /// 유저의 일반 푸쉬 상태를 설정합니다.
        /// Sets the user's general push notification status.
        /// </summary>
        public void SetPushStatus(bool enable)
        {
            GppSDK.SetPushStatus(enable, result =>
            {
                Debug.Log($"SetPushEnable IsEnabled : {result}");
                // 유저의 푸쉬 상태를 업데이트 합니다.
                // Update the user's push notification status.
            });
        }
        
        #endregion

        #region Step 3. 야간 푸쉬 상태 조회
        
        /// <summary>
        /// 유저의 야간 푸쉬 상태를 조회합니다. 만약 일반 푸쉬 상태가 OFF라면 야간 푸쉬 상태도 OFF입니다.
        /// Retrieves the user's night push notification status. If the general push status is OFF, the night push status is also OFF.
        /// </summary>
        public void GetNightPushStatus()
        {
            bool status = GppSDK.GetNightPushStatus();
            Debug.Log($"GetNightPushStatus IsEnabled : {status}");
            // 유저의 야간 푸쉬 상태를 업데이트 합니다.
            // Update the user's night push notification status.
        }

        #endregion

        #region Step 4. 야간 푸쉬 상태 설정
        
        /// <summary>
        /// 유저의 야간 푸쉬 상태를 설정합니다. 만약 일반 푸쉬 상태가 OFF라면 야간 푸쉬 상태도 OFF로 설정됩니다.
        /// Sets the user's night push notification status. If the general push status is OFF, the night push status is also set to OFF.
        /// </summary>
        public void SetNightPushStatus(bool enable)
        {
            GppSDK.SetNightPushStatus(enable, result =>
            {
                Debug.Log($"SetNightPushEnable IsEnabled : {result}");
                // 유저의 야간 푸쉬 상태를 업데이트 합니다.
                // Update the user's night push notification status.
            });
        }
        
        #endregion
    }
}