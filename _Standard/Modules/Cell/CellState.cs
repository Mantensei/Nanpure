using System;
using System.Collections.Generic;
using UnityEngine;
using MantenseiLib;

namespace Nanpure.Standard.Cell
{
    /// <summary>セルの動的データ（プレイ中に変化）</summary>
    public class CellState : MonoBehaviour
    {
        [Parent] private Cell _cell;
        [Sibling] private CellMeta _meta;

        public int InputValue { get; private set; }
        private HashSet<int> _memos = new HashSet<int>();
        public IReadOnlyCollection<int> Memos => _memos;

        public event Action<int> OnValueChanged;
        public event Action<int, bool> OnMemoChanged;

        /// <summary>値を設定</summary>
        public bool SetValue(int value)
        {
            if (_meta.IsFixed) return false;
            if (InputValue == value) return false;

            InputValue = value;
            OnValueChanged?.Invoke(value);
            return true;
        }

        /// <summary>値をクリア</summary>
        public bool ClearValue()
        {
            return SetValue(0);
        }

        /// <summary>メモを追加</summary>
        public bool AddMemo(int number)
        {
            if (_meta.IsFixed) return false;
            if (number < 1 || number > 9) return false;
            if (_memos.Contains(number)) return false;

            _memos.Add(number);
            OnMemoChanged?.Invoke(number, true);
            return true;
        }

        /// <summary>メモを削除</summary>
        public bool RemoveMemo(int number)
        {
            if (!_memos.Contains(number)) return false;

            _memos.Remove(number);
            OnMemoChanged?.Invoke(number, false);
            return true;
        }

        /// <summary>メモをトグル</summary>
        public void ToggleMemo(int number)
        {
            if (_memos.Contains(number))
                RemoveMemo(number);
            else
                AddMemo(number);
        }

        /// <summary>全メモをクリア</summary>
        public void ClearAllMemos()
        {
            var numbers = new List<int>(_memos);
            foreach (var num in numbers)
            {
                RemoveMemo(num);
            }
        }
    }
}
