using UnityEngine;
using ignt.sports.cricket.core;

namespace com.krafton.fantasysports.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("broadcasting on"), Space(2)]
        public BoolEventChannelSO AuthFadeChannel;
        public BoolEventChannelSO HomeFadeChannel, SelectFadeChannel, CreateFadeChannel, MyMatchesFadeChannel, LeaderboardFadeChannel;

        [Header("Data holders"), Space(2)]
        public UserUIDataSO UserData;

        void OnEnable()
        {
            GameStateManager.OnStateEnter += EnterView;
            GameStateManager.OnStateExit += ExitView;
        }

        void OnDisable()
        {
            GameStateManager.OnStateEnter += EnterView;
            GameStateManager.OnStateExit += ExitView;
        }
        private void SetAllViews(bool fade)
        {
            AuthFadeChannel.RaiseEvent(fade);
            HomeFadeChannel.RaiseEvent(fade);
            SelectFadeChannel.RaiseEvent(fade);
            CreateFadeChannel.RaiseEvent(fade);
            MyMatchesFadeChannel.RaiseEvent(fade);
            LeaderboardFadeChannel.RaiseEvent(fade);
        }

        private void EnterView(GState state)
        {
            switch(state)
            {
                case GState.login:
                SetUserData();
                SetAllViews(true);
                AuthFadeChannel.RaiseEvent(false);
                break;
                
                case GState.home:
                HomeFadeChannel.RaiseEvent(false);
                UserData.ResetCredits();
                
                break;
                case GState.leagueSelect:
                SelectFadeChannel.RaiseEvent(false);

                break;
                case GState.teamCreate:
                CreateFadeChannel.RaiseEvent(false);
                
                break;
                case GState.userMatches:
                MyMatchesFadeChannel.RaiseEvent(false);

                break;
                case GState.leaderboard:
                LeaderboardFadeChannel.RaiseEvent(false);
                
                break;
            }
        }

        private void ExitView(GState state)
        {
            switch(state)
            {
                case GState.login:
                AuthFadeChannel.RaiseEvent(true);
                
                break;
                case GState.home:
                HomeFadeChannel.RaiseEvent(true);
                
                break;
                case GState.leagueSelect:
                SelectFadeChannel.RaiseEvent(true);
                
                //Reset User Credits
                UserData.ResetCredits();

                break;
                case GState.teamCreate:
                CreateFadeChannel.RaiseEvent(true);
                
                break;
                case GState.userMatches:
                MyMatchesFadeChannel.RaiseEvent(true);

                break;
                case GState.leaderboard:
                LeaderboardFadeChannel.RaiseEvent(true);
                
                break;
            }
        }

        private void SetUserData()
        {
            UserData.StartSession(100);
            Debug.Log("ID is : " + UserData.UserID);
        }
    }
}
