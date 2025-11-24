using UnityEngine;
using Nanpure.Standard.Module;
using System;

namespace Nanpure.Standard.InputSystem
{
    public interface IInputHandlerProvider
    {
        IInputHandlerEntity InputHandlerReference {  get; }
    }

    public interface IInputHandlerEntity
    {
        void InputNumber(int number);
        void InputMemo(int number, bool value);
        void ToggleMemo(int number);
    }

    public interface IInputActionHandlerProvider
    {
        IInputActionHandlerEntity InputActionHandlerReference { get; }
    }

    public interface IInputActionHandlerEntity
    {
        Cell SelectedCell { get; }

        event Action<Cell> onCellSelected;
        event Action<Cell> onCellUpdate;
        event Action<Cell> onCellHoverEnter;
        event Action<Cell> onCellHoverExit;
    }

    public class InputHandler : MonoBehaviour, 
        IInputHandlerEntity, IInputHandlerProvider,
        IInputActionHandlerEntity, IInputActionHandlerProvider
    {
        public Cell SelectedCell { get; private set; }

        public IInputHandlerEntity InputHandlerReference => this;

        public IInputActionHandlerEntity InputActionHandlerReference => this;

        public event Action<Cell> onCellSelected;
        public event Action<Cell> onCellUpdate;
        public event Action<Cell> onCellHoverEnter;
        public event Action<Cell> onCellHoverExit;

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

            //if(tmp != SelectedCell)
                onCellSelected?.Invoke(SelectedCell);
        }

        public void InputNumber(int number)
        {
            if(SelectedCell?.State?.SetNum(number) == true)
            {
                onCellUpdate?.Invoke(SelectedCell);
            }
        }

        public void InputMemo(int number, bool isOn)
        {
            if(SelectedCell?.State?.SetMemo(number, isOn) == true)
            {
                onCellUpdate?.Invoke(SelectedCell);
            }
        }   

        public void ToggleMemo(int number)
        {
            if(SelectedCell?.State?.ToggleMemo(number) == true)
            {
                onCellUpdate?.Invoke(SelectedCell);
            }
        }
    }
}