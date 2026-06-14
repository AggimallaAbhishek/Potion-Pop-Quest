using System;
using System.Collections.Generic;

namespace PotionPopQuest.Core
{
    public readonly struct BoardCellSnapshot
    {
        public BoardCellSnapshot(IngredientType ingredient, PotionType potion, ObstacleType obstacle, int obstacleHealth)
        {
            Ingredient = ingredient;
            Potion = potion;
            Obstacle = obstacle;
            ObstacleHealth = obstacleHealth;
        }

        public IngredientType Ingredient { get; }
        public PotionType Potion { get; }
        public ObstacleType Obstacle { get; }
        public int ObstacleHealth { get; }
        public bool HasIngredient => Ingredient != IngredientType.None;
        public bool HasPotion => Potion != PotionType.None;
        public bool HasObstacle => Obstacle != ObstacleType.None;
        public bool BlocksIngredientSpace => Obstacle == ObstacleType.WoodenBox || Obstacle == ObstacleType.StoneBlock;
        public bool LocksIngredient => Obstacle == ObstacleType.FrozenIngredient || Obstacle == ObstacleType.MagicChain;
        public bool AcceptsIngredient => !BlocksIngredientSpace;
        public bool CanMoveIngredient => HasIngredient && AcceptsIngredient && !LocksIngredient;

        public static BoardCellSnapshot From(BoardCell cell)
        {
            if (cell == null)
            {
                return default;
            }

            return new BoardCellSnapshot(cell.Ingredient, cell.Potion, cell.Obstacle, cell.ObstacleHealth);
        }
    }

    public sealed class BoardSnapshot
    {
        private readonly BoardCellSnapshot[,] _cells;

        private BoardSnapshot(int width, int height, BoardCellSnapshot[,] cells)
        {
            Width = width;
            Height = height;
            _cells = cells;
        }

        public int Width { get; }
        public int Height { get; }

        public BoardCellSnapshot GetCell(GridPosition position)
        {
            if (!InBounds(position))
            {
                throw new ArgumentOutOfRangeException(nameof(position), $"Position {position} is outside {Width}x{Height} board snapshot.");
            }

            return _cells[position.Row, position.Column];
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

        public static BoardSnapshot From(BoardState board)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            var cells = new BoardCellSnapshot[board.Height, board.Width];
            foreach (var position in board.AllPositions())
            {
                cells[position.Row, position.Column] = BoardCellSnapshot.From(board.GetCell(position));
            }

            return new BoardSnapshot(board.Width, board.Height, cells);
        }
    }
}
