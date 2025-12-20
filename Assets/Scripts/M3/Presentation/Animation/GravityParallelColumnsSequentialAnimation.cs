using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M3.Presentation.Board;

using M3.Core.Domain;

namespace M3.Presentation.Animation
{
    public sealed class GravityParallelColumnsSequentialAnimation : IBoardAnimation
    {
        private readonly MonoBehaviour _host;
        private readonly BoardView _boardView;
        private readonly BoardSpatialMap _spatialMap;
        private readonly IReadOnlyList<ColumnGravityResolution> _columns;
        private readonly float _fallDuration;
        private readonly float _delayBetween;

        public GravityParallelColumnsSequentialAnimation(
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
            var columnRoutines = new List<IEnumerator>();

            foreach (var column in _columns)
            {
                columnRoutines.Add(
                    PlayColumn(column));
            }

            // Run ALL columns at the same time
            yield return CoroutineUtil.RunParallel(
                _host,
                columnRoutines.ToArray());
        }

        private IEnumerator PlayColumn(ColumnGravityResolution column)
        {
            foreach (var fall in column.Falls)
            {
                var view = _boardView.GetGemViewById(fall.GemId);
                if (view == null)
                    continue;

                Vector3 target =
                    _spatialMap.GridToWorld(fall.To);

                // Animate fall
                view.AnimateMove(target, _fallDuration);

                // Update logical position after landing
                view.SetLogicalPosition(fall.To);

                yield return new WaitForSeconds(_delayBetween);
            }
        }
    }
}
