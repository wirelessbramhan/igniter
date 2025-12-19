using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gpp.CommonUI.Console;
using Gpp.Models;
using Gpp.Telemetry;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Gpp.WebSocketSharp;

namespace Gpp.CommonUI.Legal.Console
{
    public class GppConsoleLegalUI : GppConsoleUI, IGppConsoleLegalUI
    {
        private Dictionary<string, GppLegalModel> policies;
        private Action<GameObject, bool, Dictionary<string, GppLegalModel>> onSubmitDelegate;
        private Action<GppLegalModel> onViewDetail;
        private Action<GameObject> onClose;
        private readonly List<GppConsolePolicyView> policyViews = new();

        [SerializeField]
        private RectTransform contentContainer;

        [SerializeField]
        private ScrollRect policyScrollRect;

        [SerializeField]
        private RectTransform policyContent;

        [SerializeField]
        private Button startButton;

        [SerializeField]
        private GameObject showDetailButton;

        [SerializeField]
        private GameObject policesContainer;

        [SerializeField]
        private RectTransform[] contentSizeFitTransform;

        public GppConsoleUIInput uiInput;

        private const string PolicyViewPath = "GppCommonUI/Console/GppConsolePolicyView";

        private Coroutine scrollCoroutine;
        private GppConsoleDefaultFocusable startButtonFocusable;

        public void SetPolicies(Dictionary<string, GppLegalModel> policies, Action<GppLegalModel> onViewDetail)
        {
            this.policies = policies;
            this.onViewDetail = onViewDetail;
            AddPolicyView();
        }

        public void SetSubmitDelegate(Action<GameObject, bool, Dictionary<string, GppLegalModel>> onSubmit)
        {
            onSubmitDelegate = onSubmit;
        }

        public void SetCloseDelegate(Action<GameObject> onClose)
        {
            this.onClose = onClose;
        }

        private void Update()
        {
            for (int i = 0; i < contentSizeFitTransform.Length; i++)
            {
                RefreshLayout(contentSizeFitTransform[i]);
            }
        }

        private void OnStartButtonFocus()
        {
            EventSystem.current.SetSelectedGameObject(startButton.gameObject);
        }
        private void OnStartButton()
        {
            if (IsAllAcceptedMandatoryPolicy())
            {
                startButtonFocusable.SetDefaultActionA(null);
                onSubmitDelegate(gameObject, true, policies);
            }
        }

        private void StartAgreeAll()
        {
            policyViews.ForEach(view => view.Check(true));
            policies.ToList().ForEach(policy => policy.Value.IsAccepted = true);
            uiInput.SetDefaultActionX(null);
            onSubmitDelegate(gameObject, true, policies);
        }

        private void CloseLegalUI()
        {
            uiInput.SetDefaultActionB(null);
            onClose?.Invoke(gameObject);
        }

        private void ShowDetail(GppLegalModel policy)
        {
            if (policy.WebUrl.IsNullOrEmpty())
                return;
            onViewDetail?.Invoke(policy);
        }

        private void OnPolicyFocus(GppLegalModel policy, RectTransform target)
        {
            if (policy.WebUrl.IsNullOrEmpty())
                showDetailButton.SetActive(false);
            else
                showDetailButton.SetActive(true);
            EnsureVisible(target);
        }
        private void OnPolicyDefocus()
        {
            showDetailButton.SetActive(false);
        }

