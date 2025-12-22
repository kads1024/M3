using M3.Core.Domain.Match;

namespace M3.Core.Domain.Bomb
{
    public interface IBombCreationRule
    {
        /// <summary>
        /// Dictates when and how a bomb is created
        /// </summary>
        BombCreationResult? TryCreate(
            ClassifiedMatch match,
            Position swapOrigin,
            bool isPlayerMove, 
            int Id);
    }
}