#nullable disable
using System.Windows;
using System.Windows.Media;
using Quire.Mirror;

namespace Quire;

// This filename keeps the TextStyleBase member ordinals aligned with Mathcad's closures.
public partial class TextStyleBase
{
    [MirrorVerified("Release IL")]
    protected void UpdateMemberValue(string propertyName, object value)
    {
        switch (propertyName)
        {
            case "FontWeight":
                _fontWeight = (FontWeight?)value;
                break;
            case "FontFamily":
                _fontFamily = (FontFamily)value;
                break;
            case "FontSize":
                _fontSize = (double?)value;
                break;
            case "FontStyle":
                _fontStyle = (FontStyle?)value;
                break;
            case "TextEffectCollection":
                _textEffectCollection = (TextEffectCollection)value;
                break;
            case "TextDecorations":
                _textDecorationCollection = (TextDecorationCollection)value;
                break;
            case "BaselineAlignment":
                _baselineAlignment = (BaselineAlignment)value;
                break;
            case "TextAlignment":
                _textAlignment = (TextAlignment?)value;
                break;
            case "Background":
                _background = (Brush)value;
                break;
            case "Foreground":
                _foreground = (Brush)value;
                break;
            case "MarkerStyle":
                _markerStyle = value as TextMarkerStyle?;
                break;
            case "TextIndent":
                _textIndent = (double?)value;
                break;
            case "Variants":
                _fontVariants = (FontVariants?)value;
                break;
            case "Margin":
                _margin = (Thickness?)value;
                break;
        }
    }
}
