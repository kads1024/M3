using NUnit.Framework;
using M3.Core.System;
using M3.Core.Domain;


namespace M3.Tests.Core.System
{
    public class GemSpawnerTests
    {
        [Test]
        public void Spawner_EmptyBoard_NoImmediateMatches()
        {
            var board = new BoardState(5, 5);
            IGemSpawner spawner = new GemSpawner(seed: 123);

            spawner.Spawn(board);

            for (int x = 0; x < board.Width; x++)
            for (int y = 0; y < board.Height; y++)
            {
                var color = board.GetCell(x, y).Gem.Color;

                // Horizontal check
                if (x >= 2)
                {
                    Assert.IsFalse(
                        board.GetCell(x - 1, y).Gem.Color == color &&
                        board.GetCell(x - 2, y).Gem.Color == color,
                        $"Horizontal match created at ({x},{y})");
                }

                // Vertical check
                if (y >= 2)
                {
                    Assert.IsFalse(
                        board.GetCell(x, y - 1).Gem.Color == color &&
                        board.GetCell(x, y - 2).Gem.Color == color,
                        $"Vertical match created at ({x},{y})");
                }
            }
        }
        
        [Test]
        public void Spawner_PartialColumn_RespectsExistingGems()
        {
            var board = new BoardState(1, 4);

            board.SetGem(0, 0, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(0, 1, new GemState(GemColor.Red, GemType.Normal));

            IGemSpawner spawner = new GemSpawner(seed: 42);
            spawner.Spawn(board);

            // The gem at y=2 must NOT be Red (would create vertical match)
            Assert.AreNotEqual(
                GemColor.Red,
                board.GetCell(0, 2).Gem.Color);
        }

        [Test]
        public void Spawner_PreventsHorizontalMatch()
        {
            var board = new BoardState(3, 1);

            board.SetGem(0, 0, new GemState(GemColor.Blue, GemType.Normal));
            board.SetGem(2, 0, new GemState(GemColor.Blue, GemType.Normal));

            IGemSpawner spawner = new GemSpawner(seed: 1);
            spawner.Spawn(board);

            Assert.AreNotEqual(
                GemColor.Blue,
                board.GetCell(1, 0).Gem.Color);
        }

        [Test]
        public void Spawner_DoesNotModifyExistingGems()
        {
            var board = new BoardState(2, 2);

            board.SetGem(0, 0, new GemState(GemColor.Green, GemType.Normal));

            IGemSpawner spawner = new GemSpawner(seed: 7);
            spawner.Spawn(board);

            Assert.AreEqual(
                GemColor.Green,
                board.GetCell(0, 0).Gem.Color);
        }

        [Test]
        public void Spawner_IsDeterministic()
        {
            var boardA = new BoardState(3, 3);
            var boardB = new BoardState(3, 3);

            IGemSpawner spawnerA = new GemSpawner(seed: 1234);
            IGemSpawner spawnerB = new GemSpawner(seed: 1234);

            spawnerA.Spawn(boardA);
            spawnerB.Spawn(boardB);

            for (int x = 0; x < 3; x++)
            for (int y = 0; y < 3; y++)
            {
                Assert.AreEqual(
                    boardA.GetCell(x, y).Gem.Color,
                    boardB.GetCell(x, y).Gem.Color);
            }
        }

    
    }
}
