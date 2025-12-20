using System.Collections;
using M3.Presentation.Gem;
using UnityEngine;

namespace M3.Presentation.Animation
{
    public class SwapAnimation: IBoardAnimation
    {
        private readonly MonoBehaviour _host;
        private readonly GemView _a;
        private readonly GemView _b;
        private readonly Vector3 _aTarget;
        private readonly Vector3 _bTarget;
        private readonly float _duration;
        private readonly bool _accepted;

        public SwapAnimation(
            MonoBehaviour host,
            GemView a,
            GemView b,
            Vector3 aTarget,
            Vector3 bTarget,
            float duration, 
            bool accepted)
        {
            _host = host;
            _a = a;
            _b = b;
            _aTarget = aTarget;
            _bTarget = bTarget;
            _duration = duration;
            _accepted = accepted;
        }

        public IEnumerator Play()
        {
            if (!_accepted)
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
            else
            {
                yield return CoroutineUtil.RunParallel(
                    _host,
                    _a.AnimateMove(_aTarget, _duration),
                    _b.AnimateMove(_bTarget, _duration)
                );
            }
           
        }
    }
}