using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PotionPopQuest.Unity
{
    public sealed class UiElementFactory
    {
        private readonly TileIconFactory _iconFactory;
        private readonly UiThemeAssets _themeAssets;
        private readonly Func<TMP_FontAsset> _fontProvider;

        public UiElementFactory(TileIconFactory iconFactory, UiThemeAssets themeAssets, Func<TMP_FontAsset> fontProvider)
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

        public TextMeshProUGUI CreateLabel(Transform parent, string text, int size, TextAnchor alignment)
        {
            var labelObject = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            labelObject.transform.SetParent(parent, false);
            var label = labelObject.GetComponent<TextMeshProUGUI>();
            label.text = text;
            label.font = _fontProvider();
            label.color = Color.white;
            label.fontSize = size;
            
            switch (alignment)
            {
                case TextAnchor.UpperLeft: label.alignment = TextAlignmentOptions.TopLeft; break;
                case TextAnchor.UpperCenter: label.alignment = TextAlignmentOptions.Top; break;
                case TextAnchor.UpperRight: label.alignment = TextAlignmentOptions.TopRight; break;
                case TextAnchor.MiddleLeft: label.alignment = TextAlignmentOptions.Left; break;
                case TextAnchor.MiddleCenter: label.alignment = TextAlignmentOptions.Center; break;
                case TextAnchor.MiddleRight: label.alignment = TextAlignmentOptions.Right; break;
                case TextAnchor.LowerLeft: label.alignment = TextAlignmentOptions.BottomLeft; break;
                case TextAnchor.LowerCenter: label.alignment = TextAlignmentOptions.Bottom; break;
                case TextAnchor.LowerRight: label.alignment = TextAlignmentOptions.BottomRight; break;
                default: label.alignment = TextAlignmentOptions.Center; break;
            }

            label.enableAutoSizing = true;
            label.fontSizeMin = 16;
            label.fontSizeMax = size;
            label.textWrappingMode = TextWrappingModes.Normal;
            label.overflowMode = TextOverflowModes.Truncate;
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
            var layoutElement = buttonObject.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = rect.sizeDelta.x;
            layoutElement.preferredHeight = rect.sizeDelta.y;
            layoutElement.flexibleWidth = 0;
            layoutElement.flexibleHeight = 0;

            var image = buttonObject.GetComponent<Image>();
            image.sprite = _iconFactory.GetPillSprite();
            image.type = Image.Type.Sliced;
            image.pixelsPerUnitMultiplier = 2f;
            image.color = color;

            var button = buttonObject.GetComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(() => action?.Invoke());



            var labelSize = rect.sizeDelta.y <= 58f ? 22 : rect.sizeDelta.y <= 70f ? 24 : 26;
            var label = CreateLabel(buttonObject.transform, text, labelSize, TextAnchor.MiddleCenter);
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

            var glow = new GameObject("BadgeGlow", typeof(RectTransform), typeof(Image));
            glow.transform.SetParent(badge.transform, false);
            glow.transform.SetAsFirstSibling();
            var glowRect = glow.GetComponent<RectTransform>();
            glowRect.anchorMin = new Vector2(-0.035f, -0.035f);
            glowRect.anchorMax = new Vector2(1.035f, 1.035f);
            glowRect.offsetMin = Vector2.zero;
            glowRect.offsetMax = Vector2.zero;
            glow.GetComponent<Image>().color = UiColorPalette.WithAlpha(color, 0.11f);
            glow.GetComponent<Image>().raycastTarget = false;
            return badge;
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
