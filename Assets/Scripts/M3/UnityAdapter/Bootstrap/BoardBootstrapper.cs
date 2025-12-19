using UnityEngine;
using M3.Core.Domain;

namespace M3.UnityAdapter.Bootstrap
{
    public sealed class BoardBootstrapper : MonoBehaviour
    {
        [Header("Board Config")]
        [SerializeField] private int _boardSeed = 0;
        [SerializeField] private int _width = 8;
        [SerializeField] private int _height = 8;

        [Header("Spatial Config")]
        [SerializeField] private float _cellSize = 1f;
        [SerializeField] private Vector2 _boardOriginOffset = Vector3.zero;

        private BoardState _board;
        private BoardSpatialMap _spatialMap; // BoardView Owns the pool. So no need to inject
        
        public BoardState Board => _board;
        
        public BoardSpatialMap SpatialMap => _spatialMap;

        private System.Random _random;
        private void Awake()
        {
            _random = new System.Random(Seed: _boardSeed);
            CreateBoard();
            CreateSpatialMap();
        }

        private void CreateBoard()
        {
            _board = new BoardState(_width, _height);

            // Temporary initialization
            for (int x = 0; x < _board.Width; x++)
            for (int y = 0; y < _board.Height; y++)
            {
                var color = (GemColor)_random.Next(5);
                _board.SetGem(x, y, new GemState(color, GemType.Normal));
            }
        }

        private void CreateSpatialMap()
        {
            // World-space origin of the board
            Vector3 origin = transform.position + new Vector3(_boardOriginOffset.x, _boardOriginOffset.y, 0);

            _spatialMap = new BoardSpatialMap(
                board: _board,
                cellSize: _cellSize,
                origin: origin);
        }
    }
}