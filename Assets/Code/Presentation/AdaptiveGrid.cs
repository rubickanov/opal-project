using UnityEngine;
using UnityEngine.UI;

namespace Rubickanov.Opal.Presentation
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class AdaptiveGrid : MonoBehaviour
    {
        [SerializeField] private float _spacing = 10f;
        [SerializeField] private float _padding = 20f;
        [SerializeField] private float _cardAspectRatio = 0.7f;

        private GridLayoutGroup _grid;
        private RectTransform _rectTransform;

        private int _rows;
        private int _columns;

        private void Awake()
        {
            _grid = GetComponent<GridLayoutGroup>();
            _rectTransform = GetComponent<RectTransform>();
        }

        public void Setup(int rows, int columns)
        {
            _rows = rows;
            _columns = columns;

            _grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _grid.constraintCount = columns;
            _grid.spacing = new Vector2(_spacing, _spacing);
            _grid.padding = new RectOffset(
                (int)_padding, (int)_padding,
                (int)_padding, (int)_padding
            );
            _grid.childAlignment = TextAnchor.MiddleCenter;

            UpdateCellSize();
        }

        private void UpdateCellSize()
        {
            if (_rows == 0 || _columns == 0)
            {
                return;
            }

            float availableWidth = _rectTransform.rect.width - _padding * 2 - _spacing * (_columns - 1);
            float availableHeight = _rectTransform.rect.height - _padding * 2 - _spacing * (_rows - 1);

            float maxCellWidth = availableWidth / _columns;
            float maxCellHeight = availableHeight / _rows;

            float cellWidth;
            float cellHeight;

            if (maxCellWidth / _cardAspectRatio <= maxCellHeight)
            {
                cellWidth = maxCellWidth;
                cellHeight = cellWidth / _cardAspectRatio;
            }
            else
            {
                cellHeight = maxCellHeight;
                cellWidth = cellHeight * _cardAspectRatio;
            }

            _grid.cellSize = new Vector2(cellWidth, cellHeight);
        }

        private void OnRectTransformDimensionsChange()
        {
            if (_rows > 0 && _columns > 0)
            {
                UpdateCellSize();
            }
        }
    }
}
