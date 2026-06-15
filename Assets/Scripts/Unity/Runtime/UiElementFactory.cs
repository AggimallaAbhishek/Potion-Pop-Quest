using System;
using UnityEngine;
using UnityEngine.UI;

namespace PotionPopQuest.Unity
{
    public sealed class UiElementFactory
    {
        private readonly TileIconFactory _iconFactory;
        private readonly UiThemeAssets _themeAssets;
        private readonly Func<Font> _fontProvider;

        public UiElementFactory(TileIconFactory iconFactory, UiThemeAssets themeAssets, Func<Font> fontProvider)
        {
            _iconFactory = iconFactory;
            _themeAssets = themeAssets;
            _fontProvider = fontProvider;
        }

        public GameObject CreatePanel(Transform parent, string name, Color color)
        {
            var panel = new GameObject(name, typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);
            var image = panel.GetComponent<Image>();
            image.sprite = _iconFactory.GetRoundedRectSprite(32);
            image.type = Image.Type.Sliced;
            image.pixelsPerUnitMultiplier = 3f;
            image.color = color;

            // Inner border highlight for depth
            CreateInnerHighlight(panel.transform);

            return panel;
        }

        /// <summary>Creates a panel with glassmorphism styling (semi-transparent with layered glow).</summary>
        public GameObject CreateGlassPanel(Transform parent, string name, float width, float height)
        {
            var panel = new GameObject(name, typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);
            var image = panel.GetComponent<Image>();
            image.sprite = _iconFactory.GetRoundedRectSprite(32);
            image.type = Image.Type.Sliced;
            image.pixelsPerUnitMultiplier = 3f;
            image.color = UiColorPalette.GlassBackground;

            var rect = panel.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(width, height);

            // Glass border
            var border = new GameObject("GlassBorder", typeof(RectTransform), typeof(Image));
            border.transform.SetParent(panel.transform, false);
            var borderRect = border.GetComponent<RectTransform>();
            borderRect.anchorMin = Vector2.zero;
            borderRect.anchorMax = Vector2.one;
            borderRect.offsetMin = Vector2.zero;
            borderRect.offsetMax = Vector2.zero;
            var borderImage = border.GetComponent<Image>();
            borderImage.sprite = _iconFactory.GetRoundedRectSprite(32);
            borderImage.type = Image.Type.Sliced;
            borderImage.pixelsPerUnitMultiplier = 3f;
            borderImage.color = UiColorPalette.GlassBorder;
            borderImage.raycastTarget = false;

            // Inner glow
            var glow = new GameObject("GlassGlow", typeof(RectTransform), typeof(Image));
            glow.transform.SetParent(panel.transform, false);
            var glowRect = glow.GetComponent<RectTransform>();
            glowRect.anchorMin = new Vector2(0.02f, 0.70f);
            glowRect.anchorMax = new Vector2(0.98f, 0.98f);
            glowRect.offsetMin = Vector2.zero;
            glowRect.offsetMax = Vector2.zero;
            glow.GetComponent<Image>().color = UiColorPalette.GlassInnerGlow;
            glow.GetComponent<Image>().raycastTarget = false;

            return panel;
        }

        public Text CreateLabel(Transform parent, string text, int size, TextAnchor alignment)
        {
            var labelObject = new GameObject("Text", typeof(RectTransform), typeof(Text));
            labelObject.transform.SetParent(parent, false);
            var label = labelObject.GetComponent<Text>();
            label.text = text;
            label.font = _fontProvider();
            label.color = Color.white;
            label.fontSize = size;
            label.alignment = alignment;
            label.resizeTextForBestFit = true;
            label.resizeTextMinSize = 16;
            label.resizeTextMaxSize = size;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Truncate;
            label.rectTransform.sizeDelta = new Vector2(840, Mathf.Max(64, size * 2));
            label.raycastTarget = false;
            return label;
        }

