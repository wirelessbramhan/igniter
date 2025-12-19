using System.Collections.Generic;
using Gpp.Core;
using Gpp.Extensions.IAP.Models;

namespace Gpp.Extensions.EpicGames
{
    public interface IGppEpicGames
    {
        public void Init();
        public void GetAuthSessionTicket(ResultCallback<string> callback);
        public string GetEpicAccountId();
        public void GetProducts(ResultCallback<List<IapProduct>> callback);
        public void Purchase(string itemId, ResultCallback<IapPurchase> callback);
    }
}