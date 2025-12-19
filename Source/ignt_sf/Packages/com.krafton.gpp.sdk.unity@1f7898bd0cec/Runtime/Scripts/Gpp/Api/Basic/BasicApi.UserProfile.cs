using System.Collections;
using Gpp.Core;
using Gpp.Models;
using Gpp.Network;

namespace Gpp.Api.Basic
{
    internal partial class BasicApi
    {
        private const string BasicUserProfileUrl = "/v1/public/namespaces/{namespace}/users/me/profiles";
        private const string BasicUserProfilePushEnableUrl = "/v1/public/namespaces/{namespace}/users/me/profiles/pushEnable";
        private const string BasicUserProfileNightPushEnableUrl = "/v1/public/namespaces/{namespace}/users/me/profiles/pushNightEnable";

        private IEnumerator RequestGetPushStatus(ResultCallback<UserPushStatus> callback)
        {
            var request = HttpRequestBuilder
                .CreateGet(ServiceUrl + BasicUserProfilePushEnableUrl)
                .WithPathParam("namespace", Namespace)
                .WithBearerAuth(Session.AccessToken)
                .Accepts(MediaType.ApplicationJson)
                .GetResult();
            
            yield return HttpClient.SendRequest(request, (response, _) =>
            {
                var result = response.TryParseJson<UserPushStatus>();
                callback.Try(result);
            });
        }

        private IEnumerator RequestGetNightPushStatus(ResultCallback<UserNightPushStatus> callback)
        {
            var request = HttpRequestBuilder
                .CreateGet(ServiceUrl + BasicUserProfileNightPushEnableUrl)
                .WithPathParam("namespace", Namespace)
                .WithBearerAuth(Session.AccessToken)
                .Accepts(MediaType.ApplicationJson)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
                    var result = response.TryParseJson<UserNightPushStatus>();
                    callback.Try(result);
                }
            );
        }

        private IEnumerator RequestPatchPushStatus(bool isEnable, ResultCallback<UserPushStatus> callback)
        {
            var body = new UserPushStatus
            {
                pushEnable = isEnable
            };
            var request = HttpRequestBuilder
                .CreatePatch(ServiceUrl + BasicUserProfilePushEnableUrl)
                .WithJsonBody(body)
                .WithPathParam("namespace", Namespace)
                .WithBearerAuth(Session.AccessToken)
                .Accepts(MediaType.ApplicationJson)
                .GetResult();
            
            yield return HttpClient.SendRequest(request, (response, _) =>
            {
                var result = response.TryParseJson<UserPushStatus>();
                callback.Try(result); 
            });
        }

        private IEnumerator RequestPatchNightPushStatus(bool isEnable, ResultCallback<UserNightPushStatus> callback)
        {
            var body = new UserNightPushStatus
            {
                pushNightEnable = isEnable
            };
            var request = HttpRequestBuilder
                .CreatePatch(ServiceUrl + BasicUserProfileNightPushEnableUrl)
                .WithJsonBody(body)
                .WithPathParam("namespace", Namespace)
                .WithBearerAuth(Session.AccessToken)
                .Accepts(MediaType.ApplicationJson)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
            {
                var result = response.TryParseJson<UserNightPushStatus>();
                callback.Try(result); 
            });
        }
        
        private IEnumerator RequestPatchUserProfile(ResultCallback callback = null)
        {
            var request = HttpRequestBuilder
                .CreatePatch(ServiceUrl + BasicUserProfileUrl)
                .WithPathParam("namespace", Namespace)
                .WithBearerAuth(Session.AccessToken)
                .Accepts(MediaType.ApplicationJson)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
            {
                var result = response.TryParse();
                callback?.Try(result);
            });
        }
    }
}