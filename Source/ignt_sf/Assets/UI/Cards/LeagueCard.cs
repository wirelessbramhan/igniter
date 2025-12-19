using System.Collections;
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
        public void PopCard(float delay)
        {
            if (!isLerping)
            {
                StartCoroutine(PopCardDelayed(delay));
            }
        }

        private IEnumerator PopCardDelayed(float delay)
        {
            isLerping = true;
            
            yield return new WaitForSeconds(delay);
            //Pop Card
            transform.localScale = new(0.1f, 0.1f, 1.0f);

            while (transform.localScale != Vector3.one)
            {
                transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.one, Time.deltaTime * 5.0f);
                yield return null;
            }

            isLerping = false;
        }

        private IEnumerator Start()
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

        void OnDisable()
        {
            StopAllCoroutines();
        }
    }
}
