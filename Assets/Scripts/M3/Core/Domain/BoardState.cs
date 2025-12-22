using System;

namespace M3.Core.Domain
{
    /// <summary>
    /// Current State of the board.
    /// </summary>
    public sealed class BoardState
    {
        private readonly CellState[,] _cells;

        public int Width { get; }
        public int Height { get; }

        public BoardState(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentException("Board dimensions must be positive.");

            Width = width;
            Height = height;

            _cells = new CellState[width, height];

            for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                _cells[x, y] = new CellState(new Position(x, y));
            }
        }

        public CellState GetCell(int x, int y)
        {
            if (!IsInside(x, y))
                throw new ArgumentOutOfRangeException();

            return _cells[x, y];
        }

        public bool IsInside(int x, int y)
        {
            return x >= 0 && x < Width &&
                   y >= 0 && y < Height;
        }
        
        public void Swap(Position a, Position b)
        {
            if (!IsInside(a.X, a.Y) || !IsInside(b.X, b.Y))
                throw new ArgumentOutOfRangeException();

            var cellA = _cells[a.X, a.Y];
            var cellB = _cells[b.X, b.Y];

            var gemA = cellA.Gem;
            var gemB = cellB.Gem;

            if (gemA == null && gemB == null)
                return;

            cellA.SetGem(gemB);
            cellB.SetGem(gemA);
        }
        
        public void SetGem(int x, int y, GemState gem)
        {
            GetCell(x, y).SetGem(gem);
        }

        public void ClearGem(int x, int y)
        {
            GetCell(x, y).ClearGem();
        }

        // Clone functions so that we can test mutation on the clone before applying it to the original board
        public BoardState Clone()
        {
            var clone = new BoardState(Width, Height);

            for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                var sourceCell = _cells[x, y];
                if (!sourceCell.IsEmpty)
                {
                    clone.SetGem(x, y, sourceCell.Gem.Clone());
                }
            }

            return clone;
        }
    }
}