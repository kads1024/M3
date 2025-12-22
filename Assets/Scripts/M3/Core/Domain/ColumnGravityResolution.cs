using System.Collections.Generic;

namespace M3.Core.Domain
{
    /// <summary>
    /// Data containing the Before and after positions of the gems affected by gravity
    /// </summary>
    public sealed class ColumnGravityResolution
    {
        public int Column { get; }
        public IReadOnlyList<GemFall> Falls { get; }

        public ColumnGravityResolution(int column, IReadOnlyList<GemFall> falls)
        {
            Column = column;
            Falls = falls;
        }
    }

    public sealed class GemFall
    {
        public int GemId { get; }
        public Position From { get; }
        public Position To { get; }

        public GemFall(int gemId, Position from, Position to)
        {
            GemId = gemId;
            From = from;
            To = to;
        }
    }

    
}