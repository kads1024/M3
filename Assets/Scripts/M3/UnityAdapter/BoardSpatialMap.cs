using UnityEngine;
using M3.Core.Domain;
namespace M3.UnityAdapter
{
    public sealed class BoardSpatialMap
    {
        private readonly float _cellSize;
        private readonly Vector3 _origin;
        private readonly BoardState _board;
        
        public float CellSize => _cellSize;
        
        public BoardSpatialMap(BoardState board, float cellSize, Vector3 origin)
        {
            _board = board;
            _cellSize = cellSize;
            _origin = origin;
            
            Debug.Log($"BoardSpatialMap created at origin: ({_origin.x},  {_origin.y}, {_origin.z})");
        }

        public Vector3 GridToWorld(Position pos)
        {
            return _origin + new Vector3(
                pos.X * _cellSize,
                pos.Y * _cellSize,
                0f);
        }

        public bool TryWorldToGrid(Vector3 world, out Position position)
        {
            Vector3 local = world - _origin;

            int x = Mathf.FloorToInt(local.x / _cellSize);
            int y = Mathf.FloorToInt(local.y / _cellSize);

            if (_board.IsInside(x, y))
            {
                position = new Position(x, y);
                return true;
            }

            position = default;
            return false;
        }
    }
}
