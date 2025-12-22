using System.Collections.Generic;

namespace M3.Core.Domain.Match
{
    /// <summary>
    /// Data containing list of gems that were grouped together
    /// </summary>
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