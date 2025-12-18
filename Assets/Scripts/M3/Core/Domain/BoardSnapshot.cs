using System.Collections.Generic;
using M3.Core.Domain;

namespace M3.Core.Domain
{
    public sealed class BoardSnapshot
    {
        public int Width { get; }
        public int Height { get; }
        
        public readonly Dictionary<Position, GemState> Cells;

        public BoardSnapshot(BoardState board)
        {
            Width = board.Width;
            Height = board.Height;

            Cells = new Dictionary<Position, GemState>();

            for (int x = 0; x < board.Width; x++)
            for (int y = 0; y < board.Height; y++)
            {
                var cell = board.GetCell(x, y);
                if (!cell.IsEmpty)
                {
                    Cells[new Position(x, y)] = cell.Gem.Clone();
                }
            }
        }

        public bool TryFindById(int id, out Position position)
        {
            foreach (var kv in Cells)
            {
                if (kv.Value.Id == id)
                {
                    position = kv.Key;
                    return true;
                }
            }

            position = default;
            return false;
        }

        /// <summary>
        /// Returns gem IDs in a column, bottom → top.
        /// </summary>
        public IReadOnlyList<int> GetColumnGemIds(int x)
        {
            var list = new List<int>();

            for (int y = 0; y < Height; y++)
            {
                if (Cells.TryGetValue(new Position(x, y), out var gem))
                {
                    list.Add(gem.Id);
                }
            }

            return list;
        }
    }
}