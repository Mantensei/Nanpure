using UnityEngine;
using UnityEngine.UI;
using Nanpure.Standard.Input;
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

        public void SetNum(int num) 
        {
            _number = num; 
            buttonText.text = num.ToString();
        }

        private void Start()
        {
            button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            var inputHandler = FindFirstObjectByType<InputHandler>();
            inputHandler.InputNumber(_number);
        }
    }
}