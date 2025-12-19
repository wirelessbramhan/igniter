using System;
using Gpp.Extensions.GooglePlayGames.Models;

namespace Gpp.Extensions.GooglePlayGames
{
    public interface IGppGooglePlayGames
    {
        public void Init(string clientId, Action<GpgResponse> callback);
        public void Login(Action<GpgResponse> callback);
        public GpgResponseObject GetGpgInfo();
        public void SetGpgInfo(GpgResponseObject info);
        public bool IsPC();
    }
}