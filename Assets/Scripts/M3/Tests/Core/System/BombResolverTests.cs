using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using M3.Core.Domain;
using M3.Core.System;


namespace M3.Tests.Core.System
{
    public class BombResolverTests
    {
        [Test]
        public void Resolve_BombInCenter_ReturnsAll13Positions()
        {
            var board = new BoardState(7, 7);
            var resolver = new BombResolver();

            var center = new Position(3, 3);
            var result = resolver.Resolve(board, center);

            Assert.AreEqual(13, result.Count);
        }

        [Test]
        public void Resolve_BombNearEdge_ClipsOutOfBounds()
        {
            var board = new BoardState(5, 5);
            var resolver = new BombResolver();

            var pos = new Position(1, 1);
            var result = resolver.Resolve(board, pos);

            foreach (var p in result)
            {
                Assert.IsTrue(
                    board.IsInside(p.X, p.Y),
                    $"Out of bounds position returned: {p}");
            }
        }

        
        [Test]
        public void Resolve_BombInCorner_ReturnsOnlyValidPositions()
        {
            var board = new BoardState(5, 5);
            var resolver = new BombResolver();

            var corner = new Position(0, 0);
            var result = resolver.Resolve(board, corner);

            // Expected positions:
            // (0,0)
            // (1,0), (2,0)
            // (0,1), (0,2)
            // (1,1)

            var expected = new HashSet<Position>
            {
                new Position(0, 0),
                new Position(1, 0),
                new Position(2, 0),
                new Position(0, 1),
                new Position(0, 2),
                new Position(1, 1)
            };

            Assert.AreEqual(expected.Count, result.Count);

            foreach (var p in expected)
            {
                Assert.IsTrue(
                    result.Contains(p),
                    $"Missing expected position {p}");
            }
        }
        
        [Test]
        public void Resolve_NoDuplicatePositions()
        {
            var board = new BoardState(7, 7);
            var resolver = new BombResolver();

            var pos = new Position(3, 3);
            var result = resolver.Resolve(board, pos);

            var set = new HashSet<Position>(result);
            Assert.AreEqual(set.Count, result.Count);
        }

        [Test]
        public void Resolve_AlwaysIncludesCenter()
        {
            var board = new BoardState(7, 7);
            var resolver = new BombResolver();

            var pos = new Position(4, 2);
            var result = resolver.Resolve(board, pos);

            Assert.IsTrue(
                result.Contains(pos),
                "Bomb center position was not included.");
        }


    }
}