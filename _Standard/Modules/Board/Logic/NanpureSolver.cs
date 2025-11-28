using System;
using System.Collections;
using System.Collections.Generic;
using Nanpure.Standard.Module;

namespace Nanpure.Standard.Analyze
{
    public enum SolverMoveType
    {
        Place,
        Eliminate
    }

    public class SolverMove
    {
        public SolverMoveType MoveType;
        public Cell TargetCell;
        public int Number;
        public string Technique;
        public List<Cell> RelatedCells;

        public SolverMove(Cell cell, int number, string technique, SolverMoveType moveType, List<Cell> relatedCells = null)
        {
            TargetCell = cell;
            Number = number;
            Technique = technique;
            MoveType = moveType;
            RelatedCells = relatedCells ?? new List<Cell>();
        }
    }

    public class SolverResult
    {
        public List<SolverMove> Moves = new List<SolverMove>();
        public event Action<SolverMove> OnAddMove;

        public void AddPlaceMove(Cell cell, int number, Type technique, List<Cell> relatedCells = null)
        {
            var move = new SolverMove(cell, number, technique.Name, SolverMoveType.Place, relatedCells);
            Moves.Add(move);
            OnAddMove?.Invoke(move);
        }

        public void AddEliminateMove(Cell cell, int number, Type technique, List<Cell> relatedCells = null)
        {
            var move = new SolverMove(cell, number, technique.Name, SolverMoveType.Eliminate, relatedCells);
            Moves.Add(move);
            OnAddMove?.Invoke(move);
        }

        public void Clear()
        {
            Moves.Clear();
        }
    }

    public class SolverBoardInfo
    {
        public readonly int BoardSize;
        public readonly int BlockSize;
        public readonly int CellCount;
        public readonly int FullMask;

        public SolverBoardInfo(Board board)
        {
            BoardSize = board.BoardSize;
            BlockSize = board.BlockSize;
            CellCount = board.Count;
            FullMask = (1 << (BoardSize + 1)) - 2;
        }

        public int GetCellIndexFromUnit(int unitType, int unitIndex, int elementIndex)
        {
            if (unitType == 0) return unitIndex * BoardSize + elementIndex;
            if (unitType == 1) return elementIndex * BoardSize + unitIndex;

            int startRow = (unitIndex / BlockSize) * BlockSize;
            int startCol = (unitIndex % BlockSize) * BlockSize;
            int r = startRow + (elementIndex / BlockSize);
            int c = startCol + (elementIndex % BlockSize);
            return r * BoardSize + c;
        }

        public Cell GetCellFromIndex(Board board, int index)
        {
            return board.GetCell(index / BoardSize, index % BoardSize);
        }

        public void GetRowColFromIndex(int index, out int row, out int col)
        {
            row = index / BoardSize;
            col = index % BoardSize;
        }

        public int GetBlockIndex(int row, int col)
        {
            return (row / BlockSize) * (BoardSize / BlockSize) + (col / BlockSize);
        }
    }

    public abstract class NanpureLogicBase
    {
        public SolverBoardInfo BoardInfo { get; set; }
        public abstract void Execute(Board board, int[] candidateMasks, SolverResult result);
    }

    public class NanpureSolver
    {
        private readonly List<NanpureLogicBase> _activeLogics;
        private int[] _candidateMasks;

        public event Action<string> OnLogicStarted;

        public NanpureSolver()
        {
            _activeLogics = new List<NanpureLogicBase>();
        }

        public void SetLogics(IEnumerable<NanpureLogicBase> logics)
        {
            _activeLogics.Clear();
            _activeLogics.AddRange(logics);
        }

