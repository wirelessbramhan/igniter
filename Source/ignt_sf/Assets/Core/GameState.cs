using System.Collections;
using UnityEngine;

namespace ignt.sports.cricket.core
{
    /// <summary>
    /// GameStateKey for quick lookup, if converted to Dictionary
    /// </summary>
    public enum GState
    {
        splash,
        auth,
        home,
        select,
        create,
        profile,
        exit
    }

    [System.Serializable]
    public class GameState
    {
        [HideInInspector]
        public string Name;

        [field: SerializeField]
        public GState StateKey { get; private set; }
        public float updateDelay = -1;
        [SerializeField]
        protected bool isActive;
        public virtual void EnterState()
        {
            //Entry event invoke
            isActive = true;
        }

        /// <summary>
        /// Updates the state, Ideally is a coroutine to have a fixed frequencys
        /// </summary>
        public virtual void FrameUpdate()
        {
            
        }

        public virtual void ExitState()
        {
            //exit event invoke
            isActive = false;
        }

        /// <summary>
        /// Assigns names to GameStates for readability
        /// </summary>
        public void SetName()
        {
            Name = StateKey.ToString();
        }
    }
}
