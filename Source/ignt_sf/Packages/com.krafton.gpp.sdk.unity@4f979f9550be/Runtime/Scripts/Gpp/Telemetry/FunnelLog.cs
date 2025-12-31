using System.Collections.Generic;
using Gpp.Constants;
using Gpp.Core;
using Gpp.Extension;
using Gpp.Models;
using Gpp.Utils;
using UnityEngine;

namespace Gpp.Telemetry
{
    internal static class FunnelLog
    {
        internal class Auth
        {

        }
        internal static class Purchase
        {
            /// <summary>
            /// wiki: https://krafton.atlassian.net/wiki/spaces/DATACAT/pages/703629877/DCAT+Store+Purchase+Funnel+Meta
            /// </summary>
            internal class StorePurchaseFunnel
            {
                /// <summary>
                /// 1. 상품 구매 버튼 클릭
                /// 스토어 결제 요청
                /// </summary>
                public static void StartPurchase(string productId)
                {
                    var contents = new Dictionary<string, object>
                    {
                        { "store_type", GppSDK.GetOptions().Store.ToString() },
                        { "product_id", productId }
                    };

                    Send(Funnel.Purchase.Type.STORE_PURCHASE_FUNNEL, Funnel.Purchase.StorePurchase.START_PURCHASE, contents);
                }

                /// <summary>
                /// 2. 구매 가능 여부 체크
                /// </summary>
                public static void CheckPurchasePolicy()
                {
                    Send(Funnel.Purchase.Type.STORE_PURCHASE_FUNNEL, Funnel.Purchase.StorePurchase.CHECK_PURCHASE_POLICY);
                }

                /// <summary>
                /// 2-1. 결제 불가 
                /// </summary>
                public static void PurchaseBlocked(string reason)
                {
                    var contents = new Dictionary<string, object>
                    {
                        { "reason", reason }
                    };

                    Send(Funnel.Purchase.Type.STORE_PURCHASE_FUNNEL, Funnel.Purchase.StorePurchase.PURCHASE_BLOCKED, contents);
                }

                /// <summary>
                /// 3. 스토어 결제 진행
                /// </summary>
                public static void StorePurchaseTry(string reservationId)
                {
                    var contents = new Dictionary<string, object>
                    {
                        { "reservation_id", reservationId }
                    };

                    Send(Funnel.Purchase.Type.STORE_PURCHASE_FUNNEL, Funnel.Purchase.StorePurchase.STORE_PURCHASE_TRY, contents);
                }

                /// <summary>
                /// 3-1. 스토어 결제 실패
                /// </summary>
                public static void StorePurchaseTryFail(int storeErrorCode, bool isCanceled)
                {
                    var contents = new Dictionary<string, object>
                    {
                        { "store_error_code", storeErrorCode },
                        { "is_canceled", isCanceled }
                    };

                    Send(Funnel.Purchase.Type.STORE_PURCHASE_FUNNEL, Funnel.Purchase.StorePurchase.STORE_PURCHASE_TRY_FAIL, contents);
                }

                /// <summary>
                /// 3-2. 스토어 결제 성공
                /// </summary>
                public static void StorePurchaseTrySuccess(string transactionId)
                {
                    var contents = new Dictionary<string, object>
                    {
                        { "transaction_id", transactionId }
                    };

                    Send(Funnel.Purchase.Type.STORE_PURCHASE_FUNNEL, Funnel.Purchase.StorePurchase.STORE_PURCHASE_TRY_SUCCESS, contents);
                }

                /// <summary>
                /// 4. SDK → Server 영수증 검증 및 인타이틀먼트 지급 요청 
                /// </summary>
                public static void RequestValidationGrant(string productId)
                {
                    var contents = new Dictionary<string, object>
                    {
                        { "product_id", productId }
                    };

                    Send(Funnel.Purchase.Type.STORE_PURCHASE_FUNNEL, Funnel.Purchase.StorePurchase.REQUEST_VALIDATION_GRANT, contents);
                }

