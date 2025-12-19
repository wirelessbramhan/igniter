using System;
using ignt.sports.cricket.network;
using UnityEngine;

namespace ignt.sports.cricket
{
    public class UIListener : MonoBehaviour
    {
        public IAuthData AuthData;
        public LeagueDataSO LeagueData;
        public MatchDataSO MatchData;


        private void OnEnable()
        {
            //AuthData.OnAuthSuccess+= AuthDataOnOnAuthSuccess;
            LeagueData.OnLeagueDataSuccess += LeagueDataOnOnLeagueDataSuccess;
            MatchData.OnMatchDataSuccess += MatchDataOnOnMatchDataSuccess;
        }
        
        private void OnDisable()
        {
            //AuthData.OnAuthSuccess -= AuthDataOnOnAuthSuccess;
            LeagueData.OnLeagueDataSuccess -= LeagueDataOnOnLeagueDataSuccess;
            MatchData.OnMatchDataSuccess -= MatchDataOnOnMatchDataSuccess;
        }

        private void MatchDataOnOnMatchDataSuccess()
        {
            Debug.Log("Match Data Success");
        }

        private void LeagueDataOnOnLeagueDataSuccess()
        {
            Debug.Log("League Data Success");
        }

        private void AuthDataOnOnAuthSuccess()
        {
            Debug.Log("Auth Data Success");
        }
    }
}
