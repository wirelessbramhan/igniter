using Gpp.Constants;
using Gpp.Core;

namespace Gpp.Api
{
    internal partial class GdprApi : GppApi
    {
        protected override string GetServiceName()
        {
            return "gdpr";
        }

        internal void DeleteAccount(string userID, ResultCallback callback)
        {
            Run(RequestDeleteAccount(userID, callback));
        }

        internal void DeleteAccount(PlatformType platformType, string platformToken, ResultCallback callback)
        {
            Run(RequestDeleteAccount(platformType, platformToken, callback));
        }
    }
}