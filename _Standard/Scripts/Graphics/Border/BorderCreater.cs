using MantenseiLib;
using Nanpure.Standard.Module;
using System.Linq;
using UnityEngine;

namespace Nanpure.Standard.Graphics
{
    public class BorderCreater : MonoBehaviour, IBoardEventReceiver
    {
        [GetComponent(HierarchyRelation.Self | HierarchyRelation.Parent)]
        CellBorderRenderer _renderer;

        [SerializeField] Sprite _sprite;

        public void OnBoardEvent(Board board, BoardEvent boardEvent)
        {
            for (var i = 0; i <= board.BoardSize; i++)
            {
                _renderer.RenderBorder(board.GetGroup(i), _sprite);
            }
            //_renderer.RenderBorder(board.Cells, _sprite);
            //_renderer.RenderBorder(board.Cells.Where(x => x.Address == new Vector2Int(2, 5)).ToArray(), _sprite);
            //_renderer.RenderBorder(board.GetGroup(0), _sprite);
            //_renderer.RenderBorder(board.Cells.Where(x => 
            //x.Address == new Vector2Int(0, 0) |
            //x.Address == new Vector2Int(0, 1) |
            //x.Address == new Vector2Int(1, 0)
            //).ToArray(), _sprite);
        }
    }

}