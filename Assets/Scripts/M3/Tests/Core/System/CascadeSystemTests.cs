using NUnit.Framework;
using M3.Core.Domain;
using M3.Core.Domain.Match;
using M3.Core.Domain.Swap;
using M3.Core.System;


namespace M3.Tests.Core.System
{
    public class CascadeSystemTests
    {
        private static ICascadeSystem CreateCascadeSystem(int seed)
        {
            var detector = new LineMatchDetector();
            var classifier = new MatchClassifier();
            var gravity = new GravitySystem();
            var spawner = new GemSpawner(seed);

            return new CascadeSystem(
                detector,
                classifier,
                gravity,
                spawner);
        }
        
        private static void AssertNoMatches(BoardState board)
        {
            var detector = new LineMatchDetector();
            var matches = detector.Detect(board);

            Assert.AreEqual(0, matches.Count, "Board still contains matches.");
        }
        
        private static GemColor?[,] Snapshot(BoardState board)
        {
            var snapshot = new GemColor?[board.Width, board.Height];

            for (int x = 0; x < board.Width; x++)
            for (int y = 0; y < board.Height; y++)
            {
                var cell = board.GetCell(x, y);
                snapshot[x, y] = cell.IsEmpty ? null : cell.Gem.Color;
            }

            return snapshot;
        }

        private static void AssertBoardEquals(
            GemColor?[,] expected,
            BoardState actual)
        {
            for (int x = 0; x < actual.Width; x++)
            for (int y = 0; y < actual.Height; y++)
            {
                var cell = actual.GetCell(x, y);
                var actualColor = cell.IsEmpty ? (GemColor?)null : cell.Gem.Color;

                Assert.AreEqual(expected[x, y], actualColor,
                    $"Mismatch at ({x},{y})");
            }
        }


    }
}