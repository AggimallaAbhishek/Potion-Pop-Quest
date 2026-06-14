using System;
using System.Collections.Generic;
using System.Linq;

namespace PotionPopQuest.Core
{
    public enum BoardAnimationEventKind
    {
        Swap = 0,
        InvalidSwap = 1,
        CascadeStarted = 2,
        Clear = 3,
        PotionCreated = 4,
        PotionActivated = 5,
        ObstacleDamaged = 6,
        ObstacleDestroyed = 7,
        TileDropped = 8,
        TileSpawned = 9,
        Win = 10,
        Lose = 11,
        BoardShuffled = 12
    }

    public sealed class BoardAnimationEvent
    {
        public BoardAnimationEvent(
            BoardAnimationEventKind kind,
            IEnumerable<GridPosition> positions = null,
            GridPosition from = default,
            GridPosition to = default,
            IngredientType ingredient = IngredientType.None,
            PotionType potion = PotionType.None,
            ObstacleType obstacle = ObstacleType.None,
            int cascadeIndex = 0,
            IEnumerable<TileMovementEvent> movements = null)
        {
            Kind = kind;
            Positions = positions?.Distinct().ToArray() ?? Array.Empty<GridPosition>();
            From = from;
            To = to;
            Ingredient = ingredient;
            Potion = potion;
            Obstacle = obstacle;
            CascadeIndex = cascadeIndex;
            Movements = movements?.ToArray() ?? Array.Empty<TileMovementEvent>();
        }

        public BoardAnimationEventKind Kind { get; }
        public IReadOnlyList<GridPosition> Positions { get; }
        public GridPosition From { get; }
        public GridPosition To { get; }
        public IngredientType Ingredient { get; }
        public PotionType Potion { get; }
        public ObstacleType Obstacle { get; }
        public int CascadeIndex { get; }
        public IReadOnlyList<TileMovementEvent> Movements { get; }
    }

    public sealed class TileMovementEvent
    {
        public TileMovementEvent(GridPosition from, GridPosition to, IngredientType ingredient, PotionType potion)
        {
            From = from;
            To = to;
            Ingredient = ingredient;
            Potion = potion;
        }

        public GridPosition From { get; }
        public GridPosition To { get; }
        public IngredientType Ingredient { get; }
        public PotionType Potion { get; }
    }

    public sealed class TileSpawnEvent
    {
        public TileSpawnEvent(GridPosition position, IngredientType ingredient)
        {
            Position = position;
            Ingredient = ingredient;
        }

        public GridPosition Position { get; }
        public IngredientType Ingredient { get; }
    }
}
