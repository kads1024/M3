using System.Collections.Generic;
using M3.Core.Domain.Bomb;

namespace M3.Core.Domain
{
    /// <summary>
    /// One cascade step refers to one iteration of resolution regardless of board stability (One pass of [swap>clear>gravity>spawn])
    /// </summary>
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