        public Button CreateButton(Transform parent, string text, Action action, Color color, Vector2? size = null)
        {
            var buttonObject = new GameObject($"Button - {text}", typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
            buttonObject.AddComponent<ButtonPressFeedback>();
            var rect = buttonObject.GetComponent<RectTransform>();
            rect.sizeDelta = size ?? new Vector2(180, 80);

            var image = buttonObject.GetComponent<Image>();
            image.sprite = _iconFactory.GetPillSprite();
            image.type = Image.Type.Sliced;
            image.pixelsPerUnitMultiplier = 2f;
            image.color = color;

            var button = buttonObject.GetComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(() => action?.Invoke());

            // Gradient depth layers for 3D button look
            CreateButtonDepthLayers(buttonObject.transform, color);

            var label = CreateLabel(buttonObject.transform, text, 28, TextAnchor.MiddleCenter);
            label.rectTransform.anchorMin = Vector2.zero;
            label.rectTransform.anchorMax = Vector2.one;
            label.rectTransform.offsetMin = new Vector2(8, 6);
            label.rectTransform.offsetMax = new Vector2(-8, -6);
            label.raycastTarget = false;
            _themeAssets.AddHighValueTextShadow(label);
            return button;
        }

        /// <summary>Creates a circular badge for HUD elements with radial gradient feel.</summary>
        public GameObject CreateGlowingBadge(Transform parent, string name, float diameter, Color color)
        {
            var badge = new GameObject(name, typeof(RectTransform), typeof(Image));
            badge.transform.SetParent(parent, false);
            var image = badge.GetComponent<Image>();
            image.sprite = _iconFactory.GetRoundedRectSprite(64);
            image.type = Image.Type.Sliced;
            image.pixelsPerUnitMultiplier = 2f;
            image.color = color;

            var rect = badge.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(diameter, diameter);

            // Glow behind badge
            var glow = new GameObject("BadgeGlow", typeof(RectTransform), typeof(Image));
            glow.transform.SetParent(badge.transform, false);
            glow.transform.SetAsFirstSibling();
            var glowRect = glow.GetComponent<RectTransform>();
            glowRect.anchorMin = new Vector2(-0.06f, -0.06f);
            glowRect.anchorMax = new Vector2(1.06f, 1.06f);
            glowRect.offsetMin = Vector2.zero;
            glowRect.offsetMax = Vector2.zero;
            glow.GetComponent<Image>().color = UiColorPalette.WithAlpha(color, 0.18f);
            glow.GetComponent<Image>().raycastTarget = false;

            // Top highlight for 3D look
            var highlight = new GameObject("BadgeHighlight", typeof(RectTransform), typeof(Image));
            highlight.transform.SetParent(badge.transform, false);
            var hlRect = highlight.GetComponent<RectTransform>();
            hlRect.anchorMin = new Vector2(0.08f, 0.75f);
            hlRect.anchorMax = new Vector2(0.92f, 0.96f);
            hlRect.offsetMin = Vector2.zero;
            hlRect.offsetMax = Vector2.zero;
            highlight.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.10f);
            highlight.GetComponent<Image>().raycastTarget = false;

            return badge;
        }

        /// <summary>Adds 3D depth layers to a button (top highlight, bottom shadow).</summary>
        private static void CreateButtonDepthLayers(Transform parent, Color baseColor)
        {
            // Top highlight band — makes button look convex/lit from above
            var topHighlight = new GameObject("ButtonHighlight", typeof(RectTransform), typeof(Image));
            topHighlight.transform.SetParent(parent, false);
            var topRect = topHighlight.GetComponent<RectTransform>();
            topRect.anchorMin = new Vector2(0.06f, 0.72f);
            topRect.anchorMax = new Vector2(0.94f, 0.96f);
            topRect.offsetMin = Vector2.zero;
            topRect.offsetMax = Vector2.zero;
            topHighlight.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.16f);
            topHighlight.GetComponent<Image>().raycastTarget = false;

            // Bottom shadow band — adds depth
            var bottomShadow = new GameObject("ButtonShadow", typeof(RectTransform), typeof(Image));
            bottomShadow.transform.SetParent(parent, false);
            var bottomRect = bottomShadow.GetComponent<RectTransform>();
            bottomRect.anchorMin = new Vector2(0.06f, 0.04f);
            bottomRect.anchorMax = new Vector2(0.94f, 0.22f);
            bottomRect.offsetMin = Vector2.zero;
            bottomRect.offsetMax = Vector2.zero;
            bottomShadow.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.14f);
            bottomShadow.GetComponent<Image>().raycastTarget = false;
        }

        /// <summary>Adds a subtle inner highlight at the top edge of a panel.</summary>
        private static void CreateInnerHighlight(Transform parent)
        {
            var highlight = new GameObject("PanelHighlight", typeof(RectTransform), typeof(Image));
            highlight.transform.SetParent(parent, false);
            var rect = highlight.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.04f, 0.92f);
            rect.anchorMax = new Vector2(0.96f, 0.99f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            highlight.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.06f);
            highlight.GetComponent<Image>().raycastTarget = false;
        }
    }
}