                //----------------------------------------------------------------------------------------------------
                //  Server Side
                //----------------------------------------------------------------------------------------------------
                //  5. receipt_validation               : 영수증 검증
                //  5-1. receipt_validation_fail        : 영수증 검증 실패
                //  5-2. receipt_validation_success     : 영수증 검증 성공
                //  6. entitlement_grant                : entitlement 지급
                //  6-1. entitlement_granted_Fail       : entitlement 지급 실패
                //  6-2. entitlement_granted_success    : entitlement 지급 성공
                //  7. deliver_validation_grant_result  : Server → SDK 영수증 검증 및 인타이틀먼트 지급 결과 전달 
                //----------------------------------------------------------------------------------------------------

                /// <summary>
                /// 8. 서버의 검증/지급 결과 SDK에서 수신 
                /// </summary>
                public static void ReceiveValidationGrantResult()
                {
                    Send(Funnel.Purchase.Type.STORE_PURCHASE_FUNNEL, Funnel.Purchase.StorePurchase.RECEIVE_VALIDATION_GRANT_RESULT);
                }

                /// <summary>
                /// 8-1. 실패
                /// </summary>
                /// <param name="serverErrorCode"></param>
                public static void ValidationGrantFail(string serverErrorCode)
                {
                    var contents = new Dictionary<string, object>
                    {
                        { "server_error_code", serverErrorCode }
                    };

                    Send(Funnel.Purchase.Type.STORE_PURCHASE_FUNNEL, Funnel.Purchase.StorePurchase.VALIDATION_GRANT_FAIL, contents);
                }

                /// <summary>
                /// 8-2 성공
                /// </summary>
                public static void ValidationGrantSucceess(string entitlementId)
                {
                    var contents = new Dictionary<string, object>
                    {
                        { "entitlement_id", entitlementId }
                    };

                    Send(Funnel.Purchase.Type.STORE_PURCHASE_FUNNEL, Funnel.Purchase.StorePurchase.VALIDATION_GRANT_SUCCESS, contents);
                }

                /// <summary>
                /// 9 스토어에 영수증 컨슘 요청
                /// </summary>
                public static void RequestStoreConsume()
                {
                    Send(Funnel.Purchase.Type.STORE_PURCHASE_FUNNEL, Funnel.Purchase.StorePurchase.REQUEST_STORE_CONSUME);
                }

                /// <summary>
                /// 9-1. 스토어 컨슘 실패
                /// </summary>
                /// <param name="storeErrorCode"></param>
                public static void RequestStoreConsumeFail(string storeErrorCode)
                {
                    var contents = new Dictionary<string, object>
                    {
                        { "store_error_code", storeErrorCode }
                    };

                    Send(Funnel.Purchase.Type.STORE_PURCHASE_FUNNEL, Funnel.Purchase.StorePurchase.STORE_CONSUME_FAIL, contents);
                }

                /// <summary>
                /// 9-2. 스토어 컨슘 성공
                /// </summary>
                public static void RequestStoreConsumeSuccess()
                {
                    Send(Funnel.Purchase.Type.STORE_PURCHASE_FUNNEL, Funnel.Purchase.StorePurchase.STORE_CONSUME_SUCCESS);
                }
            }

            /// <summary>
            /// https://krafton.atlassian.net/wiki/spaces/DATACAT/pages/698000671/DCAT+Repayment+Funnel+Meta
            /// </summary>
            internal class RepaymentFunnel
            {
                /// <summary>
                /// 1. repay 유저 진입
                /// (미컨슘 상품이 존재하면 trigger_purchase_recovery 로 갈 수 있음)
                /// </summary>
                public static void RepayUserEntry()
                {
                    var contents = new Dictionary<string, object>
                    {
                        { "store_type", GppSDK.GetOptions().Store.ToString() }
                    };

                    Send(Funnel.Purchase.Type.REPAYMENT_FUNNEL, Funnel.Purchase.Repayment.REPAY_USER_ENTRY, contents);
                }

                /// <summary>
                /// 2. 미컨슘 상품 존재 시 복구 시작, 없는 경우 재결제 상품 유무 확인
                /// </summary>
                public static void TriggerPurchaseRecovery()
                {
                    var contents = new Dictionary<string, object>
                    {
                        { "store_type", GppSDK.GetOptions().Store.ToString() }
                    };

                    Send(Funnel.Purchase.Type.REPAYMENT_FUNNEL, Funnel.Purchase.Repayment.TRIGGER_PURCHASE_RECOVERY, contents);
                }

