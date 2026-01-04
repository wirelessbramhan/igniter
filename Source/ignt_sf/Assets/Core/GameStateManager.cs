using com.krafton.fantasysports.core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ignt.sports.cricket.core
{
    public class GameStateManager : MonoBehaviour
    {
        [SerializeField]
        private List<GameState> StateList;
        [SerializeField]
        private Dictionary<GState, GameState> AllStateDict;
        [SerializeField]
        private bool isSwitching = false, isLoaded;
        public GameState CurrentState;
        //Static Events for connection
        public static event Action<GState> OnStateEnter, OnStateExit;
        [Header("Listening on"), Space(2)]
        public IntEventChannelSO StateChangeChannel;

        void OnEnable()
        {
            OnStateEnter += DebugStateEntry;
            OnStateExit += DebugStateExit;
            StateChangeChannel.OnEventRaised += StateChangeChannel_OnEventRaised;
        }

        private void StateChangeChannel_OnEventRaised(int obj)
        {
            GState newStateKey = (GState)obj;
            ChangeState(newStateKey);
        }

        void OnDisable()
        {
            OnStateEnter += DebugStateEntry;
            OnStateExit += DebugStateExit;
            StateChangeChannel.OnEventRaised -= StateChangeChannel_OnEventRaised;
        }

        void Awake()
        {
            Application.targetFrameRate = 120;
            isLoaded = false;
        }

        private IEnumerator Start()
        {
            LoadDict();
            yield return new WaitUntil(() => isLoaded);
            //Sets Default state
            EnterDefaultState(GState.login);
        }

        /// <summary>
        /// Debugs current state entry
        /// </summary>
        /// <param name="state">State key to change</param>
        private void DebugStateEntry(GState state)
        {
            Debug.Log("GSM Enter State : " + state);
        }

        /// <summary>
        /// Debugs current state exit
        /// </summary>
        /// <param name="state">State key to change</param>
        private void DebugStateExit(GState state)
        {
            Debug.Log("GSM Exit State : " + state);
        }

        /// <summary>
        /// Loads States by enum into Dictionary for Non-linear state transitions
        /// </summary>
        [ContextMenu("Load Dict")]
        private void LoadDict()
        {
            if (!isLoaded)
            {
                AllStateDict = new();
                foreach (var state in StateList)
                {
                    state.Name = state.StateKey.ToString();

                    if (AllStateDict.ContainsKey(state.StateKey))
                    {
                        AllStateDict[state.StateKey] = state;
                    }
                    
                    else
                    {
                        AllStateDict.Add(state.StateKey, state);
                    }
                }

                isLoaded = (AllStateDict.Count == StateList.Count);
            }
        }

        /// <summary>
        /// switches individual gamestate from the state collection
        /// </summary>
        public void SwitchToLogin()
        {
            ChangeState(GState.login);
        }

        public void SwitchToHome()
        {
            ChangeState(GState.home);
        }

        public void SwitchToLeagueSelect()
        {
            ChangeState(GState.leagueSelect);
        }

        public void SwitchToTeamCreate()
        {
            ChangeState(GState.teamCreate);
        }

        public void SwitchToUserMatches()
        {
            ChangeState(GState.userMatches);
        }

        public void SwitchToUserProfile()
        {
            ChangeState(GState.userProfile);
        }

        public void SwitchToLeaderboards()
        {
            ChangeState(GState.leaderboard);
        }

        private void EnterDefaultState(GState key)
        {
            CurrentState = AllStateDict[key];

            //Next State Enter
            CurrentState.EnterState();
            OnStateEnter?.Invoke(CurrentState.StateKey);
        }

        public void ChangeState(GState stateKey)
        {
            if (!isSwitching)
            {
                isSwitching = true;
                
                if (AllStateDict.ContainsKey(stateKey))
                {
                    //Previous state exit
                    OnStateExit?.Invoke(CurrentState.StateKey);
                    CurrentState.ExitState();
                    
                    //State Change
                    CurrentState = AllStateDict[stateKey];
                    
                    //Next State Enter
                    CurrentState.EnterState();
                    OnStateEnter?.Invoke(CurrentState.StateKey);
                }

                isSwitching = false;
            }

            Debug.Log("GS is " + CurrentState.StateKey.ToString());
        }

    }
}
