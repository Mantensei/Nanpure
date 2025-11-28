using _NumberPlace.UI;
using MantenseiLib;
using Nanpure.Modules;
using Nanpure.Standard;
using Nanpure.Standard.Analyze;
using Nanpure.Standard.Core;
using Nanpure.Standard.Module;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR

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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                var board = FindAnyObjectByType<BoardManager>().BoardReference;
                AllMemoMagic.MemoAll(board);
            }


            if (Input.GetKeyDown(KeyCode.H))
            {
                var board = FindAnyObjectByType<BoardManager>().BoardReference;
                var list = new List<AnalyzedMove>();

                var operation = SolverResultAnalyzer.AnalyzeAsync
                    (
                        this,
                        board,
                        new[]
                        {
                                typeof(NakedSingleLogic),
                                typeof(HiddenSingleLogic),
                        }
                    );

                operation.Completed += (o) =>
                {
                    foreach (var r in o.Result)
                    {
                        if (r.TryGetNum(out var num))
                            r.Cell.State.SetNum(num);
                    }
                };
            }


            if (Input.GetKeyDown(KeyCode.J))
            {                
                var board = FindAnyObjectByType<BoardManager>().BoardReference;
                var list = new List<AnalyzedMove>();

                var operation = SolverResultAnalyzer.AnalyzeAsync
                    (
                        this,
                        board,
                        new[]
                        {
                                typeof(NakedSingleLogic),
                                typeof(HiddenSingleLogic),
                                typeof(NakedPairLogic),
                                typeof(HiddenPairLogic),      
                                typeof(NakedTripleLogic),     
                                typeof(PointingPairLogic),
                                typeof(BoxLineReductionLogic),

                                typeof(XWingLogic),           // 追加
                                typeof(XYWingLogic)           // 追加
                        }
                    );

                operation.Completed += (o) =>
                {
                    var result = o.Result.Where(x => x.CanPlace).FirstOrDefault();
                    if (result == null) return;

                    foreach(var relation in result.RelatedCells)
                    {
                        var img = relation.GetComponentInChildren<Image>();
                        img.color = Color.green;
                    }
                    Debug.Log(result.Techniques.JoinToString());
                };
            }


        }
    }
}
#endif
