using M3.Core.Application;
using NUnit.Framework;
using M3.Core.Domain;
using M3.Core.Domain.Bomb;
using M3.Core.Domain.Match;
using M3.Core.Domain.Swap;
using M3.Core.System;


namespace M3.Tests.Core.System
{
    public class CascadeSystemTests
    {
        private static ICascadeSystem CreateCascadeSystem(int seed)
        {
            return new CascadeSystem(
                new LineMatchDetector(),
                new MatchClassifier(),
                new GravitySystem(),
                new GemSpawner(seed),
                new BombCreationRule(),
                new BombResolver());
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
        
        [Test]
        public void Cascade_SingleMatch_ResolvesOnce()
        {
            var board = new BoardState(3, 3);

            // Vertical match in column 1
            board.SetGem(1, 0, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(1, 1, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(1, 2, new GemState(GemColor.Red, GemType.Normal));

            var cascade = CreateCascadeSystem(seed: 10);

            cascade.Resolve(board,SwapContext.Cascade());

            // Board must be full
            for (int x = 0; x < board.Width; x++)
            for (int y = 0; y < board.Height; y++)
            {
                Assert.IsFalse(board.GetCell(x, y).IsEmpty);
            }

            // No immediate matches must exist
            AssertNoMatches(board);
        }

        
        [Test]
        public void Cascade_MultiCascade_ResolvesFully()
        {
            var board = new BoardState(3, 4);

            // First vertical match
            board.SetGem(1, 0, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(1, 1, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(1, 2, new GemState(GemColor.Red, GemType.Normal));

            // These will fall into a second match
            board.SetGem(1, 3, new GemState(GemColor.Blue, GemType.Normal));
            board.SetGem(0, 0, new GemState(GemColor.Blue, GemType.Normal));
            board.SetGem(2, 0, new GemState(GemColor.Blue, GemType.Normal));

            var cascade = CreateCascadeSystem(seed: 20);

            cascade.Resolve(board, SwapContext.Cascade());

            // Board must be stable
            AssertNoMatches(board);
        }

        [Test]
        public void Cascade_StableBoard_NoChanges()
        {
            var board = new BoardState(3, 3);

            board.SetGem(0, 0, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(1, 0, new GemState(GemColor.Blue, GemType.Normal));
            board.SetGem(2, 0, new GemState(GemColor.Green, GemType.Normal));

            board.SetGem(0, 1, new GemState(GemColor.Green, GemType.Normal));
            board.SetGem(1, 1, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(2, 1, new GemState(GemColor.Blue, GemType.Normal));

            board.SetGem(0, 2, new GemState(GemColor.Blue, GemType.Normal));
            board.SetGem(1, 2, new GemState(GemColor.Green, GemType.Normal));
            board.SetGem(2, 2, new GemState(GemColor.Red, GemType.Normal));

            var snapshot = Snapshot(board);
            var cascade = CreateCascadeSystem(seed: 30);

            cascade.Resolve(board, SwapContext.Cascade());

            AssertBoardEquals(snapshot, board);
        }

        [Test]
        public void Cascade_DoesNotLoopInfinitely()
        {
            var board = new BoardState(5, 5);

            // Force one guaranteed match
            board.SetGem(2, 0, new GemState(GemColor.Yellow, GemType.Normal));
            board.SetGem(2, 1, new GemState(GemColor.Yellow, GemType.Normal));
            board.SetGem(2, 2, new GemState(GemColor.Yellow, GemType.Normal));

            var cascade = CreateCascadeSystem(seed: 999);

            // If this hangs, the test runner will fail
            cascade.Resolve(board,SwapContext.Cascade());

            AssertNoMatches(board);
        }  
        
        [Test]
        public void Cascade_PlayerMove_ResolvesWithoutError()
        {
            var board = new BoardState(3, 3);

            board.SetGem(0, 0, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(1, 0, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(2, 0, new GemState(GemColor.Green, GemType.Normal));

            board.SetGem(2, 1, new GemState(GemColor.Red, GemType.Normal));

            var cascade = CreateCascadeSystem(seed: 10);

            cascade.Resolve(
                board,
                SwapContext.Cascade());

            AssertNoMatches(board);
        }

        [Test]
        public void Cascade_CascadeMove_DoesNotCreateBomb()
        {
            var board = new BoardState(3, 3);

            board.SetGem(0, 0, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(1, 0, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(2, 0, new GemState(GemColor.Red, GemType.Normal));

            var cascade = CreateCascadeSystem(seed: 20);

            cascade.Resolve(
                board,
                SwapContext.Cascade());

            AssertNoMatches(board);
        }

        [Test]
        public void Cascade_PlayerMove_ResolvesCorrectly()
        {
            var board = new BoardState(3, 3);

            board.SetGem(0, 0, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(1, 0, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(2, 0, new GemState(GemColor.Green, GemType.Normal));
            board.SetGem(2, 1, new GemState(GemColor.Red, GemType.Normal));

            var cascade = CreateCascadeSystem(seed: 1);

            cascade.Resolve(
                board,
                SwapContext.PlayerMove(
                    new Position(2, 0),
                    new Position(2, 1)));

            AssertNoMatches(board);
        }
        
        [Test]
        public void Cascade_NonPlayerMove_DoesNotCreateBomb()
        {
            var board = new BoardState(3, 3);

            board.SetGem(0, 0, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(1, 0, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(2, 0, new GemState(GemColor.Red, GemType.Normal));

            var cascade = CreateCascadeSystem(seed: 2);

            cascade.Resolve(board, SwapContext.Cascade());

            AssertNoMatches(board);
        }


        
    }
}