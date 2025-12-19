using Newtonsoft.Json;

namespace ignt.sports.cricket.network
{
   
        [System.Serializable]
       public class MatchData
       {
           public string MatchId;
           
           public string MatchName;
           
           public Team TeamA;
           
           public Team TeamB;
       }
   
       [System.Serializable]
       public class Team
       {
           public string TeamId;
           
           public string TeamName;
           
           public Player[] Players;
       }
   
       [System.Serializable]
       public class Player
       {
           public string PlayerId;
           
           public string PlayerName;
           
           public string Role;
           
           public float Credits;
           
           public float Points;
       }
       
       
   }

