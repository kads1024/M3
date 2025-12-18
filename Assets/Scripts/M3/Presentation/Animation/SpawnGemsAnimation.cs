using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M3.Presentation.Board;
using M3.UnityAdapter;
using M3.Core.Domain;

namespace M3.Presentation.Animation
{
    public sealed class SpawnGemsAnimation : IBoardAnimation
    {
        private readonly MonoBehaviour _host;
        private readonly BoardView _boardView;
        private readonly BoardSpatialMap _spatialMap;
        private readonly BoardSnapshot _boardSnapshot;
        private readonly IReadOnlyList<ColumnSpawn> _spawns;
        private readonly float _spawnDuration;

        public SpawnGemsAnimation(
            MonoBehaviour host,
            BoardView boardView,
            BoardSpatialMap spatialMap,
            IReadOnlyList<ColumnSpawn> spawns,
            float spawnDuration,
            BoardSnapshot boardSnapshot)
        {
            _host = host;
            _boardView = boardView;
            _spatialMap = spatialMap;
            _spawns = spawns;
            _spawnDuration = spawnDuration;
            _boardSnapshot = boardSnapshot;
        }

        public IEnumerator Play()
        {
            var routines = new List<IEnumerator>();

            foreach (var column in _spawns)
            {
                foreach (var gem in column.Gems)
                {
                    // Create view using BoardView’s normal path
                    var view = _boardView.CreateGemView(gem.To, gem.Gem);

                    // Logical position is final
                    view.SetLogicalPosition(gem.To);

                    // Compute spawn start position ABOVE the board
                    float cellSize = _spatialMap.CellSize;

                    Vector3 startWorld =
                        _spatialMap.GridToWorld(
                            new Position(
                                gem.To.X,
                                _boardSnapshot.Height + gem.StackIndex));

                    // Offset by half cell so it centers correctly
                    // startWorld += (Vector3.up * (cellSize * 0.5f));


                    view.transform.position = startWorld;

                }
            }

            if (routines.Count > 0)
                yield return CoroutineUtil.RunParallel(
                    _host,
                    routines.ToArray());
        }
    }
}
