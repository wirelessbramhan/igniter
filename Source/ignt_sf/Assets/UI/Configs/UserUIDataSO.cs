using ignt.sports.cricket.core;
using ignt.sports.cricket.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.krafton.fantasysports.UI
{
    [CreateAssetMenu(menuName = "UI/Data Holder/User UI Data", fileName = "LeagueData_New", order = 2)]
    public class UserUIDataSO : ScriptableObject
    {
        public string UserID;
        public FloatVariableSO UserCoins, UserCredits;
        public DummyLeagueData CurrentLeague;
        public UserTeamSO CurrentTeam;
        public Dictionary<DummyLeagueData, TeamData> UserMatches;
        public DateTime dateTimeTicks;
        public void StartSession(int startingCoins)
        {
            dateTimeTicks = DateTime.UtcNow;
            UserID = "user" + dateTimeTicks.ToShortDateString();
            UserMatches = new Dictionary<DummyLeagueData, TeamData>();
            UserCoins.Value = startingCoins;
            UserCredits.Value = 100;
        }

        public void SetLeague(LeagueCard leagueCard)
        {
            CurrentLeague = leagueCard.DummyLeague;

            SetLeague(CurrentLeague);
        }

        public void SetLeague(DummyLeagueData league)
        {
            if (UserMatches.ContainsKey(league))
            {
                UserMatches[league] = null;
            }

            else
            {
                UserMatches.Add(league, null);
            }
        }

        public void SetTeam(TeamData team, DummyLeagueData leagueData)
        {
            if (UserMatches.ContainsKey(leagueData))
            {
                UserMatches[leagueData] = team;
            }

            else
            {
                UserMatches.Add(leagueData, team);
            }
        }

        public void ResetCredits()
        {
            UserCredits.Value = 100;
            Debug.Log("Credits rest to :" + UserCredits.Value, this);
        }
    }
}
