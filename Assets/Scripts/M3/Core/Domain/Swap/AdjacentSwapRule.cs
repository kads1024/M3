using System;
using M3.Core.Domain.Match;

namespace M3.Core.Domain.Swap
{
    /// <summary>
    /// Default ISwapRule where swap is valid if gem is Orthogonally adjacent to cell to be swapped
    /// </summary>
    public sealed class AdjacentSwapRule : ISwapRule
    {
        private readonly IMatchDetector _matchDetector;

        public AdjacentSwapRule(IMatchDetector matchDetector)
        {
            _matchDetector = matchDetector;
        }

        public bool IsSwapValid(
            BoardState board,
            Position a,
            Position b)
        {
            // 1. Adjacency rule
            if (!AreAdjacent(a, b))
                return false;

            // 2. Clone board (simulation)
            var simulatedBoard = board.Clone();

            // 3. Apply swap
            simulatedBoard.Swap(a, b);

            // 4. Detect matches
            var matches = _matchDetector.Detect(simulatedBoard);

            // 5. Legal only if a match exists
            return matches.Count > 0;
        }

        private static bool AreAdjacent(Position a, Position b)
        {
            int dx = Math.Abs(a.X - b.X);
            int dy = Math.Abs(a.Y - b.Y);

            // Orthogonal adjacency only
            return (dx == 1 && dy == 0) ||
                   (dx == 0 && dy == 1);
        }
    }


}