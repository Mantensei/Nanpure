using MantenseiLib;
using UnityEngine;

namespace Nanpure.Standard.Module
{
    /// <summary>セルの固定データ（パズル生成時に確定）</summary>
    public class CellMeta : MonoBehaviour
    {
        [Parent]
        public Cell Cell { get; private set; }

        public int AnswerValue { get; private set; }
        public int Row { get; private set; }
        public int Column { get; private set; }
        public bool IsFixed { get; private set; }

        /// <summary>初期化</summary>
        public void Initialize(int row, int column, int answerValue, bool isFixed)
        {
            Row = row;
            Column = column;
            AnswerValue = answerValue;
            IsFixed = isFixed;
        }
    }
}
