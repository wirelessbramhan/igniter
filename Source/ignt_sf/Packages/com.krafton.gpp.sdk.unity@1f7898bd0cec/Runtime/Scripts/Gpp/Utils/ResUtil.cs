using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using Gpp.Core;
using UnityEngine;

namespace Gpp.Utils
{
    internal static class ResUtil
    {
        public static T LoadScriptableObject<T>(string filePath) where T : ScriptableObject
        {
            try
            {
                var scriptableObject = Resources.Load<T>(filePath);

                if (scriptableObject is null)
                {
                    Debug.LogWarning($"ScriptableObject not found at path: {filePath}");
                    return null;
                }

                return scriptableObject;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"LoadScriptableObject Failed: {ex.Message}");
                return null;
            }
        }
        
        public static IEnumerator LoadJsonFromFile(string filePath, Action<string> callback)
        {   
            var request = Resources.LoadAsync<TextAsset>(filePath);
            yield return request;

            if (request.asset is not TextAsset textAsset)
            {
                Debug.LogError($"Json file not found at path: {filePath}");
                callback?.Invoke(null);
            }
            else
            {
                callback?.Invoke(textAsset.text);
            }
        }
        
        public static IEnumerator LoadScriptableObject<T>(string filePath, Action<T> callback) where T : ScriptableObject
        {
            var request = Resources.LoadAsync<T>(filePath);
            yield return request;

            if (request.asset is not T loadedAsset)
            {
                Debug.LogError($"ScriptableObject not found at path: {filePath}");
                callback?.Invoke(null);
            }
            else
            {
                callback?.Invoke(loadedAsset);
            }
        }
        
        public static void ReadAllText(string path, Action<string> callback)
        {
            Task.Run(async () =>
            {
                using var reader = new StreamReader(path);
                var fileString = await reader.ReadToEndAsync();
                
                GppSyncContext.RunOnUnityThread(() =>
                {
                    callback?.Invoke(fileString);
                });
            });
        }
    }
}