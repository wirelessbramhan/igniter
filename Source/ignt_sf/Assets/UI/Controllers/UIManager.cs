using UnityEngine;
using ignt.sports.cricket.core;

namespace com.krafton.fantasysports.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("broadcasting on"), Space(2)]
        public BoolEventChannelSO AuthFadeChannel;
        public BoolEventChannelSO HomeFadeChannel, LeeagueFadeChannel, TeamCreateFadeChannel, MyMatchesFadeChannel, LeaderboardFadeChannel;

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

        private void Start()
        {
            SetAllViews(true);
        }

        private void SetAllViews(bool fade)
        {
            //AuthFadeChannel.RaiseEvent(fade);
            HomeFadeChannel.RaiseEvent(fade);
            LeeagueFadeChannel.RaiseEvent(fade);
            TeamCreateFadeChannel.RaiseEvent(fade);
            MyMatchesFadeChannel.RaiseEvent(fade);
            LeaderboardFadeChannel.RaiseEvent(fade);
        }

        private void EnterView(GState state)
        {
            switch(state)
            {
                case GState.login:
                SetUserData();
                AuthFadeChannel.RaiseEvent(false);
                break;
                
                case GState.home:
                HomeFadeChannel.RaiseEvent(false);
                UserData.ResetCredits();
                
                break;
                case GState.leagueSelect:
                LeeagueFadeChannel.RaiseEvent(false);

                break;
                case GState.teamCreate:
                TeamCreateFadeChannel.RaiseEvent(false);
                
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
                LeeagueFadeChannel.RaiseEvent(true);
                
                //Reset User Credits
                UserData.ResetCredits();

                break;
                case GState.teamCreate:
                TeamCreateFadeChannel.RaiseEvent(true);
                
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
            UserData.StartSession(1000);
            Debug.Log("ID is : " + UserData.UserID);
        }
    }
}
