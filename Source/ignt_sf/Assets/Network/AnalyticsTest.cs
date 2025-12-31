using System.Collections.Generic;
using Gpp;
using UnityEngine;

namespace ignt.sports.cricket.network
{
    public class AnalyticsTest : MonoBehaviour
    {
        public void SendTelemetry()
        {
            Dictionary<string, object> payload = new Dictionary<string, object>
            {
                { "test_key1", "test_value1" },
                { "test_key2", "test_value2" },
                { "test_key3", "test_value3" },
            };
            GppSDK.SendGameTelemetry("test_event_name_1", payload, result =>
            { 
                Debug.LogError($"Error sending telemetry: {result.Error.Message}");

            }, true);
            Debug.Log("Telemetry sent successfully");
        }
    }
}
