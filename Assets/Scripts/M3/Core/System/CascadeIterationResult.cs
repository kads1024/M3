using System.Collections.Generic;
using M3.Core.Domain.Bomb;

namespace M3.Core.System
{
    public sealed class  CascadeIterationResult
    {
        public bool Unresolved { get; }
        public BombPlacement? BombPlacement { get; }
        public List<BombTrigger> BombTriggers { get; }
        public CascadeIterationResult(
            bool unresolved,
            BombPlacement? bombPlacement,
            List<BombTrigger> bombTrigger)
        {
            Unresolved = unresolved;
            BombPlacement = bombPlacement;
            BombTriggers = bombTrigger;
        }
    }

}