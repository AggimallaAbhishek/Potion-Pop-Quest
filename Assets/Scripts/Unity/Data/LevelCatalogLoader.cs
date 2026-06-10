using System;
using System.Collections.Generic;
using PotionPopQuest.Core;
using UnityEngine;

namespace PotionPopQuest.Unity
{
    public sealed class LevelCatalogLoader
    {
        private const string ResourcePath = "Levels/mvp_levels";
        private readonly IGameLogger _logger;

        public LevelCatalogLoader(IGameLogger logger)
        {
            _logger = logger;
        }

        public IReadOnlyList<LevelData> LoadLevels(LevelDefinition[] levelDefinitions)
        {
            if (levelDefinitions != null && levelDefinitions.Length > 0)
            {
                var levels = new List<LevelData>();
                foreach (var definition in levelDefinitions)
                {
                    if (definition != null)
                    {
                        levels.Add(definition.ToLevelData());
                    }
                }

                if (levels.Count > 0)
                {
                    _logger.Log(LogCategory.Board, $"Loaded {levels.Count} ScriptableObject levels.");
                    return levels;
                }
            }

            var textAsset = Resources.Load<TextAsset>(ResourcePath);
            if (textAsset != null)
            {
                try
                {
                    var levels = ParseJsonCatalog(textAsset.text);
                    if (levels.Count > 0)
                    {
                        _logger.Log(LogCategory.Board, $"Loaded {levels.Count} JSON levels from Resources.");
                        return levels;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Warn(LogCategory.Board, $"Could not load JSON levels: {ex.Message}");
                }
            }

            _logger.Warn(LogCategory.Board, "Using built-in MVP level fallback.");
            return MvpLevelCatalog.CreateLevels();
        }

        private static IReadOnlyList<LevelData> ParseJsonCatalog(string json)
        {
            var catalog = JsonUtility.FromJson<LevelCatalogJson>(json);
            var levels = new List<LevelData>();
            if (catalog?.levels == null)
            {
                return levels;
            }

            foreach (var level in catalog.levels)
            {
                levels.Add(level.ToLevelData());
            }

            return levels;
        }
    }

    [Serializable]
    internal sealed class LevelCatalogJson
    {
        public LevelJson[] levels;
    }

    [Serializable]
    internal sealed class LevelJson
    {
        public int levelNumber;
        public string displayName;
        public int gridWidth;
        public int gridHeight;
        public int moves;
        public bool tutorialLevel;
        public string[] activeIngredients;
        public StarThresholdJson starThresholds;
        public GoalJson[] goals;
        public ObstacleJson[] obstacles;

        public LevelData ToLevelData()
        {
            var ingredients = new List<IngredientType>();
            foreach (var item in activeIngredients ?? Array.Empty<string>())
            {
                ingredients.Add(ParseEnum<IngredientType>(item));
            }

            var convertedGoals = new List<GoalData>();
            foreach (var goal in goals ?? Array.Empty<GoalJson>())
            {
                convertedGoals.Add(goal.ToGoalData());
            }

            var convertedObstacles = new List<ObstacleSpawnData>();
            foreach (var obstacle in obstacles ?? Array.Empty<ObstacleJson>())
            {
                convertedObstacles.Add(obstacle.ToObstacleSpawnData());
            }

            return new LevelData(
                levelNumber,
                gridWidth,
                gridHeight,
                moves,
                ingredients,
                convertedGoals,
                starThresholds?.ToStarThresholds() ?? new StarThresholds(800, 1400, 2200),
                convertedObstacles,
                tutorialLevel,
                displayName);
        }

        internal static TEnum ParseEnum<TEnum>(string value) where TEnum : struct
        {
            return Enum.TryParse(value, true, out TEnum result) ? result : default;
        }
    }

    [Serializable]
    internal sealed class GoalJson
    {
        public string goalType;
        public string goalItem;
        public int goalAmount;

        public GoalData ToGoalData()
        {
            var type = LevelJson.ParseEnum<GoalType>(goalType);
            var ingredient = LevelJson.ParseEnum<IngredientType>(goalItem);
            var obstacle = LevelJson.ParseEnum<ObstacleType>(goalItem);
            var potion = LevelJson.ParseEnum<PotionType>(goalItem);
            return new GoalData(type, goalAmount, ingredient, obstacle, potion);
        }
    }

    [Serializable]
    internal sealed class ObstacleJson
    {
        public int row;
        public int column;
        public string type;
        public int healthOverride;

        public ObstacleSpawnData ToObstacleSpawnData()
        {
            var obstacle = LevelJson.ParseEnum<ObstacleType>(type);
            return new ObstacleSpawnData(new GridPosition(row, column), obstacle, healthOverride);
        }
    }

    [Serializable]
    internal sealed class StarThresholdJson
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

