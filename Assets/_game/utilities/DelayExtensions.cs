using System;
using System.Collections;
using UnityEngine;

namespace Wakaman.Utilities
{
    public static class DelayExtensions
    {
        public static Coroutine Delay(this MonoBehaviour mono, float time, Action action, bool scaled = false)
        {
            return mono.StartCoroutine(DelayRoutine(action, time, scaled));
        }

        private static IEnumerator DelayRoutine(Action a, float time, bool scaled)
        {
            if (time > 0f)
            {
                if (scaled)
                    yield return new WaitForSeconds(time);
                else
                    yield return new WaitForSecondsRealtime(time);
            }
            a?.Invoke();
        }
    }
}