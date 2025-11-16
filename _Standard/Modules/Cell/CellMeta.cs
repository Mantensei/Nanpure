using MantenseiLib;
using UnityEngine;
using System;

namespace Nanpure.Standard.Module
{
    public class CellMeta : MonoBehaviour
    {
        [Parent]
        public Cell Cell { get; private set; }

        public int Num { get; private set; }
        public int Row { get; private set; }
        public int Column { get; private set; }
        public bool IsRevealed { get; private set; }

        public void Initialize(int row, int column, int value, bool isRevealed)
        {
            Row = row;
            Column = column;
            Num = value;
            IsRevealed = isRevealed;
        }
    }
}
