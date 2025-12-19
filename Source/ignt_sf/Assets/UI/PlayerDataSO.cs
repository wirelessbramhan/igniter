using System.Collections.Generic;
using UnityEngine;

namespace com.krafton.fantasysports
{
    [System.Serializable]
    public class PlayerData
    {
        public string Name;
        public float CreditCost, Pointsvalue;
        public bool HasPlayedLastMatch;
    }

    [CreateAssetMenu]
    public class PlayerDataSO : ScriptableObject
    {
        public List<PlayerData> PlayerDatas;

        [ContextMenu("Add Random Players")]
        public void AddPlayersRandom()
        {
            for (int i = 0; i < 11; i++)
            {
                PlayerData newPlayer = new();
                
                newPlayer.Name = "Player" + (i + 1);
                newPlayer.CreditCost = Random.Range(5, 9);
                newPlayer.Pointsvalue = Random.Range(1, 10) * 10;
                newPlayer.HasPlayedLastMatch = (Random.Range(1, 10) % 2 == 0);

                PlayerDatas.Add(newPlayer);
            }
        }
    }
}
