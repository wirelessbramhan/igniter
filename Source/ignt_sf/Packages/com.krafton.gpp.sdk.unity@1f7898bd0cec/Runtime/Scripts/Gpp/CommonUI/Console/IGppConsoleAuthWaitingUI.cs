using System;
using UnityEngine;

namespace Gpp.CommonUI.Console
{
    public interface IGppConsoleAuthWaitingUI
    {
        /// <summary>
        /// Account Check or Login 결과 대기 여부를 전달 드립니다.
        /// Indicates whether to wait for the result of account check or login.
        /// </summary>
        public void SetIsAccountCheck(bool isAccountCheck);

        /// <summary>
        /// 취소 버튼 선택 시 호출 되어야 하는 Delegate를 전달 드립니다.
        /// Passes the delegate to be called when the cancel button is selected.
        /// </summary>
        public void SetOnClickCancel(Action<GameObject> onCancel);
    }
}