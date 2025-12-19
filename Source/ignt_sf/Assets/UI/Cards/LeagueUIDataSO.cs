using System.Collections.Generic;
using UnityEngine;

namespace ignt.sports.cricket.UI
{
    [System.Serializable]
    public class DummyLeagueData
    {
        [HideInInspector] public string Name;
        public string MatchName;
        public string TeamA, TeamB;

        public float SecondsLeft;
        public int PrizeAmount;
    }
    
    [CreateAssetMenu(menuName = "UI/Data Holder/League", fileName = "LeagueData_New", order = 0)]
    public class LeagueUIDataSO : ScriptableObject
    {
        public List<DummyLeagueData> DummyLeagues;

        [ContextMenu("Create Leagues")]
        public void CreateDummyLeagues()
        {
            DummyLeagues = new();

            string leagueName = "league";
            string match = "Match Series";
            string teamA = "TeamA";
            string teamB = "TeamB";

            for (int i = 0; i <= 5; i++)
            {
                //Iterated Data
                DummyLeagueData leagueData = new();
                leagueData.Name = leagueName + (i + 1);
                leagueData.MatchName = match + (i + 1);
                leagueData.TeamA = teamA + (i + 1);
                leagueData.TeamB = teamB + (i + 1);

                //Randomized Data
                leagueData.SecondsLeft = 300 + (10 * i);
                leagueData.PrizeAmount = Random.Range(1, 2 + i);
                
                //Add to list
                DummyLeagues.Add(leagueData);
            }
        }
    }
}
