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
			if (cell.StateManager.IsCorrct)
			{
				CancelInputMode();
				foreach(var button in _hub.Buttons)
				{
					if(button.Num == cell.Value)
					{
						button.GetComponentInChildren<NumContinousInputButton>().BeginInputMode();
						return;
					}
				}
			}
			else
			{
				cell.StateManager.SetNum(Parent.Num);
			}
        }

		[Button]
		public void BeginInputMode()
		{
			InputMode = true;
			InputHandler.onCellSelected += SetNum;
			InputHandler.onCellSelected += Preserve;
			Preserve(null);

			_hub.onAnyButtonClick += CancelInputMode;
			_hub.onAnyButtonClose += CancelVisual;


			HighlightManager.UpdateHighlights();
        }

        public void CancelInputMode()
		{
			InputMode = false;
			InputHandler.onCellSelected -= SetNum;
			InputHandler.onCellSelected -= Preserve;

			_hub.onAnyButtonClick -= CancelInputMode;
			_hub.onAnyButtonClose -= CancelVisual;

			ChangeVisual(ColorSettings.White, ColorSettings.Dark);

			HighlightManager.ClearPreservedCells();
			HighlightManager.UpdateHighlights();
        }

		void Preserve(Cell cell)
		{
            HighlightManager.PreserveCell
			(
				HighlightManager.Board
				.GetSameCells(Parent.Num)
				.Where(x => x.StateManager.IsCorrct)
				.ToArray()
			);
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