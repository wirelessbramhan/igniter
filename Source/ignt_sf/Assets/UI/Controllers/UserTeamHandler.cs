using System.Collections.Generic;
using ignt.sports.cricket.UI;
using UnityEngine;
using UnityEngine.UI;

namespace com.krafton.fantasysports.UI
{
    public class UserTeamHandler : MonoBehaviour
    {
        public DummyLeagueData currentLeague;
        public UserTeamSO CurrentTeam;
        [SerializeField]
        private List<Image> playerBars;
        [SerializeField]
        private Button NextBtn;
        void Awake()
        {
            CurrentTeam.Value.Players = null;
            CurrentTeam.PlayerCount = 0;
            CurrentTeam.currentPlayerIndex = 0;
        }

        void Update()
        {
            if (CurrentTeam.PlayerCount > 0)
            {
                for (int i = 0; i < CurrentTeam.PlayerCount; i++)
                {
                    playerBars[i].color = Color.green;
                }
            }

            else
            {
                for (int i = 0; i < playerBars.Count; i++)
                {
                    playerBars[i].color = Color.white;
                }
            }

            NextBtn.interactable = (CurrentTeam.PlayerCount == 11);
        }
    }
}
