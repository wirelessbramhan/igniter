using System;
using System.Collections.Generic;
using System.Linq;
using Gpp.Constants;
using Gpp.Core;
using Gpp.Models;
using Gpp.Telemetry;
using UnityEngine;
using UnityEngine.UI;

namespace Gpp.CommonUI.Legal.PcLegal
{
    public class GppPcLegalUI : GppCommonPcUI, IGppPcLegalUI
    {
        private Dictionary<string, GppLegalModel> policies;
        private Action<GameObject, bool, Dictionary<string, GppLegalModel>> onSubmitDelegate;
        private Action<GppLegalModel> onViewDetail;
        private Action<GameObject> onClose;
        private readonly List<GppPcPolicyView> policyViews = new();

        [SerializeField]
        private Button startButton;

        [SerializeField]
        private Button allAgreeStartButton;

        [SerializeField]
        private Button closeButton;

        [SerializeField]
        private GameObject policesContainer;

        private const string PolicyViewPath = "GppCommonUI/Pc/GppPcPolicyView";

        private Action<GameObject> onCancel;

        public void SetPolicies(Dictionary<string, GppLegalModel> policies, Action<GppLegalModel> onViewDetail)
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
            SetCloseButton();
        }

        public void SetCloseDelegate(Action<GameObject> onClose)
        {
            this.onClose = onClose;
        }

        private void AddSubmitDelegateToStartButton()
        {
            startButton.onClick.AddListener(() =>
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
            allAgreeStartButton.onClick.AddListener(() =>
            {
                policyViews.ForEach(view => view.Check(true));
                onSubmitDelegate(gameObject, true, policies);
            });
        }

        private void SetCloseButton()
        {
            closeButton.onClick.AddListener(() =>
            {
                onClose?.Invoke(gameObject);
            });
        }

        private void AddPolicyView()
        {
            if (policesContainer.transform.childCount > 0)
            {
                return;
            }

            foreach (var policy in policies)
            {
                GameObject gppPolicyViewPrefab = Resources.Load<GameObject>(PolicyViewPath);
                var policyViewInstance = Instantiate(gppPolicyViewPrefab, policesContainer.transform);
                var gppPolicyView = policyViewInstance.GetComponent<GppPcPolicyView>();
                gppPolicyView.Init(policy.Value, OnCheckedPolicy, onViewDetail);
                policyViews.Add(gppPolicyView);
            }
        }

        private void OnCheckedPolicy(bool isChecked, GppLegalModel policy)
        {
            policies[policy.PolicyId].IsAccepted = isChecked;
            CheckOnOffStateOfStartButton();
        }

        private void CheckOnOffStateOfStartButton()
        {
            startButton.interactable = IsAllAcceptedMandatoryPolicy();
        }

        public bool IsAllAcceptedMandatoryPolicy()
        {
            return !policies.Values.Any(policy => policy.IsMandatory && !policy.IsAccepted);
        }

        protected override UIType ConsoleUIType()
        {
            return UIType.ConsoleLegal;
        }

        protected override void UpdateController(InputControllerType prevController, InputControllerType currentController)
        {
            if (GppSDK.GetInputController().IsGamePad())
            {
                GppUI.ShowPcLegal(policies, onSubmitDelegate, onViewDetail, onClose);
                Destroy(gameObject);
            }
        }
    }
}