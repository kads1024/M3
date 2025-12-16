using System;
using System.Collections.Generic;
using M3.Core.Domain;

namespace M3.Core.System
{
    public sealed class GemSpawner : IGemSpawner
    {
        private readonly Random _random;
        private readonly GemColor[] _availableColors;

        public GemSpawner(int seed)
        {
            _random = new Random(seed);
            _availableColors = (GemColor[])Enum.GetValues(typeof(GemColor));

            if (_availableColors.Length < 3)
                throw new InvalidOperationException(
                    "SafeGemSpawner requires at least 3 gem colors.");
        }

        public void Spawn(BoardState board)
        {
            for (int x = 0; x < board.Width; x++)
            {
                SpawnColumn(board, x);
            }
        }

        private void SpawnColumn(BoardState board, int x)
        {
            // From top to bottom
            for (int y = board.Height - 1; y >= 0; y--)
            {
                var cell = board.GetCell(x, y);
                if (!cell.IsEmpty)
                    continue;

                var color = ChooseSafeColor(board, x, y);
                cell.SetGem(new GemState(color, GemType.Normal));
            }
        }

        private GemColor ChooseSafeColor(BoardState board, int x, int y)
        {
            var candidates = new List<GemColor>(_availableColors);

            // Prevent vertical match
            if (y >= 2)
            {
                var below1 = board.GetCell(x, y - 1);
                var below2 = board.GetCell(x, y - 2);

                if (!below1.IsEmpty && !below2.IsEmpty &&
                    below1.Gem.Color == below2.Gem.Color)
                {
                    candidates.Remove(below1.Gem.Color);
                }
            }

            // Prevent horizontal match (left)
            if (x >= 2)
            {
                var left1 = board.GetCell(x - 1, y);
                var left2 = board.GetCell(x - 2, y);

                if (!left1.IsEmpty && !left2.IsEmpty &&
                    left1.Gem.Color == left2.Gem.Color)
                {
                    candidates.Remove(left1.Gem.Color);
                }
            }

            // Prevent horizontal match (right)
            if (x + 2 < board.Width)
            {
                var right1 = board.GetCell(x + 1, y);
                var right2 = board.GetCell(x + 2, y);

                if (!right1.IsEmpty && !right2.IsEmpty &&
                    right1.Gem.Color == right2.Gem.Color)
                {
                    candidates.Remove(right1.Gem.Color);
                }
            }

            if (candidates.Count == 0)
                throw new InvalidOperationException(
                    $"No valid gem color available at ({x},{y}).");

            return candidates[_random.Next(candidates.Count)];
        }
    }
}
