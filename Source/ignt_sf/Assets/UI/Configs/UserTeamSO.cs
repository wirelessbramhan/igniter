using System.Collections.Generic;
using com.krafton.fantasysports.core;
using ignt.sports.cricket.core;
using UnityEngine;
using UnityEngine.UI;

namespace com.krafton.fantasysports.UI
{
    [System.Serializable]
    public class TeamData
    {
        public string NameCode;
        public Image TeamIcon; 
        public PlayerData[] Players;
    }

    [CreateAssetMenu(fileName = "DummyPlayers_default", menuName = "UI/Data/User Team", order = 6)]
    public class UserTeamSO : VariableSO<TeamData>
    {
        public int currentPlayerIndex, PlayerCount;
        public List<PlayerCard> AllCards;
        public void AddPlayer(PlayerCard playerCard)
        {
            if (Value.Players == null)
            {
                Value.Players = new PlayerData[11];
                currentPlayerIndex = 0;
            }

            if (AllCards == null)
            {
                AllCards = new();
            }

            AllCards.Add(playerCard);

            //Value.Players[currentPlayerIndex] = playerCard.PlayerData;
            currentPlayerIndex++;
            PlayerCount++;
        }

        public void RemovePlayer(PlayerCard playerCard)
        {
            if (AllCards.Count > 0)
            {
                AllCards.Remove(playerCard);
            }

            for (int i = 0; i < Value.Players.Length; i++)
            {
                // if (Value.Players[i] == playerCard.PlayerData)
                // {
                //     Value.Players[i] = null;
                // }
            }

            currentPlayerIndex--;
            PlayerCount--;
        }

        public void ResetPlayers()
        {
            Value.Players = null;
            PlayerCount = 0;
            currentPlayerIndex = 0;
        }

        public bool CheckCapt()
        {
            bool result = false;
            
            foreach (var player in Value.Players)
            {
                if (player != null)
                {
                    result = player.IsCapt;
                }
            }

            return result;
        }

        public bool CheckVice()
        {
            bool result = false;
            
            foreach (var player in Value.Players)
            {
                if (player != null)
                {
                    //result = player.IsViceCapt;
                }
            }

            return result;
        }
    }
}