                /// <summary>
                /// 3. SDK → Server에 재결제 리스트 요청, 재결제 있는 경우 재결제 시작
                /// </summary>
                public static void RequestRepayList()
                {
                    Send(Funnel.Purchase.Type.REPAYMENT_FUNNEL, Funnel.Purchase.Repayment.REQUEST_REPAY_LIST);
                }

                /// <summary>
                /// 4. 모든 재결제가 완료되었는지 체크 (13번 컨슘 결과 무관 전체 체크)
                /// </summary>
                public static void RepayStatus()
                {
                    Send(Funnel.Purchase.Type.REPAYMENT_FUNNEL, Funnel.Purchase.Repayment.REPAY_STATUS);
                }

                /// <summary>
                /// 4-1. 재결제 항목이 남음 (5 show_repay_list로 이동)
                /// </summary>
                public static void RepayIncomplete()
                {
                    Send(Funnel.Purchase.Type.REPAYMENT_FUNNEL, Funnel.Purchase.Repayment.REPAY_INCOMPLETE);
                }

                /// <summary>
                /// 4-2 모든 재결제 완료
                /// </summary>
                public static void RepayComplete(string reason = null)
                {
                    Dictionary<string, object> contents = null;

                    if (reason is not null)
                    {
                        contents = new Dictionary<string, object>
                        {
                            { "reason", reason }
                        };
                    }

                    Send(Funnel.Purchase.Type.REPAYMENT_FUNNEL, Funnel.Purchase.Repayment.REPAY_COMPLETE, contents);
                }

                /// <summary>
                /// 5. 재결제 팝업
                /// </summary>
                public static void ShowRepayPopup()
                {
                    Send(Funnel.Purchase.Type.REPAYMENT_FUNNEL, Funnel.Purchase.Repayment.SHOW_REPAY_POPUP);
                }

                /// <summary>
                /// 5-1. (4번 재결제 불가)
                /// </summary>
                public static void RepayBlocked(string reason)
                {
                    var contents = new Dictionary<string, object>
                    {
                        { "reason", reason }
                    };

                    Send(Funnel.Purchase.Type.REPAYMENT_FUNNEL, Funnel.Purchase.Repayment.REPAY_BLOCKED, contents);
                }

                /// <summary>
                /// 6. (4번 재결제 가능) 재결제 상품 목록 수집
                /// </summary>
                /// <param name="repaymentMeta">재결제 목록 전달 transaction_id, repayment_id, store</param>
                public static void ShowRepayList(List<Dictionary<string, object>> repaymentMetaList)
                {
                    var contents = new Dictionary<string, object>
                    {
                        { "repayment_meta", repaymentMetaList }
                    };

                    Send(Funnel.Purchase.Type.REPAYMENT_FUNNEL, Funnel.Purchase.Repayment.SHOW_REPAY_LIST, contents);
                }

                /// <summary>
                /// 6-1. (4-1, 5번 고객센터 버튼 선택)
                /// </summary>
                public static void ClickCustomerService()
                {
                    Send(Funnel.Purchase.Type.REPAYMENT_FUNNEL, Funnel.Purchase.Repayment.CLICK_CUSTOMER_SERVICE);
                }

                /// <summary>
                /// 6-2. (4-1, 5번 다른 계정 로그인 선택)
                /// </summary>
                public static void ClickAccountSwitch()
                {
                    Send(Funnel.Purchase.Type.REPAYMENT_FUNNEL, Funnel.Purchase.Repayment.CLICK_ACCOUNT_SWITCH);
                }

                /// <summary>
                /// 7. 스토어에 결제 요청 (선택 재결제하기 버튼 클릭)
                /// </summary>
                public static void ClickRepayPerProduct(string repaymentId, string productId)
                {
                    var contents = new Dictionary<string, object>
                    {
                        { "repayment_id", repaymentId },
                        { "product_id", productId }
                    };

                    Send(Funnel.Purchase.Type.REPAYMENT_FUNNEL, Funnel.Purchase.Repayment.CLICK_REPAY_PER_PRODUCT, contents);
                }

