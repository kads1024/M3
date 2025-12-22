namespace M3.Core.Domain.Bomb
{
    /// <summary>
    /// Data about where to place the bomb
    /// </summary>
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