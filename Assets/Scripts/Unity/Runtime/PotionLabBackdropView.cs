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
            // Base wall
            var backWall = CreateDecorPanel(parent, "Potion Lab Back Wall", UiColorPalette.LabBackWall);
            var backRect = backWall.GetComponent<RectTransform>();
            backRect.anchorMin = Vector2.zero;
            backRect.anchorMax = Vector2.one;
            backRect.offsetMin = Vector2.zero;
            backRect.offsetMax = Vector2.zero;

            // Ambient light rays
            CreateLightRay(parent, "Light Ray Left", new Vector2(0.0f, 0.50f), new Vector2(0.40f, 1f), UiColorPalette.LightRay, -15f);
            CreateLightRay(parent, "Light Ray Right", new Vector2(0.60f, 0.50f), new Vector2(1.0f, 1f), UiColorPalette.LightRayWarm, 15f);

            // Large soft bubbles/clouds (Candy Crush dreamy atmosphere)
            CreateBubble(parent, "Cloud Bottom Left", new Vector2(-0.2f, -0.1f), 800, UiColorPalette.WithAlpha(UiColorPalette.Amethyst, 0.4f));
            CreateBubble(parent, "Cloud Bottom Right", new Vector2(1.1f, -0.05f), 900, UiColorPalette.WithAlpha(UiColorPalette.Sapphire, 0.35f));
            CreateBubble(parent, "Cloud Bottom Center", new Vector2(0.5f, -0.15f), 1000, UiColorPalette.WithAlpha(UiColorPalette.Emerald, 0.3f));

            CreateBubble(parent, "Cloud Top Right", new Vector2(1.2f, 1.1f), 600, UiColorPalette.WithAlpha(UiColorPalette.Gold, 0.25f));
            CreateBubble(parent, "Cloud Top Left", new Vector2(-0.1f, 1.05f), 700, UiColorPalette.WithAlpha(UiColorPalette.Ruby, 0.2f));

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

        private void CreateBubble(Transform parent, string name, Vector2 anchorPosition, float size, Color color)
        {
            var bubble = CreateDecorPanel(parent, name, color);
            var rect = bubble.GetComponent<RectTransform>();
            rect.anchorMin = anchorPosition;
            rect.anchorMax = anchorPosition;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(size, size);

            var image = bubble.GetComponent<Image>();
            // Since we need circles, let's see if we can use the circle sprite from icon factory?
            // Actually, UiElementFactory might not give us easy access to the factory directly here, 
            // but we can just use the standard Panel which is a rounded rect. 
            // Better: just let it be a large rounded rect, it looks like a soft cloud block.
        }

        private void CreateLightRay(Transform parent, string name, Vector2 min, Vector2 max, Color color, float rotationAngle)
        {
            var ray = CreateDecorPanel(parent, name, color);
            var rect = ray.GetComponent<RectTransform>();
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.localRotation = Quaternion.Euler(0, 0, rotationAngle);

            var group = ray.AddComponent<CanvasGroup>();
            group.alpha = 0.8f;
        }

        private void CreateVignette(Transform parent)
        {
            var vignette = CreateDecorPanel(parent, "Vignette", UiColorPalette.Vignette);
            var rect = vignette.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}
