using Nanpure.Standard;
using UnityEngine;
using MantenseiLib;
using System.Linq;

namespace Nanpure.Obsolete
{
    public class Test_01 : MonoBehaviour
    {
        private void Start()
        {
            //var a = new StandardPuzzleGenerator().Generate(Standard.Core.Difficulty.Expert);
            //Debug.Log(a.Cells.JoinToString(x => x.Value));
            var board = FindAnyObjectByType<Standard.Module.BoardManager>();
            board.Initialize();

            //FindAnyObjectByType<Standard.Module.BoardManager>()
            //    .Initialize(9);
        }

        //void Start()
        //{
        //    var g = new StandardPuzzleGenerator(3);
        //    var p = g.Generate(0, Standard.Core.Difficulty.Expert);

        //    var group = p.Cells
        //        .GroupBy(x => x.Row)
        //        .ToArray();

        //    foreach (var row in group)
        //        Debug.Log(row.JoinToString(x => x.Value));
        //}
    }

}