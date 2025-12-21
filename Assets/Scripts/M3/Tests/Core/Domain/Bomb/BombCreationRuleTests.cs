using System.Linq;
using NUnit.Framework;
using M3.Core.Domain;
using M3.Core.Domain.Bomb;
using M3.Core.Domain.Match;


namespace M3.Tests.Core.Domain.Bomb
{
    public class BombCreationRuleTests
    {
        private static ClassifiedMatch CreateMatch(GemColor color, int size)
        {
            var positions = Enumerable.Range(0, size)
                .Select(i => new Position(i, 0))
                .ToList();

            return new ClassifiedMatch(
                color,
                MatchPattern.Line,
                positions);
        }

        
        [Test]
        public void TryCreate_CascadeMatch_ReturnsNull()
        {
            var rule = new BombCreationRule();

            var match = CreateMatch(GemColor.Red, 4);
            var swapOrigin = new Position(1, 1);

            var result = rule.TryCreate(match, swapOrigin, isPlayerMove: false, GemIdGenerator.Next());

            Assert.IsNull(result);
        }
        
        [Test]
        public void TryCreate_PlayerMatch_SizeThree_ReturnsNull()
        {
            var rule = new BombCreationRule();

            var match = CreateMatch(GemColor.Blue, 3);
            var swapOrigin = new Position(0, 0);

            var result = rule.TryCreate(match, swapOrigin, isPlayerMove: true, GemIdGenerator.Next());

            Assert.IsNull(result);
        }

        [Test]
        public void TryCreate_PlayerMatch_SizeFour_CreatesBombAtSwapOrigin()
        {
            var rule = new BombCreationRule();

            var match = CreateMatch(GemColor.Green, 4);
            var swapOrigin = new Position(2, 3);

            var result = rule.TryCreate(match, swapOrigin, isPlayerMove: true, GemIdGenerator.Next());

            Assert.IsNotNull(result);
            Assert.AreEqual(swapOrigin, result!.Position);
        }

        [Test]
        public void TryCreate_BombColor_MatchesMatchColor()
        {
            var rule = new BombCreationRule();

            var match = CreateMatch(GemColor.Yellow, 5);
            var swapOrigin = new Position(1, 2);

            var result = rule.TryCreate(match, swapOrigin, isPlayerMove: true, GemIdGenerator.Next());

            Assert.AreEqual(GemColor.Yellow, result!.Color);
        }

        
        [Test]
        public void TryCreate_PlayerMatch_LShape_CreatesBomb()
        {
            var rule = new BombCreationRule();

            var positions = new[]
            {
                new Position(1, 0),
                new Position(1, 1),
                new Position(1, 2),
                new Position(2, 2)
            };

            var match = new ClassifiedMatch(
                GemColor.Red,
                MatchPattern.LShape,
                positions);

            var swapOrigin = new Position(1, 1);

            var result = rule.TryCreate(match, swapOrigin, isPlayerMove: true, GemIdGenerator.Next());

            Assert.IsNotNull(result);
            Assert.AreEqual(swapOrigin, result!.Position);
            Assert.AreEqual(GemColor.Red, result.Color);
        }
        
        [Test]
        public void TryCreate_PlayerMatch_TShape_CreatesBomb()
        {
            var rule = new BombCreationRule();

            var positions = new[]
            {
                new Position(0, 1),
                new Position(1, 1),
                new Position(2, 1),
                new Position(1, 0),
                new Position(1, 2)
            };

            var match = new ClassifiedMatch(
                GemColor.Blue,
                MatchPattern.TShape,
                positions);

            var swapOrigin = new Position(1, 1);

            var result = rule.TryCreate(match, swapOrigin, isPlayerMove: true, GemIdGenerator.Next());

            Assert.IsNotNull(result);
            Assert.AreEqual(swapOrigin, result!.Position);
            Assert.AreEqual(GemColor.Blue, result.Color);
        }

        [Test]
        public void TryCreate_CascadeMatch_LOrTShape_DoesNotCreateBomb()
        {
            var rule = new BombCreationRule();

            var positions = new[]
            {
                new Position(1, 0),
                new Position(1, 1),
                new Position(1, 2),
                new Position(2, 2)
            };

            var match = new ClassifiedMatch(
                GemColor.Green,
                MatchPattern.LShape,
                positions);

            var swapOrigin = new Position(1, 1);

            var result = rule.TryCreate(match, swapOrigin, isPlayerMove: false, GemIdGenerator.Next());

            Assert.IsNull(result);
        }

    }
}