        public IEnumerator Solve(Board board, SolverResult result)
        {
            var boardInfo = new SolverBoardInfo(board);
            _candidateMasks = new int[boardInfo.CellCount];

            for (int i = 0; i < _activeLogics.Count; i++)
            {
                _activeLogics[i].BoardInfo = boardInfo;
            }

            CalculateAllCandidates(board, boardInfo);
            yield return null;

            for (int i = 0; i < _activeLogics.Count; i++)
            {
                OnLogicStarted?.Invoke(_activeLogics[i].GetType().Name);
                _activeLogics[i].Execute(board, _candidateMasks, result);
                yield return null;
            }
        }

        private void CalculateAllCandidates(Board board, SolverBoardInfo boardInfo)
        {
            for (int i = 0; i < boardInfo.CellCount; i++)
            {
                Cell cell = boardInfo.GetCellFromIndex(board, i);

                if (cell.State.IsCorrect)
                {
                    _candidateMasks[i] = 0;
                    continue;
                }

                int mask = boardInfo.FullMask;
                Cell[] relatedCells = board.GetRelatedCells(cell);
                for (int j = 0; j < relatedCells.Length; j++)
                {
                    Cell related = relatedCells[j];
                    if (related.State.IsCorrect)
                    {
                        int valueMask = 1 << related.State.DisplayNum;
                        mask &= ~valueMask;
                    }
                }

                _candidateMasks[i] = mask;
            }
        }
    }

    public class NakedSingleLogic : NanpureLogicBase
    {
        public override void Execute(Board board, int[] candidateMasks, SolverResult result)
        {
            for (int i = 0; i < BoardInfo.CellCount; i++)
            {
                int mask = candidateMasks[i];
                if (mask == 0) continue;

                if ((mask & (mask - 1)) == 0)
                {
                    int number = SolverUtils.GetNumberFromSingleBitMask(mask);
                    Cell cell = BoardInfo.GetCellFromIndex(board, i);

                    var relatedCells = new List<Cell>();
                    Cell[] related = board.GetRelatedCells(cell);
                    foreach (Cell r in related)
                    {
                        if (r.State.IsCorrect)
                        {
                            relatedCells.Add(r);
                        }
                    }

                    result.AddPlaceMove(cell, number, GetType(), relatedCells);
                }
            }
        }
    }

    public class HiddenSingleLogic : NanpureLogicBase
    {
        public override void Execute(Board board, int[] candidateMasks, SolverResult result)
        {
            for (int unitType = 0; unitType < 3; unitType++)
            {
                for (int unitIndex = 0; unitIndex < BoardInfo.BoardSize; unitIndex++)
                {
                    for (int number = 1; number <= BoardInfo.BoardSize; number++)
                    {
                        int numberMask = 1 << number;
                        int count = 0;
                        int lastFoundIndex = -1;
                        var otherEmptyCells = new List<int>();

                        for (int k = 0; k < BoardInfo.BoardSize; k++)
                        {
                            int cellIndex = BoardInfo.GetCellIndexFromUnit(unitType, unitIndex, k);
                            Cell cell = BoardInfo.GetCellFromIndex(board, cellIndex);

                            if (cell.State.IsCorrect)
                            {
                                if (cell.State.DisplayNum == number)
                                {
                                    count = -1;
                                    break;
                                }
                                continue;
                            }

                            if ((candidateMasks[cellIndex] & numberMask) != 0)
                            {
                                count++;
                                lastFoundIndex = cellIndex;
                            }
                            else
                            {
                                otherEmptyCells.Add(cellIndex);
                            }
                        }

                        if (count == 1 && lastFoundIndex != -1)
                        {
                            Cell target = BoardInfo.GetCellFromIndex(board, lastFoundIndex);

                            var relatedCells = new HashSet<Cell>();
                            foreach (int emptyIndex in otherEmptyCells)
                            {
                                Cell emptyCell = BoardInfo.GetCellFromIndex(board, emptyIndex);
                                Cell[] related = board.GetRelatedCells(emptyCell);
                                foreach (Cell r in related)
                                {
                                    if (r.State.IsCorrect && r.State.DisplayNum == number)
                                    {
                                        relatedCells.Add(r);
                                    }
                                }
                            }

                            result.AddPlaceMove(target, number, GetType(), new List<Cell>(relatedCells));
                        }
                    }
                }
            }
        }
    }

