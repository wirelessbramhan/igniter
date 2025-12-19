using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace ignt.sports.cricket.core
{
    [CreateAssetMenu(fileName = "SaveAndLoadSO", menuName = "Handlers/SaveAndLoadSO")]
    public class FileHandlerSOBase<T1> : ScriptableObject where T1 : class
    {
        public bool Save<T>(T data)
        {
            var authDataString = JsonConvert.SerializeObject(data, Formatting.Indented);
            var filepath = Application.persistentDataPath + "/"+typeof(T) +".json";
            Debug.Log($"Saved data on path : {filepath}");
            File.WriteAllText(filepath, authDataString);
            return true;
        }

        public object Load<T>()
        {
            var filepath = Application.persistentDataPath + "/"+typeof(T) +".json";

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
        
        protected virtual bool SaveAuthData(T1 data)
        {
            return Save(data);
        }

        protected virtual T1 LoadAuthData()
        {
            return Load<T1>() as T1;
        }
    }
}
