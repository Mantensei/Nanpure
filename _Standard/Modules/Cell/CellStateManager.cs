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
        public bool IsCorrct => State == CellState.Correct || State == CellState.Revealed;
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

        public bool IsCorrect => DisplayNum == _meta.Num;
        private HashSet<int> _memo = new HashSet<int>();
        public IReadOnlyCollection<int> Memo => _memo;

        public event Action<int> OnValueChanged;
        public event Action<int, bool> OnMemoChanged;

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
            OnValueChanged?.Invoke(num);
        }

        public bool AddMemo(int number)
        {
            if (_meta.IsRevealed) return false;
            if (number < 1 || number > 9) return false;
            if (_memo.Contains(number)) return false;

            _memo.Add(number);
            OnMemoChanged?.Invoke(number, true);
            return true;
        }

        public bool RemoveMemo(int number)
        {
            if (!_memo.Contains(number)) return false;

            _memo.Remove(number);
            OnMemoChanged?.Invoke(number, false);
            return true;
        }

        public void ToggleMemo(int number)
        {
            if (_memo.Contains(number))
                RemoveMemo(number);
            else
                AddMemo(number);
        }

        public void ClearAllMemos()
        {
            var numbers = new List<int>(_memo);
            foreach (var num in numbers)
            {
                RemoveMemo(num);
            }
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