    public class NakedPairLogic : NanpureLogicBase
    {
        public override void Execute(Board board, int[] candidateMasks, SolverResult result)
        {
            for (int unitType = 0; unitType < 3; unitType++)
            {
                for (int unitIndex = 0; unitIndex < BoardInfo.BoardSize; unitIndex++)
                {
                    for (int i = 0; i < BoardInfo.BoardSize - 1; i++)
                    {
                        int indexA = BoardInfo.GetCellIndexFromUnit(unitType, unitIndex, i);
                        int maskA = candidateMasks[indexA];

                        if (SolverUtils.CountSetBits(maskA) != 2) continue;

                        for (int j = i + 1; j < BoardInfo.BoardSize; j++)
                        {
                            int indexB = BoardInfo.GetCellIndexFromUnit(unitType, unitIndex, j);
                            int maskB = candidateMasks[indexB];

                            if (maskA != maskB) continue;

                            Cell cellA = BoardInfo.GetCellFromIndex(board, indexA);
                            Cell cellB = BoardInfo.GetCellFromIndex(board, indexB);
                            var relatedCells = new List<Cell> { cellA, cellB };

                            for (int k = 0; k < BoardInfo.BoardSize; k++)
                            {
                                int targetIndex = BoardInfo.GetCellIndexFromUnit(unitType, unitIndex, k);
                                if (targetIndex == indexA || targetIndex == indexB) continue;

                                int targetMask = candidateMasks[targetIndex];
                                int overlap = targetMask & maskA;

                                if (overlap == 0) continue;

                                Cell targetCell = BoardInfo.GetCellFromIndex(board, targetIndex);
                                List<int> numbersToEliminate = SolverUtils.GetNumbersFromMask(overlap, BoardInfo.BoardSize);

                                foreach (int num in numbersToEliminate)
                                {
                                    result.AddEliminateMove(targetCell, num, GetType(), relatedCells);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public class PointingPairLogic : NanpureLogicBase
    {
        public override void Execute(Board board, int[] candidateMasks, SolverResult result)
        {
            for (int block = 0; block < BoardInfo.BoardSize; block++)
            {
                for (int number = 1; number <= BoardInfo.BoardSize; number++)
                {
                    int numberMask = 1 << number;
                    int count = 0;
                    int rowCheck = -1;
                    int colCheck = -1;
                    bool sameRow = true;
                    bool sameCol = true;
                    var foundIndices = new List<int>();

                    for (int k = 0; k < BoardInfo.BoardSize; k++)
                    {
                        int index = BoardInfo.GetCellIndexFromUnit(2, block, k);
                        if ((candidateMasks[index] & numberMask) != 0)
                        {
                            count++;
                            foundIndices.Add(index);
                            BoardInfo.GetRowColFromIndex(index, out int r, out int c);

                            if (rowCheck == -1) rowCheck = r;
                            else if (rowCheck != r) sameRow = false;

                            if (colCheck == -1) colCheck = c;
                            else if (colCheck != c) sameCol = false;
                        }
                    }

                    if (count < 2 || count > BoardInfo.BlockSize) continue;

                    var relatedCells = new List<Cell>();
                    foreach (int idx in foundIndices)
                    {
                        relatedCells.Add(BoardInfo.GetCellFromIndex(board, idx));
                    }

                    if (sameRow)
                    {
                        for (int c = 0; c < BoardInfo.BoardSize; c++)
                        {
                            int targetIndex = rowCheck * BoardInfo.BoardSize + c;
                            BoardInfo.GetRowColFromIndex(targetIndex, out int tr, out int tc);
                            int targetBlock = BoardInfo.GetBlockIndex(tr, tc);

                            if (targetBlock == block) continue;
                            if ((candidateMasks[targetIndex] & numberMask) == 0) continue;

                            Cell targetCell = BoardInfo.GetCellFromIndex(board, targetIndex);
                            result.AddEliminateMove(targetCell, number, GetType(), relatedCells);
                        }
                    }
                    else if (sameCol)
                    {
                        for (int r = 0; r < BoardInfo.BoardSize; r++)
                        {
                            int targetIndex = r * BoardInfo.BoardSize + colCheck;
                            BoardInfo.GetRowColFromIndex(targetIndex, out int tr, out int tc);
                            int targetBlock = BoardInfo.GetBlockIndex(tr, tc);

                            if (targetBlock == block) continue;
                            if ((candidateMasks[targetIndex] & numberMask) == 0) continue;

                            Cell targetCell = BoardInfo.GetCellFromIndex(board, targetIndex);
                            result.AddEliminateMove(targetCell, number, GetType(), relatedCells);
                        }
                    }
                }
            }
        }
    }

    public class BoxLineReductionLogic : NanpureLogicBase
    {
        public override void Execute(Board board, int[] candidateMasks, SolverResult result)
        {
            for (int unitType = 0; unitType < 2; unitType++)
            {
                for (int line = 0; line < BoardInfo.BoardSize; line++)
                {
                    for (int number = 1; number <= BoardInfo.BoardSize; number++)
                    {
                        int numberMask = 1 << number;
                        int count = 0;
                        int blockCheck = -1;
                        bool sameBlock = true;
                        var foundIndices = new List<int>();

                        for (int k = 0; k < BoardInfo.BoardSize; k++)
                        {
                            int index = BoardInfo.GetCellIndexFromUnit(unitType, line, k);

                            if ((candidateMasks[index] & numberMask) != 0)
                            {
                                count++;
                                foundIndices.Add(index);
                                BoardInfo.GetRowColFromIndex(index, out int r, out int c);
                                int b = BoardInfo.GetBlockIndex(r, c);

                                if (blockCheck == -1) blockCheck = b;
                                else if (blockCheck != b) sameBlock = false;
                            }
                        }

                        if (count < 2 || !sameBlock || blockCheck == -1) continue;

                        var relatedCells = new List<Cell>();
                        foreach (int idx in foundIndices)
                        {
                            relatedCells.Add(BoardInfo.GetCellFromIndex(board, idx));
                        }

                        for (int k = 0; k < BoardInfo.BoardSize; k++)
                        {
                            int targetIndex = BoardInfo.GetCellIndexFromUnit(2, blockCheck, k);
                            BoardInfo.GetRowColFromIndex(targetIndex, out int tr, out int tc);

                            bool isOnLine = (unitType == 0) ? (tr == line) : (tc == line);
                            if (isOnLine) continue;

                            if ((candidateMasks[targetIndex] & numberMask) == 0) continue;

                            Cell targetCell = BoardInfo.GetCellFromIndex(board, targetIndex);
                            result.AddEliminateMove(targetCell, number, GetType(), relatedCells);
                        }
                    }
                }
            }
        }
    }

    public class HiddenPairLogic : NanpureLogicBase
    {
        public override void Execute(Board board, int[] candidateMasks, SolverResult result)
        {
            for (int unitType = 0; unitType < 3; unitType++)
            {
                for (int unitIndex = 0; unitIndex < BoardInfo.BoardSize; unitIndex++)
                {
                    for (int num1 = 1; num1 <= BoardInfo.BoardSize - 1; num1++)
                    {
                        for (int num2 = num1 + 1; num2 <= BoardInfo.BoardSize; num2++)
                        {
                            int mask1 = 1 << num1;
                            int mask2 = 1 << num2;
                            int pairMask = mask1 | mask2;

                            var positions = new List<int>();

                            for (int k = 0; k < BoardInfo.BoardSize; k++)
                            {
                                int cellIndex = BoardInfo.GetCellIndexFromUnit(unitType, unitIndex, k);
                                int cellMask = candidateMasks[cellIndex];

                                bool hasNum1 = (cellMask & mask1) != 0;
                                bool hasNum2 = (cellMask & mask2) != 0;

                                if (hasNum1 || hasNum2)
                                {
                                    positions.Add(cellIndex);
                                }
                            }

                            if (positions.Count != 2) continue;

                            int idx1 = positions[0];
                            int idx2 = positions[1];

                            bool valid = (candidateMasks[idx1] & mask1) != 0 &&
                                         (candidateMasks[idx1] & mask2) != 0 &&
                                         (candidateMasks[idx2] & mask1) != 0 &&
                                         (candidateMasks[idx2] & mask2) != 0;

                            if (!valid) continue;

                            int extras1 = candidateMasks[idx1] & ~pairMask;
                            int extras2 = candidateMasks[idx2] & ~pairMask;

                            if (extras1 == 0 && extras2 == 0) continue;

                            Cell cell1 = BoardInfo.GetCellFromIndex(board, idx1);
                            Cell cell2 = BoardInfo.GetCellFromIndex(board, idx2);
                            var relatedCells = new List<Cell> { cell1, cell2 };

                            if (extras1 != 0)
                            {
                                var nums = SolverUtils.GetNumbersFromMask(extras1, BoardInfo.BoardSize);
                                foreach (int n in nums)
                                {
                                    result.AddEliminateMove(cell1, n, GetType(), relatedCells);
                                }
                            }

                            if (extras2 != 0)
                            {
                                var nums = SolverUtils.GetNumbersFromMask(extras2, BoardInfo.BoardSize);
                                foreach (int n in nums)
                                {
                                    result.AddEliminateMove(cell2, n, GetType(), relatedCells);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public class NakedTripleLogic : NanpureLogicBase
    {
        public override void Execute(Board board, int[] candidateMasks, SolverResult result)
        {
            for (int unitType = 0; unitType < 3; unitType++)
            {
                for (int unitIndex = 0; unitIndex < BoardInfo.BoardSize; unitIndex++)
                {
                    var indices = new List<int>();
                    for (int k = 0; k < BoardInfo.BoardSize; k++)
                    {
                        int cellIndex = BoardInfo.GetCellIndexFromUnit(unitType, unitIndex, k);
                        int mask = candidateMasks[cellIndex];
                        int count = SolverUtils.CountSetBits(mask);
                        if (count >= 2 && count <= 3)
                        {
                            indices.Add(cellIndex);
                        }
                    }

                    if (indices.Count < 3) continue;

                    for (int i = 0; i < indices.Count - 2; i++)
                    {
                        for (int j = i + 1; j < indices.Count - 1; j++)
                        {
                            for (int k = j + 1; k < indices.Count; k++)
                            {
                                int idxA = indices[i];
                                int idxB = indices[j];
                                int idxC = indices[k];

                                int combined = candidateMasks[idxA] | candidateMasks[idxB] | candidateMasks[idxC];

                                if (SolverUtils.CountSetBits(combined) != 3) continue;

                                Cell cellA = BoardInfo.GetCellFromIndex(board, idxA);
                                Cell cellB = BoardInfo.GetCellFromIndex(board, idxB);
                                Cell cellC = BoardInfo.GetCellFromIndex(board, idxC);
                                var relatedCells = new List<Cell> { cellA, cellB, cellC };

                                for (int m = 0; m < BoardInfo.BoardSize; m++)
                                {
                                    int targetIndex = BoardInfo.GetCellIndexFromUnit(unitType, unitIndex, m);
                                    if (targetIndex == idxA || targetIndex == idxB || targetIndex == idxC) continue;

                                    int overlap = candidateMasks[targetIndex] & combined;
                                    if (overlap == 0) continue;

                                    Cell cell = BoardInfo.GetCellFromIndex(board, targetIndex);
                                    var nums = SolverUtils.GetNumbersFromMask(overlap, BoardInfo.BoardSize);
                                    foreach (int n in nums)
                                    {
                                        result.AddEliminateMove(cell, n, GetType(), relatedCells);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public class XWingLogic : NanpureLogicBase
    {
        public override void Execute(Board board, int[] candidateMasks, SolverResult result)
        {
            for (int number = 1; number <= BoardInfo.BoardSize; number++)
            {
                int numberMask = 1 << number;
                FindXWing(board, candidateMasks, result, number, numberMask, true);
                FindXWing(board, candidateMasks, result, number, numberMask, false);
            }
        }

        private void FindXWing(Board board, int[] candidateMasks, SolverResult result, int number, int numberMask, bool rowBased)
        {
            var linePositions = new List<int>[BoardInfo.BoardSize];

            for (int line = 0; line < BoardInfo.BoardSize; line++)
            {
                linePositions[line] = new List<int>();
                for (int pos = 0; pos < BoardInfo.BoardSize; pos++)
                {
                    int cellIndex = rowBased
                        ? line * BoardInfo.BoardSize + pos
                        : pos * BoardInfo.BoardSize + line;

                    if ((candidateMasks[cellIndex] & numberMask) != 0)
                    {
                        linePositions[line].Add(pos);
                    }
                }
            }

            for (int line1 = 0; line1 < BoardInfo.BoardSize - 1; line1++)
            {
                if (linePositions[line1].Count != 2) continue;

                for (int line2 = line1 + 1; line2 < BoardInfo.BoardSize; line2++)
                {
                    if (linePositions[line2].Count != 2) continue;

                    if (linePositions[line1][0] != linePositions[line2][0] ||
                        linePositions[line1][1] != linePositions[line2][1]) continue;

                    int pos1 = linePositions[line1][0];
                    int pos2 = linePositions[line1][1];

                    int corner1 = rowBased ? line1 * BoardInfo.BoardSize + pos1 : pos1 * BoardInfo.BoardSize + line1;
                    int corner2 = rowBased ? line1 * BoardInfo.BoardSize + pos2 : pos2 * BoardInfo.BoardSize + line1;
                    int corner3 = rowBased ? line2 * BoardInfo.BoardSize + pos1 : pos1 * BoardInfo.BoardSize + line2;
                    int corner4 = rowBased ? line2 * BoardInfo.BoardSize + pos2 : pos2 * BoardInfo.BoardSize + line2;

                    var relatedCells = new List<Cell>
                    {
                        BoardInfo.GetCellFromIndex(board, corner1),
                        BoardInfo.GetCellFromIndex(board, corner2),
                        BoardInfo.GetCellFromIndex(board, corner3),
                        BoardInfo.GetCellFromIndex(board, corner4)
                    };

                    for (int otherLine = 0; otherLine < BoardInfo.BoardSize; otherLine++)
                    {
                        if (otherLine == line1 || otherLine == line2) continue;

                        int targetIndex1 = rowBased
                            ? otherLine * BoardInfo.BoardSize + pos1
                            : pos1 * BoardInfo.BoardSize + otherLine;

                        int targetIndex2 = rowBased
                            ? otherLine * BoardInfo.BoardSize + pos2
                            : pos2 * BoardInfo.BoardSize + otherLine;

                        if ((candidateMasks[targetIndex1] & numberMask) != 0)
                        {
                            Cell cell = BoardInfo.GetCellFromIndex(board, targetIndex1);
                            result.AddEliminateMove(cell, number, GetType(), relatedCells);
                        }

                        if ((candidateMasks[targetIndex2] & numberMask) != 0)
                        {
                            Cell cell = BoardInfo.GetCellFromIndex(board, targetIndex2);
                            result.AddEliminateMove(cell, number, GetType(), relatedCells);
                        }
                    }
                }
            }
        }
    }

    public class XYWingLogic : NanpureLogicBase
    {
        public override void Execute(Board board, int[] candidateMasks, SolverResult result)
        {
            for (int pivotIndex = 0; pivotIndex < BoardInfo.CellCount; pivotIndex++)
            {
                int pivotMask = candidateMasks[pivotIndex];
                if (SolverUtils.CountSetBits(pivotMask) != 2) continue;

                Cell pivotCell = BoardInfo.GetCellFromIndex(board, pivotIndex);
                Cell[] pivotRelated = board.GetRelatedCells(pivotCell);

                var pincerCandidates = new List<int>();
                foreach (Cell related in pivotRelated)
                {
                    int relatedIndex = GetCellIndex(related);
                    int relatedMask = candidateMasks[relatedIndex];

                    if (SolverUtils.CountSetBits(relatedMask) != 2) continue;
                    if ((relatedMask & pivotMask) == 0) continue;
                    if (relatedMask == pivotMask) continue;

                    pincerCandidates.Add(relatedIndex);
                }

                for (int i = 0; i < pincerCandidates.Count - 1; i++)
                {
                    for (int j = i + 1; j < pincerCandidates.Count; j++)
                    {
                        int pincer1Index = pincerCandidates[i];
                        int pincer2Index = pincerCandidates[j];

                        int pincer1Mask = candidateMasks[pincer1Index];
                        int pincer2Mask = candidateMasks[pincer2Index];

                        int shared1 = pivotMask & pincer1Mask;
                        int shared2 = pivotMask & pincer2Mask;

                        if (SolverUtils.CountSetBits(shared1) != 1) continue;
                        if (SolverUtils.CountSetBits(shared2) != 1) continue;
                        if (shared1 == shared2) continue;

                        int unique1 = pincer1Mask & ~pivotMask;
                        int unique2 = pincer2Mask & ~pivotMask;

                        if (unique1 != unique2) continue;
                        if (SolverUtils.CountSetBits(unique1) != 1) continue;

                        int eliminateNumber = SolverUtils.GetNumberFromSingleBitMask(unique1);

                        Cell pincer1Cell = BoardInfo.GetCellFromIndex(board, pincer1Index);
                        Cell pincer2Cell = BoardInfo.GetCellFromIndex(board, pincer2Index);

                        var relatedCells = new List<Cell> { pivotCell, pincer1Cell, pincer2Cell };

                        Cell[] pincer1Related = board.GetRelatedCells(pincer1Cell);
                        Cell[] pincer2Related = board.GetRelatedCells(pincer2Cell);

                        var pincer1Set = new HashSet<Cell>(pincer1Related);

                        foreach (Cell cell in pincer2Related)
                        {
                            if (!pincer1Set.Contains(cell)) continue;
                            if (cell == pivotCell) continue;
                            if (cell == pincer1Cell) continue;
                            if (cell == pincer2Cell) continue;

                            int targetIndex = GetCellIndex(cell);
                            if ((candidateMasks[targetIndex] & unique1) != 0)
                            {
                                result.AddEliminateMove(cell, eliminateNumber, GetType(), relatedCells);
                            }
                        }
                    }
                }
            }
        }

        private int GetCellIndex(Cell cell)
        {
            return cell.Row * BoardInfo.BoardSize + cell.Column;
        }
    }

    internal static class SolverUtils
    {
        public static int GetNumberFromSingleBitMask(int mask)
        {
            int number = 0;
            while (mask > 1)
            {
                mask >>= 1;
                number++;
            }
            return number;
        }

        public static int CountSetBits(int n)
        {
            int count = 0;
            while (n > 0)
            {
                n &= (n - 1);
                count++;
            }
            return count;
        }

        public static List<int> GetNumbersFromMask(int mask, int boardSize)
        {
            List<int> numbers = new List<int>();
            for (int i = 1; i <= boardSize; i++)
            {
                if ((mask & (1 << i)) != 0) numbers.Add(i);
            }
            return numbers;
        }
    }
}