using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Nanpure.Standard.Module;

namespace Nanpure.Standard.Analyze
{
    public enum AnalyzedMoveType
    {
        Place,
        Narrowed
    }

    public class AnalyzedMove
    {
        public AnalyzedMoveType MoveType;
        public Cell Cell;
        public List<string> Techniques;
        public HashSet<int> RemainingCandidates;
        public HashSet<int> EliminatedCandidates;
        public List<Cell> RelatedCells;

        public bool CanPlace => RemainingCandidates.Count == 1;

        public bool TryGetNum(out int num)
        {
            if (CanPlace)
            {
                num = RemainingCandidates.First();
                return true;
            }
            else
            {
                num = -1;
                return false;
            }
        }

        public AnalyzedMove(AnalyzedMoveType moveType, Cell cell, string technique, HashSet<int> remaining, HashSet<int> eliminated, List<Cell> relatedCells = null)
        {
            MoveType = moveType;
            Cell = cell;
            Techniques = new List<string> { technique };
            RemainingCandidates = remaining;
            EliminatedCandidates = eliminated;
            RelatedCells = relatedCells ?? new List<Cell>();
        }

        public AnalyzedMove(AnalyzedMoveType moveType, Cell cell, List<string> techniques, HashSet<int> remaining, HashSet<int> eliminated, List<Cell> relatedCells = null)
        {
            MoveType = moveType;
            Cell = cell;
            Techniques = techniques;
            RemainingCandidates = remaining;
            EliminatedCandidates = eliminated;
            RelatedCells = relatedCells ?? new List<Cell>();
        }
    }

    public static class SolverResultAnalyzer
    {
        public static SolverOperation AnalyzeAsync(MonoBehaviour runner, Board board, params Type[] logicTypes)
        {
            var operation = new SolverOperation();
            var logics = new List<NanpureLogicBase>();

            foreach (var type in logicTypes)
            {
                if (typeof(NanpureLogicBase).IsAssignableFrom(type))
                {
                    var instance = Activator.CreateInstance(type) as NanpureLogicBase;
                    if (instance != null) logics.Add(instance);
                }
            }

            runner.StartCoroutine(AnalyzeCoroutine(board, logics, operation));
            return operation;
        }

        public static SolverOperation AnalyzeAsync(MonoBehaviour runner, Board board, params string[] logicTypeNames)
        {
            var types = logicTypeNames
                .Select(name => Type.GetType(name))
                .Where(t => t != null)
                .ToArray();

            return AnalyzeAsync(runner, board, types);
        }

        public static List<AnalyzedMove> Analyze(SolverResult result)
        {
            var analyzedMoves = new List<AnalyzedMove>();
            var processedCells = new HashSet<Cell>();

            foreach (var move in result.Moves)
            {
                if (move.MoveType == SolverMoveType.Place)
                {
                    if (processedCells.Contains(move.TargetCell)) continue;
                    processedCells.Add(move.TargetCell);

                    var remaining = new HashSet<int> { move.Number };
                    var eliminated = new HashSet<int>();
                    analyzedMoves.Add(new AnalyzedMove(AnalyzedMoveType.Place, move.TargetCell, move.Technique, remaining, eliminated, move.RelatedCells));
                }
            }

            var eliminateByCell = result.Moves
                .Where(m => m.MoveType == SolverMoveType.Eliminate)
                .Where(m => !processedCells.Contains(m.TargetCell))
                .GroupBy(m => m.TargetCell);

            foreach (var group in eliminateByCell)
            {
                Cell cell = group.Key;
                var memo = cell.State.Memo;
                if (memo == null || memo.Count == 0) continue;

                var remaining = new HashSet<int>(memo);
                var eliminated = new HashSet<int>();
                var techniques = new List<string>();
                var allRelatedCells = new HashSet<Cell>();

                foreach (var move in group)
                {
                    if (remaining.Remove(move.Number))
                    {
                        eliminated.Add(move.Number);
                        if (!techniques.Contains(move.Technique))
                        {
                            techniques.Add(move.Technique);
                        }
                        foreach (var relatedCell in move.RelatedCells)
                        {
                            allRelatedCells.Add(relatedCell);
                        }
                    }
                }

                if (eliminated.Count > 0)
                {
                    var moveType = remaining.Count == 1 ? AnalyzedMoveType.Place : AnalyzedMoveType.Narrowed;
                    analyzedMoves.Add(new AnalyzedMove(moveType, cell, techniques, remaining, eliminated, allRelatedCells.ToList()));
                }
            }

            return analyzedMoves;
        }

        private static IEnumerator AnalyzeCoroutine(Board board, List<NanpureLogicBase> logics, SolverOperation operation)
        {
            var solver = new NanpureSolver();
            solver.SetLogics(logics);

            var result = new SolverResult();
            var processedCells = new HashSet<Cell>();
            int totalLogics = logics.Count;
            int currentLogicIndex = 0;

            result.OnAddMove += move =>
            {
                if (operation.IsCancelled) return;

                if (move.MoveType == SolverMoveType.Place)
                {
                    if (processedCells.Contains(move.TargetCell)) return;
                    processedCells.Add(move.TargetCell);

                    var remaining = new HashSet<int> { move.Number };
                    var eliminated = new HashSet<int>();
                    var analyzed = new AnalyzedMove(AnalyzedMoveType.Place, move.TargetCell, move.Technique, remaining, eliminated, move.RelatedCells);
                    operation.ReportMoveFound(analyzed);
                }
            };

            solver.OnLogicStarted += logicName =>
            {
                if (operation.IsCancelled) return;
                operation.ReportLogicChanged(logicName);
                currentLogicIndex++;
                float progress = (float)currentLogicIndex / (totalLogics + 1);
                operation.ReportProgress(progress);
            };

            yield return solver.Solve(board, result);

            if (!operation.IsCancelled)
            {
                var eliminateByCell = result.Moves
                    .Where(m => m.MoveType == SolverMoveType.Eliminate)
                    .Where(m => !processedCells.Contains(m.TargetCell))
                    .GroupBy(m => m.TargetCell);

                foreach (var group in eliminateByCell)
                {
                    if (operation.IsCancelled) break;

                    Cell cell = group.Key;
                    var memo = cell.State.Memo;
                    if (memo == null || memo.Count == 0) continue;

                    var remaining = new HashSet<int>(memo);
                    var eliminated = new HashSet<int>();
                    var techniques = new List<string>();
                    var allRelatedCells = new HashSet<Cell>();

                    foreach (var move in group)
                    {
                        if (remaining.Remove(move.Number))
                        {
                            eliminated.Add(move.Number);
                            if (!techniques.Contains(move.Technique))
                            {
                                techniques.Add(move.Technique);
                            }
                            foreach (var relatedCell in move.RelatedCells)
                            {
                                allRelatedCells.Add(relatedCell);
                            }
                        }
                    }

                    if (eliminated.Count > 0)
                    {
                        var moveType = remaining.Count == 1 ? AnalyzedMoveType.Place : AnalyzedMoveType.Narrowed;
                        var analyzed = new AnalyzedMove(moveType, cell, techniques, remaining, eliminated, allRelatedCells.ToList());
                        operation.ReportMoveFound(analyzed);
                    }
                }

                operation.Complete();
            }
        }
    }
}