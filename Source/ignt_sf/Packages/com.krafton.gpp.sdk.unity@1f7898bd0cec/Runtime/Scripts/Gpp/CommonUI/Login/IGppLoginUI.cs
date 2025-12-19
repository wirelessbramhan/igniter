using System;
using Gpp.Constants;
using UnityEngine;

namespace Gpp.CommonUI.Login
{
    public interface IGppLoginUI
    {
        /// <summary>
        /// 각 플랫폼 별 로그인 버튼 선택 시 호출되어야 할 Delegate를 인자로 전달 드립니다.
        /// Passes the delegate to be called when the login button for each platform is selected.
        /// </summary>
        public void SetOnClickLoginListener(Action<GameObject, PlatformType> onLogin);
        
        /// <summary>
        /// 로그인 화면의 닫기 버튼 선택 시 호출되어야 할 Delegate를 인자로 전달 드립니다.
        /// Passes the delegate to be called when the close button on the login screen is selected.
        /// </summary>
        public void SetOnClickCloseListener(Action<GameObject> onClose);

        public void SetUnusedPlatformType(PlatformType[] platformTypes)
        {
            
        }
    }
}