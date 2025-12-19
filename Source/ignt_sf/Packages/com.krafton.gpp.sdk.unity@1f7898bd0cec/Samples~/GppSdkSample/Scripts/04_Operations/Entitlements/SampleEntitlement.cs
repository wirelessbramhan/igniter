using System.Collections;
using System.Collections.Generic;
using Gpp;
using UnityEngine;

namespace GppSample
{
    public class SampleEntitlement : MonoBehaviour
    {
        /// <summary>
        /// GPP에서 지급한 Consumable 상품 정보를 가지고 옵니다.
        /// Retrieve the information of consumable products provided by GPP.
        /// </summary>
        public void GetConsumableEntitlements()
        {
            GppSDK.GetConsumableEntitlements(result =>
            {
                if (!result.IsError)
                {
                    Debug.Log($"Get Consumable Entitlements Success");
                    Debug.Log($"Consumable Entitlements count : {result.Value.ConsumableEntitlements.Count}");
                    result.Value.ConsumableEntitlements.ForEach(entitlement =>
                    {
                        Debug.Log($"entitlement : {entitlement.Id}");
                    });
                }
                else
                {
                    Debug.Log($"Get Consumable Entitlements Failed {result.Error.Code}");
                    Debug.Log($"Get Consumable Entitlements Failed {result.Error.Message}");
                }
            });
        }

        /// <summary>
        /// GPP에서 지급한 Durable 상품 정보를 가지고 옵니다.
        /// Retrieve the information of durable products provided by GPP.
        /// </summary>
        public void GetDurableEntitlements()
        {
            GppSDK.GetDurableEntitlements(result =>
            {
                if (!result.IsError)
                {
                    Debug.Log($"Get Durable Entitlements Success");
                    Debug.Log($"Durable Entitlements count : {result.Value.DurableEntitlements.Count}");
                    result.Value.DurableEntitlements.ForEach(entitlement =>
                    {
                        Debug.Log($"entitlement : {entitlement.Id}");
                    });
                }
                else
                {
                    Debug.Log($"Get Durable Entitlements Failed {result.Error.Code}");
                    Debug.Log($"Get Durable Entitlements Failed {result.Error.Message}");
                }
            });
        }
    }
}
