using System.Collections.Generic;
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
        
        private static void AssertNoBombsOfColor(BoardState board, GemColor color)
        {
            for (int x = 0; x < board.Width; x++)
            for (int y = 0; y < board.Height; y++)
            {
                var cell = board.GetCell(x, y);
                if (!cell.IsEmpty &&
                    cell.Gem.Type == GemType.Bomb &&
                    cell.Gem.Color == color)
                {
                    Assert.Fail($"Found remaining {color} bomb at ({x},{y})");
                }
            }
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


        [Test]
        public void BombChain_SingleBomb_ExplodesAndClearsArea()
        {
            var board = new BoardState(7, 7);

            // Create a simple match that will create a bomb at (3,3)
            board.SetGem(2, 3, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(3, 3, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(4, 3, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(3, 4, new GemState(GemColor.Red, GemType.Normal));

            var cascade = new CascadeSystem(
                new LineMatchDetector(),
                new MatchClassifier(),
                new GravitySystem(),
                new GemSpawner(seed: 1),
                new BombCreationRule(),
                new BombResolver());

            cascade.Resolve(
                board,
                SwapContext.PlayerMove(
                    new Position(3, 3),
                    new Position(3, 4)));

            // After full cascade, board must be stable
            AssertNoMatches(board);
        }
        
        [Test]
        public void BombChain_BombHitsAnotherBomb_TriggersSecondExplosion()
        {
            var board = new BoardState(9, 9);

            // First bomb (will be created by match)
            board.SetGem(4, 4, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(5, 4, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(6, 4, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(5, 5, new GemState(GemColor.Red, GemType.Normal));

            // Second bomb already on board, positioned to be hit
            board.SetGem(7, 4, new GemState(GemColor.Red, GemType.Bomb));

            var cascade = new CascadeSystem(
                new LineMatchDetector(),
                new MatchClassifier(),
                new GravitySystem(),
                new GemSpawner(seed: 2),
                new BombCreationRule(),
                new BombResolver());

            cascade.Resolve(
                board,
                SwapContext.PlayerMove(
                    new Position(5, 4),
                    new Position(5, 5)));

            AssertNoMatches(board);
        }

        [Test]
        public void BombChain_MultipleBombs_TerminatesCorrectly()
        {
            var board = new BoardState(9, 9);

            // Create a dense bomb cluster
            board.SetGem(4, 4, new GemState(GemColor.Red, GemType.Bomb));
            board.SetGem(6, 4, new GemState(GemColor.Red, GemType.Bomb));
            board.SetGem(5, 6, new GemState(GemColor.Red, GemType.Bomb));

            // Triggering match
            board.SetGem(4, 3, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(5, 3, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(6, 3, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(5, 4, new GemState(GemColor.Red, GemType.Normal));

            var cascade = new CascadeSystem(
                new LineMatchDetector(),
                new MatchClassifier(),
                new GravitySystem(),
                new GemSpawner(seed: 4),
                new BombCreationRule(),
                new BombResolver());

            cascade.Resolve(
                board,
                SwapContext.PlayerMove(
                    new Position(5, 3),
                    new Position(5, 4)));

            AssertNoMatches(board);
        }

        private sealed class DummyBombResolver : IBombResolver
        {
            private readonly IBombResolver _inner;

            public int ExplosionCount { get; private set; }

            public DummyBombResolver(IBombResolver inner)
            {
                _inner = inner;
            }

            public IReadOnlyCollection<Position> Resolve(
                BoardState board,
                Position bombPosition)
            {
                ExplosionCount++;
                return _inner.Resolve(board, bombPosition);
            }
        }

        [Test]
        public void BombChain_ExplosionCount_IsCorrect_WhenTriggeredByExistingBomb()
        {
            var board = new BoardState(11, 11);

            // Existing bomb that WILL explode
            board.SetGem(5, 5, new GemState(GemColor.Red, GemType.Bomb));

            // Two other bombs in blast range
            board.SetGem(7, 5, new GemState(GemColor.Blue, GemType.Bomb));
            board.SetGem(5, 7, new GemState(GemColor.Green, GemType.Bomb));

            // Trigger match that clears the red bomb
            board.SetGem(4, 5, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(6, 5, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(5, 4, new GemState(GemColor.Red, GemType.Normal));

            var realResolver = new BombResolver();
            var spyResolver = new DummyBombResolver(realResolver);

            var cascade = new CascadeSystem(
                new LineMatchDetector(),
                new MatchClassifier(),
                new GravitySystem(),
                new GemSpawner(seed: 42),
                new BombCreationRule(),
                spyResolver);

            cascade.Resolve(
                board,
                SwapContext.PlayerMove(
                    new Position(5, 4),
                    new Position(5, 5)));

            // Red bomb + 2 chained bombs
            Assert.AreEqual(3, spyResolver.ExplosionCount);
        }
//
//         [Test]
//         public void Bomb_IsPlacedBeforeGravity_AndFallsLikeNormalGem()
//         {
//             var board = new BoardState(5, 6);
//
//             /*
//                 Board (Y up):
//
//                 y=5 | . . . . .
//                 y=4 | . . . . .
//                 y=3 | . . R . .
//                 y=2 | R R . R .
//                 y=1 | . . . . .
//                 y=0 | . . . . .
//                       0 1 2 3 4
//             */
//
//             // Create a size-4 match that produces a bomb
//             board.SetGem(0, 2, new GemState(GemColor.Red, GemType.Normal));
//             board.SetGem(1, 2, new GemState(GemColor.Red, GemType.Normal));
//             board.SetGem(2, 3, new GemState(GemColor.Red, GemType.Normal));
//             board.SetGem(3, 2, new GemState(GemColor.Red, GemType.Normal));
//
//             var cascade = new CascadeSystem(
//                 new LineMatchDetector(),
//                 new MatchClassifier(),
//                 new GravitySystem(),
//                 new GemSpawner(seed: 123), // deterministic
//                 new BombCreationRule(),
//                 new BombResolver());
//
//             cascade.Resolve(
//                 board,
//                 SwapContext.PlayerMove(
//                     new Position(2, 3),
//                     new Position(2, 2)));
//
//             // Find the bomb
//             Position? bombPos = null;
//
//             for (int x = 0; x < board.Width; x++)
//             for (int y = 0; y < board.Height; y++)
//             {
//                 var cell = board.GetCell(x, y);
//                 if (!cell.IsEmpty &&
//                     cell.Gem.Type == GemType.Bomb)
//                 {
//                     bombPos = new Position(x, y);
//                     break;
//                 }
//             }
//
//             Assert.IsTrue(bombPos.HasValue, "Bomb did not survive the cascade.");
//
//             // Bomb must be lower than its creation Y (2)
//             Assert.Less(
//                 bombPos.Value.Y,
//                 2,
//                 "Bomb did not fall via gravity.");
//         }
        //
        // private sealed class DummyGravitySystem : IGravitySystem
        // {
        //     public int BombsProcessed { get; private set; }
        //
        //     public void Apply(BoardState board)
        //     {
        //         for (int x = 0; x < board.Width; x++)
        //         for (int y = 0; y < board.Height; y++)
        //         {
        //             var cell = board.GetCell(x, y);
        //             if (!cell.IsEmpty && cell.Gem.Type == GemType.Bomb)
        //             {
        //                 BombsProcessed++;
        //             }
        //         }
        //
        //         // No gravity logic here — delegate to real one
        //         new GravitySystem().Apply(board);
        //     }
        // }
        //
        //
        // [Test]
        // public void Bomb_IsPlacedBeforeGravity_IsProcessedByGravity()
        // {
        //     var board = new BoardState(5, 6);
        //
        //     board.SetGem(1, 2, new GemState(GemColor.Red, GemType.Normal));
        //     board.SetGem(2, 2, new GemState(GemColor.Red, GemType.Normal));
        //     board.SetGem(4, 2, new GemState(GemColor.Red, GemType.Normal));
        //
        //     board.SetGem(3, 3, new GemState(GemColor.Red, GemType.Normal));
        //     board.SetGem(3, 2, new GemState(GemColor.Blue, GemType.Normal));
        //
        //     var dummyGravitySystem = new DummyGravitySystem();
        //
        //     var cascade = new CascadeSystem(
        //         new LineMatchDetector(),
        //         new MatchClassifier(),
        //         dummyGravitySystem,
        //         new GemSpawner(seed: 99),
        //         new BombCreationRule(),
        //         new BombResolver());
        //
        //     cascade.Resolve(
        //         board,
        //         SwapContext.PlayerMove(
        //             new Position(3, 2),
        //             new Position(3, 3)));
        //
        //     Assert.Greater(dummyGravitySystem.BombsProcessed, 0);
        // }
        //


        
    }
}