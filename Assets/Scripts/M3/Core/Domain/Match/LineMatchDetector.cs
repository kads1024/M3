using System.Collections.Generic;

namespace M3.Core.Domain.Match
{
    public sealed class LineMatchDetector : IMatchDetector
    {
        public IReadOnlyList<MatchGroup> Detect(BoardState board)
        {
            var results = new List<MatchGroup>();

            DetectHorizontal(board, results);
            DetectVertical(board, results);

            return results;
        }

        // Implementation comes AFTER tests
        private void DetectHorizontal(BoardState board, List<MatchGroup> results) { }
        private void DetectVertical(BoardState board, List<MatchGroup> results) { }
    }
}