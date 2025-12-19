using System;
using Gpp.Models;
using UnityEngine;

namespace Gpp.CommonUI.Console
{
    public interface IGppConsoleLoginUI 
    {
        /// <summary>
        /// 로그인 화면 갱신에 필요한 정보를 전달 드립니다.
        /// Provides the information needed to refresh the login screen.
        /// </summary>
        public void RefreshLoginUIData(PcConsoleAuthResult data);

        /// <summary>
        /// cancel 버튼 선택 시 호출 되어야 하는 Delegate를 전달 드립니다.
        /// Passes the delegate to be called when the cancel button is selected.
        /// </summary>
        public void SetOnClickCancelListener(Action<GameObject> onClose);

        /// <summary>
        /// 인증 코드가 만료되었을 때 호출되어야 하는 Delegate를 전달 드립니다.
        /// Passes the delegate to be called when the authentication code expires.
        /// </summary>
        public void SetOnExpireListener(Action<GameObject> callback);
    }
}