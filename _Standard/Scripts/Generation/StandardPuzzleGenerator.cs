using System;
using System.Collections.Generic;
using Nanpure.Standard.Core;

namespace Nanpure.Standard
{
    public interface IPuzzleGenerator
    {
        PuzzleData Generate(int seed, Difficulty difficulty);
    }

    public interface INampurePuzzle
    {
        int[][] Board { get; }
    }

    public class StandardPuzzleGenerator : IPuzzleGenerator, INampurePuzzle
    {
        public int BoardSize { get; private set; }
        public int BlockSize { get; private set; }
        public int TotalCells { get; private set; }
        private int BlockCountPerSide => BoardSize / BlockSize;

        public int Seed { get; private set; }
        private System.Random _random;
        private int[][] _board;
        public int[][] Board => _board;

        private int[] _rowMasks;
        private int[] _colMasks;
        private int[] _blockMasks;

        public StandardPuzzleGenerator() : this(3) { }

        public StandardPuzzleGenerator(int blockSize) : this(blockSize * blockSize, blockSize) { }

        public StandardPuzzleGenerator(int boardSize, int blockSize)
        {
            BoardSize = boardSize;
            BlockSize = blockSize;
            TotalCells = boardSize * boardSize;
            _random = new System.Random();
        }

        public PuzzleData Generate(Difficulty difficulty)
            => Generate(_random.Next(), difficulty);

        public PuzzleData Generate(int seed, Difficulty difficulty)
        {
            this.Seed = seed;
            _random = new System.Random(Seed);

            _board = new int[BoardSize][];
            for (int i = 0; i < BoardSize; i++)
            {
                _board[i] = new int[BoardSize];
            }

            _rowMasks = new int[BoardSize];
            _colMasks = new int[BoardSize];
            _blockMasks = new int[BoardSize];

            GenerateCompletedBoard();

            int[] solution = BoardToArray();
            int holesToMake = GetHolesCount(difficulty);
            int[] puzzle = CreatePuzzle(solution, holesToMake);

            var puzzleData = new PuzzleData(BoardSize);
            puzzleData.Seed = Seed;

            for (int i = 0; i < TotalCells; i++)
            {
                int row = i / BoardSize;
                int col = i % BoardSize;
                int group = (row / BlockSize) * BlockCountPerSide + (col / BlockSize);
                bool isRevealed = puzzle[i] != 0;
                int answerValue = solution[i];
                puzzleData.Cells[i] = new CellData(row, col, group, isRevealed, answerValue);
            }

            return puzzleData;
        }

        private int GetHolesCount(Difficulty difficulty)
        {
            return difficulty switch
            {
                Difficulty.Easy => 35,
                Difficulty.Normal => 45,
                Difficulty.Hard => 52,
                Difficulty.Expert => 58,
                Difficulty.Master => 81,
                _ => 45
            };
        }

        private int GetBlockIndex(int row, int col)
        {
            return (row / BlockSize) * BlockCountPerSide + (col / BlockSize);
        }

        private bool CanPlace(int row, int col, int num)
        {
            int block = GetBlockIndex(row, col);
            int bit = 1 << num;
            return ((_rowMasks[row] | _colMasks[col] | _blockMasks[block]) & bit) == 0;
        }

        private void PlaceNumber(int row, int col, int num)
        {
            int block = GetBlockIndex(row, col);
            int bit = 1 << num;
            _rowMasks[row] |= bit;
            _colMasks[col] |= bit;
            _blockMasks[block] |= bit;
            _board[row][col] = num;
        }

        private void RemoveNumber(int row, int col, int num)
        {
            int block = GetBlockIndex(row, col);
            int bit = 1 << num;
            _rowMasks[row] &= ~bit;
            _colMasks[col] &= ~bit;
            _blockMasks[block] &= ~bit;
            _board[row][col] = 0;
        }

        private void GenerateCompletedBoard()
        {
            FillDiagonalBlocks();
            SolveBoard(0, 0);
        }

