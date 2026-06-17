using NUnit.Framework;
using PotionPopQuest.Core;
using PotionPopQuest.Unity;
using UnityEngine;

namespace PotionPopQuest.Unity.Tests
{
    public sealed class TileIconFactoryTests
    {
        [Test]
        public void GetIngredientSprite_UsesImportedResourceWhenAvailableAndCachesResult()
        {
            var importedTexture = Resources.Load<Texture2D>("Sprites/Ingredients/SPR_Ingredient_RedHerb_01");
            Assert.That(importedTexture, Is.Not.Null);

            var factory = new TileIconFactory();
            var first = factory.GetIngredientSprite(IngredientType.RedHerb);
            var second = factory.GetIngredientSprite(IngredientType.RedHerb);

            Assert.That(first, Is.Not.Null);
            Assert.That(first.texture, Is.SameAs(importedTexture));
            Assert.That(second, Is.SameAs(first));
        }

        [Test]
        public void GetStarSprite_UsesImportedResourceWhenAvailableAndCachesResult()
        {
            var importedTexture = Resources.Load<Texture2D>("Sprites/UI/SPR_UI_Star_Earned");
            Assert.That(importedTexture, Is.Not.Null);

            var factory = new TileIconFactory();
            var first = factory.GetStarSprite(true);
            var second = factory.GetStarSprite(true);

            Assert.That(first, Is.Not.Null);
            Assert.That(first.texture, Is.SameAs(importedTexture));
            Assert.That(second, Is.SameAs(first));
        }
    }
}
