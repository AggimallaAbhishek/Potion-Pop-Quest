using System.Collections;
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
            backRect.anchorMin = new Vector2(0, 0.62f);
            backRect.anchorMax = new Vector2(1, 1);
            backRect.offsetMin = Vector2.zero;
            backRect.offsetMax = Vector2.zero;

            // Shelves with slight depth
            for (var index = 0; index < 3; index++)
            {
                var shelf = CreateDecorPanel(parent, $"Potion Shelf {index + 1}", UiColorPalette.LabShelf);
                var shelfRect = shelf.GetComponent<RectTransform>();
                shelfRect.anchorMin = new Vector2(0.06f, 0.79f - index * 0.072f);
                shelfRect.anchorMax = new Vector2(0.94f, 0.810f - index * 0.072f);
                shelfRect.offsetMin = Vector2.zero;
                shelfRect.offsetMax = Vector2.zero;

                // Shelf shadow below
                var shelfShadow = CreateDecorPanel(parent, $"Shelf Shadow {index + 1}", new Color(0f, 0f, 0f, 0.18f));
                var shadowRect = shelfShadow.GetComponent<RectTransform>();
                shadowRect.anchorMin = new Vector2(0.08f, 0.782f - index * 0.072f);
                shadowRect.anchorMax = new Vector2(0.92f, 0.790f - index * 0.072f);
                shadowRect.offsetMin = Vector2.zero;
                shadowRect.offsetMax = Vector2.zero;
            }

            var table = CreateDecorPanel(parent, "Potion Lab Table", UiColorPalette.LabTable);
            var tableRect = table.GetComponent<RectTransform>();
            tableRect.anchorMin = new Vector2(0, 0);
            tableRect.anchorMax = new Vector2(1, 0.12f);
            tableRect.offsetMin = Vector2.zero;
            tableRect.offsetMax = Vector2.zero;

            // Decorative bottles with gradient fills
            CreateLabBottle(parent, "Bottle - Ruby Tonic", new Vector2(0.15f, 0.820f), new Vector2(48, 88), UiColorPalette.WithAlpha(UiColorPalette.Ruby, 0.46f));
            CreateLabBottle(parent, "Bottle - Sapphire Elixir", new Vector2(0.28f, 0.745f), new Vector2(42, 76), UiColorPalette.WithAlpha(UiColorPalette.SapphireLight, 0.42f));
            CreateLabBottle(parent, "Bottle - Emerald Brew", new Vector2(0.72f, 0.820f), new Vector2(52, 92), UiColorPalette.WithAlpha(UiColorPalette.Emerald, 0.40f));
            CreateLabBottle(parent, "Bottle - Golden Dust", new Vector2(0.84f, 0.668f), new Vector2(44, 74), UiColorPalette.WithAlpha(UiColorPalette.Gold, 0.42f));
            CreateLabBottle(parent, "Bottle - Amethyst Tincture", new Vector2(0.50f, 0.746f), new Vector2(38, 68), UiColorPalette.WithAlpha(UiColorPalette.AmethystLight, 0.36f));

            // Ambient light rays
            CreateLightRay(parent, "Light Ray Left", new Vector2(0.02f, 0.40f), new Vector2(0.35f, 1f), UiColorPalette.LightRay, -8f);
            CreateLightRay(parent, "Light Ray Right", new Vector2(0.68f, 0.45f), new Vector2(0.98f, 1f), UiColorPalette.LightRayWarm, 6f);

            // Wall accent highlights
            CreateLabLine(parent, "Lab Wall Highlight Left", new Vector2(0.08f, 0.56f), new Vector2(0.28f, 0.565f), UiColorPalette.WithAlpha(UiColorPalette.GoldLight, 0.10f));
            CreateLabLine(parent, "Lab Wall Highlight Right", new Vector2(0.72f, 0.53f), new Vector2(0.94f, 0.535f), UiColorPalette.WithAlpha(UiColorPalette.SapphireLight, 0.10f));

            // Cauldron
            CreateCauldronSilhouette(parent);

            // Vignette overlay (darkened edges)
            CreateVignette(parent);
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

            // Neck with lighter color
            var neck = CreateDecorPanel(parent, $"{name} Neck", UiColorPalette.WithAlpha(color, Mathf.Min(0.58f, color.a + 0.14f)));
            var neckRect = neck.GetComponent<RectTransform>();
            neckRect.anchorMin = anchor;
            neckRect.anchorMax = anchor;
            neckRect.pivot = new Vector2(0.5f, 0f);
            neckRect.anchoredPosition = new Vector2(0, size.y - 3f);
            neckRect.sizeDelta = new Vector2(size.x * 0.36f, size.y * 0.40f);

            // Glass shine
            var shine = CreateDecorPanel(parent, $"{name} Highlight", UiColorPalette.WithAlpha(Color.white, 0.14f));
            var shineRect = shine.GetComponent<RectTransform>();
            shineRect.anchorMin = anchor;
            shineRect.anchorMax = anchor;
            shineRect.pivot = new Vector2(0.5f, 0f);
            shineRect.anchoredPosition = new Vector2(-size.x * 0.20f, size.y * 0.16f);
            shineRect.sizeDelta = new Vector2(4f, size.y * 0.52f);

            // Liquid fill gradient (brighter at top)
            var liquidShine = CreateDecorPanel(parent, $"{name} Liquid Shine", UiColorPalette.WithAlpha(Color.white, 0.08f));
            var liqRect = liquidShine.GetComponent<RectTransform>();
            liqRect.anchorMin = anchor;
            liqRect.anchorMax = anchor;
            liqRect.pivot = new Vector2(0.5f, 0f);
            liqRect.anchoredPosition = new Vector2(0, size.y * 0.55f);
            liqRect.sizeDelta = new Vector2(size.x * 0.72f, size.y * 0.15f);
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

        private void CreateLightRay(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Color color, float rotation)
        {
            var ray = CreateDecorPanel(parent, name, color);
            var rect = ray.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.localRotation = Quaternion.Euler(0f, 0f, rotation);
        }

        private void CreateCauldronSilhouette(Transform parent)
        {
            var cauldron = CreateDecorPanel(parent, "Potion Lab Cauldron", UiColorPalette.WithAlpha(UiColorPalette.DarkTile, 0.60f));
            var rect = cauldron.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.10f);
            rect.anchorMax = new Vector2(0.5f, 0.10f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(260, 80);

            var rim = CreateDecorPanel(parent, "Potion Lab Cauldron Rim", UiColorPalette.WithAlpha(UiColorPalette.SapphireLight, 0.22f));
            var rimRect = rim.GetComponent<RectTransform>();
            rimRect.anchorMin = new Vector2(0.5f, 0.14f);
            rimRect.anchorMax = new Vector2(0.5f, 0.14f);
            rimRect.pivot = new Vector2(0.5f, 0.5f);
            rimRect.sizeDelta = new Vector2(282, 20);

            // Cauldron glow (simulates magical contents)
            var glow = CreateDecorPanel(parent, "Cauldron Glow", UiColorPalette.WithAlpha(UiColorPalette.Emerald, 0.10f));
            var glowRect = glow.GetComponent<RectTransform>();
            glowRect.anchorMin = new Vector2(0.5f, 0.12f);
            glowRect.anchorMax = new Vector2(0.5f, 0.12f);
            glowRect.pivot = new Vector2(0.5f, 0.5f);
            glowRect.sizeDelta = new Vector2(200, 50);
        }

        private void CreateVignette(Transform parent)
        {
            // Top vignette
            var top = CreateDecorPanel(parent, "Vignette Top", UiColorPalette.Vignette);
            var topRect = top.GetComponent<RectTransform>();
            topRect.anchorMin = new Vector2(0, 0.88f);
            topRect.anchorMax = new Vector2(1, 1);
            topRect.offsetMin = Vector2.zero;
            topRect.offsetMax = Vector2.zero;

            // Bottom vignette
            var bottom = CreateDecorPanel(parent, "Vignette Bottom", UiColorPalette.Vignette);
            var bottomRect = bottom.GetComponent<RectTransform>();
            bottomRect.anchorMin = new Vector2(0, 0);
            bottomRect.anchorMax = new Vector2(1, 0.08f);
            bottomRect.offsetMin = Vector2.zero;
            bottomRect.offsetMax = Vector2.zero;

            // Left edge
            var left = CreateDecorPanel(parent, "Vignette Left", UiColorPalette.WithAlpha(UiColorPalette.Vignette, 0.40f));
            var leftRect = left.GetComponent<RectTransform>();
            leftRect.anchorMin = new Vector2(0, 0);
            leftRect.anchorMax = new Vector2(0.04f, 1);
            leftRect.offsetMin = Vector2.zero;
            leftRect.offsetMax = Vector2.zero;

            // Right edge
            var right = CreateDecorPanel(parent, "Vignette Right", UiColorPalette.WithAlpha(UiColorPalette.Vignette, 0.40f));
            var rightRect = right.GetComponent<RectTransform>();
            rightRect.anchorMin = new Vector2(0.96f, 0);
            rightRect.anchorMax = new Vector2(1, 1);
            rightRect.offsetMin = Vector2.zero;
            rightRect.offsetMax = Vector2.zero;
        }
    }
}
