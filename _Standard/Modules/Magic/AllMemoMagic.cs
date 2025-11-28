using MantenseiLib;
using MantenseiLib.UI;
using Nanpure.Standard.Module;
using System.Linq;
using UnityEngine;

namespace Nanpure.Modules
{
	public class AllMemoMagic : MonoBehaviour
	{
		[GetComponent(HierarchyRelation.Parent)]
		CommonReferenceHub _referenceHub;

		public Board Board => _referenceHub.BoardReference;

		public static void MemoAll(Board board)
		{
			foreach (var cell in board.Cells)
			{
				var state = cell.State;
				if (!state.IsEmpty) continue;
				if (state.HasMemo) continue;

				var relatedCell = board.GetRelatedCells(cell).Where(x => x.State.IsCorrect);
				var hash = board.HashSet;
				var complement = relatedCell.Select(x => x.Value).Distinct();
				var candidate = hash.Except(complement).ToHashSet();

				cell.State.SetMemo(candidate);
			}
		}

		[Button]
		void MemoAll() => MemoAll(Board);
    } 
}
