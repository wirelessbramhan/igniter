using System;
using Gpp.Models;
using UnityEngine;

namespace Gpp.CommonUI.GPGS
{
    public interface IGppGPGSPopupUI
    {
        /// <summary>
        /// 화면에 표시할 유저 정보를 전달 드립니다.
        /// Provides user information to be displayed on the screen.
        /// </summary>
        public void SetKidInfo(KidInfoResult kidInfo);
        
        /// <summary>
        /// 확인 버튼을 선택 시 호출되어야 하는 Delegate를 전달 드립니다.
        /// Passes the delegate to be called when the confirm button is selected.
        /// </summary>
        public void SetOnClickListener(Action<GameObject, bool> onClick);
    }
}