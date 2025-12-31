using System;

namespace Gpp.CommonUI.Toast
{
    public interface IGppToastUI
    {
        /// <summary>
        /// Toast 메세지 정보가 전달 됩니다.
        /// Provides the toast message information.
        /// </summary>
        public void SetToastMessage(GppToastMessage toastMessage);
        
        /// <summary>
        /// 닫기 버튼 선택 시 호출되어야 하는 Delegate가 전달 됩니다.
        /// Passes the delegate to be called when the close button is selected.
        /// </summary>
        public void SetOnClickCloseListener(Action onClose);
    }
}