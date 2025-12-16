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

            var result = rule.TryCreate(match, swapOrigin, isPlayerMove: false);

            Assert.IsNull(result);
        }
        
        [Test]
        public void TryCreate_PlayerMatch_SizeThree_ReturnsNull()
        {
            var rule = new BombCreationRule();

            var match = CreateMatch(GemColor.Blue, 3);
            var swapOrigin = new Position(0, 0);

            var result = rule.TryCreate(match, swapOrigin, isPlayerMove: true);

            Assert.IsNull(result);
        }

        [Test]
        public void TryCreate_PlayerMatch_SizeFour_CreatesBombAtSwapOrigin()
        {
            var rule = new BombCreationRule();

            var match = CreateMatch(GemColor.Green, 4);
            var swapOrigin = new Position(2, 3);

            var result = rule.TryCreate(match, swapOrigin, isPlayerMove: true);

            Assert.IsNotNull(result);
            Assert.AreEqual(swapOrigin, result!.Position);
        }

        [Test]
        public void TryCreate_BombColor_MatchesMatchColor()
        {
            var rule = new BombCreationRule();

            var match = CreateMatch(GemColor.Yellow, 5);
            var swapOrigin = new Position(1, 2);

            var result = rule.TryCreate(match, swapOrigin, isPlayerMove: true);

            Assert.AreEqual(GemColor.Yellow, result!.Color);
        }


    }
}