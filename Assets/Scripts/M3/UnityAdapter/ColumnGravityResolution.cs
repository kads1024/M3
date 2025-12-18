using System.Collections.Generic;
using M3.Core.Domain;


namespace M3.UnityAdapter
{
    public sealed class ColumnGravityResolution
    {
        public int Column { get; }
        public IReadOnlyList<GemFall> Falls { get; }
    }

    public sealed class GemFall
    {
        public int GemId { get; }
        public Position From { get; }
        public Position To { get; }
    }

    
}