        private void FillDiagonalBlocks()
        {
            for (int block = 0; block < BlockCountPerSide; block++)
            {
                FillBlock(block * BlockSize, block * BlockSize);
            }
        }

        private void FillBlock(int row, int col)
        {
            List<int> numbers = new List<int>();
            for (int i = 1; i <= BoardSize; i++)
            {
                numbers.Add(i);
            }
            Shuffle(numbers);

            int index = 0;
            for (int i = 0; i < BlockSize; i++)
            {
                for (int j = 0; j < BlockSize; j++)
                {
                    PlaceNumber(row + i, col + j, numbers[index++]);
                }
            }
        }

        private bool SolveBoard(int row, int col)
        {
            if (col == BoardSize)
            {
                col = 0;
                row++;
                if (row == BoardSize) return true;
            }

            if (_board[row][col] != 0)
                return SolveBoard(row, col + 1);

            List<int> numbers = new List<int>();
            for (int i = 1; i <= BoardSize; i++)
            {
                numbers.Add(i);
            }
            Shuffle(numbers);

            foreach (int num in numbers)
            {
                if (CanPlace(row, col, num))
                {
                    PlaceNumber(row, col, num);
                    if (SolveBoard(row, col + 1))
                        return true;
                    RemoveNumber(row, col, num);
                }
            }

            return false;
        }

        private int[] CreatePuzzle(int[] solution, int holesToMake)
        {
            int[] puzzle = new int[TotalCells];
            Array.Copy(solution, puzzle, TotalCells);

            List<int> positions = new List<int>();
            for (int i = 0; i < TotalCells; i++) positions.Add(i);
            Shuffle(positions);

            int holesCreated = 0;
            foreach (int pos in positions)
            {
                if (holesCreated >= holesToMake) break;

                int backup = puzzle[pos];
                puzzle[pos] = 0;

                if (HasUniqueSolution(puzzle))
                {
                    holesCreated++;
                }
                else
                {
                    puzzle[pos] = backup;
                }
            }

            return puzzle;
        }

        private bool HasUniqueSolution(int[] puzzle)
        {
            int[] rowMasks = new int[BoardSize];
            int[] colMasks = new int[BoardSize];
            int[] blockMasks = new int[BoardSize];

            for (int i = 0; i < TotalCells; i++)
            {
                int num = puzzle[i];
                if (num == 0) continue;

                int row = i / BoardSize;
                int col = i % BoardSize;
                int block = GetBlockIndex(row, col);
                int bit = 1 << num;

                rowMasks[row] |= bit;
                colMasks[col] |= bit;
                blockMasks[block] |= bit;
            }

            int solutionCount = 0;
            CountSolutions(puzzle, 0, rowMasks, colMasks, blockMasks, ref solutionCount);
            return solutionCount == 1;
        }

        private void CountSolutions(int[] puzzle, int index, int[] rowMasks, int[] colMasks, int[] blockMasks, ref int count)
        {
            if (count > 1) return;

            if (index == TotalCells)
            {
                count++;
                return;
            }

            if (puzzle[index] != 0)
            {
                CountSolutions(puzzle, index + 1, rowMasks, colMasks, blockMasks, ref count);
                return;
            }

            int row = index / BoardSize;
            int col = index % BoardSize;
            int block = GetBlockIndex(row, col);
            int usedMask = rowMasks[row] | colMasks[col] | blockMasks[block];

            for (int num = 1; num <= BoardSize; num++)
            {
                int bit = 1 << num;
                if ((usedMask & bit) != 0) continue;

                rowMasks[row] |= bit;
                colMasks[col] |= bit;
                blockMasks[block] |= bit;

                CountSolutions(puzzle, index + 1, rowMasks, colMasks, blockMasks, ref count);

                rowMasks[row] &= ~bit;
                colMasks[col] &= ~bit;
                blockMasks[block] &= ~bit;
            }
        }

        private int[] BoardToArray()
        {
            int[] array = new int[TotalCells];
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    array[i * BoardSize + j] = _board[i][j];
                }
            }
            return array;
        }

        private void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }
}