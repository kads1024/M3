using M3.Core.Domain;
using M3.Core.Domain.Swap;

namespace M3.Core.System
{
    public interface ICascadeSystem
    {
        void Resolve(BoardState board, SwapContext context);
        bool ResolveOneIteration(BoardState board, SwapContext context, ref bool isPlayerMove);
    }
}