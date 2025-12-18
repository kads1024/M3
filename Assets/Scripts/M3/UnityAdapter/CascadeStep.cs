using System.Collections.Generic;
using M3.Core.Domain;
using M3.UnityAdapter;

namespace M3.UnityAdapter
{
    public sealed class CascadeStep
    {
        public BoardSnapshot Before { get; }
        public BoardSnapshot After { get; }


        public HashSet<int> ClearedGemIds { get; }
        public IReadOnlyList<ColumnGravityResolution> Gravity { get; }
        public IReadOnlyList<BombTrigger> BombTriggers { get; }
        
        public BombPlacement? BombPlacement { get; }
        
        public CascadeStep(
            BoardSnapshot before,
            BoardSnapshot after,
            HashSet<int> clearedGemIds,
            IReadOnlyList<ColumnGravityResolution> gravity,
            IReadOnlyList<BombTrigger> bombTriggers,
            BombPlacement? bombPlacement)
        {
            Before = before;
            After = after;
            ClearedGemIds = clearedGemIds;
            Gravity = gravity;
            BombTriggers = bombTriggers;
            BombPlacement = bombPlacement;

        }
    }
}