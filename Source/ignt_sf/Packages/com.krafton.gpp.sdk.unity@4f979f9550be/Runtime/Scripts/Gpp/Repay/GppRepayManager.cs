using System;
using System.Collections.Generic;
using System.Linq;
using Gpp.CommonUI;
using Gpp.CommonUI.Modal;
using Gpp.Constants;
using Gpp.Core;
using Gpp.Extension;
using Gpp.Extensions.IAP.Models;
using Gpp.Models;
using Gpp.Telemetry;
using UnityEngine;

namespace Gpp.Repay
{
    internal class GppRepayManager
    {
        #region L10N
        private const string RepayStillBanExcpetion = "LIFT_ACCOUNT_REPAY_EXCEPTION_1";
        private const string RepayOtherStoreException = "LIFT_ACCOUNT_REPAY_EXCEPTION_2";
        private const string RepayStoreErrorException = "LIFT_ACCOUNT_REPAY_EXCEPTION_3";

        private const string RepayTotalCount = "LIFT_ACCOUNT_REPAY_TOTAL";

        private const string RepayModalTitle = "LIFT_ACCOUNT_REPAY_DES";
        private const string RepayModalBody = "LIFT_ACCOUNT_REPAY_FAIL";
        private const string RepayModalButtonText = "GENERAL_CONFIRM";

        private const string RepayErrorCode1 = "REPAY_ERRORCODE_1";
        private const string RepayErrorCode2 = "REPAY_ERRORCODE_2";
        private const string RepayErrorCode3 = "REPAY_ERRORCODE_3";
        private const string RepayErrorCode4 = "REPAY_ERRORCODE_4";
        private const string RepayErrorCode5 = "REPAY_ERRORCODE_5";
        private const string RepayErrorCode6 = "REPAY_ERRORCODE_6";
        #endregion

        private List<RefundRestriction> refundRestrictionList = new List<RefundRestriction>();
        private List<RefundRestriction> purchasedRestrictionList = new List<RefundRestriction>();

        private Action successCallback;
        private Action failCallback;

        private List<IapProduct> iapProductList = new List<IapProduct>();
        private bool isGetProductsError = false;
        internal GppRepayManager(RefundRestriction[] refundRestrictions, Action onSuccess, Action onFail)
        {
            FunnelLog.Purchase.RepaymentFunnel.RepayStatus();
            refundRestrictionList = refundRestrictions.ToList();
            successCallback = onSuccess;
            failCallback = onFail;

            // 결제 할 상품이 없을 경우
            if (refundRestrictionList.Count == 0)
            {
                FunnelLog.Purchase.RepaymentFunnel.RepayComplete("complete but not yet");
                OpenRepayErrorUI(RepayStoreErrorException, null);
                return;
            }

            FunnelLog.Purchase.RepaymentFunnel.RepayIncomplete();

            var cannotBuyProducts = refundRestrictionList.Where((product) =>
            {
                return !IsSameStore(product.repaymentInfo.store);
            }).ToList();

            // 다른 상점의 상품이 있을 경우
            if (cannotBuyProducts.Count != 0)
            {
                string reason = RepaymentFailReason.DIFFERENT_STORE_REPAYMENT_FAILED.ToString();
                SendActionResult(cannotBuyProducts, reason, "");
                OpenRepayErrorUI(RepayOtherStoreException, reason);
                return;
            }

            List<string> sameStoreProductIds = refundRestrictionList
                .Where(x =>
                {
                    return IsSameStore(x.repaymentInfo.store);
                })
                .Select(x => x.repaymentInfo.productId).Distinct().ToList();

            GppSDK.GetProducts(sameStoreProductIds, result =>
            {
                isGetProductsError = result.IsError;

                //Get Product Error
                if (isGetProductsError)
                {
                    string reason = RepaymentFailReason.REPAYMENT_FAILED.ToString();
                    SendActionResult(refundRestrictionList, reason, "get products error");
                    OpenRepayErrorUI(RepayStoreErrorException, reason);
                    return;
                }

                if (!isGetProductsError)
                {
                    iapProductList = result.Value;

                    //제한걸린 상품 중 하나라도 스토어에 없을 경우
                    if (sameStoreProductIds.Count != result.Value.Count)
                    {
                        string reason = RepaymentFailReason.NOT_EXISTS_PRODUCT_REPAYMENT_FAILED.ToString();
                        SendActionResult(refundRestrictionList, reason, "products not found");
                        OpenRepayErrorUI(RepayStillBanExcpetion, reason);

                        return;
                    }
                }

                StartRepayment();
            });
        }

        private void StartRepayment()
        {
            OpenRepayUI();
        }

        private void CheckRefundRestrictions()
        {
            FunnelLog.Purchase.RepaymentFunnel.RepayStatus();
            if (refundRestrictionList.Count == purchasedRestrictionList.Count)
            {
                FunnelLog.Purchase.RepaymentFunnel.RepayComplete();
                successCallback();
                return;
            }

            FunnelLog.Purchase.RepaymentFunnel.RepayIncomplete();
            OpenRepayUI();
        }

        private void OpenRepayUI()
        {
            FunnelLog.Purchase.RepaymentFunnel.ShowRepayPopup();
            GppUI.ShowRepayRequired(refundRestrictionList, purchasedRestrictionList, iapProductList,
            (purchasedRefund) =>
            {
                GppUI.DismissUI(UIType.RepayRequired);
                FunnelLog.Purchase.RepaymentFunnel.ClickRepayPerProduct(purchasedRefund.id, purchasedRefund.repaymentInfo.productId);
                StartRepayPurchase(purchasedRefund);
            },
            (ui) =>
            {
                FunnelLog.Purchase.RepaymentFunnel.ClickAccountSwitch();
                GppUI.DismissUI(UIType.RepayRequired);

                GppSyncContext.RunOnUnityThread(() =>
                {
                    failCallback();
                });
            });
        }

