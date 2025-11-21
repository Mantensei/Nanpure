using UnityEngine;
using UnityEngine.UI;
using Nanpure.Standard.InputSystem;
using MantenseiLib;
using MantenseiLib.UI;
using TMPro;

namespace Nanpure.Standard.UI
{
    public class NumInputButton : MonoBehaviour
    {
        [SerializeField] private int _number = 1;
        public int Num => _number;

        [GetComponent(HierarchyRelation.Self | HierarchyRelation.Children)]
        public TextMeshProUGUI Text { get; private set; }

        [GetComponent]
        public Image Image { get; private set; }

        [Parent] Root root;

        [Sibling] public InputHandler InputHandler { get; private set; }

        public void SetNum(int num) 
        {
            _number = num; 
            Text.text = num.ToString();
        }

        [Button]
        private void OnClick()
        {
            InputHandler.InputNumber(_number);
        }
    }
}