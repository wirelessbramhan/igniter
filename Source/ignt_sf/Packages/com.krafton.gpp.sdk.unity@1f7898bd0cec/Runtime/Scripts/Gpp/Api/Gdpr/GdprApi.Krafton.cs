using System.Collections;
using Gpp.Constants;
using Gpp.Core;
using Gpp.Network;

namespace Gpp.Api
{
    internal partial class GdprApi
    {
        private const string GdprKraftonDeleteAccountUrl = "/public/PUBGGA/namespace/{namespace}/user/{userId}/deletion";

        private IEnumerator RequestDeleteAccount(string userID, ResultCallback callback)
        {
            var request = HttpRequestBuilder
                .CreatePost(ServiceUrl + GdprKraftonDeleteAccountUrl)
                .WithPathParam("namespace", Namespace)
                .WithPathParam("userId", userID)
                .WithBearerAuth(Session.AccessToken)
                .WithContentType(MediaType.ApplicationForm)
                .Accepts(MediaType.ApplicationJson)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
                    if (response == null)
                    {
                        callback.TryError(ErrorCode.NetworkError);
                        return;
                    }

                    var result = response.TryParse();
                    if (!result.IsError)
                    {
                        GppSDK.GetLobby().Disconnect();
                    }
                    callback.Try(result);
                }
            );
        }

        private IEnumerator RequestDeleteAccount(PlatformType platformType, string platformToken, ResultCallback callback)
        {
            var request = HttpRequestBuilder
                .CreatePost(ServiceUrl + GdprKraftonDeleteAccountUrl)
                .WithPathParam("namespace", Namespace)
                .WithPathParam("userId", Session.UserId)
                .WithFormParam("platformId", platformType.ToString().ToLower())
                .WithFormParam("platformToken", platformToken)
                .WithBearerAuth(Session.AccessToken)
                .WithContentType(MediaType.ApplicationForm)
                .Accepts(MediaType.ApplicationJson)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
                    if (response == null)
                    {
                        callback.TryError(ErrorCode.NetworkError);
                        return;
                    }

                    var result = response.TryParse();
                    if (!result.IsError)
                    {
                        GppSDK.GetLobby().Disconnect();
                    }
                    callback.Try(result);
                }
            );
        }
    }
}