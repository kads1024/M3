using System.Collections.Generic;

namespace M3.Core.Domain.Bomb
{
    /// <summary>
    /// Separate system for computing additional cells to destroy
    /// </summary>
    public interface IBombResolver
    {
        IReadOnlyCollection<Position> Resolve(
            BoardState board,
            Position bombPosition);
    }

}