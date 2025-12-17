using UnityEngine;
using VContainer;
using M3.Core.Domain;
using M3.Application.Bootstrap;
using M3.UnityAdapter;

namespace M3.Presentation.Board
{
    public sealed class BoardView : MonoBehaviour
    {
        [Inject] private BoardBootstrapper _bootstrapper;
        private BoardSpatialMap _spatialMap;

        private BoardState _board;

        private void Start()
        {
            _board = _bootstrapper.Board;
            _spatialMap = _bootstrapper.SpatialMap;
            
            if (_board == null)
            {
                Debug.LogError("BoardView: BoardState is null");
                return;
            }
            
            if (_spatialMap == null)
            {
                Debug.LogError("BoardView: BoardSpatialMap is null");
                return;
            }
            RenderBoard();
        }

        private void RenderBoard()
        {
            for (int x = 0; x < _board.Width; x++)
            for (int y = 0; y < _board.Height; y++)
            {
                var cell = _board.GetCell(x, y);
                if (cell.IsEmpty)
                    continue;

                CreateGemView(cell.Gem, new Position(x, y));
            }
        }

        private void CreateGemView(GemState gem, Position pos)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            go.name = $"Gem ({pos.X},{pos.Y})";

            go.transform.SetParent(transform);
            go.transform.position = _spatialMap.GridToWorld(pos);

            var renderer = go.GetComponent<Renderer>();
            renderer.material.color = ColorFor(gem.Color);
        }

        private static Color ColorFor(GemColor color)
        {
            return color switch
            {
                GemColor.Red => Color.red,
                GemColor.Blue => Color.blue,
                GemColor.Green => Color.green,
                GemColor.Yellow => Color.yellow,
                GemColor.Purple => Color.magenta,
                _ => Color.white
            };
        }
    }
}