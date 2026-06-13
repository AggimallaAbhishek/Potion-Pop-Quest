using System.Collections;
using System.Linq;
using NUnit.Framework;
using PotionPopQuest.Unity;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;

namespace PotionPopQuest.PlayMode.Tests
{
    public sealed class GameControllerPlayModeTests
    {
        [SetUp]
        public void SetUp()
        {
            PlayerPrefs.DeleteKey("PotionPopQuest.SaveData.v1");
            DestroyExistingRuntime();
        }

        [TearDown]
        public void TearDown()
        {
            DestroyExistingRuntime();
            PlayerPrefs.DeleteKey("PotionPopQuest.SaveData.v1");
        }

        [UnityTest]
        public IEnumerator Start_BuildsMainMenuWithPrimaryActions()
        {
            CreateRuntime();
            yield return null;

            Assert.That(GameObject.Find("Potion Pop Quest Canvas"), Is.Not.Null);
            Assert.That(FindButton("Play"), Is.Not.Null);
            Assert.That(FindButton("Levels"), Is.Not.Null);
            Assert.That(FindButton("Settings"), Is.Not.Null);
        }

        [UnityTest]
        public IEnumerator PlayButton_StartsLevelOneWithPolishedHud()
        {
            CreateRuntime();
            yield return null;

            FindButton("Play").onClick.Invoke();
            yield return null;

            Assert.That(GameObject.Find("Board Panel"), Is.Not.Null);
            Assert.That(GameObject.Find("Star Progress"), Is.Not.Null);
            Assert.That(GameObject.Find("Tutorial Banner"), Is.Not.Null);
            Assert.That(FindButton("Hint"), Is.Not.Null);
            Assert.That(GameObject.FindObjectsByType<Button>(FindObjectsSortMode.None).Count(button => button.name == "Tile"), Is.GreaterThanOrEqualTo(64));
        }

        [UnityTest]
        public IEnumerator HintButton_HighlightsCandidateMove()
        {
            CreateRuntime();
            yield return null;

            FindButton("Play").onClick.Invoke();
            yield return null;

            FindButton("Hint").onClick.Invoke();
            yield return null;

            Assert.That(GameObject.FindObjectsByType<Outline>(FindObjectsSortMode.None).Length, Is.GreaterThanOrEqualTo(2));
        }

        private static void CreateRuntime()
        {
            var root = new GameObject("Potion Pop Quest PlayMode Test Runtime");
            root.AddComponent<GameController>();
        }

        private static Button FindButton(string label)
        {
            return GameObject.FindObjectsByType<Button>(FindObjectsSortMode.None)
                .FirstOrDefault(button => button.name == $"Button - {label}");
        }

        private static void DestroyExistingRuntime()
        {
            foreach (var controller in GameObject.FindObjectsByType<GameController>(FindObjectsSortMode.None))
            {
                Object.DestroyImmediate(controller.gameObject);
            }

            foreach (var canvas in GameObject.FindObjectsByType<Canvas>(FindObjectsSortMode.None))
            {
                if (canvas.name == "Potion Pop Quest Canvas")
                {
                    Object.DestroyImmediate(canvas.gameObject);
                }
            }
        }
    }
}
