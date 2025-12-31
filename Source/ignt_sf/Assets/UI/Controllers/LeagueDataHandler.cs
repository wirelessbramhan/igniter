using System.Collections.Generic;
using com.krafton.fantasysports.UI;
using ignt.sports.cricket.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.krafton.fantasysports
{
    public class LeagueDataHandler : MonoBehaviour
    {
        public List<PrizeCard> PrizeCards;
        public RectTransform HolderTransform;
        public UserUIDataSO UserUIData;

        public TextMeshProUGUI daysLeft, timeLeft, TeamAName, TeamBName;
        public Image TeamAIcon, TeamBIcon;

        public void AddLeague(LeagueCard leagueCard)
        {
            UserUIData.SetLeague(leagueCard.DummyLeague);
        }
    }
}
