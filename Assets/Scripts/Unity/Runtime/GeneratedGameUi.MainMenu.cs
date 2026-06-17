using System;
using PotionPopQuest.Core;
using UnityEngine;
using UnityEngine.UI;

namespace PotionPopQuest.Unity
{
    public sealed partial class GeneratedGameUi
    {
        public void ShowMainMenu()
        {
            ClearHint();
            HideLevelIntro();
            TransitionTo(_mainMenu);
        }

        private void BuildMainMenu()
        {
            // Animated title with glow effects
            var titleText = CreateTitle(_mainMenu.transform, "Potion Pop Quest", 56);
            titleText.color = UiColorPalette.Gold;
            _themeAssets.AddTitleTextEffects(titleText);
            AddLayoutElement(titleText.gameObject, UiLayoutMetrics.MenuContentWidth(), 82);

            var subtitleLabel = CreateLabel(_mainMenu.transform, "2D Match-3 Potion Puzzle", 24, TextAnchor.MiddleCenter);
            subtitleLabel.color = UiColorPalette.TextSecondary;
            AddLayoutElement(subtitleLabel.gameObject, 560, 36);

            // Decorative potion icon between title and buttons
            var potionIcon = new GameObject("Menu Potion Icon", typeof(RectTransform), typeof(Image));
            potionIcon.transform.SetParent(_mainMenu.transform, false);
            AddLayoutElement(potionIcon, 82, 82);
            var potionImage = potionIcon.GetComponent<Image>();
            potionImage.sprite = _iconFactory.GetPotionSprite(PotionType.Mega);
            potionImage.preserveAspect = true;
            potionImage.raycastTarget = false;

            CreateButton(_mainMenu.transform, "Play", _play, UiColorPalette.Emerald, new Vector2(380, 72));

            var secondaryActions = new GameObject("Main Menu Secondary Actions", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            secondaryActions.transform.SetParent(_mainMenu.transform, false);
            AddLayoutElement(secondaryActions, 520, 62);
            var actionsLayout = secondaryActions.GetComponent<HorizontalLayoutGroup>();
            actionsLayout.childAlignment = TextAnchor.MiddleCenter;
            actionsLayout.spacing = 16;
            actionsLayout.childControlWidth = false;
            actionsLayout.childControlHeight = false;
            actionsLayout.childForceExpandWidth = false;
            actionsLayout.childForceExpandHeight = false;
            CreateButton(secondaryActions.transform, "Levels", _showLevels, UiColorPalette.Sapphire, new Vector2(240, 58));
            CreateButton(secondaryActions.transform, "Settings", _showSettings, UiColorPalette.Amethyst, new Vector2(240, 58));

            CreateButton(_mainMenu.transform, "Exit", _quit, UiColorPalette.Ruby, new Vector2(180, 48));
        }
    }
}
