using UnityEngine;
using Nanpure.Standard.Module;
using System;

namespace Nanpure.Standard.InputSystem
{
    public class InputHandler : MonoBehaviour
    {
        public Cell SelectedCell { get; private set; }
        public Action<Cell> onCellSelected;
        public Action<Cell> onCellUpdate;
        public Action<Cell> onCellHoverEnter;
        public Action<Cell> onCellHoverExit;

        public void OnCellHoverEnter(Cell cell)
        {
            onCellHoverEnter?.Invoke(cell);
        }

        public void OnCellHoverExit(Cell cell)
        {
            onCellHoverExit?.Invoke(cell);
        }

        public void OnCellClick(Cell cell)
        {            
            var tmp = SelectedCell;
            SelectedCell = cell;

            if(tmp != SelectedCell)
                onCellSelected?.Invoke(SelectedCell);
        }

        public void InputNumber(int number)
        {
            if(SelectedCell?.StateManager?.SetNum(number) == true)
            {
                onCellUpdate?.Invoke(SelectedCell);
            }
        }

        public void InputMemo(int number, bool isOn)
        {
            if(SelectedCell?.StateManager?.SetMemo(number, isOn) == true)
            {
                onCellUpdate?.Invoke(SelectedCell);
            }
        }

        public void ToggleMemo(int number)
        {
            if(SelectedCell?.StateManager?.ToggleMemo(number) == true)
            {
                onCellUpdate?.Invoke(SelectedCell);
            }
        }
    }
}