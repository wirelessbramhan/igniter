using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.krafton.fantasysports.UI
{
    public class MatchCard : MonoBehaviour
    {
        public MatchDataSO dataSO;

        [SerializeField]
        protected Image teamAIcon, teamBIcon;
        [SerializeField]
        protected TextMeshProUGUI teamACodeText, teamBCodeText, daysLeftText, hoursLeftText;

        private void SetData(MatchData data)
        {
            teamAIcon.sprite = data.TeamAIcon;
            teamBIcon.sprite = data.TeamBIcon;

            teamACodeText.text = data.TeamACode;
            teamBCodeText.text = data.TeamBCode;

            daysLeftText.text = data.daysLeft + " days";
            hoursLeftText.text = data.hoursLeft + "h" + data.hoursLeft % 60 + "m left";
        }
    }
}
