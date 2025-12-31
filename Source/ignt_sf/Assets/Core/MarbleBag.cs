using System.Collections.Generic;
using UnityEngine;

namespace com.krafton.fantasysports.core
{
    public class MarbleBag<T> 
    {
        private IEnumerable<T> allElements;
        private List<T> current;

        public virtual void Reset()
        {
            current.Clear();
            foreach (T item in allElements)
            {
                current.Insert(Random.Range(0, current.Count + 1), item);
            }
        }

        public T Next
        {
            get
            {
                if (current.Count == 0)
                {
                    Reset();
                }

                T result = current[current.Count - 1];
                current.RemoveAt(current.Count - 1);

                return result;
            }
        }
    }
}
