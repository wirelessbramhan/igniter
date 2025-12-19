using System.Collections.Generic;
using Gpp;
using Gpp.Core;
using Gpp.Extensions.IAP.Models;
using Gpp.Models;
using UnityEngine;

namespace GppSample
{
    public class SampleSettings : MonoBehaviour, IGppEventListener
    {
        #region Step 1. GppEventListener (Optional)

        /// <summary>
        /// 토큰에 문제가 있을 경우 호출되는 메소드 입니다. (토큰 만료, 비정상 토큰, 토큰 갱신 실패)
        /// This method is called when there is an issue with the token (e.g., token expiration, invalid token, token refresh failure).
        /// </summary>
        public void NotifyBrokenToken()
        {
            Debug.Log("Received BrokenToken Event.");
            // 유저에게 유효하지 않은 토큰 팝업을 노출하고 게임 종료 또는 재로그인을 유도합니다.
            // Displays a popup indicating an invalid token to the user and prompts them to either exit the game or re-login.
        }

        /// <summary>
        /// 중복 세션이 발생했을 경우 호출되는 메소드 입니다.
        /// This method is called when a duplicate session is detected.
        /// </summary>
        public void NotifyDuplicatedSession()
        {
            Debug.Log("Received DuplicatedSession Event.");
            // 유저에게 중복 세션 팝업을 노출하고 게임 종료합니다.
            // Displays a popup indicating a duplicate session to the user and exits the game.
        }
        
        /// <summary>
        /// 게임 외부(KraftonID 웹 같은)에서 계정이 삭제(탈퇴)된 경우 호출되는 메소드 입니다.
        /// This method is called when an account is deleted (withdrawn) outside of the game (such as from the KraftonID website).
        /// </summary>
        public void NotifyDeletedAccount()
        {
            Debug.Log("Received DeletedAccount Event.");
            // 유저에게 계정 삭제 팝업을 노출하고 게임을 종료합니다.
            // Displays a popup indicating account deletion to the user and exits the game.
        }
        
        /// <summary>
        /// 설문조사 상태가 갱신(설문 완료, 새로운 설문 추가, 설문 삭제 등)되었을 경우 호출되는 메소드 입니다.
        /// This method is called when the survey status is updated (e.g., survey completion, new survey added, survey deletion).
        /// </summary>
        public void NotifyRefreshSurvey(List<Survey> surveyList)
        {
            Debug.Log("Received RefreshSurvey Event.");
            // 갱신된 설문조사 리스트를 참고하여 유저의 설문조사 UI를 업데이트 합니다.
            // Updates the user's survey UI based on the refreshed survey list.
        }

        /// <summary>
        /// 모바일 환경에서 계정 병합을 당한 계정으로 PC 로그인 하는 경우 호출되는 메소드 입니다.
        /// This method is called when attempting to log in to a PC with an account that has been merged in a mobile environment.
        /// </summary>
        public void NotifyMergedAccount(string oldUserId, string newUserId)
        {
            Debug.Log("Received MergedAccount Event.");
            // 유저에게 계정 병합에 의해 로그인 실패 팝업을 노출하고 재로그인을 유도합니다.
            // Displays a popup indicating login failure due to account merging and prompts the user to re-login.
        }

        /// <summary>
        /// (모바일 전용) 게임 외부(GooglePlayPoint, Coupon Code)에서 결제가 발생했을 경우 결제 정보가 전달되는 메소드 입니다.
        /// (Mobile Only) This method is called when a purchase occurs outside the game (e.g., Google Play Points, Coupon Code), and the purchase information is delivered.
        /// </summary>
        public void NotifyExternalPurchase(List<IapPurchase> purchaseList)
        {
            Debug.Log("Received ExternalPurchase Event.");
            // 유저에게 외부 결제 알림 팝업을 노출합니다.
            // Displays a popup notifying the user of an external purchase.
        }

