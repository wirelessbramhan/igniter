using System;
using System.Collections;
using System.Collections.Generic;
using Gpp.Auth;
using Gpp.Constants;
using Gpp.Models;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using Gpp.Utils;

namespace Gpp.CommonUI.Login
{
    public class GppSelectMainGameUserIdUI : MonoBehaviour, IGppSelectMainGameUserIdUI
    {
        [SerializeField]
        private Button selectButton;
        [SerializeField]
        private Button cancelButton;
        [SerializeField]
        private Toggle setMainToggle;
        [SerializeField]
        private GameObject scrollContent;
        [SerializeField]
        private ScrollRect scrollRect;

        private Action<GameObject> _cancelEvent;
        private Action<SelectMainGameUserIdParam, GameObject> _onSelect;
        private SelectMainGameUserIdParam _param;
#if UNITY_GAMECORE_SCARLETT || UNITY_PS5
        private GameObject prevSelected;
#endif
        private Coroutine scrollCoroutine;
        private const string ItemPath = "GppCommonUI/Common/GppSelectMainGameUserIdItem";
        
        public void Init(MultipleGameUserResult result, Action<SelectMainGameUserIdParam, GameObject> onSelect, Action<GameObject> onCancel)
        {
#if UNITY_GAMECORE_SCARLETT || UNITY_PS5
            prevSelected = EventSystem.current.currentSelectedGameObject;
#endif

            _cancelEvent = onCancel;
            _onSelect = onSelect;
            _param = new SelectMainGameUserIdParam();
            _param.isMain = setMainToggle.isOn;
            _param.accessToken = result.AccessToken;

            var buttonList = new List<Button>();
            GameObject userInfoItem = Resources.Load<GameObject>(ItemPath);
            foreach (var userInfo in result.ErrorBody)
            {
                var item = Instantiate(userInfoItem, scrollContent.transform);
                buttonList.Add(item.GetComponent<Button>());
#if UNITY_GAMECORE_SCARLETT || UNITY_PS5
                item.GetComponent<GppSelectMainGameUserIdItem>().Init(userInfo.Key, userInfo.Value, OnPolicyFocus);
#else
                item.GetComponent<GppSelectMainGameUserIdItem>().Init(userInfo.Key, userInfo.Value);
#endif
                var button = item.GetComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    _param.userId = userInfo.Key;
                    _param.userInfo = userInfo.Value;
#if UNITY_GAMECORE_SCARLETT || UNITY_PS5
                    OnClickSelectButton();
#else
                    EventSystem.current.SetSelectedGameObject(item);
                    selectButton.interactable = true;
#endif
                });
            }

            for (int idx = 0; idx < buttonList.Count; ++idx)
            {
                Button button = buttonList[idx];
                Navigation nav = new Navigation();
                nav.mode = Navigation.Mode.Explicit;

                nav.selectOnUp = (idx > 0) ? buttonList[idx - 1] : null;
                nav.selectOnDown = (idx < buttonList.Count - 1) ? buttonList[idx + 1] : setMainToggle;
                nav.selectOnLeft = null;
                nav.selectOnRight = null;

                button.navigation = nav;
            }

            Navigation toggleNav = new Navigation();
            toggleNav.mode = Navigation.Mode.Explicit;
            toggleNav.selectOnUp = buttonList.Last();
            toggleNav.selectOnDown = null;
            toggleNav.selectOnLeft = null;
            toggleNav.selectOnRight = selectButton;
            setMainToggle.navigation = toggleNav;
            setMainToggle.onValueChanged.AddListener(value => {
                selectButton.interactable = false;
            });

            Navigation selectNav = new Navigation();
            selectNav.mode = Navigation.Mode.Explicit;
            selectNav.selectOnUp = buttonList.Last().GetComponent<Button>();
            selectNav.selectOnDown = null;
            selectNav.selectOnLeft = null;
            selectNav.selectOnRight = selectButton;
            selectButton.navigation = selectNav;
            selectButton.onClick.AddListener(OnClickSelectButton);
            selectButton.interactable = false;
            cancelButton.onClick.AddListener(OnClickCancelButton);
#if UNITY_GAMECORE_SCARLETT || UNITY_PS5
            EventSystem.current.SetSelectedGameObject(buttonList.First().gameObject);
#endif            
        }
        
        public void OnClickSelectButton()
        {
            if (_param.userInfo == null)
                return;
            _param.isMain = setMainToggle.isOn;
            _onSelect(_param, gameObject);
#if UNITY_GAMECORE_SCARLETT || UNITY_PS5
            EventSystem.current.SetSelectedGameObject(prevSelected);
#endif
        }

        public void OnClickCancelButton()
        {
            _cancelEvent(gameObject);
#if UNITY_GAMECORE_SCARLETT || UNITY_PS5
            EventSystem.current.SetSelectedGameObject(prevSelected);
#endif
        }

        private void OnPolicyFocus(RectTransform target)
        {
            EnsureVisible(target);
        }
        private void EnsureVisible(RectTransform target)
        {
            RectTransform viewport = scrollRect.viewport;

            float viewportHeight = viewport.rect.height;
            float contentHeight = scrollRect.content.rect.height;

            Vector3[] itemCorners = new Vector3[4];
            target.GetWorldCorners(itemCorners);
            Vector3[] viewportCorners = new Vector3[4];
            viewport.GetWorldCorners(viewportCorners);

            float itemTop = scrollRect.content.InverseTransformPoint(itemCorners[1]).y;
            float itemBottom = scrollRect.content.InverseTransformPoint(itemCorners[0]).y;
            float viewportTop = scrollRect.content.InverseTransformPoint(viewportCorners[1]).y;
            float viewportBottom = scrollRect.content.InverseTransformPoint(viewportCorners[0]).y;

            if (itemTop > viewportTop)
            {
                float offset = itemTop - viewportTop;
                float normalizedOffset = offset / (contentHeight - viewportHeight);
                scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition + normalizedOffset);
            }
            else if (itemBottom < viewportBottom)
            {
                float offset = viewportBottom - itemBottom;
                float normalizedOffset = offset / (contentHeight - viewportHeight);
                scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition - normalizedOffset);
            }

            if (scrollCoroutine != null)
            {
                StopCoroutine(scrollCoroutine);
            }
        }
    }
}