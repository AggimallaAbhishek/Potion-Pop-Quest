using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PotionPopQuest.Unity
{
    public sealed class BoardTilePool
    {
        private readonly Stack<Button> _tileButtonPool = new Stack<Button>();
        private readonly Stack<Image> _vfxImagePool = new Stack<Image>();
        private readonly TileIconFactory _iconFactory;

        public BoardTilePool(TileIconFactory iconFactory)
        {
            _iconFactory = iconFactory ?? new TileIconFactory();
        }

        public Button GetTileButton()
        {
            if (_tileButtonPool.Count > 0)
            {
                return _tileButtonPool.Pop();
            }

            var buttonObject = new GameObject("Tile", typeof(RectTransform), typeof(Image), typeof(Button));
            return buttonObject.GetComponent<Button>();
        }

        public void PoolTile(RectTransform rect)
        {
            ClearChildren(rect);
            ClearTileOutlines(rect);
            foreach (var animator in rect.GetComponents<UiTileAnimator>())
            {
                UnityEngine.Object.Destroy(animator);
            }

            foreach (var group in rect.GetComponents<CanvasGroup>())
            {
                group.alpha = 1f;
            }

            var button = rect.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            rect.localScale = Vector3.one;
            rect.gameObject.SetActive(false);
            _tileButtonPool.Push(button);
        }

        public Image RentVfxImage(string name, Color color, Vector2 size, Vector2 anchoredPosition, Transform parent)
        {
            Image image;
            if (_vfxImagePool.Count > 0)
            {
                image = _vfxImagePool.Pop();
                image.gameObject.name = name;
                image.gameObject.SetActive(true);
            }
            else
            {
                var effectObject = new GameObject(name, typeof(RectTransform), typeof(Image));
                image = effectObject.GetComponent<Image>();
            }

            image.transform.SetParent(parent, false);
            image.sprite = _iconFactory.GetPillSprite();
            image.color = color;
            image.raycastTarget = false;
            var rect = image.rectTransform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPosition;
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;
            return image;
        }

        public void ReleaseVfxImage(Image image, Transform parent)
        {
            if (image == null)
            {
                return;
            }

            image.gameObject.SetActive(false);
            image.transform.SetParent(parent, false);
            if (_vfxImagePool.Count >= GameplayPresentationConfig.MaxActiveVfxImages)
            {
                UnityEngine.Object.Destroy(image.gameObject);
                return;
            }

            _vfxImagePool.Push(image);
        }

        public void Clear()
        {
            _tileButtonPool.Clear();
            _vfxImagePool.Clear();
        }

        private static void ClearChildren(Transform parent)
        {
            foreach (Transform child in parent)
            {
                UnityEngine.Object.Destroy(child.gameObject);
            }
        }

        private static void ClearTileOutlines(RectTransform rect)
        {
            foreach (var outline in rect.GetComponents<Outline>())
            {
                UnityEngine.Object.Destroy(outline);
            }
        }
    }
}
