using MantenseiLib;
using Nanpure.Standard.InputSystem;
using Nanpure.Standard.Module;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nanpure.Standard.Core
{
    public class HighlightManager : MonoBehaviour
    {
        Cell _selectedCell;

        [Parent]
        Root _root;

        [Sibling] private IBoard _boardManager;
        [Sibling] private ColorSettings _colorSettings;
        [Sibling] private InputHandler inputHandler;

        public Board Board => _boardManager.Board;
        List<Cell> _preservedCells = new();

        public void PreserveCell(params Cell[] cells)
        {
            _preservedCells.AddRange(cells);
        }

        public void ClearPreservedCells()
        {
            _preservedCells.Clear();
        }

        private Color _normalColor => _colorSettings.White;
        private Color _relatedColor => _colorSettings.DarkWhite;
        private Color _sameNumberColor => _colorSettings.LightDark;
        private Color _selectedColor => _colorSettings.SystemWhite;
        private Color _hoverColor => _colorSettings.LightDark;

        private void Start()
        {
            inputHandler.onCellSelected += UpdateHighlights;
            inputHandler.onCellHoverEnter += (c) => HighLight(c, _hoverColor);
            inputHandler.onCellHoverExit += (c) => UpdateHighlights(_selectedCell);
        }

        public void UpdateHighlights() => UpdateHighlights(_selectedCell);

        private void UpdateHighlights(Cell selectedCell)
        {
            _selectedCell = selectedCell;
            // 全セルをクリア
            ClearHighLight();

            if (_preservedCells.Any())
            {
                //予約済みセルは固定表示
                HighLight(_preservedCells, _sameNumberColor);
                return;
            }

            if (_selectedCell == null) return;

            // 優先度低い順に上書き

            // 1. 関連セル（行・列・ブロック）
            var relatedCells = Board.GetRelatedCells(_selectedCell);
            HighLight(relatedCells, _relatedColor);


            // 2. 同じ数字
            if (_selectedCell.StateManager.IsCorrct)
            {
                var num = _selectedCell.Value;
                HighLighSameNum(num);
            }

            // 3. 選択中（最優先）
            HighLight(_selectedCell, _selectedColor);
        }

        public void ClearHighLight()
        {
            HighLight(Board.Cells, _normalColor);
        }

        public void HighLighSameNum(int num)
        {
            var sameNumberCells = Board.GetSameCells(num).Where(x => x.StateManager.IsCorrect);
            HighLight(sameNumberCells.ToArray(), _sameNumberColor);
        }

        void HighLight(IEnumerable<Cell> cells, Color color)
        {
            foreach (var cell in cells)
            {
                HighLight(cell, color);
            }
        }

        void HighLight(Cell cell, Color color)
        {
            cell?.Visualizer?.SetVisual(color);
        }
    }
}