                /// <summary>
                /// 8. 스토어 결제 시도
                /// </summary>
                public static void StorePurchaseTry(string reservationId)
                {
                    var contents = new Dictionary<string, object>
                    {
                        { "reservation_id", reservationId }
                    };

                    Send(Funnel.Purchase.Type.REPAYMENT_FUNNEL, Funnel.Purchase.Repayment.STORE_PURCHASE_TRY, contents);
                }

                /// <summary>
                /// 8-1. 스토어 결제 실패 시 4번 show_repay_popup 으로 돌아감
                /// </summary>
                public static void StorePurchaseTryFail(string storeErrorCode, bool isCanceled)
                {
                    var contents = new Dictionary<string, object>
                    {
                        { "store_error_code", storeErrorCode },
                        { "is_canceled", isCanceled }
                    };

                    Send(Funnel.Purchase.Type.REPAYMENT_FUNNEL, Funnel.Purchase.Repayment.STORE_PURCHASE_TRY_FAIL, contents);
                }

                /// <summary>
                /// 8-2. 스토어 결제 성공
                /// </summary>
                public static void StorePurchaseTrySuccess(string transactionId)
                {
                    var contents = new Dictionary<string, object>
                    {
                        { "transaction_id", transactionId }
                    };

                    Send(Funnel.Purchase.Type.REPAYMENT_FUNNEL, Funnel.Purchase.Repayment.STORE_PURCHASE_TRY_SUCCESS, contents);
                }

                /// <summary>
                /// 9. SDK → Server 영수증 검증 요청
                /// </summary>
                public static void RequestValidation(string productId)
                {
                    var contents = new Dictionary<string, object>
                    {
                        { "product_id", productId }
                    };

                    Send(Funnel.Purchase.Type.REPAYMENT_FUNNEL, Funnel.Purchase.Repayment.REQUEST_VALIDATION, contents);
                }

                //------------------------------------------------------------------------------------------------------------------------
                //  Server Side
                //------------------------------------------------------------------------------------------------------------------------
                //  10.     receipt_validation          : 영수증 검증
                //  10-1.   receipt_validation_fail     : 영수증 검증 실패
                //  10-2.   receipt_validation_success  : 영수증 검증 성공
                //  11.     check_remain_repayment      : 9-2에서 잔여 재결제가 있는지, repay tag 해제가 필요한지 확인하는 플로우
                //  11-1.   clear_repay_tag             : 10에서 잔여 재결제 없는 경우 repay 태그 해제 요청 (Server → IAM)
                //  12.     deliver_validation_result   : Server → SDK 영수증 검증 결과 전달
                //------------------------------------------------------------------------------------------------------------------------

                /// <summary>
                /// 13. 서버의 검증 결과 SDK에서 수신
                /// </summary>
                public static void ReceiveValidationResult()
                {
                    Send(Funnel.Purchase.Type.REPAYMENT_FUNNEL, Funnel.Purchase.Repayment.RECEIVE_VALIDATION_RESULT);
                }

                /// <summary>
                /// 13-1. 실패 후 4-1로 이동
                /// </summary>
                /// <param name="serverErrorCode"></param>
                public static void ValidationFail(string serverErrorCode)
                {
                    var contents = new Dictionary<string, object>
                    {
                        { "server_error_code", serverErrorCode }
                    };

                    Send(Funnel.Purchase.Type.REPAYMENT_FUNNEL, Funnel.Purchase.Repayment.VALIDATION_FAIL, contents);
                }

                /// <summary>
                /// 13-2. 성공
                /// </summary>
                public static void ValidationSuccess()
                {
                    Send(Funnel.Purchase.Type.REPAYMENT_FUNNEL, Funnel.Purchase.Repayment.VALIDATION_SUCCESS);
                }

                /// <summary>
                /// 14. 스토어에 영수증 컨슘 요청
                /// </summary>
                public static void RequestStoreConsume()
                {
                    Send(Funnel.Purchase.Type.REPAYMENT_FUNNEL, Funnel.Purchase.Repayment.REQUEST_STORE_CONSUME);
                }