        private void OpenRepayErrorUI(string key, string reason)
        {
            if (!string.IsNullOrEmpty(reason))
            {
                FunnelLog.Purchase.RepaymentFunnel.ShowRepayPopup();
                FunnelLog.Purchase.RepaymentFunnel.RepayBlocked(reason);
            }

            GppUI.ShowRepayRequiredError(key, (ui) =>
            {
                FunnelLog.Purchase.RepaymentFunnel.ClickAccountSwitch();
                GppUI.DismissUI(UIType.RepayRequired);

                GppSyncContext.RunOnUnityThread(() =>
                {
                    failCallback();
                });
            });
        }

        private void SendActionResult(List<RefundRestriction> restrictions, string errorCode, string errorMessage)
        {
            var restrictionFailedResult = restrictions.Select(restriction =>
            {
                return new RestrictionFailedResult()
                {
                    restrictionId = restriction.id,
                    store = restriction.repaymentInfo.store,
                    errorCode = errorCode,
                    errorMessage = errorMessage
                };
            }).ToList();

            GppSDK.GetPlatformApi().SetRestrictionsActionResults(restrictionFailedResult, result => { });
        }

        private void StartRepayPurchase(RefundRestriction refundRestriction)
        {
            GppSDK.RepayPurchase(refundRestriction.repaymentInfo.productId, refundRestriction.id, refundRestriction.repaymentInfo.transactionId,
            (result) =>
            {
                if (result.IsError)
                {
                    if (result.Error.Code == ErrorCode.GetStoreCountryCodeFailed)
                    {
                        DrawCommonErrorModal(RepayErrorCode2, ((int)result.Error.Code).ToString());
                        return;
                    }

                    if (result.Error.Code == ErrorCode.PurchaseValidationFailed)
                    {
                        if (result.Error.InnerError.Code == ErrorCode.ReceiptDoNotConsume)
                        {
                            DrawCommonErrorModal(RepayErrorCode5, ((int)result.Error.InnerError.Code).ToString());
                            purchasedRestrictionList.Add(refundRestriction);
                            return;
                        }
                    }

                    if (result.Error.Code != ErrorCode.PurchaseFailed)
                    {
                        SendActionResult(refundRestrictionList, RepaymentFailReason.REPAYMENT_FAILED.ToString(), result.Error.Message);
                        DrawCommonErrorModal(RepayErrorCode3, ((int)result.Error.Code).ToString());
                        return;
                    }

                    var storeError = GetPurchaseError(result.Error.Message);
                    if (storeError != null)
                    {
#if UNITY_IOS
                        switch (storeError.Item1)
                        {
                            case 2:
                                DrawCommonErrorModal(RepayErrorCode6, storeError.Item1.ToString());
                                break;
                            case -999:
                                DrawCommonErrorModal(RepayErrorCode4, storeError.Item1.ToString());
                                break;
                            default:
                                DrawCommonErrorModal(RepayErrorCode1, storeError.Item1.ToString());
                                break;
                        }
#elif UNITY_ANDROID
                        switch (storeError.Item1)
                        {
                            case 1:
                                DrawCommonErrorModal(RepayErrorCode6, storeError.Item1.ToString());
                                break;
                            case 4:
                                DrawCommonErrorModal(RepayErrorCode4, storeError.Item1.ToString());
                                break;
                            case 7:
                                DrawCommonErrorModal(RepayErrorCode5, storeError.Item1.ToString());
                                break;
                            default:
                                DrawCommonErrorModal(RepayErrorCode1, storeError.Item1.ToString());
                                break;
                        }
#endif
                    }
                    else
                    {
                        DrawCommonErrorModal(RepayModalBody, "");
                        return;
                    }
                }
                else
                {
                    purchasedRestrictionList.Add(refundRestriction);
                    CheckRefundRestrictions();
                }
            });
        }

        private bool IsSameStore(string store)
        {
            var storeType = GppSDK.GetOptions().Store;
            if (store == "GOOGLE")
                return storeType == Constants.StoreType.GooglePlayStore;
            else if (store == "APPLE")
                return storeType == Constants.StoreType.AppStore;
            else if (store == "GALAXY")
                return storeType == Constants.StoreType.GalaxyStore;

            return false;
        }

        private Tuple<int, string> GetPurchaseError(string errorMessage)
        {
            if (errorMessage.Contains("[") && errorMessage.Contains("]"))
            {
                string[] parsedError = errorMessage.Split(']');
                int storeErrorCode = int.Parse(parsedError[0].Replace("[", ""));
                string storeErrorMessage = parsedError[1];

                return new Tuple<int, string>(storeErrorCode, storeErrorMessage);
            }

            return null;
        }

        private void DrawCommonErrorModal(string key, string errorMessage)
        {
            var builder = new GppModalData.Builder();
            builder.SetTitle(RepayModalTitle.Localise());
            builder.SetMessage(string.Format(key.Localise(), errorMessage));
            builder.SetPositiveButtonText(RepayModalButtonText.Localise());
            builder.SetPositiveAction((ui) =>
            {
                CheckRefundRestrictions();
                GppUI.DismissUI(UIType.Modal);
            });
            GppUI.ShowModal(builder.Build());
        }
    }
}