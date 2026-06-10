using NUnit.Framework;
using PotionPopQuest.Core;

namespace PotionPopQuest.Tests
{
    public sealed class GoalTrackerTests
    {
        [Test]
        public void ApplyMatchEvents_UpdatesIngredientGoal()
        {
            var tracker = new GoalTracker(new[]
            {
                new GoalData(GoalType.CollectIngredient, 2, IngredientType.RedHerb)
            });

            tracker.ApplyMatchEvents(
                new[]
                {
                    new ClearedIngredient(new GridPosition(0, 0), IngredientType.RedHerb),
                    new ClearedIngredient(new GridPosition(0, 1), IngredientType.RedHerb)
                },
                new ObstacleEvent[0],
                new ObstacleEvent[0],
                new PotionType[0]);

            Assert.That(tracker.IsComplete, Is.True);
        }

        [Test]
        public void ApplyMatchEvents_UpdatesAnyPotionCreationGoal()
        {
            var tracker = new GoalTracker(new[]
            {
                new GoalData(GoalType.CreatePotion, 1)
            });

            tracker.ApplyMatchEvents(
                new ClearedIngredient[0],
                new ObstacleEvent[0],
                new ObstacleEvent[0],
                new[] { PotionType.LineVertical });

            Assert.That(tracker.IsComplete, Is.True);
        }
    }
}

