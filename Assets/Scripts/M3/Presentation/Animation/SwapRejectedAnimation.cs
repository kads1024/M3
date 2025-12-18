using System.Collections;
using UnityEngine;
using M3.Presentation.Gem;

namespace M3.Presentation.Animation
{
    public sealed class SwapRejectedAnimation : IBoardAnimation
    {
        private readonly MonoBehaviour _host;
        private readonly GemView _a;
        private readonly GemView _b;
        private readonly Vector3 _aTarget;
        private readonly Vector3 _bTarget;
        private readonly float _duration;

        public SwapRejectedAnimation(
            MonoBehaviour host,
            GemView a,
            GemView b,
            Vector3 aTarget,
            Vector3 bTarget,
            float duration)
        {
            _host = host;
            _a = a;
            _b = b;
            _aTarget = aTarget;
            _bTarget = bTarget;
            _duration = duration;
        }

        public IEnumerator Play()
        {
            Vector3 aStart = _a.transform.position;
            Vector3 bStart = _b.transform.position;

            // swap forward
            yield return CoroutineUtil.RunParallel(
                _host,
                _a.AnimateMove(_aTarget, _duration),
                _b.AnimateMove(_bTarget, _duration)
            );

            // swap back
            yield return CoroutineUtil.RunParallel(
                _host,
                _a.AnimateMove(aStart, _duration),
                _b.AnimateMove(bStart, _duration)
            );
        }
    }
}