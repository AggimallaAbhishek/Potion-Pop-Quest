using System.Linq;
using NUnit.Framework;
using PotionPopQuest.Core;

namespace PotionPopQuest.Tests
{
    public sealed class MatchFinderTests
    {
        [Test]
        public void FindMatches_ClassifiesStraightFourAsLinePotion()
        {
            var board = CreatePatternBoard(5, 5);
            board.SetIngredient(new GridPosition(2, 0), IngredientType.RedHerb);
            board.SetIngredient(new GridPosition(2, 1), IngredientType.RedHerb);
            board.SetIngredient(new GridPosition(2, 2), IngredientType.RedHerb);
            board.SetIngredient(new GridPosition(2, 3), IngredientType.RedHerb);
            board.SetIngredient(new GridPosition(2, 4), IngredientType.BlueCrystal);

            var matches = new MatchFinder().FindMatches(board);

            Assert.That(matches.Count, Is.EqualTo(1));
            Assert.That(matches[0].Kind, Is.EqualTo(MatchKind.Line));
            Assert.That(matches[0].CreatedPotion, Is.EqualTo(PotionType.LineHorizontal));
        }

        [Test]
        public void FindMatches_ClassifiesTShapeAsLightningPotion()
        {
            var board = CreatePatternBoard(5, 5);
            board.SetIngredient(new GridPosition(1, 2), IngredientType.RedHerb);
            board.SetIngredient(new GridPosition(2, 1), IngredientType.RedHerb);
            board.SetIngredient(new GridPosition(2, 2), IngredientType.RedHerb);
            board.SetIngredient(new GridPosition(2, 3), IngredientType.RedHerb);
            board.SetIngredient(new GridPosition(3, 2), IngredientType.RedHerb);

            var match = new MatchFinder().FindMatches(board).Single();

            Assert.That(match.Kind, Is.EqualTo(MatchKind.Lightning));
            Assert.That(match.CreatedPotion, Is.EqualTo(PotionType.Lightning));
            Assert.That(match.Positions.Count, Is.EqualTo(5));
        }

        private static BoardState CreatePatternBoard(int width, int height)
        {
            var board = new BoardState(width, height);
            var ingredients = new[]
            {
                IngredientType.RedHerb,
                IngredientType.BlueCrystal,
                IngredientType.GreenLeaf,
                IngredientType.YellowStarDust,
                IngredientType.PurpleMushroom
            };

            foreach (var position in board.AllPositions())
            {
                board.SetIngredient(position, ingredients[(position.Row * 2 + position.Column) % ingredients.Length]);
            }

            return board;
        }
    }
}

