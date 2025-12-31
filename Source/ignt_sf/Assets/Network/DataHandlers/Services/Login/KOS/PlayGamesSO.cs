using Gpp;
using Gpp.Constants;
using Gpp.Core;
using ignt.sports.cricket.core;
using UnityEngine;

namespace ignt.sports.cricket.network
{
    [CreateAssetMenu(fileName = "PlayGamesSO", menuName = "NetworkManager/Services/PlayGamesSO")]
    public class PlayGamesSO:AuthenticationSO
    {

        public override void Authenticate()
        {
            Debug.Log("PlayGamesSO Authenticate");
            GppSDK.Login(result =>
                {
                    if (!result.IsError)
                    {
                        // 로그인에 성공했으므로 게임에 진입합니다.
                        // Login is successful, proceed to enter the game.
                    }
                    else
                    {
                        Debug.Log($"Login Failed Code : {result.Error.Code}");
                        Debug.Log($"Login Failed Message : {result.Error.Message}");
                        // 로그인에 실패했으므로 유저에게 로그인 실패 팝업을 노출하고 재로그인을 유도합니다.
                        // Login failed, prompt the user with a login failure popup and suggest re-login.
                    }
                }
            );


        }
        
        
    }
}
