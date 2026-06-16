using System;
using System.Collections.Generic;
using PotionPopQuest.Core;
using UnityEngine;
using UnityEngine.UI;

namespace PotionPopQuest.Unity
{
    public sealed partial class GeneratedGameUi
    {
        public void ShowLevelSelect(IReadOnlyList<LevelData> levels, int highestUnlocked, Func<int, int> starsForLevel)
        {
            ClearHint();
            HideLevelIntro();
            ClearChildren(_levelSelect.transform);

            CreateTitle(_levelSelect.transform, "Level Select", 44);
            var scrollFrame = CreatePanel(_levelSelect.transform, "Levels Scroll View", UiColorPalette.LevelGridBackground);
            var columns = UiLayoutMetrics.LevelSelectColumnCount();
            var rows = Mathf.CeilToInt(levels.Count / (float)columns);
            var cellSize = columns <= 3 ? 124f : columns == 4 ? 116f : 110f;
            var spacing = columns <= 3 ? 12f : 10f;
            var gridWidth = columns * cellSize + (columns - 1) * spacing + 40f;
            var gridHeight = rows * cellSize + Mathf.Max(0, rows - 1) * spacing + 40f;
            var frameWidth = Mathf.Min(UiLayoutMetrics.ScreenMaxWidth, gridWidth);
            var frameHeight = Mathf.Min(680f, gridHeight);
            AddLayoutElement(scrollFrame, frameWidth, frameHeight);
            var scrollRect = scrollFrame.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.scrollSensitivity = 28f;
            scrollFrame.AddComponent<RectMask2D>();

            var grid = new GameObject("Levels Grid", typeof(RectTransform));
            grid.transform.SetParent(scrollFrame.transform, false);
            var gridRect = grid.GetComponent<RectTransform>();
            gridRect.anchorMin = new Vector2(0.5f, 1f);
            gridRect.anchorMax = new Vector2(0.5f, 1f);
            gridRect.pivot = new Vector2(0.5f, 1f);
            gridRect.anchoredPosition = Vector2.zero;
            gridRect.sizeDelta = new Vector2(gridWidth, gridHeight);
            scrollRect.content = gridRect;
            scrollRect.viewport = scrollFrame.GetComponent<RectTransform>();

            var pool = grid.AddComponent<LevelScrollPool>();
            pool.scrollRect = scrollRect;
            pool.content = gridRect;
            pool.columns = columns;
            pool.cellSize = cellSize;
            pool.spacing = spacing;
            
            pool.onBindCell = (index, cell) => {
                var levelData = levels[index];
                var locked = levelData.LevelNumber > highestUnlocked;
                var isNext = levelData.LevelNumber == highestUnlocked;
                var stars = starsForLevel(levelData.LevelNumber);
                var view = cell.GetComponent<LevelCardView>();
                view.Bind(levelData.LevelNumber, stars, locked, isNext, _startLevel);
            };

            pool.Initialize(levels.Count, () => CreateLevelCardPrefab(grid.transform).transform);

            CreateButton(_levelSelect.transform, "Back", _mainMenuAction, UiColorPalette.Amethyst, new Vector2(240, 58));
            TransitionTo(_levelSelect);
        }

