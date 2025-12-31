using com.krafton.fantasysports;
using com.krafton.fantasysports.core;
using System.Collections.Generic;
using UnityEngine;

namespace ignt.sports.cricket.core
{
    [CreateAssetMenu(fileName = "MasterDH_default", menuName = "UI/Data/Master Data Holder", order = 5)]
    public class DataHandlerSO : ScriptableObject
    {
        public GameData UserData;
        public List<KeyValuePair<PrizeLeagueData, TeamData>> UserMatches;
        public List<MatchSeries> MatchSeriesData;
        public TeamData CurrentTeam;
        public bool HasMaxWktKeepers;

        private void OnValidate()
        {
            if (UserData != null)
            {
                foreach (var data in UserMatches)
                {
                    UserMatches.Add(new KeyValuePair<PrizeLeagueData, TeamData>(data.Key, data.Value));
                }
            }
        }

        [ContextMenu("Create Team")]
        public void CreateTeam()
        {
            CurrentTeam = new("Team 1");
            HasMaxWktKeepers = false;
        }

        public void UpdateTeam(PlayerData updatedPlayer)
        {
            CurrentTeam.RemovePlayer(updatedPlayer);
            CurrentTeam.AddPlayerData(updatedPlayer);
        }

        public void AddPlayer(PlayerData player)
        {
            CurrentTeam.AddPlayerData(player);
        }

        public void RemovePlayer(PlayerData player)
        {
            CurrentTeam.RemovePlayer(player);
        }

        public void SetCapt(PlayerData capt)
        {
            CurrentTeam.Capt = capt;
        }

        public void ClearCapt()
        {
            CurrentTeam.Capt = null;
        }

        public void SetVice(PlayerData vice)
        {
            CurrentTeam.ViceCapt = vice;
        }

        public void ClearVice()
        {
            CurrentTeam.ViceCapt = null;
        }
    }
}
