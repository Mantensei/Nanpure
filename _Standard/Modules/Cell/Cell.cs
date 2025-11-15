using System.Collections.Generic;
using UnityEngine;
using MantenseiLib;
using System;

namespace Nanpure.Standard.Module
{
    /// <summary>セル全体を統括（参照ホルダー）</summary>
    public class Cell : MonoBehaviour
    {
        [GetComponent(HierarchyRelation.Children)] public CellMeta Meta { get; private set; }
        [GetComponent(HierarchyRelation.Children)] public CellState State { get; private set; }
    }
}
