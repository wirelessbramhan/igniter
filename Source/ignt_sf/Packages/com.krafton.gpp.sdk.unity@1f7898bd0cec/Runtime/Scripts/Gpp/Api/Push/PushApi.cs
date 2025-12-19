using Gpp.Core;

namespace Gpp.Api.Push
{
    internal partial class PushApi : GppApi
    {
        protected override string GetServiceName()
        {
            return "push-notification-service";
        }

        internal void RegisterToken(string token, ResultCallback callback = null)
        {
            Run(RequestRegisterToken(token, callback));
        }

        internal void DeleteToken(ResultCallback callback = null)
        {
            Run(RequestDeleteToken(callback));
        }
    }
}