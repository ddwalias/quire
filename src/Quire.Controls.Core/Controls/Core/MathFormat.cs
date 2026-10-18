#nullable disable
using System.ComponentModel;
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class MathFormat
{
    [MirrorVerified("PtcControlsCoreMathFormatMechanicalDifferentialTests")]
    private readonly DependencyObject _owner;

    [MirrorVerified("PtcControlsCoreMathFormatMechanicalDifferentialTests")]
    private void SetValue(DependencyProperty dependencyProperty, object value) => _owner.SetValue(dependencyProperty, value);

    [MirrorVerified("PtcControlsCoreMathFormatMechanicalDifferentialTests")]
    private void ClearValue(DependencyProperty dependencyProperty) => _owner.ClearValue(dependencyProperty);

    [MirrorVerified("PtcControlsCoreMathFormatMechanicalDifferentialTests")]
    private readonly IList<DependencyProperty> _readonlyProperties;

    [MirrorVerified("PtcControlsCoreMathFormatMechanicalDifferentialTests")]
    private bool IsPropertyReadonly(DependencyProperty dependencyProperty) => _readonlyProperties.Contains(dependencyProperty);

    [MirrorVerified("PtcControlsCoreMathFormatMechanicalDifferentialTests")]
    public bool IsFontFamilyNameSet => IsPropertySet(MathFormatAttachedProperties.FontFamilyNameProperty);

    [MirrorVerified("PtcControlsCoreMathFormatMechanicalDifferentialTests")]
    public bool IsFontSizeSet => IsPropertySet(MathFormatAttachedProperties.FontSizeProperty);

    [MirrorVerified("PtcControlsCoreMathFormatMechanicalDifferentialTests")]
    public bool IsItalicSet => IsPropertySet(MathFormatAttachedProperties.ItalicProperty);

    [MirrorVerified("PtcControlsCoreMathFormatMechanicalDifferentialTests")]
    public bool IsBoldSet => IsPropertySet(MathFormatAttachedProperties.BoldProperty);

    [MirrorVerified("PtcControlsCoreMathFormatMechanicalDifferentialTests")]
    public bool IsUnderlineSet => IsPropertySet(MathFormatAttachedProperties.UnderlineProperty);

    [MirrorVerified("PtcControlsCoreMathFormatMechanicalDifferentialTests")]
    public bool IsFontColorSet => IsPropertySet(MathFormatAttachedProperties.FontColorProperty);

    [MirrorVerified("PtcControlsCoreMathFormatMechanicalDifferentialTests")]
    public bool IsHighlightColorSet => IsPropertySet(MathFormatAttachedProperties.HighlightColorProperty);

    [MirrorVerified("PtcControlsCoreMathFormatMechanicalDifferentialTests")]
    public event PropertyChangedEventHandler PropertyChanged;
}
