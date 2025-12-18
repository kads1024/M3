using System.Collections.Generic;
using M3.Core.Domain;


namespace M3.UnityAdapter
{
    public sealed class ColumnGravityResolution
    {
        public int Column;
        public IReadOnlyList<GemFall> Falls;
    }

    public sealed class GemFall
    {
        public int GemId;
        public Position From;
        public Position To;
    }
    
}