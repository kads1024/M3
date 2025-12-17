using System;
using UnityEngine;
using M3.Core.Domain;

namespace M3.Application.Bootstrap
{
    public sealed class BoardBootstrapper : MonoBehaviour
    {
        [SerializeField] private int _seed = 0;
        [SerializeField] private int _width = 8;
        [SerializeField] private int _height = 8;

        private System.Random _random;
        
        private BoardState _board;

        public BoardState Board => _board;

        private void Awake()
        {
            _random = new System.Random(_seed);
            CreateBoard();
        }

        private void CreateBoard()
        {
            _board = new BoardState(_width, _height);

            // Temporary initialization (to improve later)
            FillBoardRandomly();
            Debug.Log($"Board created: {_width} x {_height}");
        }

        private void FillBoardRandomly()
        {
            for (int x = 0; x < _board.Width; x++)
            for (int y = 0; y < _board.Height; y++)
            {
                var color = (GemColor)_random.Next(5);
                _board.SetGem(x, y, new GemState(color, GemType.Normal));
            }
        }
    }
}