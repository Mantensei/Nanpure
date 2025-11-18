using UnityEngine;
using UnityEngine.UI;
using Nanpure.Standard.InputSystem;
using MantenseiLib;
using TMPro;

namespace Nanpure.Standard.Module
{
    public class InputButton : MonoBehaviour
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

        public void SetNum(int num) 
        {
            _number = num; 
            buttonText.text = num.ToString();
        }

        private void OnClick()
        {
            inputHandler.InputNumber(_number);
        }
    }
}