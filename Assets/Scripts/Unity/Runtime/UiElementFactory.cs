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

            var label = CreateLabel(buttonObject.transform, text, 28, TextAnchor.MiddleCenter);
            label.rectTransform.anchorMin = Vector2.zero;
            label.rectTransform.anchorMax = Vector2.one;
            label.rectTransform.offsetMin = new Vector2(8, 6);
            label.rectTransform.offsetMax = new Vector2(-8, -6);
            label.raycastTarget = false;
            _themeAssets.AddHighValueTextShadow(label);
            return button;
        }
    }
}
