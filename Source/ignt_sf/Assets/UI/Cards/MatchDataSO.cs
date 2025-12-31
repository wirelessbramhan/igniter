using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace com.krafton.fantasysports.UI
{
    [System.Serializable]
    public class MatchData
    {
        [HideInInspector] public string Name;
        public string TeamACode, TeamBCode;
        public float daysLeft, hoursLeft;
        public Sprite TeamAIcon, TeamBIcon;

        public void SetName()
        {
            Name = TeamACode + "vs" + TeamBCode;
        }
    }
    
    [CreateAssetMenu(menuName = "UI/Data Holder/Match", fileName = "MatchData_New", order = 1)]
    public class MatchDataSO : ScriptableObject
    {
        public List<MatchData> Matches;

        void OnValidate()
        {
            for (int i = 0; i < Matches.Count; i++)
            {
                Matches[i].SetName();
            }
        }
    }
}
