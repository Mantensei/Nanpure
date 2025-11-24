using MantenseiLib;
using MantenseiLib.UI;
using Nanpure.Standard.Core;
using Nanpure.Standard.InputSystem;
using Nanpure.Standard.Module;
using System.Linq;
using UnityEngine;

namespace Nanpure.Standard.UI
{
	public class NumContinousInputButton : MonoBehaviour
	{
		[GetComponent(HierarchyRelation.Parent)]
		public InputButtonHub _hub { get; private set; }

        [Parent] public NumInputButton Parent { get; private set; }
		public bool InputMode { get; private set; } = false;

		InputHandler _inputHandler;
		InputHandler InputHandler => _inputHandler ??= FindAnyObjectByType<InputHandler>();
		ColorSettings _colorSettings;
		ColorSettings ColorSettings => _colorSettings ??= FindAnyObjectByType<ColorSettings>();
		HighlightManager _highlightManager;
		HighlightManager HighlightManager => _highlightManager ??= FindAnyObjectByType<HighlightManager>();

		void SetNum(Cell cell)
		{
            if (cell == null)
            {
                CancelInputMode(); 
				return;
            }

            if (cell.State.IsCorrect)
			{
				CancelInputMode();
				return;
				//foreach(var button in _hub.Buttons)
				//{
				//	if (button == null) continue;

				//	if(button.Num == cell.Value)
				//	{
				//		button.GetComponentInChildren<NumContinousInputButton>().BeginInputMode();
				//		return;
				//	}
				//}
			}
			else
			{
				//cell.State.SetNum(Parent.Num);

				if (Input.GetKey(KeyCode.Mouse1))
					Parent.InputMemo();
				else
					Parent.InputNum();
			}
        }

        private void Start()
        {
			Parent.onClick += (data) =>
			{
				if (data.button == UnityEngine.EventSystems.PointerEventData.InputButton.Right)
					BeginInputMode();
			};

			Parent.onActiveChanged += (b) =>
			{
				if (b == false)
					CancelInputMode();
			};
        }

        [Button]
		public void BeginInputMode()
		{
			InputMode = true;
			InputHandler.onCellSelected += SetNum;

			_hub.onAnyButtonClick += CancelInputMode;
			_hub.onAnyButtonClose += CancelVisual;

			HighlightManager.Preserve(Parent.Num);
        }

        public void CancelInputMode()
		{
			InputMode = false;
			InputHandler.onCellSelected -= SetNum;

			_hub.onAnyButtonClick -= CancelInputMode;
			_hub.onAnyButtonClose -= CancelVisual;

			HighlightManager.ClearPreserve();
			ChangeVisual(ColorSettings.White, ColorSettings.Dark);
        }

		void CancelVisual()
		{
            ChangeVisual(ColorSettings.LightDark, ColorSettings.White);
        }

		void ChangeVisual(Color backColor, Color textColor)
		{
			Parent.Image.color = backColor;
			Parent.Text.color = textColor;
        }
    }

}