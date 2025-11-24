using MantenseiLib;
using Nanpure.Standard.InputSystem;
using Nanpure.Standard.Module;
using System;
using System.Linq;
using UnityEngine;

namespace Nanpure.Standard.Logic
{
    public class BoardProgressState 
    {
        public Board Board { get; }
        public int Count { get; }
        public Cell[] Cells { get; }

        public float Progress { get; }
        public int CorrectCount { get; }
        public int UnsolvedCount { get; }

        public BoardProgressState(Board board)
        {
            Board = board;
            Count = board.Count;
            Cells = board.Cells;
            CorrectCount = Cells.Count(x => x.State.IsCorrect);
            UnsolvedCount = Count - CorrectCount;
            Progress = (float)CorrectCount / Count;
        }
    }


    public interface IBoardMonitor 
    {
        BoardMonitor BoardMonitorReference { get; }
    }

    public class BoardMonitor : MonoBehaviour, IBoardMonitor
    {
        public BoardMonitor BoardMonitorReference => this;

        [GetComponent(HierarchyRelation.Parent)]
        IBoardProvider _board;
        Board Board => _board.BoardReference;

        [GetComponent(HierarchyRelation.Parent)]
        IInputActionHandlerProvider _inputActionHandler;
        IInputActionHandlerEntity InputActionHandler => _inputActionHandler.InputActionHandlerReference;

        public event Action<int> onUsedUp;
        public event Action<BoardProgressState> onProceed;

        private void Start()
        {
            InputActionHandler.onCellUpdate += OnCellUpdate;
        }

        void OnCellUpdate(Cell cell)
        {
            var num = cell.Value;
            var same = Board.GetSameCells(num);
            if (same.All(x => x.State.IsCorrect))
            {
                onUsedUp(num);
            }

            if (cell.State.IsCorrect)
            {
                var progress = new BoardProgressState(Board);
                onProceed?.Invoke(progress);

                if(progress.Progress == 1)
                {
                    foreach (var obj in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
                    {
                        // 呼び出し元シーンに通知したいならEventにすべき？
                        if (obj is IBoardEventReceiver receiver)
                            receiver.OnBoardEvent(Board, BoardEvent.Clear);
                    }
                }
            }
        }
    } 
}
