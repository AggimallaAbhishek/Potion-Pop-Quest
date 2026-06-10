using System.Linq;
using NUnit.Framework;
using PotionPopQuest.Core;

namespace PotionPopQuest.Tests
{
    public sealed class BoardGeneratorTests
    {
        [Test]
        public void Generate_CreatesBoardWithoutStartingMatches()
        {
            var level = MvpLevelCatalog.CreateLevels().First();
            var matchFinder = new MatchFinder();
            var generator = new BoardGenerator(matchFinder);

            var board = generator.Generate(level, new DeterministicRandomSource());

            Assert.That(board.Width, Is.EqualTo(8));
            Assert.That(board.Height, Is.EqualTo(8));
            Assert.That(matchFinder.FindMatches(board), Is.Empty);
        }

        [Test]
        public void Generate_CreatesBoardWithAtLeastOneValidMove()
        {
            var level = MvpLevelCatalog.CreateLevels().First();
            var generator = new BoardGenerator();

            var board = generator.Generate(level, new DeterministicRandomSource());

            Assert.That(generator.HasAnyValidMove(board), Is.True);
        }
    }
}

