using System.Collections;
using Gpp.Core;
using Gpp.Models;
using Gpp.Network;

namespace Gpp.Api.Push
{
    internal partial class PushApi
    {
        private const string PushRegisterUrl = "/public/namespaces/{namespace}/registration-tokens";
        private const string PushDeleteUrl = "/public/namespaces/{namespace}/logout-device";

        private IEnumerator RequestRegisterToken(string token, ResultCallback callback = null)
        {
            var tokenInfo = new RegisterToken
            {
                Country = GppSDK.GetSession().CountryCode,
                Language = GppSDK.GetSession().LanguageCode,
                DeviceId = GppDeviceProvider.GetGppDeviceId(),
#if UNITY_ANDROID
                Platform = "android",
#elif UNITY_IOS
                Platform = "ios",
#else
                Platform = "EDITOR",
#endif
                Token = token
            };

            var request = HttpRequestBuilder
                .CreatePost(ServiceUrl + PushRegisterUrl)
                .WithPathParam("namespace", Namespace)
                .WithJsonBody(tokenInfo)
                .WithBearerAuth(Session.AccessToken)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) => { callback?.Try(response.TryParse()); });
        }

        private IEnumerator RequestDeleteToken(ResultCallback callback = null)
        {
            var body = new DeleteToken
            {
                DeviceId = GppDeviceProvider.GetGppDeviceId()
            };

            var request = HttpRequestBuilder
                .CreatePost(ServiceUrl + PushDeleteUrl)
                .WithPathParam("namespace", Namespace)
                .WithJsonBody(body)
                .WithBearerAuth(Session.AccessToken)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) => { callback?.Try(response.TryParse()); });
        }
    }
}