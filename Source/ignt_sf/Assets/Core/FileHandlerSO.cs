using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace com.krafton.fantasysports.core
{
    public class FileHandlerSO : MonoBehaviour
    {
        public GameData GameData;
        // A simple class to serialize (can also be a struct)
        public void SaveData()
        {
            string data = JsonUtility.ToJson(GameData);
            PlayerPrefs.SetString("GameData", data);
            PlayerPrefs.Save();
        }

        public void LoadData()
        {
            if (PlayerPrefs.HasKey("GameData"))
            {
                GameData = JsonUtility.FromJson<GameData>(PlayerPrefs.GetString("GameData"));
            }

            else
            {
                Debug.Log("No Saved Game Data found.");
            }
        }

        public void AddCoins(int amount)
        {
            GameData.PlayerCoins += amount;
        }

        public void ClaimReward(int maxdays)
        {
            if (!GameData.CanClaim()) 
            return;
            
            GameData.CurrentDay++;
            
            if (GameData.CurrentDay > maxdays)
                GameData.CurrentDay = 0;
            
            GameData.LastClaimTicks = DateTime.UtcNow.Ticks;
            SaveData();
        }
    }
}
