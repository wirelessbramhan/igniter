using System;
using Gpp.Models;
using Gpp.Utils;
using TMPro;
using UnityEngine;

namespace Gpp.CommonUI.GPGS
{
    public class GppGPGSPopupUI : GppCommonUI, IGppGPGSPopupUI
    {
        private Action<GameObject, bool> _onClickListener;
        private string _email;

        protected override void OnChangedOrientation(ScreenOrientation orientation)
        {
            base.OnChangedOrientation(orientation);
            SetMessage();
        }

        public void SetKidInfo(KidInfoResult kidInfo)
        {
            _email = string.IsNullOrEmpty(kidInfo.KidInfo.Email) ? kidInfo.KidInfo.KraftonTag : kidInfo.KidInfo.Email;
        }

        public void SetOnClickListener(Action<GameObject, bool> onClick)
        {
            _onClickListener = onClick;
        }

        protected override void Start()
        {
            base.Start();
            SetMessage();
        }

        private void SetMessage()
        {
            var email = GppUtil.FindChildWithName<TextMeshProUGUI>(CurrentLayout, "Email");
            email.text = _email;
        }

        public void OnClickButton(bool isConfirm)
        {
            _onClickListener?.Invoke(gameObject, isConfirm);
        }
    }
}