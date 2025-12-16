using M3.Core.Domain;

namespace M3.Core.System
{
    public sealed class GravitySystem : IGravitySystem
    {
        public void Apply(BoardState board)
        {
            for (int x = 0; x < board.Width; x++)
            {
                ApplyGravityToColumn(board, x);
            }
        }

        private static void ApplyGravityToColumn(BoardState board, int x)
        {
            int writeY = 0;

            // Read from bottom to top
            for (int readY = 0; readY < board.Height; readY++)
            {
                var cell = board.GetCell(x, readY);
                if (cell.IsEmpty)
                    continue;

                if (readY != writeY)
                {
                    board.SetGem(x, writeY, cell.Gem);
                    board.ClearGem(x, readY);
                }

                writeY++;
            }
        }
    }
}