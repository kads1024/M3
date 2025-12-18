using M3.Core.Domain;

namespace M3.Presentation.Animation
{
    public sealed class GravityFallInstruction
    {
        public Position From { get; }
        public Position To { get; }

        public GravityFallInstruction(Position from, Position to)
        {
            From = from;
            To = to;
        }
    }
}