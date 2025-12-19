using System;
using System.Collections.Generic;
using UnityEngine;

namespace ignt.sports.cricket.core
{
    public class GameStateManager : MonoBehaviour
    {
        [SerializeField]
        private List<GameState> AllStates;
        private int currentIndex = -1;
        [SerializeField]
        private bool isSwitching = false;
        [SerializeField]
        private GameState currentState;
        [Header("Listening on"), Space(2)]
        public BoolEventChannelSO StateChangeChannel;

        //Static Events for connection
        public static event Action<GState> OnStateEnter, OnStateExit;

        void OnEnable()
        {
            StateChangeChannel.OnEventRaised += ChangeState;
            OnStateEnter += DebugState;
        }

        private void DebugState(GState state)
        {
            Debug.Log("GSM entry to state : " + state);
        }

        void OnDisable()
        {
            StateChangeChannel.OnEventRaised -= ChangeState;
            OnStateEnter -= DebugState;
        }

        private void ChangeState(bool change)
        {
            if (change)
            {
                SwitchStateNext();
            }

            else
            {
                SwitchStatePrev();
            }
        }

        /// <summary>
        /// Assigns names to GameStates for readability
        /// </summary>
        private void OnValidate()
        {
            if (AllStates != null && AllStates[0] != null)
            {
                if (AllStates.Count > 0 && AllStates[0] != null)
                {
                    foreach (var state in AllStates)
                    {
                        state.SetName();
                    }
                }
            }
        }

        void Awake()
        {
            Application.targetFrameRate = 120;
        }

        private void Start()
        {
            currentState = null;
            
            //Sets Default state (0th index)
            EnterDefaultState(1);
        }

        /// <summary>
        /// switches gamestate from the state collection
        /// </summary>
        private void SwitchStateNext()
        {
            if (!isSwitching)
            {
                isSwitching = true;
                
                if (currentIndex >= 0 && currentIndex < AllStates.Count)
                {
                    //Previous state exit
                    OnStateExit?.Invoke(currentState.StateKey);
                    currentState.ExitState();
                    
                    currentIndex++;
                    currentState = AllStates[currentIndex];
                    
                    //Next State Enter
                    currentState.EnterState();
                    OnStateEnter?.Invoke(currentState.StateKey);
                }

                //Debug.Log("Game entered " + currentState.Name + " State.");

                isSwitching = false;
            }

            else
            {
                Debug.Log("SM switching states already!!", this);
            }
        }

        public void SwitchStatePrev()
        {
            if (!isSwitching)
            {
                isSwitching = true;
                
                if (currentIndex >= 0 && currentIndex < AllStates.Count)
                {
                    //Previous state exit
                    OnStateExit?.Invoke(currentState.StateKey);
                    currentState.ExitState();
                    
                    currentIndex--;
                    currentState = AllStates[currentIndex];
                    
                    //Next State Enter
                    currentState.EnterState();
                    OnStateEnter?.Invoke(currentState.StateKey);
                }

                //Debug.Log("Game entered " + currentState.Name + " State.");

                isSwitching = false;
            }

            else
            {
                Debug.Log("SM switching states already!!", this);
            }
        }

        private void EnterDefaultState(int defaultindex)
        {
            currentIndex = defaultindex;
            currentState = AllStates[currentIndex];

            OnStateEnter?.Invoke(currentState.StateKey);
            currentState.EnterState();
        }

    }
}
