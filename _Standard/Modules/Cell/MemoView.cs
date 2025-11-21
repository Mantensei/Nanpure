using MantenseiLib;
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
            if (_State.State != CellState.Empty || _State.Memo.Count == 0)
            {
                SetTextAlpha(0f);
                return;
            }

            _text.text = GenerateGridText();
            SetTextAlpha(1f);
        }

        public string GetColorTagStart(Color color)
        {
            string colorTag = ColorUtility.ToHtmlStringRGBA(color);
            return $"<color=#{colorTag}>";
        }

        private string GenerateGridText()
        {
            int totalNumbers = _gridSize * _gridSize;
            StringBuilder result = new StringBuilder();
            //var colorTagStart = GetColorTagStart(Color.black);
            var alphaTagStart = GetColorTagStart(Color.clear);
            var colurTagEnd = "</color>";

            for (int i = 0; i < totalNumbers; i++)
            {
                int number = i + 1;
                bool hasMemo = _State.Memo.Contains(number);

                if (hasMemo)
                {
                    result.Append(number.ToString());
                }
                else
                {
                    result.Append($"{alphaTagStart}{number}{colurTagEnd}");
                }

                if (number % _gridSize == 0)
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