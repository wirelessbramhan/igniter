using UnityEngine;
using ignt.sports.cricket.core;

namespace com.krafton.fantasysports.UI
{
    public class UIController : MonoBehaviour
    {
        [Header("broadcasting on"), Space(2)]
        public BoolEventChannelSO SplashFadeChannel;

        public BoolEventChannelSO AuthFadeChannel, HomeFadeChannel, SelectFadeChannel, CreateFadeChannel;

        [Header("Data holders"), Space(2)]
        public UserUIDataSO UserData;

        void OnEnable()
        {
            GameStateManager.OnStateEnter += EnterView;
            GameStateManager.OnStateEnter += SetUserData;
            GameStateManager.OnStateExit += ExitView;
        }

        void OnDisable()
        {
            GameStateManager.OnStateEnter -= EnterView;
            GameStateManager.OnStateEnter -= SetUserData;
            GameStateManager.OnStateExit -= ExitView;
        }

        void Awake()
        {
            SetAllViews(true);
        }

        private void SetAllViews(bool fade)
        {
            AuthFadeChannel.RaiseEvent(fade);
            HomeFadeChannel.RaiseEvent(fade);
            SelectFadeChannel.RaiseEvent(fade);
            CreateFadeChannel.RaiseEvent(fade);
        }

        void EnterView(GState state)
        {
            switch(state)
            {
                case GState.splash:
                break;

                case GState.auth:
                AuthFadeChannel.RaiseEvent(false);
                
                break;
                case GState.home:
                HomeFadeChannel.RaiseEvent(false);
                
                break;
                case GState.select:
                SelectFadeChannel.RaiseEvent(false);

                break;
                case GState.create:
                CreateFadeChannel.RaiseEvent(false);
                
                break;
                case GState.profile:
                break;
                case GState.exit:
                break;
            }
        }

        void ExitView(GState state)
        {
            switch(state)
            {
                case GState.splash:
                break;

                case GState.auth:
                AuthFadeChannel.RaiseEvent(true);
                
                break;
                case GState.home:
                HomeFadeChannel.RaiseEvent(true);
                
                break;
                case GState.select:
                SelectFadeChannel.RaiseEvent(true);

                break;
                case GState.create:
                CreateFadeChannel.RaiseEvent(true);
                
                break;
                case GState.profile:
                break;
                case GState.exit:
                break;
            }
        }

        void SetUserData(GState state)
        {
            if (state == GState.auth)
            {
                UserData.UserID = "User" + System.DateTime.Now;
                UserData.UserCoins.Value = 0;
            }
        }
    }
}
