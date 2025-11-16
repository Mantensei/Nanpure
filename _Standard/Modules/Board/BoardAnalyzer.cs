using MantenseiLib;
using Nanpure.Standard.Module;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nanpure.Standard.Core
{
    public class BoardAnalyzer : MonoBehaviour
    {
        [Parent] IBoard board;
        Board Board => board.Board;

    }
}