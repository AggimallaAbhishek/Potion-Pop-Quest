using System;
using System.Collections.Generic;
using PotionPopQuest.Core;
using UnityEngine;

namespace PotionPopQuest.Unity
{
    [CreateAssetMenu(menuName = "Potion Pop Quest/Level Definition", fileName = "LevelDefinition")]
    public sealed class LevelDefinition : ScriptableObject
    {
        public int levelNumber = 1;
        public string displayName = "Level 1";
        public int gridWidth = 8;
        public int gridHeight = 8;
        public int moves = 20;
        public bool tutorialLevel;
        public StarThresholdDefinition starThresholds = new StarThresholdDefinition { oneStar = 800, twoStars = 1400, threeStars = 2200 };
        public IngredientType[] activeIngredients =
        {
            IngredientType.RedHerb,
            IngredientType.BlueCrystal,
            IngredientType.GreenLeaf,
            IngredientType.YellowStarDust,
            IngredientType.PurpleMushroom
        };
        public GoalDefinition[] goals = Array.Empty<GoalDefinition>();
        public ObstacleDefinition[] obstacles = Array.Empty<ObstacleDefinition>();

        public LevelData ToLevelData()
        {
            var convertedGoals = new List<GoalData>();
            foreach (var goal in goals)
            {
                convertedGoals.Add(goal.ToGoalData());
            }

            var convertedObstacles = new List<ObstacleSpawnData>();
            foreach (var obstacle in obstacles)
            {
                convertedObstacles.Add(obstacle.ToObstacleSpawnData());
            }

            return new LevelData(
                levelNumber,
                gridWidth,
                gridHeight,
                moves,
                activeIngredients,
                convertedGoals,
                starThresholds.ToStarThresholds(),
                convertedObstacles,
                tutorialLevel,
                displayName);
        }
    }

    [Serializable]
    public sealed class GoalDefinition
    {
        public GoalType goalType;
        public int amount = 1;
        public IngredientType ingredient;
        public ObstacleType obstacle;
        public PotionType potion;

        public GoalData ToGoalData()
        {
            return new GoalData(goalType, amount, ingredient, obstacle, potion);
        }
    }

    [Serializable]
    public sealed class ObstacleDefinition
    {
        public int row;
        public int column;
        public ObstacleType obstacleType = ObstacleType.WoodenBox;
        public int healthOverride;

        public ObstacleSpawnData ToObstacleSpawnData()
        {
            return new ObstacleSpawnData(new GridPosition(row, column), obstacleType, healthOverride);
        }
    }

    [Serializable]
    public sealed class StarThresholdDefinition
    {
        public int oneStar;
        public int twoStars;
        public int threeStars;

        public StarThresholds ToStarThresholds()
        {
            return new StarThresholds(oneStar, twoStars, threeStars);
        }
    }
}

