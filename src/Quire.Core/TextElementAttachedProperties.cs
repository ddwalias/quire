#nullable disable
using System.Windows;
using System.Windows;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire;

public static partial class TextElementAttachedProperties
{
    [MirrorVerified("PtcTextElementAttachedPropertiesMechanicalDifferentialTests")]
    public static DependencyProperty StyleKeyProperty;

    [MirrorVerified("PtcTextElementAttachedPropertiesMechanicalDifferentialTests")]
    public static void ClearTextStyleData(DependencyObject element) => element.ClearValue(StyleKeyProperty);

    [MirrorVerified("Release IL")]
    public static string GetStyleKey(DependencyObject element) => element.GetValue(StyleKeyProperty) as string;

    public static void SetStyleKey(DependencyObject element, string value)
    {
        if (value.IsNullOrEmpty())
        {
            value = DefaultTextStyle;
        }
        element.SetValue(StyleKeyProperty, value);
    }
}
