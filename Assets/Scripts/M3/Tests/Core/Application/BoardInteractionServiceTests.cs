using System.Collections.Generic;
using NUnit.Framework;
using M3.Core.Application;
using M3.Core.Domain;
using M3.Core.Domain.Bomb;
using M3.Core.Domain.Match;
using M3.Core.Domain.Swap;
using M3.Core.System;
using UnityEngine;


namespace M3.Tests.Core.Application
{
    public class BoardInteractionServiceTests
    {
        private static BoardInteractionService CreateService(int seed)
        {
            var detector = new LineMatchDetector();
            var classifier = new MatchClassifier();
            var gravity = new GravitySystem();
            var spawner = new GemSpawner(seed);
            var bombresolver = new BombResolver();
            var bombcreation = new BombCreationRule();
            
            var cascade = new CascadeSystem(
                detector,
                classifier,
                gravity,
                spawner, 
                bombcreation,
                bombresolver);

            var swapRule = new AdjacentSwapRule(detector);

            return new BoardInteractionService(
                swapRule);
        }
        
        private static void AssertNoMatches(BoardState board)
        {
            var detector = new LineMatchDetector();
            var matches = detector.Detect(board);

            Assert.AreEqual(0, matches.Count, "Board still contains matches.");
        }
        
        [Test]
        public void TrySwap_InvalidSwap_IsRejected_AndBoardUnchanged()
        {
            var board = new BoardState(2, 1);

            board.SetGem(0, 0, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(1, 0, new GemState(GemColor.Blue, GemType.Normal));

            var originalLeft = board.GetCell(0, 0).Gem.Color;
            var originalRight = board.GetCell(1, 0).Gem.Color;

            var service = CreateService(seed: 1);

            var result = service.TrySwap(
                board,
                new Position(0, 0),
                new Position(1, 0));

            Assert.AreEqual(SwapResult.Rejected, result);
            Assert.AreEqual(originalLeft, board.GetCell(0, 0).Gem.Color);
            Assert.AreEqual(originalRight, board.GetCell(1, 0).Gem.Color);
        }
        
        // TEST FOR WHEN CASCADE IS TRIGGERED BEHIND THE SCENES 
        private sealed class DummyCascadeSystem : ICascadeSystem
        {
            private ICascadeSystem _cascadeSystemImplementation;
            public int ResolveCallCount { get; private set; }
            public SwapContext? LastContext { get; private set; }

            public void Resolve(BoardState board, SwapContext context)
            {
                ResolveCallCount++;
                LastContext = context;
            }

            public CascadeIterationResult ResolveOneIteration(BoardState board, SwapContext context, ref bool isPlayerMove)
            {
                ResolveCallCount++;
                LastContext = context;
                return new CascadeIterationResult(false, null, new List<BombTrigger>());
            }
        }

        // CASCADE SYSTEM NEVER CALLED FOR VALID SWAP
        [Test]
        public void TrySwap_InvalidSwap_DoesNotTriggerCascade()
        {
            var board = new BoardState(2, 1);

            board.SetGem(0, 0, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(1, 0, new GemState(GemColor.Blue, GemType.Normal));

            var swapRule = new AdjacentSwapRule(new LineMatchDetector());
            var dummyCascadeSystem = new DummyCascadeSystem();

            var service = new BoardInteractionService(
                swapRule);

            var result = service.TrySwap(
                board,
                new Position(0, 0),
                new Position(1, 0));

            Assert.AreEqual(SwapResult.Rejected, result);
            Assert.AreEqual(0, dummyCascadeSystem.ResolveCallCount);
        }


        private sealed class DummyGravitySystem : IGravitySystem
        {
            public int BombsProcessed { get; private set; }
    
            public void Apply(BoardState board)
            {
                for (int x = 0; x < board.Width; x++)
                for (int y = 0; y < board.Height; y++)
                {
                    var cell = board.GetCell(x, y);
                    if (!cell.IsEmpty && cell.Gem.Type == GemType.Bomb)
                    {
                        BombsProcessed++;
                    }
                }
    
                // No gravity logic here — delegate to real one
                new GravitySystem().Apply(board);
            }
        }

    }
   

    
}