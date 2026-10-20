#nullable disable
using System.Windows;
using System.Windows;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire;

public partial class TextStyleData
{
    [MirrorVerified("PtcTextStyleDataMechanicalDifferentialTests")]
    public override string ToString() => StyleName;

    [MirrorVerified("PtcTextStyleDataMechanicalDifferentialTests")]
    public static DependencyProperty StyleProperty;

    [MirrorVerified("PtcTextStyleDataMechanicalDifferentialTests")]
    internal static void SetStyle(DependencyObject element, Style value) => element.SetValue(StyleProperty, value);

    [MirrorVerified("PtcTextStyleDataMechanicalDifferentialTests")]
    public static DependencyProperty StyleNameProperty;

    [MirrorVerified("PtcTextStyleDataMechanicalDifferentialTests")]
    internal static void SetStyleName(DependencyObject element, string value) => element.SetValue(StyleNameProperty, value);

    [MirrorVerified("PtcTextStyleDataMechanicalDifferentialTests")]
    private Style _oldStyle;

    [MirrorVerified("PtcTextStyleDataMechanicalDifferentialTests")]
    public Style OldStyle => _oldStyle;

    [MirrorVerified("PtcTextStyleDataMechanicalDifferentialTests")]
    public string StyleName { get => GetStyleName(this); set => SetStyleName(this, value); }

    [MirrorVerified("PtcTextStyleDataMechanicalDifferentialTests")]
    public string StyleKey { get; private set; }

    [MirrorVerified("PtcTextStyleDataMechanicalDifferentialTests")]
    public string BaseOnKey { get; set; }

    [MirrorVerified("PtcTextStyleDataMechanicalDifferentialTests")]
    public Style Style { get => GetStyle(this); set => SetStyle(this, value); }

    [MirrorVerified("PtcTextStyleDataMechanicalDifferentialTests")]
    public static event Delegate1<TextStyleData> StyleChanged;

    [MirrorVerified("Release IL")]
    internal static Style GetStyle(DependencyObject element) => element.GetValue(StyleProperty) as Style;

    [MirrorVerified("Release IL")]
    internal static string GetStyleName(DependencyObject element) => element.GetValue(StyleNameProperty) as string;

    [MirrorVerified("Release IL")]
    public TextStyleData()
    {
        StyleKey = string.Empty;
        StyleName = string.Empty;
        BaseOnKey = string.Empty;
    }
}
