using Nanpure.Standard.Module;
using System.Collections;
using UnityEngine;

namespace MantenseiLib
{
	public class CommonReferenceHub : MonoBehaviour, IBoard
	{
		[GetComponent(HierarchyRelation.Children)]
		IBoard _board;

		public Board Board => _board.Board;
    }

}