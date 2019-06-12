using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wakaman.Utilities
{
    public class PriorityQueue<T>
    {
        private List<KeyValuePair<T,int>> data;

        public int Count {
            get { return data.Count; }
        }

        public bool IsEmpty {
            get { return data.Count == 0; }
        }

        public PriorityQueue()
        {
            data = new List<KeyValuePair<T, int>>();
        }

        public void Add(T element, int priority)
        {
            var pair = new KeyValuePair<T, int>(element, priority);
            for (int i = 0; i < data.Count; i++)
            {
                if (priority < data[i].Value)
                {
                    data.Insert(i, pair);
                    return;
                }
            }
            data.Add(pair);
        }

        public T Dequeue()
        {
            if (data.Count > 0)
            {
                var element = data[0];
                data.RemoveAt(0);
                return element.Key;
            }
            else
                return default(T);
        }
    }
}
