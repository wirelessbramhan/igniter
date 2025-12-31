using System;
using UnityEngine;

namespace ignt.sports.cricket.core
{
    public class EventChannelSOBase<T> : ScriptableObject
    {
        public event Action<T> OnEventRaised;

        public virtual void RaiseEvent(T data)
        {
            OnEventRaised?.Invoke(data);
        }
    }
}
