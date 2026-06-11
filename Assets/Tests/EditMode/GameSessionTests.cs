using System.Linq;
using NUnit.Framework;
using PotionPopQuest.Core;

namespace PotionPopQuest.Tests
{
    public sealed class GameSessionTests
    {
        [Test]
        public void TrySwap_ValidMoveConsumesExactlyOneMove()
        {
            var session = CreateSessionWithValidMove();
            var before = session.MovesRemaining;
            var move = FindSwap(session, createsMatch: true);

            var result = session.TrySwap(move.first, move.second);

            Assert.That(result.ValidMove, Is.True);
            Assert.That(session.MovesRemaining, Is.EqualTo(before - 1));
        }

        [Test]
        public void TrySwap_InvalidMoveConsumesZeroMoves()
        {
            var session = CreateSessionWithInvalidMove();
            var before = session.MovesRemaining;
            var move = FindSwap(session, createsMatch: false);

            var result = session.TrySwap(move.first, move.second);

            Assert.That(result.ValidMove, Is.False);
            Assert.That(session.MovesRemaining, Is.EqualTo(before));
        }

        private static GameSession CreateSessionWithValidMove()
        {
            for (var seed = 0; seed < 100; seed++)
            {
                var session = CreateSession(seed);
                if (TryFindSwap(session, createsMatch: true, out _))
                {
                    return session;
                }
            }

            Assert.Fail("Could not generate a board with a valid move.");
            return null;
        }

        private static GameSession CreateSessionWithInvalidMove()
        {
            for (var seed = 0; seed < 100; seed++)
            {
                var session = CreateSession(seed);
                if (TryFindSwap(session, createsMatch: false, out _))
                {
                    return session;
                }
            }

            Assert.Fail("Could not generate a board with an invalid adjacent swap.");
            return null;
        }

        private static GameSession CreateSession(int seed)
        {
            var level = MvpLevelCatalog.CreateLevels().First();
            return new GameSession(level, random: new SystemRandomSource(seed), logger: new NullGameLogger());
        }

        private static (GridPosition first, GridPosition second) FindSwap(GameSession session, bool createsMatch)
        {
            if (TryFindSwap(session, createsMatch, out var move))
            {
                return move;
            }

            Assert.Fail($"No swap found with createsMatch={createsMatch}.");
            return default;
        }

        private static bool TryFindSwap(
            GameSession session,
            bool createsMatch,
            out (GridPosition first, GridPosition second) move)
        {
            var board = session.Board;
            var matchFinder = new MatchFinder();
            foreach (var position in board.AllPositions())
            {
                var right = new GridPosition(position.Row, position.Column + 1);
                if (IsCandidate(board, matchFinder, position, right, createsMatch))
                {
                    move = (position, right);
                    return true;
                }

                var down = new GridPosition(position.Row + 1, position.Column);
                if (IsCandidate(board, matchFinder, position, down, createsMatch))
                {
                    move = (position, down);
                    return true;
                }
            }

            move = default;
            return false;
        }

        private static bool IsCandidate(
            BoardState board,
            MatchFinder matchFinder,
            GridPosition first,
            GridPosition second,
            bool createsMatch)
        {
            if (!BoardRules.CanSwap(board, first, second))
            {
                return false;
            }

            board.SwapIngredients(first, second);
            var hasMatch = matchFinder.FindMatches(board, second).Count > 0;
            board.SwapIngredients(first, second);
            return hasMatch == createsMatch;
        }
    }
}

