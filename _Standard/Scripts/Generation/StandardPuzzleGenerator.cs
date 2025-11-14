using System;
using System.Collections.Generic;
using Nanpure.Standard.Core;

namespace Nanpure.Standard.Generation
{
    public interface IPuzzleGenerator
    {
        PuzzleData Generate(int seed, Difficulty difficulty);
    }

    public class StandardPuzzleGenerator : IPuzzleGenerator
    {
        public int BoardSize { get; private set; }
        public int BlockSize { get; private set; }
        public int TotalCells { get; private set; }

        private System.Random _random;
        private int[,] _board;

        public StandardPuzzleGenerator() : this(3) { }

        public StandardPuzzleGenerator(int blockSize) : this(blockSize * blockSize, blockSize) { }

        public StandardPuzzleGenerator(int boardSize, int blockSize)
        {
            BoardSize = boardSize;
            BlockSize = blockSize;
            TotalCells = boardSize * boardSize;
        }

        public PuzzleData Generate(int seed, Difficulty difficulty)
        {
            _random = new System.Random(seed);
            _board = new int[BoardSize, BoardSize];

            GenerateCompletedBoard();

            int[] solution = BoardToArray();
            int holesToMake = GetHolesCount(difficulty);
            int[] puzzle = CreatePuzzle(solution, holesToMake);

            return new PuzzleData
            {
                Seed = seed,
                InitialState = puzzle,
                Solution = solution
            };
        }

        // 難易度に応じた穴あけ数を返す
        private int GetHolesCount(Difficulty difficulty)
        {
            return difficulty switch
            {
                Difficulty.Easy => 35,
                Difficulty.Normal => 45,
                Difficulty.Hard => 52,
                Difficulty.Expert => 58,
                _ => 45
            };
        }

        // 完成した盤面を生成（対角ブロックを埋めてからバックトラック）
        private void GenerateCompletedBoard()
        {
            FillDiagonalBlocks();
            SolveBoard(0, 0);
        }

        // 対角線上のブロック（干渉しない）をランダムに埋める
        private void FillDiagonalBlocks()
        {
            int blockCount = BoardSize / BlockSize;
            for (int block = 0; block < blockCount; block++)
            {
                FillBlock(block * BlockSize, block * BlockSize);
            }
        }

        // 指定位置のブロックをランダムな数字で埋める
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
                    _board[row + i, col + j] = numbers[index++];
                }
            }
        }

        // バックトラック法で盤面を完成させる
        private bool SolveBoard(int row, int col)
        {
            if (col == BoardSize)
            {
                col = 0;
                row++;
                if (row == BoardSize) return true;
            }

            if (_board[row, col] != 0)
                return SolveBoard(row, col + 1);

            List<int> numbers = new List<int>();
            for (int i = 1; i <= BoardSize; i++)
            {
                numbers.Add(i);
            }
            Shuffle(numbers);

            foreach (int num in numbers)
            {
                if (IsValidPlacement(row, col, num))
                {
                    _board[row, col] = num;
                    if (SolveBoard(row, col + 1))
                        return true;
                    _board[row, col] = 0;
                }
            }

            return false;
        }

        // 指定位置に数字を配置できるかチェック（行・列・ブロックの重複確認）
        private bool IsValidPlacement(int row, int col, int num)
        {
            for (int i = 0; i < BoardSize; i++)
            {
                if (_board[row, i] == num) return false;
                if (_board[i, col] == num) return false;
            }

            int blockRow = (row / BlockSize) * BlockSize;
            int blockCol = (col / BlockSize) * BlockSize;
            for (int i = 0; i < BlockSize; i++)
            {
                for (int j = 0; j < BlockSize; j++)
                {
                    if (_board[blockRow + i, blockCol + j] == num)
                        return false;
                }
            }

            return true;
        }

        // 完成盤面から穴を開けてパズルを作成（唯一解を保証）
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

        // パズルが唯一解を持つかチェック（解が2つ以上見つかったら打ち切り）
        private bool HasUniqueSolution(int[] puzzle)
        {
            int[,] testBoard = ArrayToBoard(puzzle);
            int solutionCount = 0;
            CountSolutions(testBoard, 0, 0, ref solutionCount);
            return solutionCount == 1;
        }

        // 再帰的に解の数を数える（2つ見つかった時点で打ち切り）
        private void CountSolutions(int[,] board, int row, int col, ref int count)
        {
            if (count > 1) return;

            if (col == BoardSize)
            {
                col = 0;
                row++;
                if (row == BoardSize)
                {
                    count++;
                    return;
                }
            }

            if (board[row, col] != 0)
            {
                CountSolutions(board, row, col + 1, ref count);
                return;
            }

            for (int num = 1; num <= BoardSize; num++)
            {
                if (IsValidPlacementOnBoard(board, row, col, num))
                {
                    board[row, col] = num;
                    CountSolutions(board, row, col + 1, ref count);
                    board[row, col] = 0;
                }
            }
        }

        // 指定盤面での配置チェック（CountSolutions用）
        private bool IsValidPlacementOnBoard(int[,] board, int row, int col, int num)
        {
            for (int i = 0; i < BoardSize; i++)
            {
                if (board[row, i] == num) return false;
                if (board[i, col] == num) return false;
            }

            int blockRow = (row / BlockSize) * BlockSize;
            int blockCol = (col / BlockSize) * BlockSize;
            for (int i = 0; i < BlockSize; i++)
            {
                for (int j = 0; j < BlockSize; j++)
                {
                    if (board[blockRow + i, blockCol + j] == num)
                        return false;
                }
            }

            return true;
        }

        // 2次元配列を1次元配列に変換
        private int[] BoardToArray()
        {
            int[] array = new int[TotalCells];
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    array[i * BoardSize + j] = _board[i, j];
                }
            }
            return array;
        }

        // 1次元配列を2次元配列に変換
        private int[,] ArrayToBoard(int[] array)
        {
            int[,] board = new int[BoardSize, BoardSize];
            for (int i = 0; i < TotalCells; i++)
            {
                board[i / BoardSize, i % BoardSize] = array[i];
            }
            return board;
        }

        // リストをシャッフル
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