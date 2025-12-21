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
            // From bottom to top
            for (int y = 0; y < board.Height; y++)
            {
                var cell = board.GetCell(x, y);
                if (!cell.IsEmpty)
                    continue;

                var color = ChooseSafeColor(board, x, y);
                cell.SetGem(new GemState(color, GemType.Normal));
            }
        }

        // SAFE COLOR = color that will not yield matches
        private GemColor ChooseSafeColor(BoardState board, int x, int y)
        {
            var candidates = new List<GemColor>(_availableColors);

            foreach (var color in _availableColors)
            {
                if (WouldCreateHorizontalMatch(board, x, y, color) ||
                    WouldCreateVerticalMatch(board, x, y, color))
                {
                    candidates.Remove(color);
                }
            }

            if (candidates.Count == 0)
                throw new InvalidOperationException(
                    $"No valid gem color available at ({x},{y}).");

            return candidates[_random.Next(candidates.Count)];
        }
        
        private static bool WouldCreateHorizontalMatch(
            BoardState board, int x, int y, GemColor color)
        {
            // C C X
            if (x >= 2 &&
                HasColor(board, x - 1, y, color) &&
                HasColor(board, x - 2, y, color))
                return true;

            // C X C
            if (x >= 1 && x + 1 < board.Width &&
                HasColor(board, x - 1, y, color) &&
                HasColor(board, x + 1, y, color))
                return true;

            // X C C
            if (x + 2 < board.Width &&
                HasColor(board, x + 1, y, color) &&
                HasColor(board, x + 2, y, color))
                return true;

            return false;
        }

        private static bool WouldCreateVerticalMatch(
            BoardState board, int x, int y, GemColor color)
        {
            // C
            // C
            // X
            if (y >= 2 &&
                HasColor(board, x, y - 1, color) &&
                HasColor(board, x, y - 2, color))
                return true;

            // C
            // X
            // C
            if (y >= 1 && y + 1 < board.Height &&
                HasColor(board, x, y - 1, color) &&
                HasColor(board, x, y + 1, color))
                return true;

            // X
            // C
            // C
            if (y + 2 < board.Height &&
                HasColor(board, x, y + 1, color) &&
                HasColor(board, x, y + 2, color))
                return true;

            return false;
        }
        
        // TODO: Move to a shared helper class?
        private static bool HasColor(BoardState board, int x, int y, GemColor color)
        {
            var cell = board.GetCell(x, y);
            return !cell.IsEmpty && cell.Gem.Color == color;
        }

    }
}
