using System.Collections.Generic;
using com.krafton.fantasysports.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.krafton.fantasysports
{
    public class PreviewPanelHandler : MonoBehaviour
    {
        public UserUIDataSO UserUIData;
        public UserTeamSO UserTeam;

        public TextMeshProUGUI PlayerCount, CreditRemaining, PlayerName;
        public List<PreviewPlayerHandler> PlayerPrefabs;

        public void ShowPlayers()
        {
            for (int i = 0; i < UserTeam.PlayerCount; i++)
            {
                var playerData = UserTeam.Value.Players[i];
                PlayerPrefabs[i].SetData(playerData.Name, playerData.IsVice, playerData.IsCapt);
            }
        }

        void Update()
        {
            PlayerName.text = UserUIData.UserID;
            PlayerCount.text = "Players " + UserTeam.PlayerCount + "/11";
            CreditRemaining.text = "Credits left" + UserUIData.UserCredits.Value; 
        }
    }
}
