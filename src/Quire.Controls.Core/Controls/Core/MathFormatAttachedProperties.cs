#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class MathFormatAttachedProperties
{
    [MirrorVerified("PtcControlsCoreMathFormatAttachedPropertiesMechanicalDifferentialTests")]
    public static DependencyProperty FontFamilyNameProperty;

    [MirrorVerified("PtcControlsCoreMathFormatAttachedPropertiesMechanicalDifferentialTests")]
    public static void SetFontFamilyName(DependencyObject depObj, string fontFamilyName) => depObj.SetValue(FontFamilyNameProperty, fontFamilyName);

    [MirrorVerified("PtcControlsCoreMathFormatAttachedPropertiesMechanicalDifferentialTests")]
    public static readonly DependencyProperty OverrideLabelColorProperty;

    [MirrorVerified("PtcControlsCoreMathFormatAttachedPropertiesMechanicalDifferentialTests")]
    public static void ClearOverrideLabelColor(DependencyObject obj) => obj.ClearValue(OverrideLabelColorProperty);

    [MirrorVerified("PtcControlsCoreMathFormatAttachedPropertiesMechanicalDifferentialTests")]
    public static readonly DependencyProperty HandleChangesEnabledProperty;

    [MirrorVerified("PtcControlsCoreMathFormatAttachedPropertiesMechanicalDifferentialTests")]
    public static void ClearHandleChangesEnabled(DependencyObject obj) => obj.ClearValue(HandleChangesEnabledProperty);

    [MirrorVerified("PtcControlsCoreMathFormatAttachedPropertiesMechanicalDifferentialTests")]
    public MathFormatAttachedProperties() { }
}
