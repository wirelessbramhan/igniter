using System.Collections.Generic;

namespace Gpp.Extensions.GoogleAnalytics
{
    internal interface IGppGoogleAnalytics
    {
        public void Init();
        public void SendEvent(string eventName, Dictionary<string, object> payload);
        public void AddCommonEventParameters(Dictionary<string, string> parameters);
        public void SendPurchase(string itemId, string transactionId, string price, string currency);
        public void SetUserProperty(string key, string value);
        public void SetUserId(string userId);
    }
}