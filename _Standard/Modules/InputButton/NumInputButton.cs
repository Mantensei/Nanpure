using UnityEngine;
using UnityEngine.UI;
using Nanpure.Standard.InputSystem;
using MantenseiLib;
using MantenseiLib.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;
using Nanpure.Standard.Logic;

namespace Nanpure.Standard.UI
{
    public class NumInputButton : MonoBehaviour
    {
        [GetComponent(HierarchyRelation.Parent)]
        IGameStateProvider _gameStateProvider;
        GameStateManager GameStateManager => _gameStateProvider.GameStateReference as GameStateManager;

        [SerializeField] private int _number = 1;
        public int Num => _number;

        [GetComponent(HierarchyRelation.Self | HierarchyRelation.Children)]
        public TextMeshProUGUI Text { get; private set; }

        [GetComponent]
        public Image Image { get; private set; }

        public event Action<PointerEventData> onClick;

        [GetComponent(HierarchyRelation.Parent)]
        IInputHandlerProvider _inputProvider;
        public IInputHandlerEntity InputHandler => _inputProvider.InputHandlerReference;


        [GetComponent(HierarchyRelation.Parent)]
        IGameStateProvider _stateProvider;
        public IGameStateEntity StateManager => _stateProvider.GameStateReference;

        [GetComponent(HierarchyRelation.Parent)]
        IBoardMonitor _monitor;
        BoardMonitor BoardMonitor => _monitor.BoardMonitorReference;

        static readonly float normalSize = 64f;
        static readonly float memoSize = 24f;

        public bool IsActive { get; private set; } = true;
        public event Action<bool> onActiveChanged;
        public void Activate(bool value = true)
        {
            IsActive = value;
            Image.enabled = value;
            Text.enabled = value;
            onActiveChanged?.Invoke(value);
        }

        private void Start()
        {
            StateManager.OnMemoChanged += SetTextSize;
            BoardMonitor.onUsedUp += UsedUp;
        }

        void UsedUp(int num)
        {
            if(num == Num)
            {
                Activate(false);
            }
        }

        void SetTextSize(bool memoMode)
        {
            Text.fontSize = memoMode ? memoSize : normalSize;
        }

        public void Initialize(int num) 
        {
            _number = num; 
            Text.text = num.ToString();
        }

        [Button]
        private void OnClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                InputNum();

            onClick?.Invoke(eventData);
        }

        private void Update()
        {
            if (Input.inputString.Length > 0)
            {
                if (int.TryParse(Input.inputString, out int pressedNum))
                {
                    if (pressedNum == Num)
                    {
                        InputNum();
                    }
                }
            }
        }

        public void InputNum()
        {
            if (GameStateManager.MemoMode)
            {
                InputMemo();
            }
            else
            {
                InputHandler.InputNumber(_number);
            }
        }

        public void InputMemo()
        {
            InputHandler.ToggleMemo(_number);
        }
    }
}