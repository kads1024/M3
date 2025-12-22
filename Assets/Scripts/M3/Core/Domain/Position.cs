using System;

namespace M3.Core.Domain
{
    /// <summary>
    /// Board coordinates
    /// </summary>
    public readonly struct Position
    {
        public int X { get; }
        public int Y { get; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
        public override bool Equals(object obj) =>
            obj is Position p && p.X == X && p.Y == Y;

        public override int GetHashCode() => HashCode.Combine(X, Y);
    }
}