        /// <summary>
        /// (모바일 전용) 복구된 결제 정보가 전달되는 메소드 입니다.
        /// (Mobile Only) This method is called when restored purchase information is delivered.
        /// </summary>
        public void NotifyRestoredPurchase(List<IapPurchase> purchaseList)
        {
            Debug.Log("Received RestoredPurchase Event.");
            // 유저에게 복구된 일반 결제 알림 팝업을 노출합니다.
            // Displays a popup notifying the user of a restored purchase.
        }

        /// <summary>
        /// (Steam, Xbox 전용) 유저의 자격 정보가 전달되는 메소드 입니다.
        /// (Steam, Xbox Only) This method is called when user entitlements are delivered.
        /// </summary>
        /// <param name="entitlements"></param>
        public void NotifyOwnerShipUpdated(Entitlements entitlements)
        {
            Debug.Log("Received NotifyOwnerShipUpdated Event.");
            // 유저의 자격 변화에 따른 게임 처리가 필요합니다.
            // Game processing is required based on the user's entitlement change.       '
        }

        /// <summary>
        /// KPS 결제의 결과가 전달되는 메소드입니다.
        /// Method to receive the result of KPS checkout.
        /// </summary>
        /// <param name="checkoutResult"></param>
        public void NotifyCheckoutResult(CheckoutResult checkoutResult)
        {
            Debug.Log($"Received NotifyCheckoutResult Event. Result : {checkoutResult.ResultCode}");
            // KPS 결제 결과에 따라 유저에게 알림을 보여줍니다.
            // Displays an alert to the user based on the KPS checkout result.
        }

        #endregion

        #region Step 2. Initialize

        /// <summary>
        /// GPP SDK를 초기화 합니다.
        /// Initializes the GPP SDK.
        /// </summary>
        public void Initialize()
        {
            GppOptions options = new GppOptions.Builder()
                .SetLanguageCode("ko") // 게임 언어를 설정합니다. Sets the game language.
                // .SetEnableGameServer(true) // KOS에서 제공하는 게임 서버 ID를 설정해야 하는 경우, 이 API를 사용하여 설정할 수 있습니다.
                                           // If you need to set the game server ID provided by KOS, you can do so using this API.
                .Build();
            
            GppSDK.Initialize(result =>
                {
                    if (!result.IsError)
                    {
                        Debug.Log($"Init Success!");
                        // 초기화를 성공했으므로 GppSDK.Login()을 호출할 수 있습니다.
                        // Initialization succeeded, allowing GppSDK.Login() to be called.
                        
                        // 로그인 전에 게임 서버 ID가 설정이 필요한 경우, SetGameServerIdBeforeLogin API를 사용하여 설정할 수 있습니다.
                        // If you need to set the game server ID before logging in, you can do so using the SetGameServerIdBeforeLogin API.
                        // GppSDK.SetGameServerIdBeforeLogin("YOUR_GAME_SERVER_ID");
                    }
                    else
                    {
                        Debug.Log($"Init Failed Code : {result.Error.Code}");
                        Debug.Log($"Init Failed Message : {result.Error.Message}");
                        // 초기화를 실패했으므로 유저에게 초기화 실패 팝업을 노출하고 게임을 종료합니다.
                        // Initialization failed, so display a failure popup to the user and exit the game.
                    }
                }, this, options
            );
        }

        #endregion
        
        #region Step 3. SDK 언어 설정/조회

        /// <summary>
        /// SDK 언어 코드를 설정합니다. 게임 언어가 변경되면 호출해주세요.
        /// Sets the SDK language code. Call this method when the game language changes.
        /// </summary>
        public void SetLanguageCode(string language)
        {
            GppSDK.SetLanguageCode(language);
        }
        
        /// <summary>
        /// SDK 언어 코드를 조회합니다.
        /// </summary>
        public void GetLanguageCode()
        {
            var langCode = GppSDK.GetLanguageCode();
            Debug.Log($"Language Code : {langCode}");
        }

        #endregion

        public void GetDeviceId()
        {
            Debug.Log($"Device Id : {GppSDK.GetDeviceId()}");            
        }
    }
}