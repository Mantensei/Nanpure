using UnityEngine;
using UnityEngine.EventSystems;
using MantenseiLib;
using UnityEngine.UI;

namespace Nanpure.Standard.Module
{
    public class CellClickHandler : MonoBehaviour
    {
        [Parent] public Cell Cell { get; private set; }
        [GetComponent(HierarchyRelation.Self | HierarchyRelation.Children)]
        Button button;

        private void Start()
        {
            button.onClick.AddListener(OnClick);
        }

        public void OnClick()
        {
            var inputHandler = FindAnyObjectByType<Input.InputHandler>();
            if (inputHandler != null)
            {
                inputHandler.SelectCell(Cell);
            }
        }
    }
}