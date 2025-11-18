using UnityEngine;
using UnityEngine.EventSystems;
using MantenseiLib;
using UnityEngine.UI;
using Nanpure.Standard.InputSystem;

namespace Nanpure.Standard.Module
{
    public class CellClickHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        [Parent] public Cell Cell { get; private set; }

        [GetComponent(HierarchyRelation.Parent)]
        InputHandler _inputHandler;
        InputHandler InputHandler
        {
            get
            {
                if (!_inputHandler.Safe())
                     _inputHandler = FindAnyObjectByType<InputHandler>();

                return _inputHandler;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(Input.GetKey(KeyCode.Mouse0))
                InputHandler.OnCellClick(Cell);
            else
                InputHandler.OnCellHoverEnter(Cell);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            InputHandler.OnCellHoverExit(Cell);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            InputHandler.OnCellClick(Cell);
        }
    }
}