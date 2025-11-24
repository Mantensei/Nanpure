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

		[Button]
		public void MemoAll()
		{
			foreach (var cell in Board.Cells)
			{
				var state = cell.State;
				if (!state.IsEmpty) continue;
				if (state.HasMemo) continue;

				var relatedCell = Board.GetRelatedCells(cell).Where(x => x.State.IsCorrect);
				var hash = Board.HashSet;
				var complement = relatedCell.Select(x => x.Value).Distinct();
				var candidate = hash.Except(complement).ToHashSet();

				cell.State.SetMemo(candidate);
			}
		}
	} 
}
