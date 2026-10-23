using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Quire.Wpf;

public static partial class StylesExtensions
{
    // This filename places the lambda before generated members so its compiler ordinal matches Mathcad.
    public static object GetSetterValue(this Style style, DependencyProperty property)
    {
        return style.Setters.OfType<Setter>().LastOrDefault((Setter setter) => setter.Property == property)?.Value;
    }
}