        private void AddPolicyView()
        {
            var focusables = new List<MonoBehaviour>();
            foreach (var policy in policies)
            {
                GameObject gppPolicyViewPrefab = Resources.Load<GameObject>(PolicyViewPath);
                var policyViewInstance = Instantiate(gppPolicyViewPrefab, policesContainer.transform);
                var gppPolicyView = policyViewInstance.GetComponent<GppConsolePolicyView>();
                gppPolicyView.Init(policy.Value, OnCheckedPolicy, OnPolicyFocus, OnPolicyDefocus, ShowDetail);
                focusables.Add(gppPolicyView);
                policyViews.Add(gppPolicyView);
            }
            startButtonFocusable = startButton.GetComponent<GppConsoleDefaultFocusable>();
            startButtonFocusable.SetDefaultActionA(OnStartButton);
            startButtonFocusable.SetDefaultActionFocus(OnStartButtonFocus);
            uiInput.Init(focusables);
            uiInput.SetDefaultActionX(StartAgreeAll);
            uiInput.SetDefaultActionB(CloseLegalUI);
            //LayoutRebuilder.ForceRebuildLayoutImmediate(contentContainer);
        }

        private void OnCheckedPolicy(bool isChecked, GppLegalModel policy)
        {
            policies[policy.PolicyId].IsAccepted = isChecked;
            CheckOnOffStateOfStartButton();
            if (isChecked)
            {
                uiInput.MoveNext();
            }
        }

        private void CheckOnOffStateOfStartButton()
        {
            startButton.interactable = IsAllAcceptedMandatoryPolicy();
            if (startButton.interactable)
                uiInput.AddFocusable(startButtonFocusable);
            else
                uiInput.RemoveFocusable(startButtonFocusable);
        }

        public bool IsAllAcceptedMandatoryPolicy()
        {
            return !policies.Values.Any(policy => policy.IsMandatory && !policy.IsAccepted);
        }

        private void EnsureVisible(RectTransform target)
        {
            RectTransform viewport = policyScrollRect.viewport;

            float viewportHeight = viewport.rect.height;
            float contentHeight = policyScrollRect.content.rect.height;

            Vector3[] itemCorners = new Vector3[4];
            target.GetWorldCorners(itemCorners);
            Vector3[] viewportCorners = new Vector3[4];
            viewport.GetWorldCorners(viewportCorners);

            float itemTop = policyScrollRect.content.InverseTransformPoint(itemCorners[1]).y;
            float itemBottom = policyScrollRect.content.InverseTransformPoint(itemCorners[0]).y;
            float viewportTop = policyScrollRect.content.InverseTransformPoint(viewportCorners[1]).y;
            float viewportBottom = policyScrollRect.content.InverseTransformPoint(viewportCorners[0]).y;

            if (itemTop > viewportTop)
            {
                float offset = itemTop - viewportTop;
                float normalizedOffset = offset / (contentHeight - viewportHeight);
                policyScrollRect.verticalNormalizedPosition = Mathf.Clamp01(policyScrollRect.verticalNormalizedPosition + normalizedOffset);
            }
            else if (itemBottom < viewportBottom)
            {
                float offset = viewportBottom - itemBottom;
                float normalizedOffset = offset / (contentHeight - viewportHeight);
                policyScrollRect.verticalNormalizedPosition = Mathf.Clamp01(policyScrollRect.verticalNormalizedPosition - normalizedOffset);
            }

            if (scrollCoroutine != null)
            {
                StopCoroutine(scrollCoroutine);
            }
        }

        private IEnumerator SmoothScrollTo(float targetPosition)
        {
            float start = policyScrollRect.verticalNormalizedPosition;
            float elapsedTime = 0f;
            float duration = 0.2f; // 스크롤 애니메이션 시간

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                policyScrollRect.verticalNormalizedPosition = Mathf.Lerp(start, targetPosition, t);
                yield return null;
            }

            // 최종 위치 설정
            policyScrollRect.verticalNormalizedPosition = targetPosition;
        }

        private void RefreshLayout(RectTransform target)
        {
            StartCoroutine(ForceRebuildNextFrame(target));
        }

        private IEnumerator ForceRebuildNextFrame(RectTransform target)
        {
            yield return null;
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(target);
        }

        protected override UIType PcUIType()
        {
            return UIType.PcLegal;
        }

        protected override void ChangeToPcUI()
        {
            GppUI.ShowPcLegal(policies, onSubmitDelegate, onViewDetail, onClose);
            Destroy(gameObject);
        }
    }
}