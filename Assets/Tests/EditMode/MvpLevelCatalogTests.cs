using System.Linq;
using NUnit.Framework;
using PotionPopQuest.Core;

namespace PotionPopQuest.Tests
{
    public sealed class MvpLevelCatalogTests
    {
        [Test]
        public void CreateLevels_DefinesTenSequentialMvpLevels()
        {
            var levels = MvpLevelCatalog.CreateLevels();

            Assert.That(levels.Count, Is.EqualTo(10));
            Assert.That(levels.Select(level => level.LevelNumber), Is.EqualTo(Enumerable.Range(1, 10)));
        }

        [Test]
        public void CreateLevels_GeneratesPlayableStartingBoardsForEveryMvpLevel()
        {
            var matchFinder = new MatchFinder();
            var moveFinder = new BoardMoveFinder(matchFinder);
            var generator = new BoardGenerator(matchFinder);

            foreach (var level in MvpLevelCatalog.CreateLevels())
            {
                var board = generator.Generate(level, new SystemRandomSource(level.LevelNumber));

                Assert.That(matchFinder.FindMatches(board), Is.Empty, $"Level {level.LevelNumber} starts with an automatic match.");
                Assert.That(moveFinder.TryFindValidMove(board, out _), Is.True, $"Level {level.LevelNumber} has no valid opening move.");
            }
        }

        [Test]
        public void CreateLevels_HasObstacleCountsThatCanSatisfyObstacleGoals()
        {
            foreach (var level in MvpLevelCatalog.CreateLevels())
            {
                foreach (var goal in level.Goals)
                {
                    if (goal.GoalType != GoalType.BreakObstacle && goal.GoalType != GoalType.ClearTile)
                    {
                        continue;
                    }

                    var available = level.Obstacles.Count(obstacle => obstacle.ObstacleType == goal.Obstacle);
                    Assert.That(
                        available,
                        Is.GreaterThanOrEqualTo(goal.Amount),
                        $"Level {level.LevelNumber} has goal {goal.GoalType} {goal.Obstacle} x{goal.Amount}, but only {available} are placed.");
                }
            }
        }
    }
}

