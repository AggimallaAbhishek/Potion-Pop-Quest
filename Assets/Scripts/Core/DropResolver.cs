using System;
using System.Collections.Generic;
using System.Linq;

namespace PotionPopQuest.Core
{
    public sealed class ClearedIngredient
    {
        public ClearedIngredient(GridPosition position, IngredientType ingredient)
        {
            Position = position;
            Ingredient = ingredient;
        }

        public GridPosition Position { get; }
        public IngredientType Ingredient { get; }
    }

    public sealed class ObstacleEvent
    {
        public ObstacleEvent(GridPosition position, ObstacleType obstacleType)
        {
            Position = position;
            ObstacleType = obstacleType;
        }

        public GridPosition Position { get; }
        public ObstacleType ObstacleType { get; }
    }

    public sealed class DropResult
    {
        public DropResult(
            IEnumerable<ClearedIngredient> clearedIngredients,
            IEnumerable<ObstacleEvent> destroyedObstacles,
            IEnumerable<ObstacleEvent> clearedTiles,
            int droppedCount,
            int spawnedCount)
        {
            ClearedIngredients = clearedIngredients.ToArray();
            DestroyedObstacles = destroyedObstacles.ToArray();
            ClearedTiles = clearedTiles.ToArray();
            DroppedCount = droppedCount;
            SpawnedCount = spawnedCount;
        }

        public IReadOnlyList<ClearedIngredient> ClearedIngredients { get; }
        public IReadOnlyList<ObstacleEvent> DestroyedObstacles { get; }
        public IReadOnlyList<ObstacleEvent> ClearedTiles { get; }
        public int DroppedCount { get; }
        public int SpawnedCount { get; }
    }

    public sealed class DropResolver
    {
        public DropResult ClearDropAndSpawn(
            BoardState board,
            IEnumerable<GridPosition> clearPositions,
            IReadOnlyList<IngredientType> activeIngredients,
            IRandomSource random,
            IEnumerable<GridPosition> impactPositions = null)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            var positions = clearPositions?.Where(board.InBounds).Distinct().ToArray() ?? Array.Empty<GridPosition>();
            var impacts = (impactPositions ?? positions).Where(board.InBounds).Distinct().ToArray();
            var clearedIngredients = new List<ClearedIngredient>();
            var clearedTiles = new List<ObstacleEvent>();
            var destroyedObstacles = new List<ObstacleEvent>();

            foreach (var position in positions)
            {
                var cell = board.GetCell(position);
                if (cell.Ingredient != IngredientType.None)
                {
                    clearedIngredients.Add(new ClearedIngredient(position, cell.Ingredient));
                    cell.ClearIngredient();
                }
            }

            foreach (var position in impacts)
            {
                var cell = board.GetCell(position);
                if (cell.Obstacle == ObstacleType.DarkTile)
                {
                    clearedTiles.Add(new ObstacleEvent(position, ObstacleType.DarkTile));
                    cell.Obstacle = ObstacleType.None;
                    cell.ObstacleHealth = 0;
                }
            }

            DamageAdjacentObstacles(board, impacts, destroyedObstacles);
            var movement = ApplyGravity(board, activeIngredients, random);

            return new DropResult(clearedIngredients, destroyedObstacles, clearedTiles, movement.dropped, movement.spawned);
        }

        private static void DamageAdjacentObstacles(
            BoardState board,
            IEnumerable<GridPosition> clearPositions,
            ICollection<ObstacleEvent> destroyedObstacles)
        {
            var damaged = new HashSet<GridPosition>();
            foreach (var position in clearPositions)
            {
                foreach (var neighbor in Adjacent(position))
                {
                    if (!board.InBounds(neighbor) || damaged.Contains(neighbor))
                    {
                        continue;
                    }

                    var cell = board.GetCell(neighbor);
                    if (cell.Obstacle != ObstacleType.WoodenBox && cell.Obstacle != ObstacleType.StoneBlock)
                    {
                        continue;
                    }

                    damaged.Add(neighbor);
                    cell.ObstacleHealth--;
                    if (cell.ObstacleHealth <= 0)
                    {
                        destroyedObstacles.Add(new ObstacleEvent(neighbor, cell.Obstacle));
                        cell.Obstacle = ObstacleType.None;
                        cell.ObstacleHealth = 0;
                    }
                }
            }
        }

        private static IEnumerable<GridPosition> Adjacent(GridPosition position)
        {
            yield return new GridPosition(position.Row - 1, position.Column);
            yield return new GridPosition(position.Row + 1, position.Column);
            yield return new GridPosition(position.Row, position.Column - 1);
            yield return new GridPosition(position.Row, position.Column + 1);
        }

        private static (int dropped, int spawned) ApplyGravity(
            BoardState board,
            IReadOnlyList<IngredientType> activeIngredients,
            IRandomSource random)
        {
            var dropped = 0;
            var spawned = 0;

            for (var column = 0; column < board.Width; column++)
            {
                var row = board.Height - 1;
                while (row >= 0)
                {
                    if (board.GetCell(new GridPosition(row, column)).BlocksIngredientSpace)
                    {
                        row--;
                        continue;
                    }

                    var segmentBottom = row;
                    while (row >= 0 && !board.GetCell(new GridPosition(row, column)).BlocksIngredientSpace)
                    {
                        row--;
                    }

                    var segmentTop = row + 1;
                    var falling = new List<(IngredientType ingredient, PotionType potion, int originalRow)>();
                    for (var scan = segmentBottom; scan >= segmentTop; scan--)
                    {
                        var cell = board.GetCell(new GridPosition(scan, column));
                        if (cell.Ingredient == IngredientType.None)
                        {
                            continue;
                        }

                        falling.Add((cell.Ingredient, cell.Potion, scan));
                        cell.ClearIngredient();
                    }

                    var write = segmentBottom;
                    foreach (var item in falling)
                    {
                        var target = new GridPosition(write, column);
                        board.SetIngredient(target, item.ingredient, item.potion);
                        if (write != item.originalRow)
                        {
                            dropped++;
                        }

                        write--;
                    }

                    while (write >= segmentTop)
                    {
                        var target = new GridPosition(write, column);
                        var ingredient = activeIngredients[random.Range(0, activeIngredients.Count)];
                        board.SetIngredient(target, ingredient);
                        spawned++;
                        write--;
                    }
                }
            }

            return (dropped, spawned);
        }
    }
}
