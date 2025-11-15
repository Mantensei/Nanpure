using System.Collections.Generic;
using UnityEngine;
using MantenseiLib;

namespace Nanpure.Standard.Cell
{
    /// <summary>セル全体を統括（参照ホルダー）</summary>
    public class Cell : MonoBehaviour
    {
        [GetComponent(HierarchyRelation.Children)] public  CellMeta meta { get; private set; }
        [GetComponent(HierarchyRelation.Children)] public CellState state { get; private set; }

        // 外部アクセス用プロパティ
        public int Row => meta.Row;
        public int Column => meta.Column;
        public int AnswerValue => meta.AnswerValue;
        public bool IsFixed => meta.IsFixed;
        public int InputValue => state.InputValue;
        public IReadOnlyCollection<int> Memos => state.Memos;

        // イベント中継
        public event System.Action<int> OnValueChanged
        {
            add => state.OnValueChanged += value;
            remove => state.OnValueChanged -= value;
        }

        public event System.Action<int, bool> OnMemoChanged
        {
            add => state.OnMemoChanged += value;
            remove => state.OnMemoChanged -= value;
        }

        // 操作メソッド
        public bool SetValue(int value) => state.SetValue(value);
        public bool ClearValue() => state.ClearValue();
        public bool AddMemo(int number) => state.AddMemo(number);
        public bool RemoveMemo(int number) => state.RemoveMemo(number);
        public void ToggleMemo(int number) => state.ToggleMemo(number);
        public void ClearAllMemos() => state.ClearAllMemos();

        /// <summary>正解判定</summary>
        public bool IsCorrect() => InputValue == AnswerValue;
    }
}
