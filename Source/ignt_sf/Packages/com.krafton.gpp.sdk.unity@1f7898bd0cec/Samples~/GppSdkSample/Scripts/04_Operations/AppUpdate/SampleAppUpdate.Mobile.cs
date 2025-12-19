using Gpp;
using Gpp.Models;
using UnityEngine;

namespace GppSample
{
    public class SampleAppUpdate : MonoBehaviour
    {
        #region Step 1. 앱 업데이트 확인

        /// <summary>
        /// Gpp AdminPortal에 등록된 업데이트 정보를 조회합니다.
        /// Retrieves update information registered in the Gpp AdminPortal.
        /// </summary>
        public void CheckAppUpdateWithUI()
        {
            GppSDK.CheckAppUpdate(result =>
                {
                    if (!result.IsError)
                    {
                        Debug.Log($"User Action : {result.Value.UserActionType.ToString()}");
                        switch (result.Value.UserActionType)
                        {
                            case DcsModels.AppUpdateUserAction.MandatoryCancel:
                                // 필수 업데이트 취소를 했기 때문에 게임 종료를 해야 합니다.
                                // Mandatory update was canceled, so the game needs to exit.
                                break;
                            case DcsModels.AppUpdateUserAction.OptionalLater:
                                // 선택 업데이트를 연기했습니다. 특별한 처리가 필요하지 않고 게임이 진행됩니다.
                                // Optional update was postponed; no special action is needed, and the game will proceed.
                                break;
                        }
                    }
                    else
                    {
                        Debug.Log($"CheckAppUpdate Failed Code: {result.Error.Code}");
                        Debug.Log($"CheckAppUpdate Failed Message: {result.Error.Message}");
                        // 업데이트 확인에 실패했습니다. 게임을 계속 진행하는 것을 추천합니다.
                        // Update check failed. It is recommended to continue with the game.
                    }
                }
            );
        }

        public void CheckAppUpdateWithoutUI()
        {
            GppSDK.CheckAppUpdate(result =>
                {
                    if (!result.IsError)
                    {
                        Debug.Log($"Update available? : {result.Value.UpdateAvailable}");
                        Debug.Log($"Mandatory? : {result.Value.Mandatory}");
                        Debug.Log($"ShowOptionalUpdate? : {result.Value.ShowOptionalUpdate}");
                        Debug.Log($"Message : {result.Value.Message}");
                        Debug.Log($"Url : {result.Value.Url}");
                        Debug.Log($"Latest version : {result.Value.LatestVersion}");

                        // 위의 업데이트 정보를 활용하여 업데이트 팝업을 노출합니다.
                    }
                    else
                    {
                        Debug.Log($"CheckAppUpdate Failed Code: {result.Error.Code}");
                        Debug.Log($"CheckAppUpdate Failed Message: {result.Error.Message}");
                    }
                }, false
            );
        }

        #endregion
    }
}