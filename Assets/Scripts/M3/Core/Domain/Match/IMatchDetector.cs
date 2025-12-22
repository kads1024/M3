using System.Collections.Generic;

namespace M3.Core.Domain.Match
{
    /// <summary>
    /// Detects raw matches on the board without board mutation. 
    /// </summary>
    public interface IMatchDetector
    {
        IReadOnlyList<MatchGroup> Detect(BoardState board);
    }
}