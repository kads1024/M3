using NUnit.Framework;
using M3.Core.Domain;
using M3.Core.Domain.Match;

namespace M3.Tests.Core.Domain.Match
{
    public class MatchClassifierTests
    {
        [Test]
        public void Classify_LineMatch_RemainsLine()
        {
            var positions = new[]
            {
                new Position(1, 2),
                new Position(2, 2),
                new Position(3, 2)
            };
            
            

            var match = new MatchGroup(
                GemColor.Red,
                MatchDirection.Horizontal,
                positions
            );

            var classifier = new MatchClassifier();
            var result = classifier.Classify(new[] { match });

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(MatchPattern.Line, result[0].Pattern);
        }

        [Test]
        public void Classify_OverlappingMatches_FormsTShape()
        {
            var horizontal = new MatchGroup(
                GemColor.Blue,
                MatchDirection.Horizontal,
                new[]
                {
                    new Position(1, 1),
                    new Position(2, 1),
                    new Position(3, 1)
                });

            var vertical = new MatchGroup(
                GemColor.Blue,
                MatchDirection.Vertical,
                new[]
                {
                    new Position(2, 1),
                    new Position(2, 2),
                    new Position(2, 3)
                });

            var classifier = new MatchClassifier();
            var result = classifier.Classify(new[] { horizontal, vertical });

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(MatchPattern.TShape, result[0].Pattern);
            Assert.AreEqual(5, result[0].Positions.Count);
        }

        
        [Test]
        public void Classify_LShapeDetected()
        {
            var horizontal = new MatchGroup(
                GemColor.Green,
                MatchDirection.Horizontal,
                new[]
                {
                    new Position(1, 3),
                    new Position(2, 3),
                    new Position(3, 3)
                });

            var vertical = new MatchGroup(
                GemColor.Green,
                MatchDirection.Vertical,
                new[]
                {
                    new Position(1, 3),
                    new Position(1, 4),
                    new Position(1, 5)
                });

            var classifier = new MatchClassifier();
            var result = classifier.Classify(new[] { horizontal, vertical });

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(MatchPattern.LShape, result[0].Pattern);
        }

        [Test]
        public void Classify_NonOverlappingMatches_RemainSeparate()
        {
            var matchA = new MatchGroup(
                GemColor.Red,
                MatchDirection.Horizontal,
                new[]
                {
                    new Position(0, 0),
                    new Position(1, 0),
                    new Position(2, 0)
                });

            var matchB = new MatchGroup(
                GemColor.Red,
                MatchDirection.Horizontal,
                new[]
                {
                    new Position(4, 4),
                    new Position(5, 4),
                    new Position(6, 4)
                });

            var classifier = new MatchClassifier();
            var result = classifier.Classify(new[] { matchA, matchB });

            Assert.AreEqual(2, result.Count);
        }
    }
}