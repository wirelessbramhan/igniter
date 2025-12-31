using System;
using System.Collections.Generic;
using System.Linq;
using Gpp.Models;
using Gpp.Telemetry;
using Gpp.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Gpp.CommonUI.Legal
{
    public class GppLegalUI : GppCommonUI, IGppLegalUI
    {
        private Dictionary<string, GppLegalModel> policies;
        private Action<GameObject, bool, Dictionary<string, GppLegalModel>> onSubmitDelegate;
        private Action<GameObject> onLoginAnotherAccountDelegate;
        private Action onViewDetail;
        private bool isEnableLoginAnotherAccount = false;

        private readonly Dictionary<ScreenOrientation, string> policyViewPath = new()
        {
            { ScreenOrientation.Portrait, "GppCommonUI/Mobile/GppPolicyViewPortrait" },
            { ScreenOrientation.LandscapeLeft, "GppCommonUI/Mobile/GppPolicyViewLandscape" }
        };

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnChangedOrientation(ScreenOrientation orientation)
        {
            base.OnChangedOrientation(orientation);
            AddPolicyView();
            AddSubmitDelegateToStartButton();
            AddSubmitDelegateToAllSubmitButton();
            AddSubmitDelegateToLoginAnotherAccointButton();
        }

        public void SetPolicies(Dictionary<string, GppLegalModel> policies, Action onViewDetail)
        {
            this.policies = policies;
            this.onViewDetail = onViewDetail;
            AddPolicyView();
        }

        public void SetSubmitDelegate(Action<GameObject, bool, Dictionary<string, GppLegalModel>> onSubmit)
        {
            onSubmitDelegate = onSubmit;
            AddSubmitDelegateToStartButton();
            AddSubmitDelegateToAllSubmitButton();
        }

        public void SetLoginAnotherAccountDelegate(Action<GameObject> onLoginAnotherAccount)
        {
            isEnableLoginAnotherAccount = true;
            onLoginAnotherAccountDelegate = onLoginAnotherAccount;
            AddSubmitDelegateToLoginAnotherAccointButton();
        }

        private void AddSubmitDelegateToStartButton()
        {
            GetStartButton().onClick.RemoveAllListeners();
            GetStartButton().onClick.AddListener(() =>
                {
                    if (IsAllAcceptedMandatoryPolicy())
                    {
                        onSubmitDelegate(gameObject, true, policies);
                    }
                }
            );
        }

        private void AddSubmitDelegateToAllSubmitButton()
        {
            GetAllSubmitButton().onClick.RemoveAllListeners();
            GetAllSubmitButton().onClick.AddListener(() =>
                {
                    foreach (var policy in policies)
                    {
                        policy.Value.IsAccepted = true;
                    }
                    onSubmitDelegate(gameObject, true, policies);
                }
            );
        }

        private void AddSubmitDelegateToLoginAnotherAccointButton()
        {
            EnableLoginAnotherAccountButton(isEnableLoginAnotherAccount);
            GetLoginOtherAccountButton().onClick.RemoveAllListeners();
            GetLoginOtherAccountButton().onClick.AddListener(() =>
                {
                    if (isEnableLoginAnotherAccount)
                        onLoginAnotherAccountDelegate(this.gameObject);
                });
        }

        private void AddPolicyView()
        {
            GameObject policyBox = GppUtil.FindChildWithName(CurrentLayout, "PolicyBox");
            if (policyBox.transform.childCount > 0)
            {
                GppUtil.DeleteChildren(policyBox.transform);
            }
            foreach (var policy in policies)
            {
                GameObject gppPolicyViewPrefab = Resources.Load<GameObject>(policyViewPath[GetOrientation()]);
                var policyViewInstance = Instantiate(gppPolicyViewPrefab, policyBox.transform);
                var gppPolicyView = policyViewInstance.GetComponent<GppPolicyView>();
                gppPolicyView.Init(policy.Value, OnCheckedPolicy, onViewDetail);
                gppPolicyView.Check(policy.Value.IsAccepted);
            }
        }

        private void OnCheckedPolicy(bool isChecked, GppLegalModel policy)
        {
            policies[policy.PolicyId].IsAccepted = isChecked;
            CheckOnOffStateOfStartButton();
        }

        private void CheckOnOffStateOfStartButton()
        {
            SetEnableStartButtonOfCurrentLayout(IsAllAcceptedMandatoryPolicy());
        }

        private void SetEnableStartButtonOfCurrentLayout(bool isEnable)
        {
            Button startButton = GetStartButton();
            startButton.interactable = isEnable;
        }

        private void EnableLoginAnotherAccountButton(bool isEnable)
        {
            GppUtil.FindChildWithName(CurrentLayout, "Content").GetComponent<VerticalLayoutGroup>().padding.bottom = isEnable ? 220 : 120;
            GetLoginOtherAccountButton().gameObject.SetActive(isEnable);
        }

        private Button GetStartButton()
        {
            return GppUtil.FindChildWithName(CurrentLayout, "StartButton").GetComponent<GppCommonButton>();
        }

        private Button GetAllSubmitButton()
        {
            return GppUtil.FindChildWithName(CurrentLayout, "AgreeAll").GetComponent<GppCommonButton>();
        }

        private Button GetLoginOtherAccountButton()
        {
            return GppUtil.FindChildWithName(CurrentLayout, "LoginAnotherAccount").GetComponent<Button>();
        }

        public bool IsAllAcceptedMandatoryPolicy()
        {
            return !policies.Values.Any(policy => policy.IsMandatory && !policy.IsAccepted);
        }
    }
}