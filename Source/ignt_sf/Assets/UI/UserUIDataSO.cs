using ignt.sports.cricket.core;
using UnityEngine;

namespace com.krafton.fantasysports.UI
{
    [CreateAssetMenu(menuName = "UI/Data Holder/User UI Data", fileName = "LeagueData_New", order = 2)]
    public class UserUIDataSO : ScriptableObject
    {
        public string UserID;
        public FloatVariableSO UserCoins, UserCredits;
    }
}
