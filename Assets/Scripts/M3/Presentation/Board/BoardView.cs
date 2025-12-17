using UnityEngine;
using M3.Core.Domain;
using M3.Application.Bootstrap;
using VContainer;

namespace M3.Presentation.Board
{
    public sealed class BoardView : MonoBehaviour
    {
        [SerializeField] private float _cellSize = 1f;
        private BoardState _board;

        [Inject]
        private BoardBootstrapper _bootstrapper;

        private void Start()
        {
            _board = _bootstrapper.Board;

            if (_board == null)
            {
                Debug.LogError("BoardView: BoardState is null");
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

                CreateGemView(cell.Gem, x, y);
            }
        }

        private void CreateGemView(GemState gem, int x, int y)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            go.name = $"Gem ({x},{y})";

            go.transform.SetParent(transform);
            go.transform.localPosition = new Vector3(
                x * _cellSize,
                y * _cellSize,
                0);

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