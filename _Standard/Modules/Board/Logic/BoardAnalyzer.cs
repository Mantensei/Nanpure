using MantenseiLib;
using Nanpure.Standard.Module;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nanpure.Standard.Core
{
    public interface IAnalyzerProvider : IMonoBehaviour 
    {
        IAnalyzerProvider AnalyzerReference { get; }
    }

    public class BoardAnalyzer : MonoBehaviour, IAnalyzerProvider
    {
        [Parent] Root root;
        [Sibling] IBoardProvider board;
        Board Board => board.BoardReference;
        public IAnalyzerProvider AnalyzerReference => this;
    }
}