using UnityEngine;
using Nanpure.Standard.Module;

namespace Nanpure.Standard.Input
{
    public class InputHandler : MonoBehaviour
    {
        public Cell SelectedCell { get; private set; }

        public void SelectCell(Cell cell)
        {            
            SelectedCell = cell;
        }

        public void InputNumber(int number)
        {
            SelectedCell?.StateManager?.SetNum(number);
        }
    }
}