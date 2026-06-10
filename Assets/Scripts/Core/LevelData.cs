using System;
using System.Collections.Generic;
using System.Linq;

namespace PotionPopQuest.Core
{
    public sealed class GoalData
    {
        public GoalData(
            GoalType goalType,
            int amount,
            IngredientType ingredient = IngredientType.None,
            ObstacleType obstacle = ObstacleType.None,
            PotionType potion = PotionType.None)
        {
            GoalType = goalType;
            Amount = Math.Max(1, amount);
            Ingredient = ingredient;
            Obstacle = obstacle;
            Potion = potion;
        }

        public GoalType GoalType { get; }
        public int Amount { get; }
        public IngredientType Ingredient { get; }
        public ObstacleType Obstacle { get; }
        public PotionType Potion { get; }
    }

    public sealed class ObstacleSpawnData
    {
        public ObstacleSpawnData(GridPosition position, ObstacleType obstacleType, int healthOverride = 0)
        {
            Position = position;
            ObstacleType = obstacleType;
            HealthOverride = healthOverride;
        }

        public GridPosition Position { get; }
        public ObstacleType ObstacleType { get; }
        public int HealthOverride { get; }
    }

    public readonly struct StarThresholds
    {
        public StarThresholds(int oneStar, int twoStars, int threeStars)
        {
            OneStar = Math.Max(0, oneStar);
            TwoStars = Math.Max(OneStar, twoStars);
            ThreeStars = Math.Max(TwoStars, threeStars);
        }

        public int OneStar { get; }
        public int TwoStars { get; }
        public int ThreeStars { get; }

        public int StarsForScore(int score)
        {
            if (score >= ThreeStars)
            {
                return 3;
            }

            if (score >= TwoStars)
            {
                return 2;
            }

            return score >= OneStar ? 1 : 0;
        }
    }

    public sealed class LevelData
    {
        public LevelData(
            int levelNumber,
            int gridWidth,
            int gridHeight,
            int moves,
            IEnumerable<IngredientType> activeIngredients,
            IEnumerable<GoalData> goals,
            StarThresholds starThresholds,
            IEnumerable<ObstacleSpawnData> obstacles = null,
            bool tutorialLevel = false,
            string displayName = null)
        {
            LevelNumber = levelNumber;
            GridWidth = Math.Max(3, gridWidth);
            GridHeight = Math.Max(3, gridHeight);
            Moves = Math.Max(1, moves);
            ActiveIngredients = activeIngredients?.Where(i => i != IngredientType.None).Distinct().ToArray()
                ?? throw new ArgumentNullException(nameof(activeIngredients));
            Goals = goals?.ToArray() ?? throw new ArgumentNullException(nameof(goals));
            Obstacles = obstacles?.ToArray() ?? Array.Empty<ObstacleSpawnData>();
            StarThresholds = starThresholds;
            TutorialLevel = tutorialLevel;
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? $"Level {levelNumber}" : displayName;

            if (ActiveIngredients.Count == 0)
            {
                throw new ArgumentException("At least one active ingredient is required.", nameof(activeIngredients));
            }

            if (Goals.Count == 0)
            {
                throw new ArgumentException("At least one level goal is required.", nameof(goals));
            }
        }

        public int LevelNumber { get; }
        public string DisplayName { get; }
        public int GridWidth { get; }
        public int GridHeight { get; }
        public int Moves { get; }
        public IReadOnlyList<IngredientType> ActiveIngredients { get; }
        public IReadOnlyList<GoalData> Goals { get; }
        public IReadOnlyList<ObstacleSpawnData> Obstacles { get; }
        public StarThresholds StarThresholds { get; }
        public bool TutorialLevel { get; }
    }
}

