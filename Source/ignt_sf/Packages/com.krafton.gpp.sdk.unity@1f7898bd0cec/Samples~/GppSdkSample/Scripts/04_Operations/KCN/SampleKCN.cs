using System.Collections;
using System.Collections.Generic;
using Gpp;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GppSample
{
    public class SampleKCN : MonoBehaviour
    {
        public GameObject kcnUI;
        public TMP_InputField InputField;

        /// <summary>
        /// 가장 마지막에 입력한 크리에이터 코드를 가지고 옵니다.
        /// Retrieve the last entered creator code.
        /// </summary>
        public void GetLastCreatorCode()
        {
            GppSDK.GetLastCreatorCode(result =>
            {
                if (!result.IsError)
                {
                    Debug.Log($"Get Last Creator Code Success : {result.Value.Code}");
                }
                else
                {
                    Debug.Log($"Get Last Creator Code Failed : {result.Error.Code}");
                    Debug.Log($"Get Last Creator Code Failed : {result.Error.Message}");
                }
            });
        }

        /// <summary>
        /// 크리에이터 코드를 입력합니다.
        /// Enter creator code.
        /// </summary>
        public void SetCreatorCode()
        {            
            GppSDK.SetCreatorCode(InputField.text, result =>
            {
                if (!result.IsError)
                {
                    Debug.Log($"Set Creator Code Success : {result.Value.Code}");
                }
                else
                {
                    Debug.Log($"Set Creator Code Failed : {result.Error.Code}");
                    Debug.Log($"Set Creator Code Failed : {result.Error.Message}");
                }
            });
        }

        public void OpenUI()
        {
            kcnUI.SetActive(true);
        }

        public void CloseUI()
        {
            kcnUI.SetActive( false );
        }
    }
}