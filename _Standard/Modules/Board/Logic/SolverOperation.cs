using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nanpure.Standard.Analyze
{
    public class SolverOperation : CustomYieldInstruction
    {
        public List<AnalyzedMove> Result { get; private set; }
        public bool IsDone { get; private set; }
        public bool IsCancelled { get; private set; }
        public float Progress { get; private set; }
        public string CurrentLogic { get; private set; }

        public override bool keepWaiting => !IsDone && !IsCancelled;

        public event Action<SolverOperation> Completed;
        public event Action<AnalyzedMove> OnMoveFound;
        public event Action<string> OnLogicChanged;
        public event Action<float> OnProgress;
        public event Action OnCancelled;

        public SolverOperation()
        {
            Result = new List<AnalyzedMove>();
            Progress = 0f;
        }

        public void Cancel()
        {
            if (IsDone) return;
            IsCancelled = true;
            OnCancelled?.Invoke();
        }

        internal void ReportMoveFound(AnalyzedMove move)
        {
            Result.Add(move);
            OnMoveFound?.Invoke(move);
        }

        internal void ReportLogicChanged(string logicName)
        {
            CurrentLogic = logicName;
            OnLogicChanged?.Invoke(logicName);
        }

        internal void ReportProgress(float progress)
        {
            Progress = progress;
            OnProgress?.Invoke(progress);
        }

        internal void Complete()
        {
            if (IsCancelled) return;
            Progress = 1f;
            IsDone = true;
            Completed?.Invoke(this);
        }
    }
}
