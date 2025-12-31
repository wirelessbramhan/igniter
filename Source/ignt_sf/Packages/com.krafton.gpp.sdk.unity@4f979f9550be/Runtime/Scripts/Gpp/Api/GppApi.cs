using System.Collections;
using Gpp.Core;
using Gpp.Network;
using Gpp.Utils;

namespace Gpp.Api
{
    internal abstract class GppApi
    {
        protected readonly LoginSession Session;
        protected readonly string Namespace;
        protected readonly string ServiceUrl;
        protected readonly GppHttpClient HttpClient;
        protected readonly CoroutineRunner CoroutineRunner;

        protected GppApi()
        {
            Session = GppSDK.GetSession();
            Namespace = GppSDK.GetConfig().Namespace;
            ServiceUrl = $"{GppUtil.EnsureTrailingSlash(GppSDK.GetConfig().BaseUrl)}{GetServiceName()}";
            HttpClient = GppSDK.GetHttpClient();
            CoroutineRunner = GppSDK.GetCoroutineRunner();
        }

        protected abstract string GetServiceName();

        internal void Run(IEnumerator enumerator)
        {
            CoroutineRunner.Run(enumerator);
        }
    }
}