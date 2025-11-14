using System;

namespace Nanpure.Standard.Core
{
    public enum Difficulty
    {
        Easy,
        Normal,
        Hard,
        Expert
    }

    [Serializable]
    public struct CellPosition
    {
        public int Row;
        public int Column;

        public CellPosition(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public int ToIndex() => Row * 9 + Column;

        public static CellPosition FromIndex(int index)
        {
            return new CellPosition(index / 9, index % 9);
        }

        public int BlockIndex => (Row / 3) * 3 + (Column / 3);

        public override string ToString() => $"R{Row}C{Column}";
    }

    [Serializable]
    public class PuzzleData
    {
        public int Seed;
        public int[] InitialState;
        public int[] Solution;

        public PuzzleData()
        {
            InitialState = new int[81];
            Solution = new int[81];
        }

        public int GetInitialValue(int row, int column)
        {
            return InitialState[row * 9 + column];
        }

        public int GetSolution(int row, int column)
        {
            return Solution[row * 9 + column];
        }

        public bool IsFixed(int row, int column)
        {
            return InitialState[row * 9 + column] != 0;
        }
    }
}