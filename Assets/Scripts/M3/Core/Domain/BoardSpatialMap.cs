using UnityEngine;
using M3.Core.Domain;

namespace M3.Core.Domain
{
    public sealed class BoardSpatialMap
    {
        private readonly BoardState _board;
        private readonly float _cellSize;
        private readonly Vector3 _origin;

        private readonly float _halfCell;

        public float CellSize => _cellSize;
        public Vector3 Origin => _origin;
        
        public BoardSpatialMap(BoardState board, float cellSize, Vector3 origin)
        {
            _board = board;
            _cellSize = cellSize;
            _origin = origin;
            _halfCell = cellSize * 0.5f;
        }
        
        public Vector3 GridToWorld(Position pos)
        {
            return _origin + new Vector3(
                (pos.X * _cellSize) + _halfCell,
                (pos.Y * _cellSize) + _halfCell,
                0f);
        }
        
        public bool TryWorldToGrid(Vector3 world, out Position position)
        {
            Vector3 local = world - _origin;

            int x = Mathf.FloorToInt((local.x) / _cellSize);
            int y = Mathf.FloorToInt((local.y) / _cellSize);

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