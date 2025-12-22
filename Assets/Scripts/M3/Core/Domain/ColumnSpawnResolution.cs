using System.Collections.Generic;

namespace M3.Core.Domain
{
    /// <summary>
    /// Data containing gems that were spawned and where they were spawned
    /// </summary>
    public sealed class ColumnSpawn
    {
        public int Column { get; }
        public IReadOnlyList<SpawnedGem> Gems { get; }

        public ColumnSpawn(int column, IReadOnlyList<SpawnedGem> gems)
        {
            Column = column;
            Gems = gems;
        }
    }

    public sealed class SpawnedGem
    {
        public GemState Gem { get; }
        public Position To { get; }
        public int StackIndex { get; }

        public SpawnedGem(GemState gem, Position to, int stackIndex)
        {
            Gem = gem;
            To = to;
            StackIndex = stackIndex;
        }
    }
    
}