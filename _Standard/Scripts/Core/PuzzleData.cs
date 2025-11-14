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

    public class CellData
    {
        public int Row;
        public int Column;
        public bool IsRevealed;
        public int Value;

        public CellData(int row, int column, bool isRevealed, int answerValue)
        {
            Row = row;
            Column = column;
            IsRevealed = isRevealed;
            Value = answerValue;
        }
    }

    [Serializable]
    public class PuzzleData
    {
        public int BoardSize;
        public int Seed;
        public CellData[] Cells;

        public PuzzleData(int boardSize)
        {
            BoardSize = boardSize;
            Cells = new CellData[boardSize * boardSize];
        }
    }
}