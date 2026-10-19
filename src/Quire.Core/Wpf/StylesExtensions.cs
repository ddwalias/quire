#nullable disable
using System.Linq;
using System.Windows;
using Quire.Mirror;

namespace Quire.Wpf;

public static partial class StylesExtensions
{
    [MirrorVerified("PtcWpfStylesExtensionsTierADifferentialTests")]
    public static object GetSetterValue(this Style style, DependencyProperty property) =>
        style.Setters.OfType<Setter>().LastOrDefault(setter => setter.Property == property)?.Value;
}
