using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M3.Presentation.Board;
using M3.UnityAdapter;
using M3.Core.Domain;

namespace M3.Presentation.Animation
{
    public sealed class GravityWithSpawnSequentialAnimation : IBoardAnimation
    {
        private readonly MonoBehaviour _host;
        private readonly BoardView _boardView;
        private readonly BoardSpatialMap _spatialMap;
        private readonly IReadOnlyList<ColumnGravityResolution> _columns;
        private readonly float _fallDuration;
        private readonly float _delayBetween;

        public GravityWithSpawnSequentialAnimation(
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
                // Bottom → top
                foreach (var fall in column.Falls)
                {
                    var view = _boardView.GetGemViewById(fall.GemId);

                    // Spawn case: no GemView yet
                    if (view == null)
                    {
                        //view = _boardView.CreateGemViewFromBoard(fall.GemId);
                        view.transform.position =
                            _spatialMap.GridToWorld(fall.From);
                    }

                    // Update logical position immediately
                    view.SetLogicalPosition(fall.To);

                    Vector3 target = _spatialMap.GridToWorld(fall.To);
                    yield return view.AnimateMove(target, _fallDuration);

                    yield return new WaitForSeconds(_delayBetween);
                }
            }
        }
    }
}
