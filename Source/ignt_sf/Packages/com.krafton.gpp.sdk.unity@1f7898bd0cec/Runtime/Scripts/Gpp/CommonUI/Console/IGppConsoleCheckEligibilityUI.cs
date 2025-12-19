using System;
using Gpp.Models;
using UnityEngine;

namespace Gpp.CommonUI.Console
{
    public interface IGppConsoleCheckEligibilityUI
    {
        /// <summary>
        /// 계정 상태 확인을 위한 UI에 필요한 정보를 전달 드립니다. URL을 QR 이미지로 생성하여 노출해주세요.
        /// Provides the UI Data for account status verification UI. Please display URL as a QR code image.
        /// </summary>
        public void SetCheckEligibilityData(CheckEligibilityResult checkEligibilityResult);

        /// <summary>
        /// 확인 버튼 선택 시 호출해야 하는 Delegate를 전달 드립니다.
        /// Passes the delegate to be called when the check button is selected.
        /// </summary>
        public void SetOnClickCheck(Action<GameObject> onCheck);

        /// <summary>
        /// 취소 버튼 선택 시 호출해야 하는 Delegate를 전달 드립니다.
        /// Passes the delegate to be called when the cancel button is selected.
        /// </summary>
        public void SetOnClickCancel(Action<GameObject> onCancel);

        /// <summary>
        /// 인증 코드가 만료되었을 때 호출되어야 하는 Delegate를 전달 드립니다.
        /// Passes the delegate to be called when the authentication code expires.
        /// </summary>
        public void SetOnExpireListener(Action<GameObject> callback);
    }
}