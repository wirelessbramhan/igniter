using Gpp;
using UnityEngine;

namespace GppSample
{
    public class SampleSurvey : MonoBehaviour
    {
        #region Step 1. 설문 리스트 조회

        /// <summary>
        /// 일반 설문 조사 리스트를 조회합니다.
        /// Retrieves a list of general surveys.
        /// </summary>
        public void GetSurveys()
        {
            GppSDK.GetSurveys(result =>
                {
                    if (!result.IsError)
                    {
                        foreach (var survey in result.Value)
                        {
                            Debug.Log($"Id : {survey.Id}");
                            Debug.Log($"Url : {survey.Url}");
                            Debug.Log($"DefaultLanguage : {survey.DefaultLanguage}");
                            foreach (var message in survey.Messages)
                            {
                                Debug.Log($"LangCode : {message.Key}");
                                Debug.Log($"Title : {message.Value.Title}");
                                Debug.Log($"Description : {message.Value.Description}");
                            }

                            Debug.Log($"Localized title : {survey.Messages}");
                            Debug.Log($"Start Date : {survey.StartDate}");
                            Debug.Log($"End Date : {survey.EndDate}");
                            Debug.Log($"Reward Id : {survey.Reward.Id}");
                            Debug.Log($"Reward Quantity : {survey.Reward.Quantity}");
                        }
                        // 조회된 설문조사 리스트 정보를 사용하여 UI를 노츨합니다.
                        // Use the retrieved survey list information to display it in the UI.
                    }
                    else
                    {
                        Debug.Log($"GetSurveys Failed Code: {result.Error.Code}");
                        Debug.Log($"GetSurveys Failed Message: {result.Error.Message}");
                    }
                }
            );
        }

        #endregion

        #region Step 2. 이벤트 설문 조회

        /// <summary>
        /// 이벤트 키에 매칭되는 설문 조사 정보를 조회합니다.
        /// Retrieves survey information matching the specified event key.
        /// </summary>
        public void GetSurveyByEventKey(string eventKey)
        {
            GppSDK.GetSurveyByEventKey(eventKey, result =>
                {
                    if (!result.IsError)
                    {
                        Debug.Log($"Title : {result.Value.Title}");
                        Debug.Log($"Url : {result.Value.Url}");
                        Debug.Log($"Active? : {result.Value.Active}");
                        Debug.Log($"Reward Id : {result.Value.reward.Id}");
                        Debug.Log($"Reward Quantity : {result.Value.reward.Quantity}");
                        // 조회된 설문조사 정보를 사용하여 UI를 노츨합니다.
                        // Use the retrieved survey information to display it in the UI.
                    }
                    else
                    {
                        Debug.Log($"GetSurveyByEventKey Failed Code: {result.Error.Code}");
                        Debug.Log($"GetSurveyByEventKey Failed Message: {result.Error.Message}");
                    }
                }
            );
        }

        #endregion
    }
}