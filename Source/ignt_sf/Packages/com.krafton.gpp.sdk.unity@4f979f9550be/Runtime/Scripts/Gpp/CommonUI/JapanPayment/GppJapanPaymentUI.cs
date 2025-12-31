using System;
using System.Collections.Generic;
using System.Linq;
using Gpp.Extension;
using Gpp.Localization;
using Gpp.Models;
using Gpp.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gpp.CommonUI.JapanPayment
{
    public class GppJapanPaymentUI : GppCommonUI, IGppJapanPaymentUI
    {
        private ViolationRule _violationRule;
        private Action<GameObject, string> _onConfirm;
        private Action<GameObject> _onCancel;

        protected override void OnChangedOrientation(ScreenOrientation orientation)
        {
            base.OnChangedOrientation(orientation);
            InitComponents();
        }

        public void SetViolationRule(ViolationRule rule)
        {
            _violationRule = rule;
            InitComponents();
        }

        public void SetOnClickConfirmListener(Action<GameObject, string> onConfirm)
        {
            _onConfirm = onConfirm;
        }

        public void SetOnClickCancelListener(Action<GameObject> onCancel)
        {
            _onCancel = onCancel;
        }

        private void InitComponents()
        {
            SetTitle(_violationRule.title);
            SetMessage(_violationRule.description);
            SetRadioButtons(_violationRule.radioButtons);
            SetCloseButton(_violationRule.buttons[0]);
            SetCancelButton();
        }

        private void SetCloseButton(CommonUIButton violationRuleButton)
        {
            var confirmButton = GppUtil.FindChildWithName<GppCommonButton>(CurrentLayout, "Confirm");
            var closeButtonLabel = GppUtil.FindChildWithName<TextMeshProUGUI>(confirmButton.gameObject, "Text");
            closeButtonLabel.text = violationRuleButton.title;
            confirmButton.onClick.AddListener(() =>
                {
                    var radioButtonGroup = GetRadioButtonGroup();
                    var toggle = radioButtonGroup.GetFirstActiveToggle();
                    var label = GppUtil.FindChildWithName<TextMeshProUGUI>(toggle.gameObject, "Label");
                    var selectedValue = _violationRule.radioButtons.First(button => button.title.Equals(label.text)).value;
                    _onConfirm?.Invoke(gameObject, selectedValue);
                }
            );
        }
        
        private void SetCancelButton()
        {
            var cancelButton = GppUtil.FindChildWithName<GppCommonButton>(CurrentLayout, "Cancel");
            var cancelButtonLabel = GppUtil.FindChildWithName<TextMeshProUGUI>(cancelButton.gameObject, "Text");
            cancelButtonLabel.text = LocalizationKey.GeneralCancel.Localise("ja");
            cancelButton.onClick.AddListener(() =>
                {
                    _onCancel?.Invoke(gameObject);
                }
            );
        }

        private void SetRadioButtons(IEnumerable<CommonRadioButton> violationRuleRadioButtons)
        {
            var radioButtonGroup = GetRadioButtonGroup();
            if (radioButtonGroup.transform.childCount > 1)
            {
                return;
            }

            var ageButton = GppUtil.FindChildWithName<Toggle>(radioButtonGroup.gameObject, "AgeButtonTemplate");
            foreach (var violationRuleRadioButton in violationRuleRadioButtons)
            {
                var ageBtnIns = Instantiate(ageButton, radioButtonGroup.transform);
                ageBtnIns.gameObject.SetActive(true);
                var label = GppUtil.FindChildWithName<TextMeshProUGUI>(ageBtnIns.gameObject, "Label");
                label.text = violationRuleRadioButton.title;
            }

            radioButtonGroup.transform.GetChild(0).GetComponent<Toggle>().SetIsOnWithoutNotify(true);
        }

        private ToggleGroup GetRadioButtonGroup()
        {
            return GppUtil.FindChildWithName<ToggleGroup>(CurrentLayout, "RadioButtonGroup");
        }

        private void SetTitle(string violationRuleTitle)
        {
            var title = GppUtil.FindChildWithName<TextMeshProUGUI>(CurrentLayout, "Title");
            title.text = violationRuleTitle;
        }

        private void SetMessage(string violationRuleMessage)
        {
            var message = GppUtil.FindChildWithName<TextMeshProUGUI>(CurrentLayout, "Message");
            message.text = violationRuleMessage;
        }
    }
}