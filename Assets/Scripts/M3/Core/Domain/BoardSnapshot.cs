using System.Collections.Generic;

namespace M3.Core.Domain
{
    /// <summary>
    /// A copy of a state of the board
    /// </summary>
    public sealed class BoardSnapshot
    {
        public int Width { get; }
        public int Height { get; }
        
        public readonly Dictionary<Position, GemState> Cells;
        public readonly Dictionary<int, Position> CellsById;
        
        public BoardSnapshot(BoardState board)
        {
            Width = board.Width;
            Height = board.Height;

            Cells = new Dictionary<Position, GemState>();
            CellsById =  new Dictionary<int, Position>();
            for (int x = 0; x < board.Width; x++)
            for (int y = 0; y < board.Height; y++)
            {
                var cell = board.GetCell(x, y);
                if (!cell.IsEmpty)
                {
                    var position = new Position(x, y);
                    Cells[position] = cell.Gem.Clone();
                    CellsById[cell.Gem.Id] = position;
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
    }
}