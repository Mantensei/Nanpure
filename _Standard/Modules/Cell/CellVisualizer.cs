using UnityEngine;
using UnityEngine.UI;
using MantenseiLib;

namespace Nanpure.Standard.Cell
{
    /// <summary>セル表示の総合マネージャー（現在は背景色のみ制御）</summary>
    public class CellVisualizer : MonoBehaviour
    {
        [SerializeField] private Image _background;
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _fixedColor = new Color(0.9f, 0.9f, 0.9f);

        [Sibling] private CellMeta _meta;

        private void Start()
        {
            UpdateBackground();
        }

        private void UpdateBackground()
        {
            if (_background == null) return;

            _background.color = _meta.IsFixed ? _fixedColor : _normalColor;
        }
    }
}