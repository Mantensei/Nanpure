using System.Collections.Generic;
using UnityEngine;
using MantenseiLib;
using System;

namespace Nanpure.Standard.Module
{
    public partial class Cell : MonoBehaviour
    {
        [GetComponent(HierarchyRelation.Parent)]
        Root a;
        [GetComponent(HierarchyRelation.Parent)]
        public IBoard _board;
        public Board Board => _board.Board;

        [GetComponent(HierarchyRelation.Children)] 
        public CellMeta Data { get; private set; }
        [GetComponent(HierarchyRelation.Children)] 
        public CellStateManager State { get; private set; }
        [GetComponent(HierarchyRelation.Children)] 
        public IVisual Visualizer { get; private set; }
    }

    public partial class Cell
    {
        public int Value => Data.Num;
        public int Row => Data.Row;
        public int Column => Data.Column;
        public int Group => Data.Group;

        public Vector2Int Address => new Vector2Int(Data.Row, Data.Column);
    }
}
