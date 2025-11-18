using UnityEngine;
using UnityEngine.UI;
using MantenseiLib;

namespace Nanpure.Standard.Module
{
    public interface IVisual
    {
        public void SetVisual(object data);
    }

    public class CellVisualizer : MonoBehaviour, IVisual
    {
        [Parent] public Cell Cell { get; private set; }
        private CellMeta _meta => Cell.Data;

        [Sibling] 
        private Image _background;

        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _fixedColor = new Color(0.9f, 0.9f, 0.9f);

        public void SetBackgroundColor(Color color)
        {
            _background.color = color;
        }

        public void SetVisual(object data)
        {
            if(data is Color color)
            {
                SetBackgroundColor(color);
            }
        }
    }
}