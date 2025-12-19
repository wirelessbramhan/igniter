using System;
using UnityEngine;

namespace Gpp.CommonUI.PcAccountCheck
{
    public interface IGppPcAccountCheckUI
    {
        /// <summary>
        /// 계정 상태 확인을 위한 WebPage의 주소를 전달 드립니다. QR 이미지로 생성하여 노출해주세요.
        /// Provides the URL of the webpage for account status verification. Please display it as a QR code image.
        /// </summary>
        public void SetRedirectUri(string redirectUrl, int expiresIn);
        
        /// <summary>
        /// 취소 버튼 선택 시 호출해야 하는 Delegate를 전달 드립니다.
        /// Passes the delegate to be called when the cancel button is selected.
        /// </summary>
        public void SetOnClickCancel(Action<GameObject> onCancel);

        /// <summary>
        /// 확인 버튼 선택 시 호출해야 하는 Delegate를 전달 드립니다.
        /// Passes the delegate to be called when the check button is selected.
        /// </summary>
        public void SetOnClickCheck(Action<GameObject> onCheck);

        /// <summary>
        /// 인증 코드가 만료되었을 때 호출되어야 하는 Delegate를 전달 드립니다.
        /// Passes the delegate to be called when the authentication code expires.
        /// </summary>
        public void SetOnExpireListener(Action<GameObject> callback);
    }
}