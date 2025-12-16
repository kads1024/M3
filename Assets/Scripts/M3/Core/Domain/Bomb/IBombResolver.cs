using System.Collections.Generic;
using M3.Core.Domain;

namespace M3.Core.Domain.Bomb
{
    public interface IBombResolver
    {
        IReadOnlyCollection<Position> Resolve(
            BoardState board,
            Position bombPosition);
    }

}