using NUnit.Framework;
using System.Linq;
using PotionPopQuest.Core;
using PotionPopQuest.Unity;

namespace PotionPopQuest.Unity.Tests
{
    public sealed class LevelCatalogLoaderTests
    {
        [Test]
        public void LoadLevels_LoadsJsonCatalogWithExpandedLevels()
        {
            var levels = new LevelCatalogLoader(new NullGameLogger()).LoadLevels(null);

            Assert.That(levels.Count, Is.GreaterThanOrEqualTo(20));
            Assert.That(levels[0].LevelNumber, Is.EqualTo(1));
            Assert.That(levels[levels.Count - 1].LevelNumber, Is.EqualTo(20));
        }

        [Test]
        public void LoadLevels_ObstacleGoalsDoNotExceedPlacedObstacles()
        {
            var levels = new LevelCatalogLoader(new NullGameLogger()).LoadLevels(null);

            foreach (var level in levels)
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
                        $"Level {level.LevelNumber} asks for {goal.Amount} {goal.Obstacle} goals but only places {available}.");
                }
            }
        }
    }
}
