using System;
using Gpp.Utils;
using TMPro;
using UnityEngine;

using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Gpp.CommonUI.Modal
{
    public class GppModalUI : GppCommonUI, IGppModalUI
    {
        private GppModalData _modalData;
        private GameObject prevSelected;

        protected override void OnChangedOrientation(ScreenOrientation orientation)
        {
            base.OnChangedOrientation(orientation);
            InitComponents();
        }

        protected override void Start()
        {
            base.Start();
            if (!GppSDK.GetInputController().IsGamePad())
                return;

            prevSelected = EventSystem.current.currentSelectedGameObject;
            GameObject buttonGroup;
            if (_modalData.GetButtonDirection() == GppModalUIButtonDirection.VERTICAL)
            {
                buttonGroup = GetButtonGroup(true);
            }
            else if (_modalData.GetButtonDirection() == GppModalUIButtonDirection.HORIZONTAL)
            {
                buttonGroup = GetButtonGroup(false);
            }
            else
            {
                buttonGroup = GetButtonGroup(GetOrientation() == ScreenOrientation.Portrait);
            }
            EventSystem.current.SetSelectedGameObject(GppUtil.FindChildWithName(buttonGroup, "Confirm"));
        }

        protected override void Update()
        {
            base.Update();
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _modalData?.GetBackButtonAction()?.Invoke(gameObject);
            }
        }

        private void InitComponents()
        {
            if (_modalData == null)
            {
                return;
            }

            BindingSortOrder();
            BindingTitleText();
            BindingMessageText();
            BindingNegativeButton();
            BindingPositiveButton();
        }

        public void SetModalData(GppModalData data)
        {
            _modalData = data;
            InitComponents();
        }

        private void BindingSortOrder()
        {
            CurrentLayout.GetComponent<Canvas>().sortingOrder = _modalData.GetUIPriority();
        }

        private void BindingTitleText()
        {
            TextMeshProUGUI title = GppUtil.FindChildWithName<TextMeshProUGUI>(CurrentLayout, "Title");
            title.text = _modalData.GetTitle();
        }

        private void BindingMessageText()
        {
            TextMeshProUGUI message = GppUtil.FindChildWithName<TextMeshProUGUI>(CurrentLayout, "Message");
            message.text = _modalData.GetMessage();
        }

        private void BindingNegativeButton()
        {
            GameObject buttonGroup;
            if (_modalData.GetButtonDirection() == GppModalUIButtonDirection.VERTICAL)
            {
                buttonGroup = GetButtonGroup(true);
            }
            else if (_modalData.GetButtonDirection() == GppModalUIButtonDirection.HORIZONTAL)
            {
                buttonGroup = GetButtonGroup(false);
            }
            else
            {
                buttonGroup = GetButtonGroup(GetOrientation() == ScreenOrientation.Portrait);
            }

            string buttonText = _modalData.GetNegativeButtonText();
            Action<GameObject> onClick = _modalData.GetNegativeAction();
            GameObject negativeButton = GppUtil.FindChildWithName(buttonGroup, "Cancel");
            if (string.IsNullOrEmpty(buttonText) || onClick == null)
            {
                negativeButton.SetActive(false);
                return;
            }

            negativeButton.GetComponentInChildren<TextMeshProUGUI>().text = buttonText;
            negativeButton.GetComponent<GppCommonButton>().onClick.AddListener(() =>
            {
                if (GppSDK.GetInputController().IsGamePad())
                    EventSystem.current.SetSelectedGameObject(prevSelected);

                onClick(gameObject);
            });
        }

        private void BindingPositiveButton()
        {
            GameObject buttonGroup;
            if (_modalData.GetButtonDirection() == GppModalUIButtonDirection.VERTICAL)
            {
                buttonGroup = GetButtonGroup(true);
            }
            else if (_modalData.GetButtonDirection() == GppModalUIButtonDirection.HORIZONTAL)
            {
                buttonGroup = GetButtonGroup(false);
            }
            else
            {
                buttonGroup = GetButtonGroup(GetOrientation() == ScreenOrientation.Portrait);
            }

            string buttonText = _modalData.GetPositiveButtonText();
            Action<GameObject> onClick = _modalData.GetPositiveAction();
            var positiveButton = GppUtil.FindChildWithName(buttonGroup, "Confirm");
            positiveButton.GetComponentInChildren<TextMeshProUGUI>().text = buttonText;
            positiveButton.GetComponent<GppCommonButton>().onClick.AddListener(() =>
            {
                if (GppSDK.GetInputController().IsGamePad())
                    EventSystem.current.SetSelectedGameObject(prevSelected);

                onClick.Invoke(gameObject);
            });
        }

        private GameObject GetButtonGroup(bool isVertical)
        {
            GameObject buttonGroup;
            if (isVertical)
            {
                buttonGroup = GppUtil.FindChildWithName(CurrentLayout, "VerticalButtons");
                buttonGroup.SetActive(true);
                GppUtil.FindChildWithName(CurrentLayout, "HorizontalButtons").SetActive(false);
            }
            else
            {
                buttonGroup = GppUtil.FindChildWithName(CurrentLayout, "HorizontalButtons");
                buttonGroup.SetActive(true);
                GppUtil.FindChildWithName(CurrentLayout, "VerticalButtons").SetActive(false);
            }

            return buttonGroup;
        }
    }
}