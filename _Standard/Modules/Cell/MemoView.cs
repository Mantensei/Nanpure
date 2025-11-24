using MantenseiLib;
using Nanpure.Standard.Core;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

namespace Nanpure.Standard.Module
{
    public class MemoView : MonoBehaviour
    {
        [GetComponent(HierarchyRelation.Self | HierarchyRelation.Children)]
        private TextMeshProUGUI _text;
        [Parent] public Cell Cell { get; private set; }
        private CellStateManager _state => Cell.State;

        private int _RowCount = 3;
        private int GridSize => _RowCount * _RowCount;

        HighlightManager _highlightManager;
        HighlightManager HighlightManager => _highlightManager ??= FindAnyObjectByType<HighlightManager>();
        int _highlightNum = -1;

        private void Start()
        {
            _state.OnValueChanged += OnValueChanged;
            _state.OnMemoChanged += OnMemoChanged;
            HighlightManager.OnPreserve += HighlightMemo;
            UpdateDisplay();
        }

        private void OnDestroy()
        {
            _state.OnValueChanged -= OnValueChanged;
            _state.OnMemoChanged -= OnMemoChanged;
        }

        public void HighlightMemo(int num)
        {
            _highlightNum = num;
            UpdateDisplay();
        }

        static void RemoveRelatedMemo(Cell cell)
        {
            if (cell?.State?.IsCorrect == true)
            {
                foreach (var relation in cell.Board.GetRelatedCells(cell))
                {
                    relation.State.SetMemo(cell.Value, false);
                }
            }
        }

        private void OnValueChanged(Cell cell)
        {
            UpdateDisplay();
            RemoveRelatedMemo(cell);
        }

        private void OnMemoChanged()
        {
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (_state.State != CellState.Empty || _state.Memo.Count == 0)
            {
                SetTextAlpha(0f);
                return;
            }

            _text.text = GenerateGridText();
            SetTextAlpha(1f);
        }

        public string ToBold(string str)
        {
            return $"<b>{WrapWithColorTag(str, new Color(0.8f, 0.2f, 0.9f))}</b>";
            //return $"<b>{WrapWithColorTag(str, HighlightManager.SameNumberColor)}</b>";
        }

        public string WrapWithColorTag(string str, Color color)
        {
            string colorTag = ColorUtility.ToHtmlStringRGBA(color);
            return $"<color=#{colorTag}>{str}</color>";
        }

        private string GenerateGridText()
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < GridSize; i++)
            {
                int number = i + 1;
                bool hasMemo = _state.Memo.Contains(number);
                string s = number.ToString();

                if (hasMemo)
                {
                    if (number == _highlightNum)
                        s = ToBold(s);
                }
                else
                {
                    s = WrapWithColorTag(s, Color.clear);
                }
                result.Append(s);

                if (number % _RowCount == 0)
                {
                    result.Append("\n");
                }
            }

            return result.ToString();
        }

        private void SetTextAlpha(float alpha)
        {
            Color color = _text.color;
            color.a = alpha;
            _text.color = color;
        }
    }
}