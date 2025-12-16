using M3.Core.Domain.Match;

namespace M3.Core.Domain.Bomb
{
    public interface IBombCreationRule
    {
        BombCreationResult? TryCreate(
            ClassifiedMatch match,
            Position swapOrigin,
            bool isPlayerMove);
    }
}