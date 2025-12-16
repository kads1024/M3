using System.Collections.Generic;
using M3.Core.Domain;

namespace M3.Core.Domain.Bomb
{
    public sealed class BombResolver : IBombResolver
    {
        private static readonly (int dx, int dy)[] Offsets =
        {
            // center
            ( 0,  0),

            // vertical (±1, ±2)
            ( 0,  1), ( 0,  2),
            ( 0, -1), ( 0, -2),

            // horizontal (±1, ±2)
            ( 1,  0), ( 2,  0),
            (-1,  0), (-2,  0),

            // diagonals (±1)
            ( 1,  1), (-1,  1),
            ( 1, -1), (-1, -1)
        };

        public IReadOnlyCollection<Position> Resolve(
            BoardState board,
            Position bombPosition)
        {
            var results = new HashSet<Position>();

            foreach (var (dx, dy) in Offsets)
            {
                int x = bombPosition.X + dx;
                int y = bombPosition.Y + dy;

                if (board.IsInside(x, y))
                {
                    results.Add(new Position(x, y));
                }
            }

            return results;
        }
    }
}