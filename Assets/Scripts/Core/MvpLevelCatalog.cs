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
            
            var simpleIngredients = new[]
            {
                IngredientType.RedHerb,
                IngredientType.BlueCrystal,
                IngredientType.GreenLeaf,
                IngredientType.YellowStarDust
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
                // Level 1: "The Recipe" (Tutorial - Basic Matching)
                new LevelData(1, 8, 8, 15, baseIngredients,
                    new[] { 
                        new GoalData(GoalType.CollectIngredient, 15, IngredientType.RedHerb),
                        new GoalData(GoalType.CollectIngredient, 15, IngredientType.BlueCrystal)
                    },
                    new StarThresholds(3000, 5000, 8000), 
                    tutorialLevel: true, 
                    displayName: "The Recipe"),

                // Level 2: "Brewing Potions" (Tutorial - Special Items)
                new LevelData(2, 8, 8, 18, simpleIngredients, // 4 ingredients makes 4-matches much easier
                    new[] { new GoalData(GoalType.CreatePotion, 2, potion: PotionType.LineHorizontal) },
                    new StarThresholds(5000, 8000, 12000), 
                    tutorialLevel: true, 
                    displayName: "Brewing Potions"),

                // Level 3: "Smash the Crates" (Obstacle: Wooden Box)
                new LevelData(3, 8, 8, 16, baseIngredients,
                    new[] { new GoalData(GoalType.BreakObstacle, 16, obstacle: ObstacleType.WoodenBox) },
                    new StarThresholds(8000, 12000, 16000),
                    BoxLines(),
                    displayName: "Smash the Crates"),

                // Level 4: "Cornered" (Board Shape Mastery)
                new LevelData(4, 8, 8, 20, baseIngredients,
                    new[] { 
                        new GoalData(GoalType.CollectIngredient, 30, IngredientType.GreenLeaf),
                        new GoalData(GoalType.CollectIngredient, 30, IngredientType.YellowStarDust)
                    },
                    new StarThresholds(10000, 15000, 20000),
                    BoxCorners(),
                    displayName: "Cornered"),

                // Level 5: "Heavy Lifting" (Obstacle: Stone Block)
                new LevelData(5, 8, 8, 18, baseIngredients,
                    new[] { new GoalData(GoalType.BreakObstacle, 8, obstacle: ObstacleType.StoneBlock) },
                    new StarThresholds(9000, 14000, 19000),
                    StonePlus(),
                    displayName: "Heavy Lifting"),

                // Level 6: "Clean up the Spill" (Obstacle: Dark Tiles)
                new LevelData(6, 8, 8, 22, fireIngredients,
                    new[] { new GoalData(GoalType.ClearTile, 16, obstacle: ObstacleType.DarkTile) },
                    new StarThresholds(12000, 18000, 25000),
                    CheckerboardDarkTiles(),
                    displayName: "Clean up the Spill"),

                // Level 7: "Unfreeze the Ingredients" (Obstacle: Frozen Ingredient)
                new LevelData(7, 8, 8, 22, fireIngredients,
                    new[] { new GoalData(GoalType.CollectIngredient, 40, IngredientType.OrangeFireDrop) },
                    new StarThresholds(15000, 22000, 30000),
                    FrozenRows(),
                    displayName: "Unfreeze the Ingredients"),

                // Level 8: "Break the Chains" (Obstacle: Magic Chain)
                new LevelData(8, 8, 8, 25, baseIngredients,
                    new[] { 
                        new GoalData(GoalType.ClearTile, 10, obstacle: ObstacleType.MagicChain), // Assuming ClearTile handles chains, or BreakObstacle? GoalType.BreakObstacle is used for chains.
                        new GoalData(GoalType.CollectIngredient, 25, IngredientType.PurpleMushroom)
                    },
                    new StarThresholds(18000, 25000, 35000),
                    VerticalChains(),
                    displayName: "Break the Chains"),

                // Level 9: "The Fortress" (Combined Obstacles)
                new LevelData(9, 8, 8, 28, baseIngredients,
                    new[] { 
                        new GoalData(GoalType.BreakObstacle, 16, obstacle: ObstacleType.WoodenBox),
                        new GoalData(GoalType.BreakObstacle, 4, obstacle: ObstacleType.StoneBlock),
                        new GoalData(GoalType.ClearTile, 8, obstacle: ObstacleType.DarkTile)
                    },
                    new StarThresholds(20000, 30000, 45000),
                    FortressPattern(),
                    displayName: "The Fortress"),

                // Level 10: "Restore the Potion Lab" (Boss / Mastery Level)
                new LevelData(10, 8, 8, 30, fireIngredients,
                    new[] { new GoalData(GoalType.RestorePotionLab, 150) },
                    new StarThresholds(25000, 35000, 50000),
                    MandalaPattern(),
                    displayName: "Restore the Lab")
            };
        }

        // Level 3 Layout
        private static IEnumerable<ObstacleSpawnData> BoxLines()
        {
            for (var column = 0; column < 8; column++)
            {
                yield return new ObstacleSpawnData(new GridPosition(3, column), ObstacleType.WoodenBox);
                yield return new ObstacleSpawnData(new GridPosition(4, column), ObstacleType.WoodenBox);
            }
        }

        // Level 4 Layout
        private static IEnumerable<ObstacleSpawnData> BoxCorners()
        {
            for (var r = 0; r < 2; r++)
            for (var c = 0; c < 2; c++)
            {
                yield return new ObstacleSpawnData(new GridPosition(r, c), ObstacleType.WoodenBox);
                yield return new ObstacleSpawnData(new GridPosition(r, 7 - c), ObstacleType.WoodenBox);
                yield return new ObstacleSpawnData(new GridPosition(7 - r, c), ObstacleType.WoodenBox);
                yield return new ObstacleSpawnData(new GridPosition(7 - r, 7 - c), ObstacleType.WoodenBox);
            }
        }

        // Level 5 Layout
        private static IEnumerable<ObstacleSpawnData> StonePlus()
        {
            yield return new ObstacleSpawnData(new GridPosition(2, 3), ObstacleType.StoneBlock);
            yield return new ObstacleSpawnData(new GridPosition(2, 4), ObstacleType.StoneBlock);
            
            yield return new ObstacleSpawnData(new GridPosition(3, 2), ObstacleType.StoneBlock);
            yield return new ObstacleSpawnData(new GridPosition(3, 5), ObstacleType.StoneBlock);
            
            yield return new ObstacleSpawnData(new GridPosition(4, 2), ObstacleType.StoneBlock);
            yield return new ObstacleSpawnData(new GridPosition(4, 5), ObstacleType.StoneBlock);
            
            yield return new ObstacleSpawnData(new GridPosition(5, 3), ObstacleType.StoneBlock);
            yield return new ObstacleSpawnData(new GridPosition(5, 4), ObstacleType.StoneBlock);
        }

        // Level 6 Layout
        private static IEnumerable<ObstacleSpawnData> CheckerboardDarkTiles()
        {
            // Spawn exactly 16 Dark Tiles in a checkerboard pattern
            for (var r = 2; r <= 5; r++)
            for (var c = 0; c < 8; c++)
            {
                if ((r + c) % 2 == 0)
                {
                    yield return new ObstacleSpawnData(new GridPosition(r, c), ObstacleType.DarkTile);
                }
            }
        }

        // Level 7 Layout
        private static IEnumerable<ObstacleSpawnData> FrozenRows()
        {
            for (var column = 1; column < 7; column++)
            {
                yield return new ObstacleSpawnData(new GridPosition(1, column), ObstacleType.FrozenIngredient);
                yield return new ObstacleSpawnData(new GridPosition(6, column), ObstacleType.FrozenIngredient);
            }
        }

        // Level 8 Layout
        private static IEnumerable<ObstacleSpawnData> VerticalChains()
        {
            for (var row = 1; row < 7; row++)
            {
                yield return new ObstacleSpawnData(new GridPosition(row, 2), ObstacleType.MagicChain);
                yield return new ObstacleSpawnData(new GridPosition(row, 5), ObstacleType.MagicChain);
            }
        }

        // Level 9 Layout
        private static IEnumerable<ObstacleSpawnData> FortressPattern()
        {
            // Core
            yield return new ObstacleSpawnData(new GridPosition(3, 3), ObstacleType.StoneBlock);
            yield return new ObstacleSpawnData(new GridPosition(3, 4), ObstacleType.StoneBlock);
            yield return new ObstacleSpawnData(new GridPosition(4, 3), ObstacleType.StoneBlock);
            yield return new ObstacleSpawnData(new GridPosition(4, 4), ObstacleType.StoneBlock);

            // Dark tiles under core and adjacent
            for (var r = 2; r <= 5; r++)
            for (var c = 2; c <= 5; c++)
            {
                if (r >= 3 && r <= 4 && c >= 3 && c <= 4) continue; // Already handled by stone
                yield return new ObstacleSpawnData(new GridPosition(r, c), ObstacleType.DarkTile);
            }

            // Wall of wooden boxes
            for (var column = 1; column <= 6; column++)
            {
                yield return new ObstacleSpawnData(new GridPosition(1, column), ObstacleType.WoodenBox);
                yield return new ObstacleSpawnData(new GridPosition(6, column), ObstacleType.WoodenBox);
            }
            for (var row = 2; row <= 5; row++)
            {
                yield return new ObstacleSpawnData(new GridPosition(row, 1), ObstacleType.WoodenBox);
                yield return new ObstacleSpawnData(new GridPosition(row, 6), ObstacleType.WoodenBox);
            }
        }

        // Level 10 Layout
        private static IEnumerable<ObstacleSpawnData> MandalaPattern()
        {
            // Center Dark Tiles
            for (var r = 3; r <= 4; r++)
            for (var c = 3; c <= 4; c++)
            {
                yield return new ObstacleSpawnData(new GridPosition(r, c), ObstacleType.DarkTile);
            }

            // Frozen Ingredients
            yield return new ObstacleSpawnData(new GridPosition(2, 3), ObstacleType.FrozenIngredient);
            yield return new ObstacleSpawnData(new GridPosition(2, 4), ObstacleType.FrozenIngredient);
            yield return new ObstacleSpawnData(new GridPosition(5, 3), ObstacleType.FrozenIngredient);
            yield return new ObstacleSpawnData(new GridPosition(5, 4), ObstacleType.FrozenIngredient);
            yield return new ObstacleSpawnData(new GridPosition(3, 2), ObstacleType.FrozenIngredient);
            yield return new ObstacleSpawnData(new GridPosition(4, 2), ObstacleType.FrozenIngredient);
            yield return new ObstacleSpawnData(new GridPosition(3, 5), ObstacleType.FrozenIngredient);
            yield return new ObstacleSpawnData(new GridPosition(4, 5), ObstacleType.FrozenIngredient);

            // Magic Chains on diagonals
            yield return new ObstacleSpawnData(new GridPosition(1, 1), ObstacleType.MagicChain);
            yield return new ObstacleSpawnData(new GridPosition(1, 6), ObstacleType.MagicChain);
            yield return new ObstacleSpawnData(new GridPosition(6, 1), ObstacleType.MagicChain);
            yield return new ObstacleSpawnData(new GridPosition(6, 6), ObstacleType.MagicChain);
            
            // Wooden Boxes on outer mid points
            yield return new ObstacleSpawnData(new GridPosition(0, 3), ObstacleType.WoodenBox);
            yield return new ObstacleSpawnData(new GridPosition(0, 4), ObstacleType.WoodenBox);
            yield return new ObstacleSpawnData(new GridPosition(7, 3), ObstacleType.WoodenBox);
            yield return new ObstacleSpawnData(new GridPosition(7, 4), ObstacleType.WoodenBox);
            yield return new ObstacleSpawnData(new GridPosition(3, 0), ObstacleType.WoodenBox);
            yield return new ObstacleSpawnData(new GridPosition(4, 0), ObstacleType.WoodenBox);
            yield return new ObstacleSpawnData(new GridPosition(3, 7), ObstacleType.WoodenBox);
            yield return new ObstacleSpawnData(new GridPosition(4, 7), ObstacleType.WoodenBox);
        }
    }
}
