namespace M3.Core.Domain.Bomb
{
    /// <summary>
    /// Data about the creation of Bombs
    /// </summary>
    public sealed class BombCreationResult
    {
        public Position Position { get; }
        public GemColor Color { get; }
        public int Id { get; }
        public BombCreationResult(int id, Position position, GemColor color)
        {
            Position = position;
            Color = color;
            Id = id;
        }
    }
}