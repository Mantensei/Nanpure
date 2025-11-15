using UnityEngine;
using TMPro;
using MantenseiLib;

namespace Nanpure.Standard.Module
{
    public class MemoView : MonoBehaviour
    {
        [GetComponent(HierarchyRelation.Self | HierarchyRelation.Children)] 
        private TextMeshProUGUI _text;
        [Parent] public Cell Cell { get; private set; }
        private CellState _state => Cell.State;

        private void Start()
        {
            _state.OnValueChanged += OnValueChanged;
            _state.OnMemoChanged += OnMemoChanged;
            UpdateDisplay();
        }

        private void OnDestroy()
        {
            _state.OnValueChanged -= OnValueChanged;
            _state.OnMemoChanged -= OnMemoChanged;
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

            // 値が入力されている、またはメモが空なら非表示
            if (_state.InputValue > 0 || _state.Memos.Count == 0)
            {
                _text.gameObject.SetActive(false);
                return;
            }

            _text.text = string.Join(" ", _state.Memos);
            _text.gameObject.SetActive(true);
        }
    }
}