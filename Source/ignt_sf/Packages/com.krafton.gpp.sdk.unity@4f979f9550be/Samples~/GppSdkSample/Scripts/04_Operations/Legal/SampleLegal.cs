using Gpp;
using Gpp.Models;
using UnityEngine;

namespace GppSample
{
    public class SampleLegal : MonoBehaviour
    {
        #region Step 1. 약관 페이지 열기
        
        /// <summary>
        /// TAG 정보를 이용하여 GPP AdminPortal에 등록된 약관을 브라우저로 엽니다.
        /// Opens the terms registered in the GPP AdminPortal in a browser using the specified TAG information.
        /// </summary>
        public void OpenLegalWithTag(LegalPolicyType type, string tag)
        {
            GppSDK.OpenLegalWithTag(type, tag, result =>
                {
                    if (!result.IsError)
                    {
                        Debug.Log($"OpenLegalWithTag Success!");
                    }
                    else
                    {
                        Debug.Log($"OpenLegalWithTag Failed Code: {result.Error.Code}");
                        Debug.Log($"OpenLegalWithTag Failed Message: {result.Error.Message}");
                        // 실패 팝업을 유저에게 노출할 수 있습니다.
                        // You can display a failure popup to the user.
                    }
                }
            );
        }

        public void OpenLegalDocumentWithTag(string legalTag)
        {
            GppSDK.OpenLegalWithTag(LegalPolicyType.LEGAL_DOCUMENT_TYPE, legalTag, result =>
                {
                    if (!result.IsError)
                    {
                        Debug.Log($"OpenLegalDocumentWithTag Success!");
                    }
                    else
                    {
                        Debug.Log($"OpenLegalDocumentWithTag Failed Code: {result.Error.Code}");
                        Debug.Log($"OpenLegalDocumentWithTag Failed Message: {result.Error.Message}");
                    }
                }
            );
        }
        
        public void OpenLegalMarketingWithTag(string legalTag)
        {
            GppSDK.OpenLegalWithTag(LegalPolicyType.MARKETING_PREFERENCE_TYPE, legalTag, result =>
                {
                    if (!result.IsError)
                    {
                        Debug.Log($"OpenLegalMarketingWithTag Success!");
                    }
                    else
                    {
                        Debug.Log($"OpenLegalMarketingWithTag Failed Code: {result.Error.Code}");
                        Debug.Log($"OpenLegalMarketingWithTag Failed Message: {result.Error.Message}");
                    }
                }
            );
        }
        
        public void OpenLegalEcommerceWithTag(string legalTag)
        {
            GppSDK.OpenLegalWithTag(LegalPolicyType.ECOMMERCE_TYPE, legalTag, result =>
                {
                    if (!result.IsError)
                    {
                        Debug.Log($"OpenLegalEcommerceWithTag Success!");
                    }
                    else
                    {
                        Debug.Log($"OpenLegalEcommerceWithTag Failed Code: {result.Error.Code}");
                        Debug.Log($"OpenLegalEcommerceWithTag Failed Message: {result.Error.Message}");
                    }
                }
            );
        }

        #endregion
    }
}