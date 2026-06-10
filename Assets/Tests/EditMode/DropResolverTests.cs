using NUnit.Framework;
using PotionPopQuest.Core;

namespace PotionPopQuest.Tests
{
    public sealed class DropResolverTests
    {
        [Test]
        public void ClearDropAndSpawn_DamagesAdjacentWoodenBox()
        {
            var board = new BoardState(3, 3);
            foreach (var position in board.AllPositions())
            {
                board.SetIngredient(position, IngredientType.BlueCrystal);
            }

            var boxPosition = new GridPosition(1, 1);
            board.SetObstacle(boxPosition, ObstacleType.WoodenBox, 1);
            var clearPosition = new GridPosition(1, 0);

            var result = new DropResolver().ClearDropAndSpawn(
                board,
                new[] { clearPosition },
                new[] { IngredientType.RedHerb, IngredientType.BlueCrystal },
                new DeterministicRandomSource());

            Assert.That(result.DestroyedObstacles.Count, Is.EqualTo(1));
            Assert.That(result.DestroyedObstacles[0].Position, Is.EqualTo(boxPosition));
            Assert.That(board.GetCell(boxPosition).Obstacle, Is.EqualTo(ObstacleType.None));
        }

        [Test]
        public void ClearDropAndSpawn_ClearsDarkTileUnderMatchedIngredient()
        {
            var board = new BoardState(3, 3);
            foreach (var position in board.AllPositions())
            {
                board.SetIngredient(position, IngredientType.GreenLeaf);
            }

            var darkTile = new GridPosition(1, 1);
            board.SetObstacle(darkTile, ObstacleType.DarkTile, 1);

            var result = new DropResolver().ClearDropAndSpawn(
                board,
                new[] { darkTile },
                new[] { IngredientType.RedHerb, IngredientType.BlueCrystal },
                new DeterministicRandomSource());

            Assert.That(result.ClearedTiles.Count, Is.EqualTo(1));
            Assert.That(board.GetCell(darkTile).Obstacle, Is.EqualTo(ObstacleType.None));
        }

        [Test]
        public void ClearDropAndSpawn_UsesImpactPositionsForPotionAnchorObstacleEffects()
        {
            var board = new BoardState(3, 3);
            foreach (var position in board.AllPositions())
            {
                board.SetIngredient(position, IngredientType.GreenLeaf);
            }

            var anchor = new GridPosition(1, 1);
            var box = new GridPosition(1, 2);
            board.SetObstacle(anchor, ObstacleType.DarkTile, 1);
            board.SetObstacle(box, ObstacleType.WoodenBox, 1);

            var result = new DropResolver().ClearDropAndSpawn(
                board,
                new GridPosition[0],
                new[] { IngredientType.RedHerb, IngredientType.BlueCrystal },
                new DeterministicRandomSource(),
                new[] { anchor });

            Assert.That(result.ClearedTiles.Count, Is.EqualTo(1));
            Assert.That(result.DestroyedObstacles.Count, Is.EqualTo(1));
            Assert.That(board.GetCell(anchor).Obstacle, Is.EqualTo(ObstacleType.None));
            Assert.That(board.GetCell(box).Obstacle, Is.EqualTo(ObstacleType.None));
        }
    }
}
