using System;
using System.Collections.Generic;

namespace com.krafton.fantasysports.core
{
    [System.Serializable()]
    public class GameData
    {
        public int PlayerCoins;
        public int CurrentDay;
        public long LastClaimTicks;
        public string UserName;

        //public Dictionary<PrizeLeagueData, TeamData> UserMatchDict;

        public GameData(string username, int startingCoins)
        {
            UserName = username;
            PlayerCoins = startingCoins;
        }

        public bool CanClaim()
        {
            DateTime now = DateTime.UtcNow;
            DateTime lastClaim = new DateTime(LastClaimTicks);
            if (LastClaimTicks == 0)
                return true;
            return (now - lastClaim).TotalHours >= 24;
        }
    }
    
    [System.Serializable]
    public class MatchSeries
    {
        public string Name, TeamA, TeamB;
        public float SecondsLeft;
        public int PrizePoolAmount;
        public PrizeLeagueData[] Leagues;

        public MatchSeries(string name, string teamA, string teamB, float secondsLeft, int prizePoolAmount, int numberOfLeagues)
        {
            Name = name;
            TeamA = teamA;
            TeamB = teamB;
            SecondsLeft = secondsLeft;
            PrizePoolAmount = prizePoolAmount;
            Leagues = new PrizeLeagueData[numberOfLeagues];
        }
    }

    [System.Serializable]
    public class PrizeLeagueData
    {
        public string SeriesName;
        public int PrizeAmount;
        public int PlayersJoined;

        public PrizeLeagueData(string seriesName, int prizeAmount, int Playersjoined)
        {
            this .SeriesName = seriesName;
            this .PrizeAmount = prizeAmount;
            this .PlayersJoined = Playersjoined;
        }
    }

    [System.Serializable]
    public class TeamData
    {
        public string NameCode;
        public List<PlayerData> Players;
        public int PlayerCount => Players.Count;
        public PlayerData Capt, ViceCapt;

        public TeamData(string TeamName)
        {
            this.NameCode = TeamName;
            Players = new();
            Capt = null;
            ViceCapt = null;
        }

        public void AddPlayerData(PlayerData PlayerData)
        {
            if (Players.Count < 11)
            {
                Players.Add(PlayerData);
            }
        }

        public void RemovePlayer(PlayerData PlayerData)
        {
            Players.Remove(PlayerData);
        }

        public void ResetPlayers()
        {
            Players.Clear();
        }

        public void SetCaptain(PlayerData capt)
        {
            Capt = capt;
        }

        public void SetVice(PlayerData vice)
        {
            ViceCapt = vice;
        }
    }
    
    public enum PlayerType
    {
        allrounder,
        batsman,
        wicketkeeper,
        bowler
    }

    [System.Serializable]
    public class PlayerData
    {
        public string Name;
        public PlayerType Type;
        public float CreditCost, Pointsvalue;
        public bool HasPlayedLastMatch, IsCapt, IsVice, IsChosen;

        public PlayerData(string playerName, PlayerType playerType, float creditCost, float skillpoints, bool hasPlayedLast)
        {
            Name = playerName;
            Type = playerType;
            CreditCost = creditCost;
            Pointsvalue = skillpoints;
            HasPlayedLastMatch = hasPlayedLast;
            IsCapt = false;
            IsVice = false;
            IsChosen = false;
        }

        public void SetPlayerData(string playerName, PlayerType playerType, float creditCost, float skillpoints, bool hasPlayedLast)
        {
            Name = playerName;
            Type = playerType;
            CreditCost = creditCost;
            Pointsvalue = skillpoints;
            HasPlayedLastMatch = hasPlayedLast;
            IsCapt = false;
            IsVice = false;
            IsChosen = false;
        }
    }
}
