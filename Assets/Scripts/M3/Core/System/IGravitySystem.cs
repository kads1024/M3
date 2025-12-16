using M3.Core.Domain;

namespace M3.Core.System
{
    public interface IGravitySystem
    {
        void Apply(BoardState board);
    }
}