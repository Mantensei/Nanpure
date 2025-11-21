using MantenseiLib;
using Nanpure.Standard.InputSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Nanpure.Standard.Module
{
    public class MemoToggleButton : MonoBehaviour
    {
        [SerializeField] private int _number = 1;

        [GetComponent(HierarchyRelation.Self | HierarchyRelation.Children)]
        Button button;

        [GetComponent(HierarchyRelation.Self | HierarchyRelation.Children)]
        TextMeshProUGUI buttonText;

        [Parent] Root root;

        [Sibling] InputHandler inputHandler;

        private void Start()
        {
            button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            inputHandler.InputNumber(_number);
        }
    }
}
