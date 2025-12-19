using System;
using UnityEngine;

namespace Gpp.CommonUI.Console
{
    public interface IGppConsoleMaintenanceUI
    {
        /// <summary>
        /// GPP 점검 정보를 전달 드립니다.
        /// Provides GPP maintenance information.
        /// </summary>
        public void SetMaintenanceData(Models.Maintenance maintenance);

        /// <summary>
        /// 점검 화면의 확인 버튼을 선택시 호출 되어야 하는 Delegate를 전달 드립니다.
        /// Passes the delegate to be called when the confirm button on the maintenance screen is selected.
        /// </summary>
        public void SetOnClickConfirmListener(Action<GameObject> onConfirm);
    }
}