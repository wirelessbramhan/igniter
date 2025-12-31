using Gpp;
using UnityEngine;

namespace GppSample
{
    public partial class SampleBilling : MonoBehaviour
    {
        // KPS 기능은 iOS만 지원합니다.
        // KPS features are only supported on iOS.
        
        #region Step 1. GetOffers

        /// <summary>
        /// KPS 상품 정보를 조회합니다.
        /// Retrieves KPS product information.
        /// </summary>
        public void GetOffers()
        {
            GppSDK.GetOffers(result =>
                {
                    if (!result.IsError)
                    {
                        
                        foreach (var offer in result.Value.Offers)
                        {
                            Debug.Log($"Offer Id: {offer.OfferId}");
                            Debug.Log($"Offer Title: {offer.Title}");
                            Debug.Log($"Offer Description: {offer.Description}");
                            Debug.Log($"Offer ImageUrl: {offer.ImageUrl}");
                            Debug.Log($"Offer Price: {offer.Price.DisplayPrice}");
                            Debug.Log($"Offer Currency: {offer.Price.Currency}");
                            // 조회된 상품 정보를 사용하여 상품을 진열합니다.
                            // Display the retrieved offer information.
                        }
                    }
                    else
                    {
                        Debug.Log($"GetOffers Failed Code: {result.Error.Code}");
                        Debug.Log($"GetOffers Failed Message: {result.Error.Message}");
                        // 상품 조회에 실패했으므로 유저에게 오류 팝업을 노출하고 재시도를 유도합니다.
                        // If product retrieval fails, show an error popup to the user and prompt a retry.
                    }
                }
            );
        }

        #endregion

        #region Step 2. Checkout

        /// <summary>
        /// KPS 상품 결제 페이지를 요청합니다.
        /// Requests the KPS product checkout page.
        /// </summary>
        public void Checkout(string offerId)
        {
            GppSDK.Checkout(offerId, result =>
                {
                    if (!result.IsError)
                    {
                        Debug.Log($"Open Checkout Success!");
                        // KPS 결제를 위해 외부 브라우저를 열었습니다. 결제 결과는 비동기로 IGppEventListener.NotifyCheckoutResult로 전달 됩니다.
                        // KPS checkout was successful, opening an external browser to complete the purchase. The purchase result is delivered asynchronously via IGppEventListener.NotifyCheckoutResult.
                    }
                    else
                    {
                        Debug.Log($"Checkout Failed Code: {result.Error.Code}");
                        Debug.Log($"Checkout Failed Message: {result.Error.Message}");
                        // KPS 결제 페이지를 열지 못했습니다. 유저에게 실패를 알리고 재시도를 유도합니다.
                        // Failed to open the KPS checkout page. Display a failure message to the user and prompt a retry.
                    }
                }
            );
        }

        #endregion

        #region Step 3. PaymentHistory

        /// <summary>
        /// 결제 히스토리 페이지를 요청합니다.
        /// Requests the payment history page.
        /// </summary>
        public void PaymentHistory()
        {
            GppSDK.PaymentHistory(result =>
                {
                    if (!result.IsError)
                    {
                        Debug.Log($"Open PaymentHistory Success!");
                        // KPS 결제 히스토리 페이지를 열었습니다. Checkout과 같은 비동기 콜백이 필요하지 않습니다.
                        // KPS payment history page was opened successfully. No asynchronous callbacks are required.
                    }
                    else
                    {
                        Debug.Log($"PaymentHistory Failed Code: {result.Error.Code}");
                        Debug.Log($"PaymentHistory Failed Message: {result.Error.Message}");
                        // KPS 결제 히스토리 페이지를 열지 못했습니다. 유저에게 실패를 알리고 재시도를 유도합니다.
                        // Failed to open the KPS payment history page. Display a failure message to the user and prompt a retry.
                    }
                }
            );
        }

        #endregion
    }
}