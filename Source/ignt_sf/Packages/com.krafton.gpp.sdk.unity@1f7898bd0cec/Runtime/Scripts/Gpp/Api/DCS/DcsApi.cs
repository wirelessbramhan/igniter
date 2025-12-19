using Gpp.Core;
using Gpp.Models;
using static Gpp.Models.DcsModels;

namespace Gpp.Api.DCS
{
    internal partial class DcsApi : GppApi
    {
        protected override string GetServiceName()
        {
            return "dcs";
        }

        internal void CheckAppUpdate(ResultCallback<AppUpdateInfo> callback)
        {
            Run(RequestGetAppUpdateInfo(callback));
        }

        internal void GetSdkConfig(ResultCallback<SdkConfigInfo> callback)
        {
            Run(RequestGetSdkConfig(callback));
        }

        internal void GetPublicDCSInfo(string key, DcsDataAccess dataAccess, ResultCallback<PublicDCSInfo> callback)
        {
            Run(RequestGetPublicDCS(key, dataAccess, callback));
        }
    }
}