using M3.Core.Domain;

namespace M3.Core.System
{
    /// <summary>
    /// Responsible for spawning logical gems on the board
    /// </summary>
    public interface IGemSpawner
    {
        void Spawn(BoardState board);
    }
}