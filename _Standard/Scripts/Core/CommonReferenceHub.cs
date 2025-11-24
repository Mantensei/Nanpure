using Nanpure.Standard;
using Nanpure.Standard.Core;
using Nanpure.Standard.InputSystem;
using Nanpure.Standard.Logic;
using Nanpure.Standard.Module;
using System.Collections;
using UnityEngine;

namespace MantenseiLib
{
    public class CommonReferenceHub : MonoBehaviour,
        IBoardProvider, IAnalyzerProvider, IGameStateProvider,
		IInputHandlerProvider, IInputActionHandlerProvider,
        IBoardMonitor
		
	{
		[GetComponent(HierarchyRelation.Children)]
		IBoardProvider _board;

		public Board BoardReference => _board.BoardReference;

		[GetComponent(HierarchyRelation.Children)]
		public IAnalyzerProvider AnalyzerReference { get; private set; }

		[GetComponent(HierarchyRelation.Children)]
		IGameStateProvider _gameStateProvider;
		public IGameStateEntity GameStateReference => _gameStateProvider.GameStateReference;

        [GetComponent(HierarchyRelation.Children)]
		IInputHandlerProvider _inputHandlerProvider;
        public IInputHandlerEntity InputHandlerReference => _inputHandlerProvider.InputHandlerReference;

		[GetComponent(HierarchyRelation.Children)]
		IInputActionHandlerProvider _inputActionHandlerProvider;
		public IInputActionHandlerEntity InputActionHandlerReference => _inputActionHandlerProvider.InputActionHandlerReference;

		[GetComponent(HierarchyRelation.Children)]
        public BoardMonitor BoardMonitorReference { get; private set; }
    }

}