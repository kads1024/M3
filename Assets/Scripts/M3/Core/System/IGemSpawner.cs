using M3.Core.Domain;

namespace M3.Core.System
{
    public interface IGemSpawner
    {
        void Spawn(BoardState board);
    }
}