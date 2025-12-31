using System.Collections;
using System.Collections.Generic;
using Gpp;
using Gpp.Extension;
using TMPro;
using UnityEngine;

namespace GppSample
{
    public class SamplePublicDCS : MonoBehaviour
    {
        public GameObject publicDCSUI;
        public TMP_InputField inputField;

        /// <summary>
        /// PreLogin 타입 동적 설정을 가지고 옵니다.
        /// Get PreLogin type dynamic config.
        /// </summary>
        public void GetPublicDCSPreLogin()
        {
            GppSDK.GetPublicDCSInfo(inputField.text, Gpp.Models.DcsDataAccess.PreLogin, result =>
            {
                if (!result.IsError)
                {
                    Debug.Log($"Get Public DCS Info PreLogin : {result.Value.Key}");
                    Debug.Log($"Get Public DCS Info PreLogin : {result.Value.Value}");
                }
                else
                {
                    Debug.Log($"Get Public DCS Info PreLogin : {result.Error.Code}");
                    Debug.Log($"Get Public DCS Info PreLogin : {result.Error.Message}");
                }
            });
        }

        /// <summary>
        /// PostLogin 타입 동적 설정을 가지고 옵니다.
        /// Get PostLogin type dynamic config.
        /// </summary>
        public void GetPublicDCSPostLogin()
        {
            GppSDK.GetPublicDCSInfo(inputField.text, Gpp.Models.DcsDataAccess.PostLogin, result =>
            {
                if (!result.IsError)
                {
                    Debug.Log($"Get Public DCS Info PostLogin : {result.Value.Key}");
                    Debug.Log($"Get Public DCS Info PostLogin : {result.Value.Value}");
                }
                else
                {
                    Debug.Log($"Get Public DCS Info PostLogin : {result.Error.Code}");
                    Debug.Log($"Get Public DCS Info PostLogin : {result.Error.Message}");
                }
            });
        }

        public void OpenUI()
        {
            publicDCSUI.SetActive(true);
        }

        public void CloseUI()
        {
            publicDCSUI.SetActive(false);
        }
    }
}

