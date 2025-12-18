using M3.Core.Domain;

namespace M3.Core.Domain.Bomb
{
    public sealed class BombPlacement
    {
        public int BombGemId { get; }
        public Position Position { get; }

        public BombPlacement(int bombGemId, Position position)
        {
            BombGemId = bombGemId;
            Position = position;
        }
    }
}