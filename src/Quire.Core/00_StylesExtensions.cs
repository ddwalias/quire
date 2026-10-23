using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Quire.Wpf;

public static partial class StylesExtensions
{
    // This filename places the lambda before generated members so its compiler ordinal matches Mathcad.
    public static object GetSetterValue(this Style style, DependencyProperty property)
    {
        return style.Setters.OfType<Setter>().LastOrDefault((Setter setter) => setter.Property == property)?.Value;
    }

    public static bool HasParagraphProps(this Style style)
    {
        return style.Setters.OfType<Setter>().Any((Setter setter) => setter.Property.OwnerType == typeof(Paragraph) || setter.Property.OwnerType == typeof(Block));
    }

    public static bool HasListProps(this Style style)
    {
        return style.Setters.OfType<Setter>().Any((Setter setter) => setter.Property.OwnerType == typeof(List));
    }
}
