using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Nanpure.Standard;
using Nanpure.Standard.Module;
using MantenseiLib;
using System;
using System.Linq;

namespace Nanpure.Standard.Graphics
{
    public class CellBorderRenderer : MonoBehaviour
    {
        [SerializeField] private Transform _borderContainer;
        Transform _parent;

        const int RIGHT_ANGLE = 90;

        public void RenderBorder(Cell[][] groups, Sprite sprite)
        {
            foreach (var group in groups)
            {
                RenderBorder(group, sprite);
            }
        }

        public void RenderBorder(Cell[] groupCells, Sprite sprite)
        {
            HashSet<Cell> groupSet = new HashSet<Cell>(groupCells);
            Dictionary<(int row, int col), Cell> cellMap = CreateCellMap(groupCells);
            _parent ??= new GameObject("Border").transform;
            _parent.SetParent(_borderContainer);
            _parent.transform.localScale = Vector3.one;
            _parent.transform.localPosition = Vector3.zero;

            foreach (var cell in groupCells)
            {
                int row = cell.Data.Row;
                int col = cell.Data.Column;

                foreach(var direction in Enum.GetValues(typeof(BorderDirection)).Cast<BorderDirection>())
                {
                    var dir = AngleToVector((int)direction * RIGHT_ANGLE);
                    if(IsBoundary(cell, dir, groupSet, cellMap))
                    {
                        GameObject borderObj = CreateBorderObject(sprite);
                        PositionBorder(cell, borderObj, direction);
                    }
                }
            }
        }

        public static Vector2Int AngleToVector(int angle)
        {
            float radians = angle * Mathf.Deg2Rad;
            float x = Mathf.Cos(radians);
            float y = Mathf.Sin(radians);

            // 四捨五入して整数に変換
            return new Vector2Int(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
        }

        private GameObject CreateBorderObject(Sprite sprite)
        {
            var borderObj = new GameObject($"Border");
            var image = borderObj.AddComponent<Image>();
            image.sprite = sprite;
            image.raycastTarget = false;

            borderObj.transform.SetParent(_parent, false);
            return borderObj;
        }

        private void PositionBorder(Cell cell, GameObject borderObject, BorderDirection direction)
        {
            var cellRect = cell.GetComponent<RectTransform>();
            var cellSize = cellRect.sizeDelta;

            var borderRect = borderObject.GetComponent<RectTransform>();
            var borderSize = borderRect.sizeDelta;

            var x1 = cellSize.x / borderSize.x;
            var y1 = cellSize.y / borderSize.y;
            
            var x2 = cell.transform.localScale.x / borderObject.transform.localScale.x;
            var y2 = cell.transform.localScale.y / borderObject.transform.localScale.y;

            var scale = borderObject.transform.localScale;
            scale.x *= x1 * x2;
            scale.y *= y1 * y2;
            borderObject.transform.localScale = scale;

            borderObject.transform.eulerAngles = Vector3.back * (int)direction * RIGHT_ANGLE;
            borderObject.transform.position = cell.transform.position;
        }

        private Dictionary<(int, int), Cell> CreateCellMap(Cell[] cells)
        {
            Dictionary<(int, int), Cell> map = new Dictionary<(int, int), Cell>();
            foreach (var cell in cells)
            {
                map[(cell.Data.Row, cell.Data.Column)] = cell;
            }
            return map;
        }

        private bool IsBoundary(Cell currentCell, Vector2Int direction, HashSet<Cell> groupSet, Dictionary<(int, int), Cell> cellMap)
        {
            int adjacentRow = currentCell.Data.Row + direction.y;
            int adjacentCol = currentCell.Data.Column + direction.x;

            if (!cellMap.ContainsKey((adjacentRow, adjacentCol)))
                return true;

            return !groupSet.Contains(cellMap[(adjacentRow, adjacentCol)]);
        }

        private enum BorderDirection
        {
            Right = 0,
            Top = 1,
            Left = 2,
            Bottom = 3,
        }
    }
}