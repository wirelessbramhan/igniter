using Gpp;
using UnityEngine;

namespace GppSample
{
    public partial class SampleBilling : MonoBehaviour
    {
        #region Step 1. GetProducts

        /// <summary>
        /// 상품 정보를 조회합니다.
        /// Retrieves product information.
        /// </summary>
        public void GetProducts()
        {
            GppSDK.GetProducts(result =>
                {
                    if (!result.IsError)
                    {
                        foreach (var product in result.Value)
                        {
                            Debug.Log($"Product Id: {product.ProductId}");
                            Debug.Log($"Product Name: {product.Name}");
                            Debug.Log($"Product Title: {product.Title}");
                            Debug.Log($"Product Description: {product.Description}");
                            Debug.Log($"Product Price: {product.Price}");
                            Debug.Log($"Product CurrencyCode: {product.CurrencyCode}");
                            Debug.Log($"Product PriceString: {product.PriceString}");
                            // 조회된 상품 정보를 사용하여 상품을 진열합니다.
                            // Display the retrieved product information.
                        }
                    }
                    else
                    {
                        Debug.Log($"GetProduct Failed Code: {result.Error.Code}");
                        Debug.Log($"GetProduct Failed Message: {result.Error.Message}");
                        // 상품 조회에 실패했으므로 유저에게 오류 팝업을 노출하고 재시도를 유도합니다.
                        // If product retrieval fails, show an error popup to the user and prompt a retry.
                    }
                }
            );
        }

        #endregion

        #region Step 2. Purchase

        /// <summary>
        /// 조회된 상품 아이디로 결제를 진행합니다.
        /// </summary>
        public void Purchase(string productId)
        {
            GppSDK.Purchase(productId, result =>
                {
                    if (!result.IsError)
                    {
                        Debug.Log($"EntitlementId: {result.Value.EntitlementId}");
                        Debug.Log($"TransactionId: {result.Value.TransactionId}");
                        // 결제가 성공했으므로 유저에게 결제 성공을 알릴 수 있습니다.
                    }
                    else
                    {
                        Debug.Log($"Purchase Failed Code: {result.Error.Code}");
                        Debug.Log($"Purchase Failed Message: {result.Error.Message}");
                        // 결제가 실패했으므로 유저에게 실패를 알리고 재시도를 유도합니다.
                    }
                }
            );
        }

        #endregion

        #region Step 3. Restore

        /// <summary>
        /// 결제는 성공했지만 지급되지 않은 상품을 복구 처리합니다.
        /// Restores products that were successfully purchased but not granted to the user.
        /// </summary>
        public void Restore()
        {
            GppSDK.Restore(result =>
                {
                    if (!result.IsError)
                    {
                        foreach (var purchase in result.Value)
                        {
                            Debug.Log($"EntitlementId: {purchase.EntitlementId}");
                            Debug.Log($"TransactionId: {purchase.TransactionId}");
                            // 복구가 되었기 때문에 유저는 지급된 상품을 확인할 수 있습니다. 
                            // Restoration completed, allowing the user to verify the granted products.
                        }
                    }
                    else
                    {
                        Debug.Log($"Restore Failed Code: {result.Error.Code}");
                        Debug.Log($"Restore Failed Message: {result.Error.Message}");
                        // 다음 로그인 성공 후 재시도 하며, 3일이 지나면 자동 환불처리 됩니다.
                        // Retry after the next successful login; after 3 days, an automatic refund will be processed.
                    }
                }
            );
        }

        #endregion
    }
}