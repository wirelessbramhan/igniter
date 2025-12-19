using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace ignt.sports.cricket.network
{
    [CreateAssetMenu(fileName = "AuthData", menuName = "NetworkManager/Services/Login/Authentication")]
    public class AuthenticationSO : ScriptableObject, IAuthData
    {
        public AuthData authData;
        public bool isSuccess;

        public virtual bool SaveData()
        {
            Save(authData);
            return true;
        }

        public virtual object LoadData()
        {
            return Load<AuthData>();
        }
        

        public virtual void InitializeService()
        {
            //throw new NotImplementedException();
        }

        public event Action OnAuthSuccess;

        public AuthData AuthData
        {
            get
            {
                return authData ??= new AuthData();
            }
            set => authData = value;
        }

        public bool IsSuccess
        {
            get => isSuccess;
            set => isSuccess = value;
        }
        public virtual void Authenticate()
        {
            //throw new NotImplementedException();
        }

        protected void OnAuthSuccessInvoke()
        {
            OnAuthSuccess?.Invoke();
        }

        public bool Save<T>(T data)
        {
            var authDataString = JsonConvert.SerializeObject(data, Formatting.Indented);
            var filepath = Application.persistentDataPath + "/authdata.json";
            Debug.Log($"Saved data on path : {filepath}");
            File.WriteAllText(filepath, authDataString);
            return true;
        }

        public object Load<T>()
        {
            var filepath = Application.persistentDataPath + "/authdata.json";

            if (!File.Exists(filepath))
            {
                Debug.Log($"Save data does not exist on path : {filepath}");
                return null;
            }

            Debug.Log($"Save data exists on path : {filepath}");

            var jsonString = File.ReadAllText(filepath);
            var data = JsonConvert.DeserializeObject<T>(jsonString);


            return data;
        }
    }
}