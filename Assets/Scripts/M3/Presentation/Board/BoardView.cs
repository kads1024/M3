using UnityEngine;
using VContainer;
using System.Collections.Generic;
using M3.Core.Domain;
using M3.Application.Bootstrap;
using M3.UnityAdapter;
using M3.Presentation.Gem;

namespace M3.Presentation.Board
{
    public sealed class BoardView : MonoBehaviour
    {
        [Inject] private BoardBootstrapper _bootstrapper;

        private BoardState _board;
        private BoardSpatialMap _spatialMap;

        private readonly Dictionary<Position, GemView> _gemViews = new();

        private void Start()
        {
            _board = _bootstrapper.Board;
            _spatialMap = _bootstrapper.SpatialMap;

            RenderInitialBoard();
        }

        private void RenderInitialBoard()
        {
            for (int x = 0; x < _board.Width; x++)
            for (int y = 0; y < _board.Height; y++)
            {
                var cell = _board.GetCell(x, y);
                if (cell.IsEmpty)
                    continue;

                var pos = new Position(x, y);
                CreateGemView(pos, cell.Gem);
            }
        }

        private void CreateGemView(Position pos, GemState gem)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            go.name = $"GemView ({pos.X},{pos.Y})";
            go.transform.SetParent(transform);

            var view = go.AddComponent<GemView>();
            view.Initialize(pos, gem);

            go.transform.position = _spatialMap.GridToWorld(pos);
            go.transform.localScale *= _spatialMap.CellSize;
                
            _gemViews[pos] = view;
        }
    }
}