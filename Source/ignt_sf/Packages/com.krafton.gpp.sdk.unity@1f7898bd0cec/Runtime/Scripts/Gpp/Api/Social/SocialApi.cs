using System.Collections.Generic;
using Gpp.Core;
using Gpp.Models;

namespace Gpp.Api.Social
{
    internal partial class SocialApi : GppApi
    {
        protected override string GetServiceName()
        {
            return "social";
        }

        public void GetSurveysByUser(ResultCallback<List<Survey>> callback, string userId = null)
        {
            Run(RequestSurvey(callback, userId));
        }
        
        public void GetSurveysByEventName(string eventKey, ResultCallback<SurveyEventResult> callback)
        {
            if (string.IsNullOrEmpty(eventKey))
            {
                callback.TryError(ErrorCode.EmptySurveyKey, "EventKey must not be null or empty.");
                return;
            }
            Run(RequestSurveyEvent(eventKey, callback));
        }
        
        public void GetSurveysCompletedCount(ResultCallback<SurveyCompletedCount> callback, string userId = null)
        {
            Run(RequestSurveyCompletedCount(callback, userId));
        }

        public void SurveyDoNotShowAgain(ResultCallback callback, string surveyId, string userId = null)
        {
            Run(RequestSurveyDoNotShowAgain(callback, surveyId, userId));
        }
    }
}