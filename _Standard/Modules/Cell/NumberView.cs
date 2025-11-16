using UnityEngine;
using TMPro;
using MantenseiLib;

namespace Nanpure.Standard.Module
{
    public class NumberView : MonoBehaviour
    {
        [GetComponent(HierarchyRelation.Self | HierarchyRelation.Children)] 
        private TextMeshProUGUI _text;
        [Parent] public Cell Cell { get; private set; }
        private CellStateManager _state => Cell.StateManager;

        Color Empty = new Color(1f, 1f, 1f, 0f);
        Color Revealed = new Color(0f, 0f, 0f, 1f);
        Color Correct = new Color(0f, 1f, 0f, 1f);
        Color Incorrect = new Color(1f, 0f, 0f, 1f);

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
            int displayValue = _state.DisplayNum;
            _text.text = displayValue.ToString();

            switch(_state.State)
            {
                case CellState.Empty:
                    _text.color = Empty;
                    break;
                case CellState.Revealed:
                    _text.color = Revealed;
                    break;
                case CellState.Correct:
                    _text.color = Correct;
                    break;
                case CellState.Incorrect:
                    _text.color = Incorrect;
                    break;
            }
        }
    }
}