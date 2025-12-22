using System.Collections.Generic;

namespace M3.Core.Domain.Match
{
    /// <summary>
    /// Data containing matches after classification (L Shape, T Shape, Line)
    /// </summary>
    public sealed class ClassifiedMatch
    {
        public GemColor Color { get; }
        public MatchPattern Pattern { get; }
        public IReadOnlyList<Position> Positions { get; }

        public ClassifiedMatch(
            GemColor color,
            MatchPattern pattern,
            IReadOnlyList<Position> positions)
        {
            Color = color;
            Pattern = pattern;
            Positions = positions;
        }
    }
}