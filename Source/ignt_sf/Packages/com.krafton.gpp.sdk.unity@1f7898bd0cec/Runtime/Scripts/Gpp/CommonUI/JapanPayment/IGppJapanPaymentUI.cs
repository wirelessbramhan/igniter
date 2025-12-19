using System;
using Gpp.Models;
using UnityEngine;

namespace Gpp.CommonUI.JapanPayment
{
    public interface IGppJapanPaymentUI
    {
        public void SetViolationRule(ViolationRule rule);
        public void SetOnClickConfirmListener(Action<GameObject, string> onConfirm);
        public void SetOnClickCancelListener(Action<GameObject> onCancel);
    }
}