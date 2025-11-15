using UnityEngine;
using Nanpure.Standard.Core;

namespace Nanpure.Standard.Module
{
    /// <summary>盤面全体を管理</summary>
    public class BoardManager : MonoBehaviour
    {
        [SerializeField] private Cell _cellPrefab;
        [SerializeField] private Transform _cellContainer;

        public int BoardSize { get; private set; }
        private Cell[,] _cells;

        /// <summary>盤面を初期化</summary>
        public void Initialize(int boardSize)
        {
            BoardSize = boardSize;
            _cells = new Cell[boardSize, boardSize];

            CreateCells();
        }

        /// <summary>パズルデータを盤面に投入</summary>
        public void LoadPuzzle(PuzzleData puzzleData)
        {
            if (puzzleData.BoardSize != BoardSize)
            {
                Debug.LogError($"BoardSize mismatch: {BoardSize} vs {puzzleData.BoardSize}");
                return;
            }

            foreach (var cellData in puzzleData.Cells)
            {
                var cell = GetCell(cellData.Row, cellData.Column);
                if (cell != null)
                {
                    InitializeCell(cell, cellData);
                }
            }
        }

        /// <summary>指定位置のセルを取得</summary>
        public Cell GetCell(int row, int column)
        {
            if (row < 0 || row >= BoardSize || column < 0 || column >= BoardSize)
                return null;

            return _cells[row, column];
        }

        /// <summary>全セルを取得</summary>
        public Cell[,] GetAllCells()
        {
            return _cells;
        }

        private void CreateCells()
        {
            if (_cellPrefab == null)
            {
                Debug.LogError("Cell Prefab is not assigned!");
                return;
            }

            for (int row = 0; row < BoardSize; row++)
            {
                for (int col = 0; col < BoardSize; col++)
                {
                    var cellObj = Instantiate(_cellPrefab, _cellContainer);
                    cellObj.name = $"Cell_{row}_{col}";
                    _cells[row, col] = cellObj;
                }
            }
        }

        private void InitializeCell(Cell cell, CellData cellData)
        {
            var meta = cell.GetComponentInChildren<Module.CellMeta>();
            if (meta != null)
            {
                int initialValue = cellData.IsRevealed ? cellData.Value : 0;
                meta.Initialize(cellData.Row, cellData.Column, cellData.Value, cellData.IsRevealed);
                
                if (initialValue > 0)
                {
                    cell.State.SetValue(initialValue);
                }
            }
        }
    }
}
