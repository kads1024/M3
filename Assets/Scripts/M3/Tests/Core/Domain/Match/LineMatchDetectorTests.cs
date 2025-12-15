using NUnit.Framework;
using M3.Core.Domain;
using M3.Core.Domain.Match;

namespace M3.Tests.Core.Domain.Match
{
    public class LineMatchDetectorTests
    {
        [Test]
        public void Detect_FindsHorizontalMatchOfThree()
        {
            var board = new BoardState(5, 5);

            board.SetGem(1, 2, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(2, 2, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(3, 2, new GemState(GemColor.Red, GemType.Normal));

            var detector = new LineMatchDetector();
            var matches = detector.Detect(board);

            Assert.AreEqual(1, matches.Count);
            Assert.AreEqual(GemColor.Red, matches[0].Color);
            Assert.AreEqual(3, matches[0].Length);
            Assert.AreEqual(MatchDirection.Horizontal, matches[0].Direction);
        }
        
        [Test]
        public void Detect_FindsVerticalMatchOfThree()
        {
            var board = new BoardState(5, 5);

            board.SetGem(2, 1, new GemState(GemColor.Blue, GemType.Normal));
            board.SetGem(2, 2, new GemState(GemColor.Blue, GemType.Normal));
            board.SetGem(2, 3, new GemState(GemColor.Blue, GemType.Normal));

            var detector = new LineMatchDetector();
            var matches = detector.Detect(board);

            Assert.AreEqual(1, matches.Count);
            Assert.AreEqual(GemColor.Blue, matches[0].Color);
            Assert.AreEqual(3, matches[0].Length);
            Assert.AreEqual(MatchDirection.Vertical, matches[0].Direction);
        }

        [Test]
        public void Detect_DoesNotReturnMatchesForTwoInLine()
        {
            var board = new BoardState(5, 5);

            board.SetGem(1, 1, new GemState(GemColor.Green, GemType.Normal));
            board.SetGem(2, 1, new GemState(GemColor.Green, GemType.Normal));

            var detector = new LineMatchDetector();
            var matches = detector.Detect(board);

            Assert.AreEqual(0, matches.Count);
        }

        [Test]
        public void Detect_BreaksMatchOnDifferentColor()
        {
            var board = new BoardState(5, 5);

            board.SetGem(1, 1, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(2, 1, new GemState(GemColor.Blue, GemType.Normal));
            board.SetGem(3, 1, new GemState(GemColor.Red, GemType.Normal));

            var detector = new LineMatchDetector();
            var matches = detector.Detect(board);

            Assert.AreEqual(0, matches.Count);
        }

        [Test]
        public void Detect_FindsMatchLongerThanThree()
        {
            var board = new BoardState(6, 6);

            board.SetGem(1, 3, new GemState(GemColor.Yellow, GemType.Normal));
            board.SetGem(2, 3, new GemState(GemColor.Yellow, GemType.Normal));
            board.SetGem(3, 3, new GemState(GemColor.Yellow, GemType.Normal));
            board.SetGem(4, 3, new GemState(GemColor.Yellow, GemType.Normal));

            var detector = new LineMatchDetector();
            var matches = detector.Detect(board);

            Assert.AreEqual(1, matches.Count);
            Assert.AreEqual(4, matches[0].Length);
        }

    }
}