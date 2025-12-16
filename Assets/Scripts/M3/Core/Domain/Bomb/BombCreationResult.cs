namespace M3.Core.Domain.Bomb
{
    public sealed class BombCreationResult
    {
        public Position Position { get; }
        public GemColor Color { get; }

        public BombCreationResult(Position position, GemColor color)
        {
            Position = position;
            Color = color;
        }
    }
}