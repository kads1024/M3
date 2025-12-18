using System.Collections.Generic;
using M3.Core.Domain;

namespace M3.Presentation.Playback
{
    public sealed class BoardSnapshot
    {
        public readonly Dictionary<Position, GemState> Cells;

        public BoardSnapshot(BoardState board)
        {
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

    }
}