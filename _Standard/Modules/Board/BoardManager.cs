using UnityEngine;
using Nanpure.Standard.Core;
using System.Collections.Generic;
using System.Linq;

namespace Nanpure.Standard.Module
{
    public interface IBoard
    {
        Board Board { get; }
    }

    public class Board
    {
        public Cell[] Cells { get; private set; }
        public int BlockSize { get; private set; }
        public int BoardSize => BlockSize * BlockSize;

        public Board(int blockSize, Cell[] board)
        {
            BlockSize = blockSize;
            Cells = board;
        }

        public Cell GetCell(Vector2Int address) => GetCell(address.x, address.y);

        public Cell GetCell(int row, int col)
        {
            return Cells[row * BoardSize + col];
        }

        public Cell[] GetRow(int column)
        {
            return Cells.Where(x => x.Column == column).ToArray();
        }

        public Cell[] GetColumn(int row)
        {
            return Cells.Where(x => x.Row == row).ToArray();
        }

        public Cell[] GetGroup(int blockIndex)
        {
            return Cells.Where(x => x.Data.Group == blockIndex).ToArray();
        }

        public Cell[] GetRelatedCells(Cell cell)
        {
            HashSet<Cell> relatedCells = new HashSet<Cell>();
            relatedCells.UnionWith(GetRow(cell.Data.Column));
            relatedCells.UnionWith(GetColumn(cell.Data.Row));
            relatedCells.UnionWith(GetGroup(cell.Data.Group));
            return relatedCells.ToArray();
        }
    }

    public class BoardManager : MonoBehaviour, IBoard
    {
        [SerializeField] private Cell _cellPrefab;
        [SerializeField] private InputButton _inputButtonPrefab;
        [SerializeField] private Transform _buttonParent;
        [SerializeField] private Transform _cellContainer;

        public int blockSize { get; private set; } = 3;
        public int BoardSize => blockSize * blockSize;
        StandardPuzzleGenerator puzzleGenerator;

        private List<Cell> _cells = new();
        public Board Board { get; private set; }

        public void Initialize() => Initialize(3);
        public void Initialize(int blockSize)
        {
            this.blockSize = blockSize;
            puzzleGenerator = new StandardPuzzleGenerator(blockSize);

            CreateCells();
            CreateButtons();

            Board = new Board(blockSize, _cells.ToArray());
            _cells = null;
        }

        void CreateButtons()
        {
            for(int i = 1; i <= BoardSize; i++)
            {
                var button = Instantiate(_inputButtonPrefab, _buttonParent);
                button.SetNum(i);
            }
        }

        private void CreateCells()
        {
            var puzzle = puzzleGenerator.Generate(Difficulty.Expert);

            foreach (var cellData in puzzle.Cells)
            {
                var cell = Instantiate(_cellPrefab);
                var num = cellData.Value;
                var row = cellData.Row;
                var col = cellData.Column;

                cell.Data.Initialize(cellData);
                cell.StateManager.Initialize();

                cell.name = $"{nameof(Cell)}_{row}_{col}";
                _cells.Add(cell);

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
