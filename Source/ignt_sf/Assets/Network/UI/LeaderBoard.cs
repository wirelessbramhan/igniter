using System;
using UnityEngine;
using UnityEngine.UI;

namespace com.krafton.fantasysports
{
    public class LeaderBoard : MonoBehaviour
    {
        public Button LeaderboardBtn;
        public Button WinningsBtn;
        public Button ScoreCardBtn;
        public Button ScorecardBackBtn;
        
        public GameObject LeaderboardPanel;
        public GameObject WinningsPanel;
        public GameObject ScoreCardPanel;
        
        
        public Button TeamABtn, TeamBBtn;
        
        public GameObject[] TeamACards, TeamBCards;
        
        private void Start()
        {
            LeaderboardBtn.Select();
            
            LeaderboardBtn.onClick.AddListener(OnLeaderboardBtnClick);
            WinningsBtn.onClick.AddListener(OnWinningsBtnClick);
            ScoreCardBtn.onClick.AddListener(OnScoreCardBtnClicked);
            ScorecardBackBtn.onClick.AddListener(OnScoreCardBackBtnClicked);
            // TeamABtn.onClick.AddListener(TeamABtnClicked);
            // TeamABtn.onClick.AddListener(TeamBBtnClicked);
        }

        public void TeamABtnClicked()
        {
            foreach (var card in TeamACards)
            {
                card.SetActive(!card.activeSelf);
            }
        }
        
        public void TeamBBtnClicked()
        {
            foreach (var card in TeamBCards)
            {
                card.SetActive(!card.activeSelf);
            }
        }

        private void OnScoreCardBackBtnClicked()
        {
            ScoreCardPanel.SetActive(false);
        }

        private void OnScoreCardBtnClicked()
        {
            ScoreCardPanel.SetActive(true);
        }

        private void OnDestroy()
        {
            LeaderboardBtn.onClick.RemoveListener(OnLeaderboardBtnClick);
            WinningsBtn.onClick.RemoveListener(OnWinningsBtnClick);
            ScoreCardBtn.onClick.RemoveListener(OnScoreCardBtnClicked);
            ScorecardBackBtn.onClick.RemoveListener(OnScoreCardBackBtnClicked);
            // TeamABtn.onClick.RemoveListener(TeamABtnClicked);
            // TeamBBtn.onClick.RemoveListener(TeamBBtnClicked);

        }


        private void OnLeaderboardBtnClick()
        {
            LeaderboardPanel.SetActive(true);
            WinningsPanel.SetActive(false);
        }
        private void OnWinningsBtnClick()
        {
            WinningsPanel.SetActive(true);
            LeaderboardPanel.SetActive(false);
        }
    }
}
