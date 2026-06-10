using System;
using System.Collections.Generic;
using System.Linq;

namespace PotionPopQuest.Core
{
    public sealed class BoardGenerator
    {
        private const int MaxGenerationAttempts = 100;
        private readonly MatchFinder _matchFinder;

        public BoardGenerator(MatchFinder matchFinder = null)
        {
            _matchFinder = matchFinder ?? new MatchFinder();
        }

        public BoardState Generate(LevelData level, IRandomSource random)
        {
            if (level == null)
            {
                throw new ArgumentNullException(nameof(level));
            }

            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            for (var attempt = 0; attempt < MaxGenerationAttempts; attempt++)
            {
                var board = CreateCandidate(level, random);
                if (_matchFinder.FindMatches(board).Count == 0 && HasAnyValidMove(board))
                {
                    return board;
                }
            }

            return CreateCandidate(level, random);
        }

        public bool HasAnyValidMove(BoardState board)
        {
            foreach (var position in board.AllPositions())
            {
                var right = new GridPosition(position.Row, position.Column + 1);
                var down = new GridPosition(position.Row + 1, position.Column);

                if (CreatesMatchAfterSwap(board, position, right) || CreatesMatchAfterSwap(board, position, down))
                {
                    return true;
                }
            }

            return false;
        }

        private BoardState CreateCandidate(LevelData level, IRandomSource random)
        {
            var board = new BoardState(level.GridWidth, level.GridHeight);
            foreach (var obstacle in level.Obstacles)
            {
                if (!board.InBounds(obstacle.Position))
                {
                    continue;
                }

                var health = obstacle.HealthOverride > 0
                    ? obstacle.HealthOverride
                    : BoardRules.DefaultObstacleHealth(obstacle.ObstacleType);
                board.SetObstacle(obstacle.Position, obstacle.ObstacleType, health);
            }

            foreach (var position in board.AllPositions())
            {
                var cell = board.GetCell(position);
                if (!cell.AcceptsIngredient || cell.HasIngredient)
                {
                    continue;
                }

                board.SetIngredient(position, PickIngredientAvoidingImmediateMatch(board, position, level.ActiveIngredients, random));
            }

            return board;
        }

        private IngredientType PickIngredientAvoidingImmediateMatch(
            BoardState board,
            GridPosition position,
            IReadOnlyList<IngredientType> ingredients,
            IRandomSource random)
        {
            var candidates = new List<IngredientType>(ingredients);
            while (candidates.Count > 0)
            {
                var index = random.Range(0, candidates.Count);
                var ingredient = candidates[index];
                candidates.RemoveAt(index);

                if (!WouldCreateImmediateMatch(board, position, ingredient))
                {
                    return ingredient;
                }
            }

            return ingredients[random.Range(0, ingredients.Count)];
        }

        private static bool WouldCreateImmediateMatch(BoardState board, GridPosition position, IngredientType ingredient)
        {
            return CountSame(board, position, ingredient, 0, -1) >= 2
                   || CountSame(board, position, ingredient, -1, 0) >= 2;
        }

        private static int CountSame(BoardState board, GridPosition start, IngredientType ingredient, int rowDelta, int columnDelta)
        {
            var count = 0;
            var position = new GridPosition(start.Row + rowDelta, start.Column + columnDelta);
            while (board.InBounds(position) && board.GetCell(position).Ingredient == ingredient)
            {
                count++;
                position = new GridPosition(position.Row + rowDelta, position.Column + columnDelta);
            }

            return count;
        }

        private bool CreatesMatchAfterSwap(BoardState board, GridPosition first, GridPosition second)
        {
            if (!BoardRules.CanSwap(board, first, second))
            {
                return false;
            }

            board.SwapIngredients(first, second);
            var hasMatch = _matchFinder.FindMatches(board, second).Count > 0;
            board.SwapIngredients(first, second);
            return hasMatch;
        }
    }
}

