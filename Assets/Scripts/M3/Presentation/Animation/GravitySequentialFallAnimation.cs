using System.Collections;
using System.Collections.Generic;
using System.Linq;
using M3.Core.Domain;
using UnityEngine;
using M3.Presentation.Animation;
using M3.Presentation.Board;
using M3.Presentation.Gem;


namespace M3.Presentation.Playback
{
    public sealed class GravitySequentialFallAnimation : IBoardAnimation
    {
        private readonly MonoBehaviour _host;
        private readonly BoardView _boardView;
        private readonly BoardSpatialMap _spatialMap;
        private readonly List<GravityFallInstruction> _falls;
        private readonly float _fallDuration;
        private readonly float _delayBetween;

        public GravitySequentialFallAnimation(
            MonoBehaviour host,
            BoardView boardView,
            BoardSpatialMap spatialMap,
            List<GravityFallInstruction> falls,
            float fallDuration,
            float delayBetween)
        {
            _host = host;
            _boardView = boardView;
            _spatialMap = spatialMap;
            _falls = falls;
            _fallDuration = fallDuration;
            _delayBetween = delayBetween;
        }

        public IEnumerator Play()
        {
            var byColumn = _falls
                .GroupBy(f => f.From.X)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(f => f.From.Y).ToList());

            foreach (var column in byColumn.Values)
            {
                // Start a coroutine per column
                _host.StartCoroutine(PlayColumn(column));
            }

            // Wait until all animations finish
            float maxTime =
                byColumn.Values.Max(c =>
                    c.Count * (_fallDuration + _delayBetween));

            yield return new WaitForSeconds(maxTime);
        }

        private IEnumerator PlayColumn(List<GravityFallInstruction> column)
        {
            foreach (var fall in column)
            {
                var view = _boardView.GetGemViewAt(fall.From); // identity-based
                if (view == null)
                    continue;

                var target = _spatialMap.GridToWorld(fall.To);
                yield return view.AnimateMove(target, _fallDuration);
                yield return new WaitForSeconds(_delayBetween);
            }
        }
    }
}
