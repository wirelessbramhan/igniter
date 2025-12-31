using System.Collections.Generic;
using Gpp.Core;
using Gpp.Extensions.IAP.Models;
using Gpp.Models;

namespace Gpp.Extensions.Steam
{
    internal interface IGppSteam
    {
        public void Init();
        public void GetAuthSessionTicket(ResultCallback<string> callback);
        public string GetLatestAuthSessionTicket();
        public bool IsOnline();
        public string GetAppId();
        public string GetSteamUserId();
        public bool IsSteamDeck();
        public void GetUserDLCAppIdList(ResultCallback<List<string>> callback);
        public void GetProducts(ResultCallback<List<IapProduct>> callback);
        public void Purchase(string itemId, ResultCallback<IapPurchase> callback);
    }
}