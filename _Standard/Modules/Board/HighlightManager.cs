using MantenseiLib;
using Nanpure.Standard.InputSystem;
using Nanpure.Standard.Module;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Nanpure.Standard.Core
{
    public class HighlightManager : MonoBehaviour
    {
        Cell _selectedCell;

        [Parent]
        Root _root;

        [Sibling] private IBoardProvider _boardManager;
        [Sibling] private ColorSettings _colorSettings;
        [Sibling] private InputHandler inputHandler;

        public Board Board => _boardManager.BoardReference;
        int _preserveNum = -1;
        bool Preserved => _preserveNum > 0;

        public event Action<int> OnPreserve;

        public void Preserve(int num)
        {
            _preserveNum = num;
            OnPreserve?.Invoke(num);
            UpdateHighlights();
        }

        public void ClearPreserve() => Preserve(-1);

        public Color NormalColor => _colorSettings.White;
        public Color RelatedColor => _colorSettings.DarkWhite;
        public Color SameNumberColor => _colorSettings.LightDark;
        public Color SelectedColor => _colorSettings.SystemWhite;
        public Color HoverColor => _colorSettings.LightDark;

        private void Start()
        {
            inputHandler.onCellSelected += OnSelected;
            inputHandler.onCellHoverEnter += (c) => HighLight(c, HoverColor);
            inputHandler.onCellHoverExit += (c) => UpdateHighlights(_selectedCell);
        }

        void OnSelected(Cell cell)
        {
            //if(cell.State.IsCorrect)
            //    Preserve(cell.Value);

            UpdateHighlights(cell);
        }

        public void UpdateHighlights() => UpdateHighlights(_selectedCell);

        private void UpdateHighlights(Cell selectedCell)
        {
            _selectedCell = selectedCell;
            // 全セルをクリア
            ClearHighLight();

            if (Preserved)
            {
                var cells = Board.GetSameCells(_preserveNum).Where(x => x.State.IsCorrect); ;
                HighLight(cells, SameNumberColor);

                return;
            }

            if (_selectedCell == null) return;

            if(_selectedCell.State.IsCorrect)
                OnPreserve?.Invoke(_selectedCell.Value);
            else
                OnPreserve?.Invoke(-1);

            // 優先度低い順に上書き

            // 1. 関連セル（行・列・ブロック）
            var relatedCells = Board.GetRelatedCells(_selectedCell);
            HighLight(relatedCells, RelatedColor);


            // 2. 同じ数字
            if (_selectedCell.State.IsCorrect)
            {
                var num = _selectedCell.Value;
                HighLighSameNum(num);
            }

            // 3. 選択中（最優先）
            HighLight(_selectedCell, SelectedColor);
        }

        public void ClearHighLight()
        {
            HighLight(Board.Cells, NormalColor);
        }

        public void HighLighSameNum(int num)
        {
            var sameNumberCells = Board.GetSameCells(num).Where(x => x.State.IsCorrect);
            HighLight(sameNumberCells.ToArray(), SameNumberColor);
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
