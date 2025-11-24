using MantenseiLib;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Nanpure.Standard.Module
{
    public class CellStateManager : MonoBehaviour
    {
        [Parent] private Cell _cell;
        private CellMeta _meta => _cell.Data;

        public int DisplayNum { get; private set; } = -1;
        public bool IsEmpty => State == CellState.Empty;
        public bool IsCorrect => State == CellState.Correct || State == CellState.Revealed;
        public CellState State
        {
            get
            {
                if (_meta.IsRevealed)
                    return CellState.Revealed;
                else if (_meta.Num == DisplayNum)
                    return CellState.Correct;
                else if (DisplayNum <= 0)
                    return CellState.Empty;
                else
                    return CellState.Incorrect;
            }
        }

        private HashSet<int> _memo = new HashSet<int>();
        public bool HasMemo => _memo.Count > 0;
        public IReadOnlyCollection<int> Memo => _memo;

        public event Action<Cell> OnValueChanged;
        public event Action OnMemoChanged;

        public void Initialize()
        {
            if (!_meta.IsRevealed) return;

            ForcibilityInject(_meta.Num);
        }

        public bool SetNum(int num)
        {
            if (IsCorrect)
                return false;

            if (num == DisplayNum)
                return false;

            ForcibilityInject(num);
            return true;
        }

        void ForcibilityInject(int num)
        {
            DisplayNum = num;            
            OnValueChanged?.Invoke(_cell);
        }

        public bool MemoContains(int num)
        {
            return _memo.Contains(num);
        }

        public bool ToggleMemo(int num)
        {
            bool contains = MemoContains(num);
            return SetMemo(num, !contains);
        }

        public bool SetMemo(HashSet<int> memo)
        {
            if (_memo.SetEquals(memo))
            {
                return false;
            }

            _memo.Clear();
            _memo.UnionWith(memo);

            OnMemoChanged?.Invoke();
            return true;
        }

        public bool SetMemo(int num, bool add)
        {
            if (State != CellState.Empty)
                return false;

            bool contains = MemoContains(num);

            if (add)
            {
                if (contains)
                    return false;
                _memo.Add(num);
            }
            else
            {
                if (!contains)
                    return false;
                _memo.Remove(num);
            }

            OnMemoChanged?.Invoke();
            return true;
        }
    }

    public enum CellState
    {
        Revealed,
        Correct,
        Incorrect,
        Empty,
    }
}
