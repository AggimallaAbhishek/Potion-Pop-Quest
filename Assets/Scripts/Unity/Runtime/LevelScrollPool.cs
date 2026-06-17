using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PotionPopQuest.Core;

namespace PotionPopQuest.Unity
{
    public class LevelScrollPool : MonoBehaviour
    {
        public ScrollRect scrollRect;
        public RectTransform content;
        public int totalItems;
        public float cellSize = 124f;
        public float spacing = 40f; // More vertical spacing for the path
        public float amplitude = 180f; // Horizontal swing of the winding path
        public float frequency = 0.6f; // How fast the path winds

        public Action<int, Transform> onBindCell;

        private readonly List<RectTransform> _activeCells = new List<RectTransform>();
        private int _visibleItems;
        private int _firstVisibleIndex = -1;

        public void Initialize(int itemCount, Func<Transform> createCell)
        {
            totalItems = itemCount;
            
            // Total height is items * (size + spacing)
            var contentHeight = totalItems * (cellSize + spacing) + 120f;
            content.sizeDelta = new Vector2(content.sizeDelta.x, contentHeight);

            var viewportHeight = scrollRect.viewport.rect.height;
            _visibleItems = Mathf.CeilToInt(viewportHeight / (cellSize + spacing)) + 2;

            for (int i = 0; i < _visibleItems; i++)
            {
                var cell = createCell();
                var rt = cell.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0f); // Anchor bottom to build upwards like Candy Crush!
                rt.anchorMax = new Vector2(0.5f, 0f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.sizeDelta = new Vector2(cellSize, cellSize);
                _activeCells.Add(rt);
                cell.gameObject.SetActive(false);
            }

            scrollRect.onValueChanged.AddListener(OnScroll);
            
            // Set scroll position to bottom (start of journey)
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, 0);
            
            UpdateVisibleCells();
        }

        private void OnScroll(Vector2 pos)
        {
            UpdateVisibleCells();
        }

        private void UpdateVisibleCells()
        {
            var contentHeight = content.rect.height;
            var viewportHeight = scrollRect.viewport.rect.height;
            var maxScroll = Mathf.Max(0, contentHeight - viewportHeight);
            
            // normalizedPosition.y goes from 0 (bottom) to 1 (top)
            var scrollDistance = scrollRect.normalizedPosition.y * maxScroll;
            var startIndex = Mathf.FloorToInt((scrollDistance - 60f) / (cellSize + spacing));
            startIndex = Mathf.Clamp(startIndex, 0, Mathf.Max(0, totalItems - _visibleItems));

            if (startIndex == _firstVisibleIndex) return;
            _firstVisibleIndex = startIndex;

            for (int i = 0; i < _activeCells.Count; i++)
            {
                var index = startIndex + i;
                var cell = _activeCells[i];
                
                if (index < totalItems)
                {
                    cell.gameObject.SetActive(true);
                    
                    // Winding path math
                    var x = Mathf.Sin(index * frequency) * amplitude;
                    var y = 60f + index * (cellSize + spacing);
                    
                    cell.anchoredPosition = new Vector2(x, y);
                    onBindCell?.Invoke(index, cell);
                }
                else
                {
                    cell.gameObject.SetActive(false);
                }
            }
        }
    }
}
