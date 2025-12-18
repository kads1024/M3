using System.Collections.Generic;

namespace M3.Core.Domain.Bomb
{
    public sealed class BombTrigger
    {
        public int BombGemId { get; }
        public Position Position { get; }
        public IReadOnlyList<int> AffectedGemIds { get; }

        public BombTrigger(
            int bombGemId,
            Position position,
            IReadOnlyList<int> affectedGemIds)
        {
            BombGemId = bombGemId;
            Position = position;
            AffectedGemIds = affectedGemIds;
        }
    }
}