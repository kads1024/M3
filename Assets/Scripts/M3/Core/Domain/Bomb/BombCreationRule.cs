using M3.Core.Domain.Match;

namespace M3.Core.Domain.Bomb
{
    public sealed class BombCreationRule : IBombCreationRule
    {
        private const int MinimumMatchSize = 4;

        public BombCreationResult? TryCreate(
            ClassifiedMatch match,
            Position swapOrigin,
            bool isPlayerMove,
            int Id)
        {
            if (!isPlayerMove)
                return null;

            if (match.Positions.Count < MinimumMatchSize)
                return null;

            return new BombCreationResult(
                Id,
                swapOrigin,
                match.Color);
        }
    }
}