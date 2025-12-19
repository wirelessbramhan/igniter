using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Gpp.CommonUI.Modal;
using Gpp.Core;
using Gpp.Extension;
using Gpp.Extensions.IAP.Models;
using Gpp.Models;
using Gpp.Utils;
using RTLTMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gpp.CommonUI.GppRepay
{
    public class GppRepayUI : GppCommonUI, IGppRepay
    {
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


        private List<RefundRestriction> refundRestrictionList = new List<RefundRestriction>();
        private List<RefundRestriction> purchasedRestrictionList = new List<RefundRestriction>();
        private List<Extensions.IAP.Models.IapProduct> iapProductList = new List<Extensions.IAP.Models.IapProduct>();
        private Action onSuccess;
        private Action<GameObject> onFail;

        private RefundRestriction selectedRestirction;
        private GameObject selectedButton;

        private bool drawNextFrame = false;
        private int updateFrame = 0;

        private bool isOpenErrorMessage = false;
        private string errorMessage = string.Empty;

        private readonly Dictionary<ScreenOrientation, string> productViewPath = new()
        {
            { ScreenOrientation.Portrait, "GppCommonUI/Mobile/GppRepayProductViewPortrait" },
            { ScreenOrientation.LandscapeLeft, "GppCommonUI/Mobile/GppRepayProductViewLandscape" }
        };

        private Dictionary<ScreenOrientation, Dictionary<string, GameObject>> productObjectDictionary = new Dictionary<ScreenOrientation, Dictionary<string, GameObject>>();

        public void SetRestrictionProducts(RefundRestriction[] refundRestrictions, List<Extensions.IAP.Models.IapProduct> iapProducts, bool isGetProductsError)
        {
            AddLoginAnotherAccountButton();
            AddNoteButtons();

            if (refundRestrictions.Length == 0)
            {
                DrawCommonErrorMessage(RepayStoreErrorException);
                return;
            }

            if (isGetProductsError)
            {
                SendActionResult(refundRestrictions.ToList(), RepaymentFailReason.REPAYMENT_FAILED.ToString(), "get products error");
                DrawCommonErrorMessage(RepayStoreErrorException);
                return;
            }

            var cannotBuyProducts = refundRestrictions.Where((product) =>
            {
                return !IsSameStore(product.repaymentInfo.store);
            })
            .Select(restriction =>
            {
                return new RestrictionFailedResult()
                {
                    restrictionId = restriction.id,
                    store = restriction.repaymentInfo.store,
                    errorCode = RepaymentFailReason.DIFFERENT_STORE_REPAYMENT_FAILED.ToString(),
                    errorMessage = ""
                };
            }).ToList();

            if (cannotBuyProducts.Count != 0)
            {
                GppSDK.GetPlatformApi().SetRestrictionsActionResults(cannotBuyProducts, result => { });
                DrawCommonErrorMessage(RepayOtherStoreException);
                return;
            }

            refundRestrictionList = refundRestrictions.Where((product) =>
            {
                return IsSameStore(product.repaymentInfo.store);
            }).ToList();

            if (iapProducts.Count == 0)
            {
                SendActionResult(refundRestrictions.ToList(), RepaymentFailReason.NOT_EXISTS_PRODUCT_REPAYMENT_FAILED.ToString(), "products not found");
                DrawCommonErrorMessage(RepayStillBanExcpetion);
                return;
            }

            var restrictionProductIdList = refundRestrictions.Select(x => x.repaymentInfo.productId).Distinct().ToList();
            bool hasDisabledProduct = restrictionProductIdList.Where(x => !iapProducts.Any(y => y.ProductId == x)).Count() != 0;

            if (hasDisabledProduct)
            {
                SendActionResult(refundRestrictions.ToList(), RepaymentFailReason.NOT_EXISTS_PRODUCT_REPAYMENT_FAILED.ToString(), "products not found");
                DrawCommonErrorMessage(RepayStillBanExcpetion);
                return;
            }

            iapProductList = iapProducts;

            refundRestrictionList.RemoveAll((refundProduct) => !iapProductList.Any((refreshedProduct) => refreshedProduct.ProductId == refundProduct.repaymentInfo.productId));

            if (refundRestrictionList.Count() == 0)
            {
                DrawCommonErrorMessage(RepayStillBanExcpetion);
                return;
            }

            GppUtil.FindChildWithName<RTLTextMeshPro>(CurrentLayout, "CountTitle").text = string.Format(RepayTotalCount.Localise(), refundRestrictionList.Count);

            DrawRefundProductsUI();
            AddBuyButton();

            drawNextFrame = true;
        }

        public void SetSuccessRepayCallback(Action onSuccess)
        {
            this.onSuccess = onSuccess;
        }
        public void SetFailRepayCallback(Action<GameObject> onFail)
        {
            this.onFail = onFail;
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Update()
        {
            base.Update();
            updateFrame++;

            if (updateFrame == 1)
            {
                ResetRepayNote();
            }

            if (drawNextFrame)
            {
                updateFrame = 0;
                drawNextFrame = false;
            }
        }

        protected override void OnChangedOrientation(ScreenOrientation orientation)
        {
            base.OnChangedOrientation(orientation);

            drawNextFrame = true;

            AddLoginAnotherAccountButton();
            AddNoteButtons();

            if (isOpenErrorMessage)
            {
                DrawCommonErrorMessage(errorMessage);
            }
            else
            {
                if (refundRestrictionList.Count != 0)
                {
                    DrawRefundProductsUI();
                    UpdateRefundProductsUI();
                    AddBuyButton();
                    ResetRepayNote();
                    ChangeSelectProduct(selectedRestirction, productObjectDictionary[GetOrientation()][selectedRestirction.id]);
                }
            }
        }

        private void ResetRepayNote()
        {
            GppSyncContext.RunOnUnityThread(() =>
            {
                GameObject repayNote = GppUtil.FindChildWithName(CurrentLayout, "RepayNote");
                GameObject noteBox = GppUtil.FindChildWithName(repayNote, "NoteBox");
                GameObject listNote = GppUtil.FindChildWithName(noteBox, "ListNote");
                GameObject dotNoteFirst = GppUtil.FindChildWithName(listNote, "DotNote1");
                GameObject dotNoteSecond = GppUtil.FindChildWithName(listNote, "DotNote2");
                GameObject buttons = GppUtil.FindChildWithName(noteBox, "Buttons");

                dotNoteFirst.SetActive(false);
                dotNoteFirst.SetActive(true);

                dotNoteSecond.SetActive(false);
                dotNoteSecond.SetActive(true);

                buttons.SetActive(false);
                buttons.SetActive(true);

                listNote.SetActive(false);
                listNote.SetActive(true);

                noteBox.SetActive(false);
                noteBox.SetActive(true);

                repayNote.SetActive(false);
                repayNote.SetActive(true);
            });
        }

        private void DrawRefundProductsUI()
        {
            if (!productObjectDictionary.ContainsKey(GetOrientation()))
            {
                productObjectDictionary.Add(GetOrientation(), new Dictionary<string, GameObject>());
            }
            else
            {
                return;
            }

            var productBox = GppUtil.FindChildWithName(CurrentLayout, "ProductBox");
            GameObject productView = Resources.Load<GameObject>(productViewPath[GetOrientation()]);

            for (int i = 0; i < refundRestrictionList.Count; i++)
            {
                var restriction = refundRestrictionList[i];

                GameObject instance = Instantiate(productView, productBox.transform);
                string productName = iapProductList.First(iapProduct => iapProduct.ProductId == restriction.repaymentInfo.productId).Name;
                if (productName.Length > 30)
                {
                    productName = productName.Substring(0, 27);
                    productName += "...";
                }

                DateTime dateTime = new DateTime(1970, 1, 1).AddSeconds(Convert.ToInt64(restriction.repaymentInfo.purchaseAt)).ToLocalTime();

                GppUtil.FindChildWithName(instance, "Name").GetComponent<RTLTextMeshPro>().text = productName;
                GppUtil.FindChildWithName(instance, "Date").GetComponent<RTLTextMeshPro>().text = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
                GppUtil.FindChildWithName(instance, "Price").GetComponent<RTLTextMeshPro>().text = iapProductList.First(iapProduct => iapProduct.ProductId == restriction.repaymentInfo.productId).PriceString;

                if (i == 0 && selectedRestirction == null)
                {
                    Toggle toggle = GppUtil.FindChildWithName<Toggle>(instance, "Toggle");
                    toggle.isOn = true;

                    selectedRestirction = restriction;
                    selectedButton = instance;

                    instance.GetComponent<Outline>().enabled = true;
                    selectedButton.GetComponent<Button>().interactable = false;
                }

                var button = instance.GetComponent<Button>();

                button.onClick.AddListener(() =>
                {
                    if (selectedRestirction.id != restriction.id)
                    {
                        ChangeSelectProduct(restriction, instance);
                    }
                });

                productObjectDictionary[GetOrientation()].Add(restriction.id, instance);

                instance.SetActive(false);
                instance.SetActive(true);
            }
        }

        private void DrawCommonErrorMessage(string key)
        {
            isOpenErrorMessage = true;
            errorMessage = key;

            GppUtil.FindChildWithName(CurrentLayout, "BuyButton").SetActive(false);
            GppUtil.FindChildWithName(CurrentLayout, "FailButton").SetActive(true);
            GppUtil.FindChildWithName(CurrentLayout, "CountTitle").SetActive(false);
            GppUtil.FindChildWithName(CurrentLayout, "LoginAnotherButton").SetActive(false);

            GameObject errorPage = GppUtil.FindChildWithName(CurrentLayout, "ErrorPage");
            errorPage.SetActive(true);

            GppUtil.FindChildWithName(errorPage.transform.parent.gameObject, "ProductView").SetActive(false);
            GppUtil.FindChildWithName<RTLTextMeshPro>(errorPage, "ErrorText").text = key.Localise();
        }

        private void DrawCommonErrorModal(string key, string errorMessage)
        {
            gameObject.SetActive(false);

            var builder = new GppModalData.Builder();
            builder.SetTitle(RepayModalTitle.Localise());
            builder.SetMessage(string.Format(key.Localise(), errorMessage));
            builder.SetPositiveButtonText(RepayModalButtonText.Localise());
            builder.SetPositiveAction((ui) =>
            {
                GppUI.DismissUI(UIType.Modal);
                gameObject.SetActive(true);
            });
            GppUI.ShowModal(builder.Build());
        }

        private void AddLoginAnotherAccountButton()
        {
            var failButton = GppUtil.FindChildWithName<GppCommonButton>(CurrentLayout, "FailButton");

            failButton.onClick.RemoveAllListeners();
            failButton.onClick.AddListener(() =>
            {
                LoginAnotherAccount();
            });

            var loginAnotherButton = GppUtil.FindChildWithName<GppCommonButton>(CurrentLayout, "LoginAnotherButton");

            loginAnotherButton.onClick.RemoveAllListeners();
            loginAnotherButton.onClick.AddListener(() =>
            {
                LoginAnotherAccount();
            });
        }

        private void AddBuyButton()
        {
            var button = GppUtil.FindChildWithName<GppCommonButton>(CurrentLayout, "BuyButton");
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                button.interactable = false;
                RepayPurchase(selectedRestirction, button);
            });
        }

        private void AddNoteButtons()
        {
            Button csButton = GppUtil.FindChildWithName<GppCommonButton>(CurrentLayout, "CSButton");
            csButton.onClick.RemoveAllListeners();
            csButton.onClick.AddListener(() =>
            {
                GppSDK.OpenRepayCS((result) => { });
            });
        }

        private void UpdateRefundProductsUI()
        {
            GppUtil.FindChildWithName<RTLTextMeshPro>(CurrentLayout, "CountTitle").text = string.Format(RepayTotalCount.Localise(), refundRestrictionList.Count - purchasedRestrictionList.Count);

            if (purchasedRestrictionList.Count == 0)
                return;

            var productBox = GppUtil.FindChildWithName(CurrentLayout, "ProductBox");
            var repayText = GppUtil.FindChildWithName(productBox, "RepaySuccess");

            if (repayText.transform.GetSiblingIndex() == 0)
            {
                repayText.transform.SetAsLastSibling();
            }

            bool isChanged = false;
            foreach (var pair in productObjectDictionary[GetOrientation()])
            {
                if (purchasedRestrictionList.Any(x => x.id == pair.Key))
                {
                    int siblingIdx = repayText.transform.GetSiblingIndex();

                    GameObject productObject = pair.Value;

                    productObject.transform.SetSiblingIndex(siblingIdx + 1);
                    var productButton = productObject.GetComponent<Button>();
                    productButton.interactable = false;
                    productButton.onClick.RemoveAllListeners();
                    GppUtil.FindChildWithName(productObject, "Toggle").SetActive(false);
                    GppUtil.FindChildWithName(productObject, "Date").SetActive(false);
                    GppUtil.FindChildWithName(productObject, "Price").SetActive(false);
                    string hexColor = "#777777";

                    if (ColorUtility.TryParseHtmlString(hexColor, out Color color))
                    {
                        GppUtil.FindChildWithName<RTLTextMeshPro>(productObject, "Name").color = color;
                    }

                    RectTransform productRect = productObject.GetComponent<RectTransform>();
                    Vector2 productRectSize = productRect.sizeDelta;
                    productRect.sizeDelta = new Vector2(productRectSize.x, 190);
                    isChanged = true;
                }
            }

            RefundRestriction restriction = refundRestrictionList
                .Where(x => !purchasedRestrictionList.Any(purchasedRestriction => x.id == purchasedRestriction.id))
                .First(x => productObjectDictionary[GetOrientation()].ContainsKey(x.id));

            if (restriction != null)
            {
                ChangeSelectProduct(restriction, productObjectDictionary[GetOrientation()][restriction.id]);
            }
            else
            {
                ChangeSelectProduct(null, null);
            }

            if (isChanged)
                repayText.SetActive(true);
            //
        }

        public void RepayPurchase(RefundRestriction refundRestriction, Button button)
        {
            GppSDK.RepayPurchase(refundRestriction.repaymentInfo.productId, refundRestriction.id, refundRestriction.repaymentInfo.transactionId,
                (result) =>
                {
                    Debug.Log("response");
                    button.transition = Selectable.Transition.SpriteSwap;
                    button.interactable = true;

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
                                if (refundRestrictionList.Count == purchasedRestrictionList.Count)
                                {
                                    onSuccess.Invoke();
                                }
                                else
                                {
                                    UpdateRefundProductsUI();
                                }
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
                        if (refundRestrictionList.Count == purchasedRestrictionList.Count)
                        {
                            onSuccess.Invoke();
                        }
                        else
                        {
                            UpdateRefundProductsUI();
                        }
                    }
                });

            Debug.Log($"button.interactable :: {button.IsInteractable()}");
        }

        private void ChangeSelectProduct(RefundRestriction restriction, GameObject selectButton)
        {
            if (productObjectDictionary.ContainsKey(ScreenOrientation.Portrait))
            {
                GameObject selectedRestrictionButton = productObjectDictionary[ScreenOrientation.Portrait][selectedRestirction.id];
                Toggle toggle = GppUtil.FindChildWithName<Toggle>(selectedRestrictionButton, "Toggle");
                toggle.isOn = false;

                selectedRestrictionButton.GetComponent<Outline>().enabled = false;
                selectedRestrictionButton.GetComponent<Button>().interactable = true;
            }

            if (productObjectDictionary.ContainsKey(ScreenOrientation.LandscapeLeft))
            {
                GameObject selectedRestrictionButton = productObjectDictionary[ScreenOrientation.LandscapeLeft][selectedRestirction.id];
                Toggle toggle = GppUtil.FindChildWithName<Toggle>(selectedRestrictionButton, "Toggle");
                toggle.isOn = false;

                selectedRestrictionButton.GetComponent<Outline>().enabled = false;
                selectedRestrictionButton.GetComponent<Button>().interactable = true;
            }


            if (restriction != null)
            {
                selectedRestirction = restriction;
                selectedButton = selectButton;
                selectedButton.GetComponent<Button>().interactable = false;

                Toggle selectedToggle = GppUtil.FindChildWithName<Toggle>(selectButton, "Toggle");
                selectedToggle.isOn = true;

                selectedButton.GetComponent<Outline>().enabled = true;
            }
        }

        private void LoginAnotherAccount()
        {
            onFail(gameObject);
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
    }
}
