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
            var move = FindValidMove(session);

            var result = session.TrySwap(move.First, move.Second);

            Assert.That(result.ValidMove, Is.True);
            Assert.That(session.MovesRemaining, Is.EqualTo(before - 1));
        }

        [Test]
        public void TrySwap_InvalidMoveConsumesZeroMoves()
        {
            var session = CreateSessionWithInvalidMove();
            var before = session.MovesRemaining;
            var move = FindInvalidMove(session);

            var result = session.TrySwap(move.First, move.Second);

            Assert.That(result.ValidMove, Is.False);
            Assert.That(session.MovesRemaining, Is.EqualTo(before));
        }

        private static GameSession CreateSessionWithValidMove()
        {
            for (var seed = 0; seed < 100; seed++)
            {
                var session = CreateSession(seed);
                if (new BoardMoveFinder().TryFindValidMove(session.Board, out _))
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
                if (new BoardMoveFinder().TryFindInvalidAdjacentMove(session.Board, out _))
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

        private static CandidateMove FindValidMove(GameSession session)
        {
            if (new BoardMoveFinder().TryFindValidMove(session.Board, out var move))
            {
                return move;
            }

            Assert.Fail("No valid swap found.");
            return default;
        }

        private static CandidateMove FindInvalidMove(GameSession session)
        {
            if (new BoardMoveFinder().TryFindInvalidAdjacentMove(session.Board, out var move))
            {
                return move;
            }

            Assert.Fail("No invalid adjacent swap found.");
            return default;
        }
    }
}
