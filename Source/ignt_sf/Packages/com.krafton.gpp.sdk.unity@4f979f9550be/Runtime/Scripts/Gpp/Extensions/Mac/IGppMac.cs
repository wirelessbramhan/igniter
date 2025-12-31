using Gpp.Core;

namespace Gpp.Extensions.Mac
{
    internal interface IGppMac
    {
        public void Init();
        public void Login(ResultCallback<AppleSignInResultObject> callback);
        public void GetAppTransaction(ResultCallback<string> callback);
        public string GetAppStoreAppId();
    }
}
