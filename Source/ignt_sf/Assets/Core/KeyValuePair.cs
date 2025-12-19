using UnityEngine;

namespace ignt.sports.cricket.core
{
    [System.Serializable]
    public class KeyValuePair<T1, T2>
    {
        [HideInInspector]
        public string Name;
        public T1 Key;
        public T2 Value;

        public KeyValuePair(T1 key, T2 value)
        {
            Key = key;
            Value = value;
            Name = key.ToString();
        }
    }
}
