using UnityEngine;
using UnityEngine.UI;

namespace PotionPopQuest.Unity
{
    public sealed class PotionLabBackdropView
    {
        private readonly UiElementFactory _uiFactory;

        public PotionLabBackdropView(UiElementFactory uiFactory)
        {
            _uiFactory = uiFactory;
        }

        public void Build(Transform parent)
        {
            var backWall = CreateDecorPanel(parent, "Potion Lab Back Wall", UiColorPalette.LabBackWall);
            var backRect = backWall.GetComponent<RectTransform>();
            backRect.anchorMin = new Vector2(0, 0.66f);
            backRect.anchorMax = new Vector2(1, 1);
            backRect.offsetMin = Vector2.zero;
            backRect.offsetMax = Vector2.zero;

            for (var index = 0; index < 3; index++)
            {
                var shelf = CreateDecorPanel(parent, $"Potion Shelf {index + 1}", UiColorPalette.LabShelf);
                var shelfRect = shelf.GetComponent<RectTransform>();
                shelfRect.anchorMin = new Vector2(0.08f, 0.80f - index * 0.075f);
                shelfRect.anchorMax = new Vector2(0.92f, 0.815f - index * 0.075f);
                shelfRect.offsetMin = Vector2.zero;
                shelfRect.offsetMax = Vector2.zero;
            }

            var table = CreateDecorPanel(parent, "Potion Lab Table", UiColorPalette.LabTable);
            var tableRect = table.GetComponent<RectTransform>();
            tableRect.anchorMin = new Vector2(0, 0);
            tableRect.anchorMax = new Vector2(1, 0.13f);
            tableRect.offsetMin = Vector2.zero;
            tableRect.offsetMax = Vector2.zero;

            CreateLabBottle(parent, "Bottle - Ruby Tonic", new Vector2(0.17f, 0.825f), new Vector2(44, 82), UiColorPalette.WithAlpha(UiColorPalette.Ruby, 0.42f));
            CreateLabBottle(parent, "Bottle - Sapphire Elixir", new Vector2(0.29f, 0.748f), new Vector2(40, 72), UiColorPalette.WithAlpha(UiColorPalette.SapphireLight, 0.38f));
            CreateLabBottle(parent, "Bottle - Emerald Brew", new Vector2(0.70f, 0.824f), new Vector2(48, 86), UiColorPalette.WithAlpha(UiColorPalette.Emerald, 0.36f));
            CreateLabBottle(parent, "Bottle - Golden Dust", new Vector2(0.82f, 0.672f), new Vector2(42, 70), UiColorPalette.WithAlpha(UiColorPalette.Gold, 0.38f));
            CreateLabLine(parent, "Lab Wall Highlight Left", new Vector2(0.10f, 0.58f), new Vector2(0.30f, 0.585f), UiColorPalette.WithAlpha(UiColorPalette.GoldLight, 0.14f));
            CreateLabLine(parent, "Lab Wall Highlight Right", new Vector2(0.70f, 0.55f), new Vector2(0.92f, 0.555f), UiColorPalette.WithAlpha(UiColorPalette.SapphireLight, 0.14f));
            CreateCauldronSilhouette(parent);
        }

        private GameObject CreateDecorPanel(Transform parent, string name, Color color)
        {
            var panel = _uiFactory.CreatePanel(parent, name, color);
            panel.GetComponent<Image>().raycastTarget = false;
            panel.AddComponent<LayoutElement>().ignoreLayout = true;
            return panel;
        }

        private void CreateLabBottle(Transform parent, string name, Vector2 anchor, Vector2 size, Color color)
        {
            var body = CreateDecorPanel(parent, name, color);
            var bodyRect = body.GetComponent<RectTransform>();
            bodyRect.anchorMin = anchor;
            bodyRect.anchorMax = anchor;
            bodyRect.pivot = new Vector2(0.5f, 0f);
            bodyRect.anchoredPosition = Vector2.zero;
            bodyRect.sizeDelta = size;

            var neck = CreateDecorPanel(parent, $"{name} Neck", UiColorPalette.WithAlpha(color, Mathf.Min(0.55f, color.a + 0.12f)));
            var neckRect = neck.GetComponent<RectTransform>();
            neckRect.anchorMin = anchor;
            neckRect.anchorMax = anchor;
            neckRect.pivot = new Vector2(0.5f, 0f);
            neckRect.anchoredPosition = new Vector2(0, size.y - 3f);
            neckRect.sizeDelta = new Vector2(size.x * 0.38f, size.y * 0.42f);

            var shine = CreateDecorPanel(parent, $"{name} Highlight", UiColorPalette.WithAlpha(Color.white, 0.12f));
            var shineRect = shine.GetComponent<RectTransform>();
            shineRect.anchorMin = anchor;
            shineRect.anchorMax = anchor;
            shineRect.pivot = new Vector2(0.5f, 0f);
            shineRect.anchoredPosition = new Vector2(-size.x * 0.18f, size.y * 0.18f);
            shineRect.sizeDelta = new Vector2(5f, size.y * 0.48f);
        }

        private void CreateLabLine(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Color color)
        {
            var line = CreateDecorPanel(parent, name, color);
            var rect = line.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private void CreateCauldronSilhouette(Transform parent)
        {
            var cauldron = CreateDecorPanel(parent, "Potion Lab Cauldron", UiColorPalette.WithAlpha(UiColorPalette.DarkTile, 0.56f));
            var rect = cauldron.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.11f);
            rect.anchorMax = new Vector2(0.5f, 0.11f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(250, 76);

            var rim = CreateDecorPanel(parent, "Potion Lab Cauldron Rim", UiColorPalette.WithAlpha(UiColorPalette.SapphireLight, 0.20f));
            var rimRect = rim.GetComponent<RectTransform>();
            rimRect.anchorMin = new Vector2(0.5f, 0.145f);
            rimRect.anchorMax = new Vector2(0.5f, 0.145f);
            rimRect.pivot = new Vector2(0.5f, 0.5f);
            rimRect.sizeDelta = new Vector2(274, 18);
        }
    }
}
