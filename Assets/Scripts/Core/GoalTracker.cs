using System;
using System.Collections.Generic;
using System.Linq;

namespace PotionPopQuest.Core
{
    public sealed class GoalProgress
    {
        public GoalProgress(GoalData goal)
        {
            Goal = goal;
        }

        public GoalData Goal { get; }
        public int CurrentAmount { get; private set; }
        public int RemainingAmount => Math.Max(0, Goal.Amount - CurrentAmount);
        public bool IsComplete => CurrentAmount >= Goal.Amount;

        public void AddProgress(int amount)
        {
            CurrentAmount = Math.Min(Goal.Amount, CurrentAmount + Math.Max(0, amount));
        }
    }

    public sealed class GoalTracker
    {
        private readonly List<GoalProgress> _goals;

        public GoalTracker(IEnumerable<GoalData> goals)
        {
            _goals = goals.Select(goal => new GoalProgress(goal)).ToList();
        }

        public IReadOnlyList<GoalProgress> Goals => _goals;
        public bool IsComplete => _goals.All(goal => goal.IsComplete);

        public void ApplyMatchEvents(
            IEnumerable<ClearedIngredient> clearedIngredients,
            IEnumerable<ObstacleEvent> destroyedObstacles,
            IEnumerable<ObstacleEvent> clearedTiles,
            IEnumerable<PotionType> createdPotions)
        {
            var ingredientCounts = clearedIngredients
                .GroupBy(item => item.Ingredient)
                .ToDictionary(group => group.Key, group => group.Count());
            var obstacleCounts = destroyedObstacles
                .GroupBy(item => item.ObstacleType)
                .ToDictionary(group => group.Key, group => group.Count());
            var tileCounts = clearedTiles
                .GroupBy(item => item.ObstacleType)
                .ToDictionary(group => group.Key, group => group.Count());
            var potionCounts = createdPotions
                .GroupBy(item => item)
                .ToDictionary(group => group.Key, group => group.Count());

            foreach (var goal in _goals)
            {
                switch (goal.Goal.GoalType)
                {
                    case GoalType.CollectIngredient:
                        if (ingredientCounts.TryGetValue(goal.Goal.Ingredient, out var ingredientAmount))
                        {
                            goal.AddProgress(ingredientAmount);
                        }

                        break;
                    case GoalType.BreakObstacle:
                        if (obstacleCounts.TryGetValue(goal.Goal.Obstacle, out var obstacleAmount))
                        {
                            goal.AddProgress(obstacleAmount);
                        }

                        break;
                    case GoalType.ClearTile:
                        if (tileCounts.TryGetValue(goal.Goal.Obstacle, out var tileAmount))
                        {
                            goal.AddProgress(tileAmount);
                        }

                        break;
                    case GoalType.CreatePotion:
                        if (goal.Goal.Potion == PotionType.None)
                        {
                            goal.AddProgress(potionCounts.Values.Sum());
                        }
                        else if (goal.Goal.Potion == PotionType.LineHorizontal || goal.Goal.Potion == PotionType.LineVertical)
                        {
                            potionCounts.TryGetValue(PotionType.LineHorizontal, out var horizontalLines);
                            potionCounts.TryGetValue(PotionType.LineVertical, out var verticalLines);
                            goal.AddProgress(horizontalLines + verticalLines);
                        }
                        else if (potionCounts.TryGetValue(goal.Goal.Potion, out var potionAmount))
                        {
                            goal.AddProgress(potionAmount);
                        }

                        break;
                    case GoalType.RestorePotionLab:
                        goal.AddProgress(
                            ingredientCounts.Values.Sum()
                            + obstacleCounts.Values.Sum()
                            + tileCounts.Values.Sum()
                            + potionCounts.Values.Sum());
                        break;
                }
            }
        }
    }
}
