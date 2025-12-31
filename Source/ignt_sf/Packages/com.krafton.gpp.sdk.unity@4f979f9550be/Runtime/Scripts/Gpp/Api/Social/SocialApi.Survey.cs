using System.Collections;
using System.Collections.Generic;
using System.Text;
using Gpp.Core;
using Gpp.Extension;
using Gpp.Models;
using Gpp.Network;
using Gpp.Surveys;

namespace Gpp.Api.Social
{
    internal partial class SocialApi
    {
        private const string SocialSurveyUrl = "/v2/public/namespaces/{namespace}/users/{userId}/surveys";
        private const string SocialSurveyEventUrl = "/v1/public/namespaces/{namespace}/users/{userId}/eventSurvey";
        private const string SocialSurveyCompletedCountUrl = "/v1/public/namespaces/{namespace}/users/{userId}/surveys/completed-count";
        private const string SocialSurveyDoNotShowAgainUrl = "/v1/public/namespaces/{namespace}/users/{userId}/surveys/{surveyId}/do-not-show-again";

        private IEnumerator RequestSurvey(ResultCallback<List<Survey>> callback, string userId = null)
        {
            var request = HttpRequestBuilder
                .CreateGet(ServiceUrl + SocialSurveyUrl)
                .WithPathParam("namespace", Namespace)
                .WithPathParam("userId", userId ?? Session.UserId)
                .WithBearerAuth(Session.AccessToken)
                .WithContentType(MediaType.ApplicationJson)
                .Accepts(MediaType.ApplicationJson)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
            {
                if (response == null)
                {
                    callback.TryError(ErrorCode.NetworkError);
                    return;
                }

                var result = response.TryParseJson<PopupSurveyResult>();
                if (result.IsError)
                {
                    callback.TryError(result.Error);
                    return;
                }

                SetPopupSurvey(result.Value.PopupSurveys);
                response.BodyBytes = Encoding.UTF8.GetBytes(result.Value.Surveys.ToJsonString());

                callback.Try(response.TryParseJson<List<Survey>>());
            });
        }
        
        private void SetPopupSurvey(List<PopupSurvey> popupSurveys)
        {
            GppSurveyDataManager.Initialize(popupSurveys);
        }
        
        private IEnumerator RequestSurveyEvent(string eventKey, ResultCallback<SurveyEventResult> callback)
        {
            var request = HttpRequestBuilder
                .CreateGet(ServiceUrl + SocialSurveyEventUrl)
                .WithPathParam("namespace", Namespace)
                .WithPathParam("userId", Session.UserId)
                .WithQueryParam("eventKey", eventKey)
                .WithBearerAuth(Session.AccessToken)
                .WithContentType(MediaType.ApplicationJson)
                .Accepts(MediaType.ApplicationJson)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
                    if (response == null)
                    {
                        callback.TryError(ErrorCode.NetworkError);
                        return;
                    }

                    var result = response.TryParseJson<SurveyEventResult>();
                    callback.Try(result);
                }
            );
        }
        
        private IEnumerator RequestSurveyCompletedCount(ResultCallback<SurveyCompletedCount> callback, string userId = null)
        {
            var request = HttpRequestBuilder
                .CreateGet(ServiceUrl + SocialSurveyCompletedCountUrl)
                .WithPathParam("namespace", Namespace)
                .WithPathParam("userId", userId ?? Session.UserId)
                .WithBearerAuth(Session.AccessToken)
                .WithContentType(MediaType.ApplicationJson)
                .Accepts(MediaType.ApplicationJson)
                .GetResult();

            yield return HttpClient.SendRequest(request, (response, _) =>
                {
                    if (response == null)
                    {
                        callback.TryError(ErrorCode.NetworkError);
                        return;
                    }

                    var result = response.TryParseJson<SurveyCompletedCount>();
                    callback.Try(result);
                }
            );
        }
        
        private IEnumerator RequestSurveyDoNotShowAgain(ResultCallback callback, string surveyId, string userId = null)
        {   
            var request = HttpRequestBuilder
                .CreatePost(ServiceUrl + SocialSurveyDoNotShowAgainUrl)
                .WithPathParam("namespace", Namespace)
                .WithPathParam("surveyId", surveyId)
                .WithPathParam("userId", userId ?? Session.UserId)
                .WithBearerAuth(Session.AccessToken)
                .WithContentType(MediaType.ApplicationJson)
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
                    callback.Try(result);
                }
            );
        }
    }
}