using System.Collections.Generic;

namespace M3.Core.Domain.Match
{
    public sealed class MatchGroup
    {
        public GemColor Color { get; }
        public MatchDirection Direction { get; }
        public IReadOnlyList<Position> Positions { get; }

        public MatchGroup(
            GemColor color,
            MatchDirection direction,
            IReadOnlyList<Position> positions)
        {
            Color = color;
            Direction = direction;
            Positions = positions;
        }

        public int Length => Positions.Count;
    }
}