                /// <summary>
                /// 14-1. 스토어 컨슘 실패
                /// </summary>
                public static void StoreConsumeFail(string storeErrorCode)
                {
                    var contents = new Dictionary<string, object>
                    {
                        { "store_error_code", storeErrorCode }
                    };

                    Send(Funnel.Purchase.Type.REPAYMENT_FUNNEL, Funnel.Purchase.Repayment.STORE_CONSUME_FAIL, contents);
                }

                /// <summary>
                /// 14-2. 스토어 컨슘 성공
                /// </summary>
                public static void StoreConsumeSuccess()
                {
                    Send(Funnel.Purchase.Type.REPAYMENT_FUNNEL, Funnel.Purchase.Repayment.STORE_CONSUME_SUCCESS);
                }
            }

            /// <summary>
            /// https://krafton.atlassian.net/wiki/spaces/DATACAT/pages/744687985/DCAT+Recovery+Funnel+Meta
            /// </summary>
            internal class PurchaseRecoveryFunnel
            {
                /// <summary>
                /// 1. 구매 복구 시작
                /// </summary>
                public static void TriggerPurchaseRecovery()
                {
                    var contents = new Dictionary<string, object>
                    {
                        { "store_type", GppSDK.GetOptions().Store.ToString() }
                    };

                    Send(Funnel.Purchase.Type.PURCHASE_RECOVERY_FUNNEL, Funnel.Purchase.PurchaseRecovery.TRIGGER_PURCHASE_RECOVERY, contents);
                }

                /// <summary>
                /// 2. 미컨슘 상품 유무 체크
                /// </summary>
                public static void CheckUnconsumedList()
                {
                    Send(Funnel.Purchase.Type.PURCHASE_RECOVERY_FUNNEL, Funnel.Purchase.PurchaseRecovery.CHECK_UNCONSUMED_LIST);
                }

                /// <summary>
                /// 3. SDK → Server 영수증 검증 및 인타이틀먼트 지급 요청 
                /// </summary>
                /// <param name="productId"></param>
                public static void RequestValidationGrant(string productId)
                {
                    var contents = new Dictionary<string, object>
                    {
                        { "product_id", productId }
                    };

                    Send(Funnel.Purchase.Type.PURCHASE_RECOVERY_FUNNEL, Funnel.Purchase.PurchaseRecovery.REQUEST_VALIDATION_GRANT, contents);
                }

                //------------------------------------------------------------------------------------------------------------------------
                //  Server Side
                //------------------------------------------------------------------------------------------------------------------------
                // 4. receipt_validation                : 영수증 검증
                // 4-1. receipt_validation_fail         : 영수증 검증 실패
                // 4-2. receipt_validation_success      : 영수증 검증 성공
                // 5. entitlement_grant                 : entitlement 지급
                // 5-1. entitlement_granted_Fail        : entitlement 지급 실패
                // 5-2. entitlement_granted_success     : entitlement 지급 성공
                // 6. check_remain_repayment            : Repayment → Check Remained Repaymen
                // 6-1. clear_repay_tag                 : 6에서 잔여 재결제 없는 경우 repay 태그 해제 요청 (Server → IAM)
                // 7. deliver_validation_grant_result   : Server → SDK 영수증 검증 및 인타이틀먼트 지급 결과 전달 
                //------------------------------------------------------------------------------------------------------------------------

                /// <summary>
                /// 8. 서버의 검증/지급 결과 SDK에서 수신
                /// </summary>
                public static void ReceiveValidationGrantResult()
                {
                    Send(Funnel.Purchase.Type.PURCHASE_RECOVERY_FUNNEL, Funnel.Purchase.PurchaseRecovery.RECEIVE_VALIDATION_GRANT_RESULT);
                }

                /// <summary>
                /// 8-1. 실패
                /// </summary>
                public static void ValidationGrantFail(string serverErrorCode)
                {
                    var contents = new Dictionary<string, object>
                    {
                        { "server_error_code", serverErrorCode }
                    };

                    Send(Funnel.Purchase.Type.PURCHASE_RECOVERY_FUNNEL, Funnel.Purchase.PurchaseRecovery.VALIDATION_GRANT_FAIL, contents);
                }

