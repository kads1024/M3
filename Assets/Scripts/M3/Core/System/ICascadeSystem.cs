using M3.Core.Domain;

namespace M3.Core.System
{
    public interface ICascadeSystem
    {
        void Resolve(
            BoardState board,
            bool isPlayerMove,
            Position swapA,
            Position swapB);
    }
}