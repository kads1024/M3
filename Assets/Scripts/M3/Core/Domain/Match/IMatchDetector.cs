using System.Collections.Generic;

namespace M3.Core.Domain.Match
{
    public interface IMatchDetector
    {
        IReadOnlyList<MatchGroup> Detect(BoardState board);
    }
}