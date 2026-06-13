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
                    new StarThresholds(800, 1400, 2200), tutorialLevel: true),
                new LevelData(2, 8, 8, 14, baseIngredients,
                    new[] { new GoalData(GoalType.CollectIngredient, 24, IngredientType.BlueCrystal) },
                    new StarThresholds(1000, 1700, 2600), tutorialLevel: true),
                new LevelData(3, 8, 8, 16, baseIngredients,
                    new[] { new GoalData(GoalType.CollectIngredient, 30, IngredientType.GreenLeaf) },
                    new StarThresholds(1200, 2000, 3000), tutorialLevel: true),
                new LevelData(4, 8, 8, 8, baseIngredients,
                    new[] { new GoalData(GoalType.CreatePotion, 1, potion: PotionType.LineHorizontal) },
                    new StarThresholds(1300, 2200, 3300), tutorialLevel: true),
                new LevelData(5, 8, 8, 10, baseIngredients,
                    new[] { new GoalData(GoalType.BreakObstacle, 8, obstacle: ObstacleType.WoodenBox) },
                    new StarThresholds(1500, 2500, 3700),
                    BoxLine(row: 3, startColumn: 0, count: 8)),
                new LevelData(6, 8, 8, 16, fireIngredients,
                    new[] { new GoalData(GoalType.CollectIngredient, 14, IngredientType.OrangeFireDrop) },
                    new StarThresholds(1600, 2600, 3900)),
                new LevelData(7, 8, 8, 16, fireIngredients,
                    new[] { new GoalData(GoalType.ClearTile, 6, obstacle: ObstacleType.DarkTile) },
                    new StarThresholds(1700, 2800, 4200),
                    DarkTiles()),
                new LevelData(8, 8, 8, 16, baseIngredients,
                    new[] { new GoalData(GoalType.CreatePotion, 1, potion: PotionType.Bomb) },
                    new StarThresholds(1800, 3000, 4500)),
                new LevelData(9, 8, 8, 12, baseIngredients,
                    new[] { new GoalData(GoalType.BreakObstacle, 12, obstacle: ObstacleType.WoodenBox) },
                    new StarThresholds(2000, 3300, 4900),
                    BoxFrame()),
                new LevelData(10, 8, 8, 16, baseIngredients,
                    new[] { new GoalData(GoalType.RestorePotionLab, 100) },
                    new StarThresholds(2400, 3800, 5600),
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
