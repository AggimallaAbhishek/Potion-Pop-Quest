using System;
using System.Collections.Generic;

namespace PotionPopQuest.Core
{
    public readonly struct CandidateMove
    {
        public CandidateMove(GridPosition first, GridPosition second)
        {
            First = first;
            Second = second;
        }

        public GridPosition First { get; }
        public GridPosition Second { get; }
    }

    public sealed class BoardMoveFinder
    {
        private readonly MatchFinder _matchFinder;

        public BoardMoveFinder(MatchFinder matchFinder = null)
        {
            _matchFinder = matchFinder ?? new MatchFinder();
        }

        public bool TryFindValidMove(BoardState board, out CandidateMove move)
        {
            return TryFindMove(board, createsMatch: true, out move);
        }

        public bool TryFindInvalidAdjacentMove(BoardState board, out CandidateMove move)
        {
            return TryFindMove(board, createsMatch: false, out move);
        }

        public IReadOnlyList<CandidateMove> FindValidMoves(BoardState board)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            var moves = new List<CandidateMove>();
            foreach (var candidate in CandidateMoves(board))
            {
                if (CreatesMatchAfterSwap(board, candidate.First, candidate.Second))
                {
                    moves.Add(candidate);
                }
            }

            return moves;
        }

        private bool TryFindMove(BoardState board, bool createsMatch, out CandidateMove move)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            foreach (var candidate in CandidateMoves(board))
            {
                if (CreatesMatchAfterSwap(board, candidate.First, candidate.Second) == createsMatch)
                {
                    move = candidate;
                    return true;
                }
            }

            move = default;
            return false;
        }

        private static IEnumerable<CandidateMove> CandidateMoves(BoardState board)
        {
            foreach (var position in board.AllPositions())
            {
                var right = new GridPosition(position.Row, position.Column + 1);
                if (BoardRules.CanSwap(board, position, right))
                {
                    yield return new CandidateMove(position, right);
                }

                var down = new GridPosition(position.Row + 1, position.Column);
                if (BoardRules.CanSwap(board, position, down))
                {
                    yield return new CandidateMove(position, down);
                }
            }
        }

        private bool CreatesMatchAfterSwap(BoardState board, GridPosition first, GridPosition second)
        {
            board.SwapIngredients(first, second);
            var hasMatch = _matchFinder.FindMatches(board, second).Count > 0;
            board.SwapIngredients(first, second);
            return hasMatch;
        }
    }
}

