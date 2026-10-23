using System.Linq;
using System.Collections.Generic;
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

    public static IEnumerable<DependencyProperty> GetPropertiesForAnyType(this Style style)
    {
        return (from setter in style.Setters.OfType<Setter>()
            select setter.Property).Distinct();
    }

    public static IEnumerable<DependencyProperty> GetPropertiesForType<OwnerT>(this Style style)
    {
        return (from property in style.GetPropertiesForAnyType()
            where property.OwnerType == typeof(OwnerT)
            select property).Distinct();
    }
}
