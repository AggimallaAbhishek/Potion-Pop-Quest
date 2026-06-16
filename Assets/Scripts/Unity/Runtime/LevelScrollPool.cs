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
        public int columns = 3;
        public float cellSize = 124f;
        public float spacing = 12f;
        
        public Action<int, Transform> onBindCell;

        private readonly List<RectTransform> _activeCells = new List<RectTransform>();
        private int _visibleRows;
        private int _totalRows;
        private int _firstVisibleRow = -1;

        public void Initialize(int itemCount, Func<Transform> createCell)
        {
            totalItems = itemCount;
            _totalRows = Mathf.CeilToInt(totalItems / (float)columns);
            
            var contentHeight = _totalRows * cellSize + Mathf.Max(0, _totalRows - 1) * spacing + 40f;
            content.sizeDelta = new Vector2(content.sizeDelta.x, contentHeight);

            var viewportHeight = scrollRect.viewport.rect.height;
            _visibleRows = Mathf.CeilToInt(viewportHeight / (cellSize + spacing)) + 1;

            var poolSize = _visibleRows * columns;
            
            for (int i = 0; i < poolSize; i++)
            {
                var cell = createCell();
                var rt = cell.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 1);
                rt.anchorMax = new Vector2(0.5f, 1);
                rt.pivot = new Vector2(0.5f, 1);
                rt.sizeDelta = new Vector2(cellSize, cellSize);
                _activeCells.Add(rt);
                cell.gameObject.SetActive(false);
            }

            scrollRect.onValueChanged.AddListener(OnScroll);
            UpdateVisibleCells();
        }

        private void OnScroll(Vector2 pos)
        {
            UpdateVisibleCells();
        }

        private void UpdateVisibleCells()
        {
            var contentY = content.anchoredPosition.y;
            var startRow = Mathf.FloorToInt((contentY - 20f) / (cellSize + spacing));
            startRow = Mathf.Clamp(startRow, 0, Mathf.Max(0, _totalRows - _visibleRows));

            if (startRow == _firstVisibleRow) return;
            _firstVisibleRow = startRow;

            for (int i = 0; i < _activeCells.Count; i++)
            {
                var rowOffset = i / columns;
                var col = i % columns;
                var actualRow = startRow + rowOffset;
                var index = actualRow * columns + col;

                var cell = _activeCells[i];
                
                if (index < totalItems)
                {
                    cell.gameObject.SetActive(true);
                    
                    var gridWidth = columns * cellSize + (columns - 1) * spacing;
                    var startX = -gridWidth / 2f + cellSize / 2f;
                    
                    var x = startX + col * (cellSize + spacing);
                    var y = -20f - actualRow * (cellSize + spacing);
                    
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
