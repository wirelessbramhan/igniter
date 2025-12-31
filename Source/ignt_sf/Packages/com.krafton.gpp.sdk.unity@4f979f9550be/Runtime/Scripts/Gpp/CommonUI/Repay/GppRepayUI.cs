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
using Gpp.Telemetry;
using Gpp.Utils;
using RTLTMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gpp.CommonUI.GppRepay
{
    public class GppRepayUI : GppCommonUI, IGppRepay
    {
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
        private Action<RefundRestriction> onStartPurchase;
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

        public void SetRestrictionProductList(List<RefundRestriction> refundRestrictions, List<RefundRestriction> purchasedRefunds, List<Extensions.IAP.Models.IapProduct> iapProducts)
        {
            AddLoginAnotherAccountButton();
            AddNoteButtons();

            GppUtil.FindChildWithName<RTLTextMeshPro>(CurrentLayout, "CountTitle").text = string.Format(RepayTotalCount.Localise(), refundRestrictionList.Count);

            refundRestrictionList = refundRestrictions;
            purchasedRestrictionList = purchasedRefunds;
            iapProductList = iapProducts;

            DrawRefundProductsUI();
            AddBuyButton();

            UpdateRefundProductsUI();

            var remainRestrictions = refundRestrictions.Where(restriction =>
            {
                return !purchasedRefunds.Any(purchasedRestriction => purchasedRestriction.id == restriction.id);
            });

            List<Dictionary<string, object>> repaymentMetaDataList = new List<Dictionary<string, object>>();

            foreach (var restriction in remainRestrictions)
            {
                Dictionary<string, object> repaymentMetadata = new Dictionary<string, object>
                {
                    ["transaction_id"] = restriction.repaymentInfo.transactionId,
                    ["repayment_id"] = restriction.id,
                    ["store"] = restriction.repaymentInfo.store
                };

                repaymentMetaDataList.Add(repaymentMetadata);
            }

            FunnelLog.Purchase.RepaymentFunnel.ShowRepayList(repaymentMetaDataList);

            drawNextFrame = true;
        }

        public void SetSuccessRepayCallback(Action<RefundRestriction> onSuccess)
        {
            onStartPurchase = onSuccess;
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

        public void DrawCommonErrorMessage(string key)
        {
            isOpenErrorMessage = true;
            errorMessage = key;

            AddNoteButtons();

            var failButton = GppUtil.FindChildWithName<GppCommonButton>(CurrentLayout, "FailButton");
            failButton.onClick.RemoveAllListeners();
            failButton.onClick.AddListener(() =>
            {
                onFail(gameObject);
            });

            GppUtil.FindChildWithName(CurrentLayout, "BuyButton").SetActive(false);
            GppUtil.FindChildWithName(CurrentLayout, "FailButton").SetActive(true);
            GppUtil.FindChildWithName(CurrentLayout, "CountTitle").SetActive(false);
            GppUtil.FindChildWithName(CurrentLayout, "LoginAnotherButton").SetActive(false);

            GameObject errorPage = GppUtil.FindChildWithName(CurrentLayout, "ErrorPage");
            errorPage.SetActive(true);

            GppUtil.FindChildWithName(errorPage.transform.parent.gameObject, "ProductView").SetActive(false);
            GppUtil.FindChildWithName<RTLTextMeshPro>(errorPage, "ErrorText").text = key.Localise();
        }

        public void DrawCommonErrorMessage(string key, Action<GameObject> onFail)
        {
            isOpenErrorMessage = true;
            errorMessage = key;
            this.onFail = onFail;

            AddNoteButtons();

            var failButton = GppUtil.FindChildWithName<GppCommonButton>(CurrentLayout, "FailButton");
            failButton.onClick.RemoveAllListeners();
            failButton.onClick.AddListener(() =>
            {
                this.onFail(gameObject);
            });

            GppUtil.FindChildWithName(CurrentLayout, "BuyButton").SetActive(false);
            GppUtil.FindChildWithName(CurrentLayout, "FailButton").SetActive(true);
            GppUtil.FindChildWithName(CurrentLayout, "CountTitle").SetActive(false);
            GppUtil.FindChildWithName(CurrentLayout, "LoginAnotherButton").SetActive(false);

            GameObject errorPage = GppUtil.FindChildWithName(CurrentLayout, "ErrorPage");
            errorPage.SetActive(true);

            GppUtil.FindChildWithName(errorPage.transform.parent.gameObject, "ProductView").SetActive(false);
            GppUtil.FindChildWithName<RTLTextMeshPro>(errorPage, "ErrorText").text = key.Localise();
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
                onStartPurchase(selectedRestirction);
                //RepayPurchase(selectedRestirction, button);
            });
        }

        private void AddNoteButtons()
        {
            Button csButton = GppUtil.FindChildWithName<GppCommonButton>(CurrentLayout, "CSButton");
            csButton.onClick.RemoveAllListeners();
            csButton.onClick.AddListener(() =>
            {
                FunnelLog.Purchase.RepaymentFunnel.ClickCustomerService();
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
    }
}
