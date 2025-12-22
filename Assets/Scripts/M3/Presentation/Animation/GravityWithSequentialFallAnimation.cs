using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M3.Presentation.Board;
using M3.Core.Domain;

namespace M3.Presentation.Animation
{
    public sealed class GravityWithSequentialFallAnimation : IBoardAnimation
    {
        private readonly MonoBehaviour _host;
        private readonly BoardView _boardView;
        private readonly BoardSpatialMap _spatialMap;
        private readonly IReadOnlyList<ColumnGravityResolution> _columns;
        private readonly float _fallDuration;
        private readonly float _delayBetween;

        public GravityWithSequentialFallAnimation(
            MonoBehaviour host,
            BoardView boardView,
            BoardSpatialMap spatialMap,
            IReadOnlyList<ColumnGravityResolution> columns,
            float fallDuration,
            float delayBetween)
        {
            _host = host;
            _boardView = boardView;
            _spatialMap = spatialMap;
            _columns = columns;
            _fallDuration = fallDuration;
            _delayBetween = delayBetween;
        }

        public IEnumerator Play()
        {
            foreach (var column in _columns)
            {
                foreach (var fall in column.Falls)
                {
                    var view = _boardView.GetGemViewById(fall.GemId);
                    if (!view)
                        continue;

                    // Compute target world position
                    Vector3 target =
                        _spatialMap.GridToWorld(fall.To);

                    // Animate movement
                    yield return view.AnimateMove(target, _fallDuration);

                    // Update logical position ONLY after landing
                    view.SetLogicalPosition(fall.To);

                    yield return new WaitForSeconds(_delayBetween);
                }
            }
        }
    }
}