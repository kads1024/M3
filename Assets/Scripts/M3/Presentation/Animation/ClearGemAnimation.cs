using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M3.Core.Domain;
using M3.Presentation.Board;


namespace M3.Presentation.Animation
{
    public sealed class ClearGemsAnimation : IBoardAnimation
    {
        private readonly MonoBehaviour _host;
        private readonly BoardView _boardView;
        private readonly IReadOnlyList<Position> _positions;
        private readonly float _duration;

        public ClearGemsAnimation(
            MonoBehaviour host,
            BoardView boardView,
            IReadOnlyList<Position> positions,
            float duration)
        {
            _host = host;
            _boardView = boardView;
            _positions = positions;
            _duration = duration;
        }

        public IEnumerator Play()
        {
            var routines = new List<IEnumerator>();

            foreach (var pos in _positions)
            {
                var view = _boardView.GetGemViewAt(pos);
                if (view == null)
                    continue;

                routines.Add(view.ClearGem(_duration));
            }

            if (routines.Count > 0)
                yield return CoroutineUtil.RunParallel(_host, routines.ToArray());

            // Destroy cleared gems visually
            foreach (var pos in _positions)
            {
                _boardView.DestroyGemViewAt(pos);
            }
        }
    }
}