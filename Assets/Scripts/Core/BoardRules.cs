using System;

namespace PotionPopQuest.Core
{
    public static class BoardRules
    {
        public static bool AreAdjacent(GridPosition first, GridPosition second)
        {
            var rowDelta = Math.Abs(first.Row - second.Row);
            var columnDelta = Math.Abs(first.Column - second.Column);
            return rowDelta + columnDelta == 1;
        }

        public static bool CanSwap(BoardState board, GridPosition first, GridPosition second)
        {
            if (board == null || !board.InBounds(first) || !board.InBounds(second) || !AreAdjacent(first, second))
            {
                return false;
            }

            var firstCell = board.GetCell(first);
            var secondCell = board.GetCell(second);
            return firstCell.CanMoveIngredient && secondCell.CanMoveIngredient;
        }

        public static int DefaultObstacleHealth(ObstacleType obstacleType)
        {
            switch (obstacleType)
            {
                case ObstacleType.WoodenBox:
                    return 1;
                case ObstacleType.StoneBlock:
                    return 2;
                case ObstacleType.DarkTile:
                case ObstacleType.FrozenIngredient:
                case ObstacleType.MagicChain:
                    return 1;
                default:
                    return 0;
            }
        }
    }
}

