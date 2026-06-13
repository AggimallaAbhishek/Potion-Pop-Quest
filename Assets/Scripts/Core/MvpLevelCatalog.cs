using System.Collections.Generic;

namespace PotionPopQuest.Core
{
    public static class MvpLevelCatalog
    {
        public static IReadOnlyList<LevelData> CreateLevels()
        {
            var baseIngredients = new[]
            {
                IngredientType.RedHerb,
                IngredientType.BlueCrystal,
                IngredientType.GreenLeaf,
                IngredientType.YellowStarDust,
                IngredientType.PurpleMushroom
            };
            var fireIngredients = new[]
            {
                IngredientType.RedHerb,
                IngredientType.BlueCrystal,
                IngredientType.GreenLeaf,
                IngredientType.YellowStarDust,
                IngredientType.PurpleMushroom,
                IngredientType.OrangeFireDrop
            };

            return new[]
            {
                new LevelData(1, 8, 8, 13, baseIngredients,
                    new[] { new GoalData(GoalType.CollectIngredient, 18, IngredientType.RedHerb) },
                    new StarThresholds(8000, 12000, 16000), tutorialLevel: true),
                new LevelData(2, 8, 8, 14, baseIngredients,
                    new[] { new GoalData(GoalType.CollectIngredient, 24, IngredientType.BlueCrystal) },
                    new StarThresholds(13000, 18000, 23000), tutorialLevel: true),
                new LevelData(3, 8, 8, 16, baseIngredients,
                    new[] { new GoalData(GoalType.CollectIngredient, 30, IngredientType.GreenLeaf) },
                    new StarThresholds(16000, 22000, 24500), tutorialLevel: true),
                new LevelData(4, 8, 8, 6, baseIngredients,
                    new[] { new GoalData(GoalType.CreatePotion, 1, potion: PotionType.LineHorizontal) },
                    new StarThresholds(2000, 5000, 10500), tutorialLevel: true),
                new LevelData(5, 8, 8, 8, baseIngredients,
                    new[] { new GoalData(GoalType.BreakObstacle, 8, obstacle: ObstacleType.WoodenBox) },
                    new StarThresholds(4500, 7500, 12000),
                    BoxLine(row: 3, startColumn: 0, count: 8)),
                new LevelData(6, 8, 8, 16, fireIngredients,
                    new[] { new GoalData(GoalType.CollectIngredient, 14, IngredientType.OrangeFireDrop) },
                    new StarThresholds(7000, 9000, 11000)),
                new LevelData(7, 8, 8, 16, fireIngredients,
                    new[] { new GoalData(GoalType.ClearTile, 6, obstacle: ObstacleType.DarkTile) },
                    new StarThresholds(6500, 9000, 12000),
                    DarkTiles()),
                new LevelData(8, 8, 8, 21, baseIngredients,
                    new[] { new GoalData(GoalType.CreatePotion, 1, potion: PotionType.Bomb) },
                    new StarThresholds(13000, 21000, 32000)),
                new LevelData(9, 8, 8, 10, baseIngredients,
                    new[] { new GoalData(GoalType.BreakObstacle, 12, obstacle: ObstacleType.WoodenBox) },
                    new StarThresholds(7000, 10500, 13500),
                    BoxFrame()),
                new LevelData(10, 8, 8, 11, baseIngredients,
                    new[] { new GoalData(GoalType.RestorePotionLab, 100) },
                    new StarThresholds(11000, 13500, 16000),
                    BossPattern(),
                    displayName: "Restore Potion Lab")
            };
        }

        private static IEnumerable<ObstacleSpawnData> BoxLine(int row, int startColumn, int count)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new ObstacleSpawnData(new GridPosition(row, startColumn + i), ObstacleType.WoodenBox);
            }
        }

        private static IEnumerable<ObstacleSpawnData> DarkTiles()
        {
            yield return new ObstacleSpawnData(new GridPosition(2, 2), ObstacleType.DarkTile);
            yield return new ObstacleSpawnData(new GridPosition(2, 5), ObstacleType.DarkTile);
            yield return new ObstacleSpawnData(new GridPosition(3, 3), ObstacleType.DarkTile);
            yield return new ObstacleSpawnData(new GridPosition(3, 4), ObstacleType.DarkTile);
            yield return new ObstacleSpawnData(new GridPosition(5, 2), ObstacleType.DarkTile);
            yield return new ObstacleSpawnData(new GridPosition(5, 5), ObstacleType.DarkTile);
        }

        private static IEnumerable<ObstacleSpawnData> BoxFrame()
        {
            for (var column = 2; column <= 5; column++)
            {
                yield return new ObstacleSpawnData(new GridPosition(2, column), ObstacleType.WoodenBox);
                yield return new ObstacleSpawnData(new GridPosition(5, column), ObstacleType.WoodenBox);
            }

            yield return new ObstacleSpawnData(new GridPosition(3, 2), ObstacleType.WoodenBox);
            yield return new ObstacleSpawnData(new GridPosition(3, 5), ObstacleType.WoodenBox);
            yield return new ObstacleSpawnData(new GridPosition(4, 2), ObstacleType.WoodenBox);
            yield return new ObstacleSpawnData(new GridPosition(4, 5), ObstacleType.WoodenBox);
        }

        private static IEnumerable<ObstacleSpawnData> BossPattern()
        {
            foreach (var obstacle in DarkTiles())
            {
                yield return obstacle;
            }

            yield return new ObstacleSpawnData(new GridPosition(1, 3), ObstacleType.WoodenBox);
            yield return new ObstacleSpawnData(new GridPosition(1, 4), ObstacleType.WoodenBox);
            yield return new ObstacleSpawnData(new GridPosition(6, 3), ObstacleType.WoodenBox);
            yield return new ObstacleSpawnData(new GridPosition(6, 4), ObstacleType.WoodenBox);
        }
    }
}
