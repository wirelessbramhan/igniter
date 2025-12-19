using System;
using UnityEngine;

namespace ignt.sports.cricket.network
{
    [CreateAssetMenu(fileName = "AuthHandler", menuName = "NetworkManager/AuthHandler")]
    public class AuthHandler : ScriptableObject
    {
        public AuthenticationSO AuthenticationSo;


        public void InitializeService()
        {
            AuthenticationSo.InitializeService();
        }

        public bool Save()
        {
            return AuthenticationSo.SaveData();
        }

        public object Load()
        {
            return AuthenticationSo.LoadData();
        }
        
        public void Authenticate()
        {
            AuthenticationSo.Authenticate();
        }

    }
}
