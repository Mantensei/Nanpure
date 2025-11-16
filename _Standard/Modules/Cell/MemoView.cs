using MantenseiLib;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Nanpure.Standard.Module
{
    public class MemoView : MonoBehaviour
    {
        [GetComponent(HierarchyRelation.Self | HierarchyRelation.Children)]
        private TextMeshProUGUI _text;
        [Parent] public Cell Cell { get; private set; }
        private CellStateManager _State => Cell.StateManager;

        private int _gridSize = 3; // 3×3グリッド（変更可能）

        private void Start()
        {
            _State.OnValueChanged += OnValueChanged;
            _State.OnMemoChanged += OnMemoChanged;
            UpdateDisplay();
        }

        private void OnDestroy()
        {
            _State.OnValueChanged -= OnValueChanged;
            _State.OnMemoChanged -= OnMemoChanged;
        }

        private void OnValueChanged(int value)
        {
            UpdateDisplay();
        }

        private void OnMemoChanged(int number, bool isAdded)
        {
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (_text == null) return;

            // 値が入力されている場合は透明にする
            if (_State.DisplayNum > 0)
            {
                _text.text = GenerateGridText();
                SetTextAlpha(0f);
                return;
            }

            // メモが空なら透明
            if (_State.Memo.Count == 0)
            {
                _text.text = GenerateGridText();
                SetTextAlpha(0f);
                return;
            }

            // メモ表示
            _text.text = GenerateGridText();
            SetTextAlpha(1f);
        }

        private string GenerateGridText()
        {
            int totalNumbers = _gridSize * _gridSize;
            var result = "";

            for (int i = 0; i < totalNumbers; i++)
            {
                int number = i + 1;
                bool hasMemo = _State.Memo.Contains(number);

                if (hasMemo)
                {
                    result += number.ToString();
                }
                else
                {
                    // メモがない数字は透明化
                    result += $"<color=#00000000>{number}</color>";
                }

                // グリッドサイズごとに改行
                if (number % _gridSize == 0)
                {
                    result += "\n";
                }
            }

            return result;
        }

        private void SetTextAlpha(float alpha)
        {
            Color color = _text.color;
            color.a = alpha;
            _text.color = color;
        }
    }
}