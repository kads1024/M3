using System.Collections;
using UnityEngine;

namespace M3.Presentation.Animation
{
    /// <summary>
    /// Utility where you can pass in a list of routines and run them in parallel
    /// </summary>
    public static class CoroutineUtil
    {
        public static IEnumerator RunParallel(MonoBehaviour host, params IEnumerator[] routines)
        {
            int remaining = routines.Length;

            foreach (var routine in routines)
            {
                host.StartCoroutine(Wrap(routine, () => remaining--));
            }

            while (remaining > 0)
                yield return null;
        }

        private static IEnumerator Wrap(IEnumerator routine, System.Action onComplete)
        {
            yield return routine;
            onComplete?.Invoke();
        }
    }
}