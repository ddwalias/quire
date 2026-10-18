#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire;

public static partial class TextElementAttachedProperties
{
    [MirrorVerified("PtcTextElementAttachedPropertiesMechanicalDifferentialTests")]
    public static DependencyProperty StyleKeyProperty;

    [MirrorVerified("PtcTextElementAttachedPropertiesMechanicalDifferentialTests")]
    public static void ClearTextStyleData(DependencyObject element) => element.ClearValue(StyleKeyProperty);
}
