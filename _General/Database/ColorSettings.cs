using MantenseiLib.UI;
using UnityEngine;

namespace Nanpure.Standard.Core
{
    public class ColorSettings : MonoBehaviour
    {
        [SerializeField] private Color _dark = new Color(0.9f, 0.9f, 0.9f);
        [SerializeField] private Color _lightDark = new Color(0.95f, 0.95f, 0.95f);
        [SerializeField] private Color _darkWhite = new Color(0.8f, 0.9f, 1f);
        [SerializeField] private Color _white = new Color(0.7f, 0.85f, 1f);
        [SerializeField] private Color _systemWhite = Color.white;

        public Color Dark => _dark;
        public Color LightDark => _lightDark;
        public Color DarkWhite => _darkWhite;
        public Color White => _white;
        public Color SystemWhite => _systemWhite;
    }
}