        private GameObject CreateLevelCardPrefab(Transform parent)
        {
            var button = CreateButton(parent, "", null, UiColorPalette.LevelCardUnlocked);
            var cardView = button.gameObject.AddComponent<LevelCardView>();
            cardView.button = button;
            cardView.backgroundImage = button.GetComponent<Image>();

            // Gradient overlay for unlocked cards
            var gradient = new GameObject("CardGradient", typeof(RectTransform), typeof(Image));
            gradient.transform.SetParent(button.transform, false);
            gradient.transform.SetAsFirstSibling();
            var gradRect = gradient.GetComponent<RectTransform>();
            gradRect.anchorMin = new Vector2(0f, 0f);
            gradRect.anchorMax = new Vector2(1f, 0.50f);
            gradRect.offsetMin = Vector2.zero;
            gradRect.offsetMax = Vector2.zero;
            gradient.GetComponent<Image>().color = UiColorPalette.WithAlpha(UiColorPalette.LevelCardUnlockedGradient, 0.40f);
            gradient.GetComponent<Image>().raycastTarget = false;
            cardView.gradientOverlay = gradient;

            // Current level outline
            var outline = new GameObject("CurrentOutline", typeof(RectTransform), typeof(Image));
            outline.transform.SetParent(button.transform, false);
            var outRect = outline.GetComponent<RectTransform>();
            outRect.anchorMin = Vector2.zero;
            outRect.anchorMax = Vector2.one;
            outRect.offsetMin = new Vector2(-4, -4);
            outRect.offsetMax = new Vector2(4, 4);
            outline.GetComponent<Image>().sprite = _iconFactory.GetRoundedRectSprite(24);
            outline.GetComponent<Image>().type = Image.Type.Sliced;
            outline.GetComponent<Image>().color = UiColorPalette.WithAlpha(UiColorPalette.Gold, 0.6f);
            outline.GetComponent<Image>().raycastTarget = false;
            
            var innerMask = new GameObject("InnerMask", typeof(RectTransform), typeof(Image));
            innerMask.transform.SetParent(outline.transform, false);
            var innerRect = innerMask.GetComponent<RectTransform>();
            innerRect.anchorMin = Vector2.zero;
            innerRect.anchorMax = Vector2.one;
            innerRect.offsetMin = new Vector2(4, 4);
            innerRect.offsetMax = new Vector2(-4, -4);
            innerMask.GetComponent<Image>().sprite = _iconFactory.GetRoundedRectSprite(20);
            innerMask.GetComponent<Image>().type = Image.Type.Sliced;
            innerMask.GetComponent<Image>().color = UiColorPalette.LevelCardUnlocked;
            innerMask.GetComponent<Image>().raycastTarget = false;
            
            outline.transform.SetAsFirstSibling();
            cardView.currentOutline = outline;

            // Lock icon placeholder (just a text label "Lock")
            var lockLabel = CreateLabel(button.transform, "Lock", 20, TextAnchor.MiddleCenter);
            lockLabel.rectTransform.anchorMin = new Vector2(0, 0.5f);
            lockLabel.rectTransform.anchorMax = new Vector2(1, 1);
            lockLabel.rectTransform.offsetMin = Vector2.zero;
            lockLabel.rectTransform.offsetMax = Vector2.zero;
            lockLabel.color = UiColorPalette.TextMuted;
            cardView.lockIcon = lockLabel.gameObject;

            // Level number
            var numberLabel = CreateLabel(button.transform, "1", 34, TextAnchor.MiddleCenter);
            numberLabel.rectTransform.anchorMin = new Vector2(0, 0.32f);
            numberLabel.rectTransform.anchorMax = new Vector2(1, 1);
            numberLabel.rectTransform.offsetMin = Vector2.zero;
            numberLabel.rectTransform.offsetMax = Vector2.zero;
            numberLabel.color = UiColorPalette.TextPrimary;
            _themeAssets.AddHighValueTextShadow(numberLabel);
            cardView.levelText = numberLabel;

            // Star row
            var starRow = new GameObject("Level Card Stars", typeof(RectTransform), typeof(LayoutElement));
            starRow.transform.SetParent(button.transform, false);
            var starRect = starRow.GetComponent<RectTransform>();
            starRect.anchorMin = new Vector2(0, 0);
            starRect.anchorMax = new Vector2(1, 0.34f);
            starRect.offsetMin = Vector2.zero;
            starRect.offsetMax = Vector2.zero;
            starRow.GetComponent<LayoutElement>().ignoreLayout = true;
            var starLayout = starRow.AddComponent<HorizontalLayoutGroup>();
            starLayout.childAlignment = TextAnchor.MiddleCenter;
            starLayout.spacing = 4;
            cardView.stars = new Image[3];
            for (var i = 0; i < 3; i++)
            {
                // We recreate this from main file to break cyclic dep
                var starObj = new GameObject("Star", typeof(RectTransform), typeof(Image));
                starObj.transform.SetParent(starRow.transform, false);
                AddLayoutElement(starObj, 24, 24);
                var img = starObj.GetComponent<Image>();
                img.sprite = _iconFactory.GetStarSprite(true);
                img.preserveAspect = true;
                img.raycastTarget = false;
                cardView.stars[i] = img;
            }

            // Golden top border
            var border = CreatePanel(button.transform, "CardBorder", UiColorPalette.LevelCardBorder);
            var borderRect = border.GetComponent<RectTransform>();
            borderRect.anchorMin = new Vector2(0, 0.96f);
            borderRect.anchorMax = new Vector2(1, 1);
            borderRect.offsetMin = Vector2.zero;
            borderRect.offsetMax = Vector2.zero;
            border.GetComponent<Image>().raycastTarget = false;
            border.AddComponent<LayoutElement>().ignoreLayout = true;

            return button.gameObject;
        }
    }
}