                /// <summary>
                /// 8-2. 성공
                /// </summary>
                public static void ValidationGrantSuccess(string entitlementId)
                {
                    var contents = new Dictionary<string, object>
                    {
                        { "entitlement_id", entitlementId }
                    };

                    Send(Funnel.Purchase.Type.PURCHASE_RECOVERY_FUNNEL, Funnel.Purchase.PurchaseRecovery.VALIDATION_GRANT_SUCCESS, contents);
                }

                /// <summary>
                /// 9. 스토어에 영수증 컨슘 요청
                /// </summary>
                public static void RequestStoreConsume()
                {
                    Send(Funnel.Purchase.Type.PURCHASE_RECOVERY_FUNNEL, Funnel.Purchase.PurchaseRecovery.REQUEST_STORE_CONSUME);
                }

                /// <summary>
                /// 9-1. 스토어 컨슘 실패
                /// </summary>
                public static void StoreConsumeFail(string storeErrorCode)
                {
                    var contents = new Dictionary<string, object>
                    {
                        { "store_error_code", storeErrorCode }
                    };

                    Send(Funnel.Purchase.Type.PURCHASE_RECOVERY_FUNNEL, Funnel.Purchase.PurchaseRecovery.STORE_CONSUME_FAIL, contents);
                }

                /// <summary>
                /// 9-2. 스토어 컨슘 성공
                /// </summary>
                public static void StoreConsumeSuccess()
                {
                    Send(Funnel.Purchase.Type.PURCHASE_RECOVERY_FUNNEL, Funnel.Purchase.PurchaseRecovery.STORE_CONSUME_SUCCESS);
                }
            }

            private static void Send(string funnelType, string step, Dictionary<string, object> contents = null)
            {
                // Supports Android and iOS only
                if (PlatformUtil.IsMobileNotEditor() is false)
                {
                    return;
                }
                if (GppSDK.IsInitialized is false)
                {
                    return;
                }
                var funnelLogInfo = GetFunnelLogInfo(funnelType, step);
                if (funnelLogInfo is null)
                {
                    // invalid funnel log
                    // ex) Sent without a step to start the funnel.
                    return;
                }
                var metadata = new Dictionary<string, object>
                {
                    { "funnel_type", funnelType },
                    { "event_type", funnelLogInfo.CurrentStep },
                    { "prev_event_type", funnelLogInfo.PrevStep },
                    { "time_since_prev_event", funnelLogInfo.GetElapsedSinceLastLog() },
                    { "funnel_id", GppTokenProvider.CreateGuestUserId() },
                    { "krafton_id", GppSDK.GetSession().cachedTokenData?.KraftonId ?? "" }
                };

                if (contents is not null)
                {
                    metadata.Add("contents", contents);
                }

                if (GppTelemetry.IsFunnelImmediateSend)
                {
                    GppSDK.SendImmediatelyGppTelemetry(GppClientLogModels.LogType.KOS_EVENT_FUNNEL, metadata);
                }
                else
                {
                    GppSDK.SendGppTelemetry(GppClientLogModels.LogType.KOS_EVENT_FUNNEL, metadata);
                }
            }

            private static FunnelLogInfo GetFunnelLogInfo(string funnelType, string step)
            {
                var funnelId = FunnelLogManager.GetFunnelId(funnelType, step);
                if (string.IsNullOrEmpty(funnelId))
                {
                    return null;
                }

                var funnelLogInfo = FunnelLogManager.GetFunnelLogInfo(funnelId);
                funnelLogInfo.CurrentLogType = funnelType;
                funnelLogInfo.CurrentStep = step;

                return funnelLogInfo;
            }

            /// <summary>
            /// Purchase error codes by market
            /// https://krafton.atlassian.net/wiki/spaces/GGD/pages/130580286#tab-27afe0bd-2272-47dd-b62e-b54678a23845
            /// </summary>
            public static bool IsUserCanceled(StoreType storeType, int storeErrorCode)
            {
                switch (storeType)
                {
                    case StoreType.AppStore when storeErrorCode is 2:
                    case StoreType.GooglePlayStore or StoreType.GalaxyStore when storeErrorCode is 1:
                        return true;
                }

                return false;
            }
        }
    }
}