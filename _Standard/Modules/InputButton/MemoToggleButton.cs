using MantenseiLib;
using MantenseiLib.UI;
using Nanpure.Standard.InputSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Nanpure.Standard.Module
{
    public class MemoToggleButton : MonoBehaviour
    {
        [SerializeField]
        Sprite _default;

        [SerializeField]
        Sprite _pressed;

        [GetComponent(HierarchyRelation.Self | HierarchyRelation.Children)]
        Image _image;

        [GetComponent(HierarchyRelation.Parent)]
        IGameStateProvider _provider;
        IGameStateEntity StateManager => _provider.GameStateReference;

        public bool MemoModeToggled { get; private set; } = false;

        public bool MemoMode
        {
            get => StateManager.MemoMode;
            set
            {
                if(MemoModeToggled)
                    StateManager.MemoMode = true;
                else
                    StateManager.MemoMode = value;
                _image.sprite = MemoMode ? _pressed : _default;
            }
        }

        [Button]
        void ToggleMemoMode()
        {
            MemoModeToggled = !MemoModeToggled;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ToggleMemoMode();
            }

            // 👇 Shift押下時は一時的にメモモードON
            // NumLockがOffだとShiftが暴発する！！！！！！１１１１１
            bool isShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            MemoMode = isShift ? true : MemoModeToggled;
        }
    }
}
