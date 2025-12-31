using System.Collections;
using Gpp.Core;
using Gpp.Models;
using Gpp.Network;
using static Gpp.Models.DcsModels;

namespace Gpp.Api.DCS
{
    internal partial class DcsApi
    {
        private const string GetAppVersionUrl = "/v1/public/namespaces/{namespace}/app-store-infos/{appStoreType}/app-version-infos/{version}";
        private const string GetSDKConfigUrl = "/v1/public/namespaces/gpp/configs/{key}";
        private const string GetPublicDCSUrl = "/v1/public/namespaces/{namespace}/configs/{key}";

        private IEnumerator RequestGetAppUpdateInfo(ResultCallback<AppUpdateInfo> callback)
        {
            var request = HttpRequestBuilder
                .CreateGet(ServiceUrl + GetAppVersionUrl)
                .WithPathParam("namespace", Namespace)
                .WithPathParam("appStoreType", GppSDK.GetOptions().Store.ToString())
                .WithPathParam("version", GppSDK.GetOptions().AppVersion)
                .Accepts(MediaType.ApplicationJson)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
                    var result = response.TryParseJson<AppUpdateInfo>();
                    callback.Try(result);
                }
            );
        }

        private IEnumerator RequestGetSdkConfig(ResultCallback<SdkConfigInfo> callback)
        {
            var request = HttpRequestBuilder
                .CreateGet(ServiceUrl + GetSDKConfigUrl)
                .WithPathParam("key", Namespace)
                .Accepts(MediaType.ApplicationJson)
                .SetDisableRetry()
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
            {
                var result = response.TryParseJson<SdkConfigInfo>();
                callback.Try(result);
            });
        }

        private IEnumerator RequestGetPublicDCS(string key, DcsDataAccess dataAccess, ResultCallback<PublicDCSInfo> callback)
        {
            var requestBuilder = HttpRequestBuilder
                .CreateGet(ServiceUrl + GetPublicDCSUrl)
                .WithPathParam("namespace", Namespace)
                .WithPathParam("key", key)
                .WithQueryParam("dataAccess", dataAccess.ToString())
                .Accepts(MediaType.ApplicationJson);

            if (dataAccess == DcsDataAccess.PostLogin)
            {
                requestBuilder.WithBearerAuth();
            }

            var request = requestBuilder
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
            {
                var result = response.TryParseJson<PublicDCSInfo>();
                callback.Try(result);
            });
        }
    }
}