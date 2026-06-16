using System;
using PotionPopQuest.Core;
using UnityEngine;
using UnityEngine.UI;

namespace PotionPopQuest.Unity
{
    public sealed class BoardInputHandler
    {
        private Action<GridPosition> _tilePressed;
        private Action<GameSfxCue> _playSfx;

        public void Configure(Action<GridPosition> tilePressed, Action<GameSfxCue> playSfx)
        {
            _tilePressed = tilePressed;
            _playSfx = playSfx;
        }

        public void ConfigureTileInteraction(GridPosition position, RectTransform rect, BoardCellSnapshot cell)
        {
            var button = rect.GetComponent<Button>();
            var image = rect.GetComponent<Image>();
            button.targetGraphic = image;
            button.interactable = cell.CanMoveIngredient;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                _playSfx?.Invoke(GameSfxCue.Tap);
                _tilePressed?.Invoke(position);
            });
        }
    }
}
