using M3.Core.Domain;
using M3.Core.Domain.Swap;

namespace M3.Core.System
{
    /// <summary>
    /// Responsible for full cascade iteration
    /// Also responsible for coordinating each domain logic (detect>classify>clear>gravity>spawn)
    /// and mutate the board to get a one finalized stable board
    /// </summary>
    public interface ICascadeSystem
    {
        void Resolve(BoardState board, SwapContext context);
        CascadeIterationResult ResolveOneIteration(BoardState board, SwapContext context, ref bool isPlayerMove);
    }
}