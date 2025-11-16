using UnityEngine;
using Nanpure.Standard.Core;

namespace Nanpure.Standard.Module
{
    public class BoardManager : MonoBehaviour
    {
        [SerializeField] private Cell _cellPrefab;
        [SerializeField] private Transform _cellContainer;

        public int blockSize { get; private set; } = 3;
        public int BoardSize => blockSize * blockSize;
        private Cell[,] _cells;
        StandardPuzzleGenerator puzzleGenerator;

        public void Initialize() => Initialize(3);
        public void Initialize(int blockSize)
        {
            _cells = new Cell[BoardSize, BoardSize];
            puzzleGenerator = new StandardPuzzleGenerator(blockSize);

            CreateCells();
        }

        public Cell GetCell(Vector2Int address) => GetCell(address.x, address.y);

        public Cell GetCell(int row, int column)
        {
            if (row < 0 || row >= BoardSize || column < 0 || column >= BoardSize)
                return null;

            return _cells[row, column];
        }

        public Cell[,] GetAllCells()
        {
            return _cells;
        }

        private void CreateCells()
        {
            var puzzle = puzzleGenerator.Generate(Difficulty.Expert);
            _cells = new Cell[BoardSize, BoardSize];

            foreach (var cellData in puzzle.Cells)
            {
                var cell = Instantiate(_cellPrefab);
                var num = cellData.Value;
                var row = cellData.Row;
                var col = cellData.Column;

                cell.Data.Initialize(row, col, num, cellData.IsRevealed);
                cell.StateManager.Initialize();

                cell.name = $"Cell_{row}_{col}";
                _cells[row, col] = cell;

                SetCellPosition(cell, row, col);

                cell.transform.SetParent(_cellContainer, false);
            }
        }

        private void SetCellPosition(Cell cell, int row, int col)
        {
            var containerRect = _cellContainer.GetComponent<RectTransform>();

            if (containerRect != null)
            {
                float width = containerRect.rect.width;
                float height = containerRect.rect.height;

                float cellSizeX = width / BoardSize;
                float cellSizeY = height / BoardSize;

                float x = (col + 0.5f) * cellSizeX - width / 2f;
                float y = height / 2f - (row + 0.5f) * cellSizeY;

                cell.transform.localPosition = new Vector3(x, y, 0f);
                cell.transform.localScale = new Vector3(cellSizeX, cellSizeY, 1f);
            }
        }

    }
}
