using MantenseiLib;
using UnityEngine;
using System;
using Nanpure.Standard.Core;

namespace Nanpure.Standard.Module
{
    public class CellMeta : MonoBehaviour
    {
        [Parent]
        public Cell Cell { get; private set; }

        public int Num { get; private set; }
        public int Row { get; private set; }
        public int Column { get; private set; }
        public int Group { get; private set; }
        public bool IsRevealed { get; private set; }

        public void Initialize(CellData cellData)
        {
            Num = cellData.Value;
            Row = cellData.Row;
            Column = cellData.Column;
            Group = cellData.Group;
            IsRevealed = cellData.IsRevealed;
        }
    }
}
