using MantenseiLib;
using MantenseiLib.UI;
using System;
using UnityEngine;

namespace Nanpure.Standard.UI
{
	public class InputButtonHub : MonoBehaviour
	{
        [SerializeField] private NumInputButton _inputButtonPrefab;

		public NumInputButton[] Buttons { get; private set; }

		public event Action onAnyButtonClick;
        public event Action onAnyButtonOpen;
        public event Action onAnyButtonClose;
        void CreateButtons(int BoardSize)
        {
            Buttons = new NumInputButton[BoardSize];

            for (int i = 1; i <= BoardSize; i++)
            {
                var button = Instantiate(_inputButtonPrefab, transform);
                var swipeMenu = button.GetComponent<SwipeMenuParent>();
                swipeMenu.onClick += () => onAnyButtonClick?.Invoke();
                swipeMenu.onOpenMenu += () => onAnyButtonOpen?.Invoke();
                swipeMenu.onCloseMenu += () => onAnyButtonClose?.Invoke();
                button.SetNum(i);
            }
        }

        private void Start()
        {
            CreateButtons(9);
        }
    } 
}
