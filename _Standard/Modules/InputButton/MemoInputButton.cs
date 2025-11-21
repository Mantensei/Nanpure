using MantenseiLib;
using MantenseiLib.UI;
using Nanpure.Standard.InputSystem;
using UnityEngine;

namespace Nanpure.Standard.UI
{
    public class MemoInputButton : MonoBehaviour
    {
        [GetComponent(HierarchyRelation.Parent)]
        public NumInputButton numInputButton { get; private set; }


        [Button]
        private void OnClick()
        {
            numInputButton.InputHandler.ToggleMemo(numInputButton.Num);
        }
    } 
}
