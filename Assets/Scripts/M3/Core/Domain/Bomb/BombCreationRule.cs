using M3.Core.Domain.Match;

namespace M3.Core.Domain.Bomb
{
    /// <summary>
    /// Default IBombCreationRule where match is created when greater than 3 gems were matchedd
    /// </summary>
    public sealed class BombCreationRule : IBombCreationRule
    {
        private const int MinimumMatchSize = 4;

        public BombCreationResult? TryCreate(
            ClassifiedMatch match,
            Position swapOrigin,
            bool isPlayerMove,
            int id)
        {
            if (!isPlayerMove)
                return null;

            if (match.Positions.Count < MinimumMatchSize)
                return null;

            // Created only when match is greater than 3 and came from player input (don't create bomb on fallen matches)
            return new BombCreationResult(
                id,
                swapOrigin,
                match.Color);
        }
    }
}