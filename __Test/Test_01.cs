using Nanpure.Standard.Generation;
using UnityEngine;
using MantenseiLib;
using System.Linq;

namespace Nanpure.Obsolete
{
    public class Test_01 : MonoBehaviour
    {
        void Start()
        {
            var g = new StandardPuzzleGenerator(3);
            var p = g.Generate(0, Standard.Core.Difficulty.Expert);

            var group = p.Cells
                .GroupBy(x => x.Row)
                .ToArray();

            foreach (var row in group)
                Debug.Log(row.JoinToString(x => x.Value));
        }
    }

}