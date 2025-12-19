using Gpp;
using Gpp.Core;
using ignt.sports.cricket.core;
using UnityEngine;

namespace ignt.sports.cricket.network
{
    [CreateAssetMenu(fileName = "KOSInitialization", menuName = "NetworkManager/Services/KOSInitialization")]
    public class KosSO:ScriptableObject, IService, IGppEventListener
    {
 
        public void InitializeService()
        {
            GppOptions options = new GppOptions.Builder().Build();
             
            GppSDK.Initialize(result =>
                {
                    if (!result.IsError)
                    {
                        Debug.Log($"Init Success!");
                        // 초기화를 성공했으므로 GppSDK.Login()을 호출할 수 있습니다.
                        // Initialization succeeded, allowing GppSDK.Login() to be called.
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
        
    }
}
