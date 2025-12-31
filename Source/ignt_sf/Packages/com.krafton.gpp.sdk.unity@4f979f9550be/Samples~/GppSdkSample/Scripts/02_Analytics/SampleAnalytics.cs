using System.Collections.Generic;
using Gpp;
using UnityEngine;

namespace GppSample
{
    public class SampleAnalytics : MonoBehaviour
    {
        #region Step 1. Telemetry (Eventually)

        /// <summary>
        /// 게임 지표를 전송할 수 있는 메소드 입니다. 로그인 이후에 호출해야 합니다.
        /// Method to send game metrics. This should be called after logging in.
        /// </summary>
        public void TelemetryEventually()
        {
            Dictionary<string, object> payload = new Dictionary<string, object>
            {
                { "test_key1", "test_value1" },
                { "test_key2", "test_value2" },
                { "test_key3", "test_value3" },
            };

            GppSDK.SendGameTelemetry("test_event_name_1", payload);
        }

        #endregion

        #region Step 2. Telemetry (Immediately)

        public void TelemetryImmediately()
        {
            Dictionary<string, object> payload = new Dictionary<string, object>
            {
                { "test_key1", "test_value1" },
                { "test_key2", "test_value2" },
                { "test_key3", "test_value3" },
            };

            GppSDK.SendGameTelemetryImmediately("test_event_name_2", payload);
        }

        #endregion

        #region Step 3. Send GoogleAnalytics Event

        /// <summary>
        /// GoogleAnalytics를 이용하여 Event를 전송합니다.
        /// Sends an event using Google Analytics.
        /// </summary>
        public void SendGoogleAnalyticsEvent()
        {
            const string eventName = "sample_event_name";
            Dictionary<string, object> payload = new Dictionary<string, object>
            {
                { "test_key1", "test_value1" },
                { "test_key2", "test_value2" },
                { "test_key3", "test_value3" }
            };
            GppSDK.SendGoogleAnalyticsEvent(eventName, payload);
        }


        /// <summary>
        /// Google Analytics의 User Property 정보를 설정합니다.
        /// Set User Property information for Google Analytics.
        /// </summary>
        public void SetGAUserProperty()
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            GppSDK.SetGAUserProperty("key", "value");
#endif
        }

        #endregion
    }
}