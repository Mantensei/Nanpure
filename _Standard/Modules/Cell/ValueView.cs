using UnityEngine;
using TMPro;
using MantenseiLib;

namespace Nanpure.Standard.Cell
{
    public class ValueView : MonoBehaviour
    {
        [GetComponent(HierarchyRelation.Children)] private TextMeshProUGUI _text;
        [Parent] private Cell _cell;
        [Sibling] private CellState _state;

        private void Start()
        {
            _state.OnValueChanged += OnValueChanged;
            UpdateDisplay();
        }

        private void OnDestroy()
        {
            _state.OnValueChanged -= OnValueChanged;
        }

        private void OnValueChanged(int value)
        {
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (_text == null) return;

            if (_state.InputValue > 0)
            {
                _text.text = _state.InputValue.ToString();
                _text.gameObject.SetActive(true);
            }
            else
            {
                _text.gameObject.SetActive(false);
            }
        }
    }
}