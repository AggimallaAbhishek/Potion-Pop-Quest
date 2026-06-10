using System;
using System.Collections.Generic;

namespace PotionPopQuest.Core
{
    public sealed class BoardState
    {
        private readonly BoardCell[,] _cells;

        public BoardState(int width, int height)
        {
            Width = Math.Max(3, width);
            Height = Math.Max(3, height);
            _cells = new BoardCell[Height, Width];

            for (var row = 0; row < Height; row++)
            {
                for (var column = 0; column < Width; column++)
                {
                    _cells[row, column] = new BoardCell();
                }
            }
        }

        public int Width { get; }
        public int Height { get; }

        public BoardCell GetCell(GridPosition position)
        {
            EnsureInBounds(position);
            return _cells[position.Row, position.Column];
        }

        public void SetCell(GridPosition position, BoardCell cell)
        {
            EnsureInBounds(position);
            _cells[position.Row, position.Column] = cell ?? new BoardCell();
        }

        public bool InBounds(GridPosition position)
        {
            return position.Row >= 0 && position.Row < Height && position.Column >= 0 && position.Column < Width;
        }

        public IEnumerable<GridPosition> AllPositions()
        {
            for (var row = 0; row < Height; row++)
            {
                for (var column = 0; column < Width; column++)
                {
                    yield return new GridPosition(row, column);
                }
            }
        }

        public BoardState Clone()
        {
            var clone = new BoardState(Width, Height);
            foreach (var position in AllPositions())
            {
                clone.SetCell(position, GetCell(position).Clone());
            }

            return clone;
        }

        public void SetIngredient(GridPosition position, IngredientType ingredient, PotionType potion = PotionType.None)
        {
            var cell = GetCell(position);
            if (!cell.AcceptsIngredient)
            {
                return;
            }

            cell.Ingredient = ingredient;
            cell.Potion = ingredient == IngredientType.None ? PotionType.None : potion;
        }

        public void SetObstacle(GridPosition position, ObstacleType obstacle, int health)
        {
            var cell = GetCell(position);
            cell.Obstacle = obstacle;
            cell.ObstacleHealth = obstacle == ObstacleType.None ? 0 : Math.Max(1, health);

            if (cell.BlocksIngredientSpace)
            {
                cell.ClearIngredient();
            }
        }

        public void SwapIngredients(GridPosition first, GridPosition second)
        {
            EnsureInBounds(first);
            EnsureInBounds(second);

            var firstCell = GetCell(first);
            var secondCell = GetCell(second);
            var firstIngredient = firstCell.Ingredient;
            var firstPotion = firstCell.Potion;

            firstCell.Ingredient = secondCell.Ingredient;
            firstCell.Potion = secondCell.Potion;
            secondCell.Ingredient = firstIngredient;
            secondCell.Potion = firstPotion;
        }

        private void EnsureInBounds(GridPosition position)
        {
            if (!InBounds(position))
            {
                throw new ArgumentOutOfRangeException(nameof(position), $"Position {position} is outside {Width}x{Height} board.");
            }
        }
    }
}

