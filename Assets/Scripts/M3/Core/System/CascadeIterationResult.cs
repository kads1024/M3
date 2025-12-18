using System.Collections.Generic;
using M3.Core.Domain.Bomb;

namespace M3.Core.System
{
    public sealed class CascadeIterationResult
    {
        public bool Resolved { get; }
        public BombPlacement? BombPlacement { get; }
        public List<BombTrigger> BombTriggers { get; }
        public CascadeIterationResult(
            bool resolved,
            BombPlacement? bombPlacement,
            List<BombTrigger> bombTrigger)
        {
            Resolved = resolved;
            BombPlacement = bombPlacement;
            BombTriggers = bombTrigger;
        }
    }

}