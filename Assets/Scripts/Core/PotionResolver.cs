using System;
using System.Collections.Generic;
using System.Linq;

namespace PotionPopQuest.Core
{
    public sealed class PotionActivation
    {
        public PotionActivation(PotionType potionType, IEnumerable<GridPosition> affectedPositions)
        {
            PotionType = potionType;
            AffectedPositions = affectedPositions.Distinct().ToArray();
        }

        public PotionType PotionType { get; }
        public IReadOnlyList<GridPosition> AffectedPositions { get; }
    }

    public sealed class PotionResolver
    {
        public PotionActivation Resolve(BoardState board, GridPosition position, PotionType potionType)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            var affected = new List<GridPosition>();
            switch (potionType)
            {
                case PotionType.LineHorizontal:
                    for (var column = 0; column < board.Width; column++)
                    {
                        affected.Add(new GridPosition(position.Row, column));
                    }

                    break;
                case PotionType.LineVertical:
                    for (var row = 0; row < board.Height; row++)
                    {
                        affected.Add(new GridPosition(row, position.Column));
                    }

                    break;
                case PotionType.Bomb:
                    for (var row = position.Row - 1; row <= position.Row + 1; row++)
                    {
                        for (var column = position.Column - 1; column <= position.Column + 1; column++)
                        {
                            var candidate = new GridPosition(row, column);
                            if (board.InBounds(candidate))
                            {
                                affected.Add(candidate);
                            }
                        }
                    }

                    break;
                case PotionType.Lightning:
                    var target = board.GetCell(position).Ingredient;
                    if (target == IngredientType.None)
                    {
                        target = FirstGoalLikeIngredient(board);
                    }

                    affected.AddRange(board.AllPositions().Where(p => board.GetCell(p).Ingredient == target));
                    break;
                case PotionType.Mega:
                    affected.AddRange(board.AllPositions());
                    break;
            }

            return new PotionActivation(potionType, affected);
        }

        private static IngredientType FirstGoalLikeIngredient(BoardState board)
        {
            foreach (var position in board.AllPositions())
            {
                var ingredient = board.GetCell(position).Ingredient;
                if (ingredient != IngredientType.None)
                {
                    return ingredient;
                }
            }

            return IngredientType.None;
        }
    }
}

