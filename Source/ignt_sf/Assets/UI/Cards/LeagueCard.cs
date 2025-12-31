using System.Collections;
using com.krafton.fantasysports.UI;
using DG.Tweening;
using ignt.sports.cricket.core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ignt.sports.cricket.UI
{
    public class LeagueCard : MonoBehaviour
    {
        [SerializeField]
        private Image teamAIcon, teamBIcon, prizeIcon;
        [SerializeField]
        private string teamAName, teamBName, seriesName;
        [SerializeField]
        private float TimerThreshold = 2;
        public float secondsLeft {get; private set; } 
        [SerializeField]
        private int prizeAmount;

        [SerializeField] private TextMeshProUGUI match, teamNames, timerText, prizeText, btnText;
        private bool isLoaded = false, isLerping = false;

        [SerializeField]
        protected CanvasGroup CanvasGroup;
        public DummyLeagueData DummyLeague;
        public UserUIDataSO UserUIData;
        public void PopCard(float delay)
        {
            //Tween Card
            if (TryGetComponent<RectTransform>(out var rect))
            {
                rect.DOShakeScale(0.5f, 1, 10, 30, false, ShakeRandomnessMode.Full);
            }
        }

        private IEnumerator PopCardDelayed(float delay)
        {
            yield return null;
        }

        public void InitCard()
        {
            StartCoroutine(InitCardAsync());
        }

        private IEnumerator InitCardAsync()
        {
            yield return new WaitUntil(() => isLoaded);

            //assign values
            match.text = seriesName;
            teamNames.text = teamAName + " VS " + teamBName;
            prizeText.text = prizeAmount + " Lakh";
            btnText.text = "Join";

            //Set Active
            CanvasGroup.alpha = 1;
            CanvasGroup.interactable = true;
            CanvasGroup.blocksRaycasts = true;

            //run timer
            while(secondsLeft > 0)
            {
                secondsLeft -= Time.deltaTime;
                
                float minutes = Mathf.FloorToInt(secondsLeft / 60);
                float hour = Mathf.FloorToInt(secondsLeft / 360);
                string hoursLeft = hour + " h : ";
                string minutesLeft = minutes + " m";
                
                string exactTime = "0:00 PM";

                timerText.text = hoursLeft + minutesLeft + " | " + exactTime;

                if (hour < TimerThreshold)
                {
                    timerText.faceColor = Color.red;
                }

                yield return null;
            }

            CanvasGroup.alpha = 0.5f;
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
        }

        public void SetData(string teamA, string teamB, string matchSeries, float timeleft, int prizeamount)
        {
            teamAName = teamA;
            teamBName = teamB;
            seriesName = matchSeries;
            secondsLeft = timeleft;
            prizeAmount = prizeamount;

            isLoaded = true;
        }

        public void SetData(DummyLeagueData leagueData)
        {
            DummyLeague = leagueData;
            teamAName = leagueData.TeamA;
            teamBName = leagueData.TeamB;
            seriesName = leagueData.SeriesName;
            secondsLeft = leagueData.SecondsLeft;
            prizeAmount = leagueData.PrizeAmount;

            isLoaded = true;
        }

        void OnDisable()
        {
            StopAllCoroutines();
        }
    }
}
