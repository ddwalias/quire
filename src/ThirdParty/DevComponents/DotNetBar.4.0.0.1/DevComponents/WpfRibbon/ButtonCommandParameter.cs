#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class ButtonCommandParameter
{
    [MirrorVerified("Release IL")]
    public ButtonCommandParameter() { }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsCheckedProperty;

    [MirrorVerified("Release IL")]
    public bool IsChecked { get => (bool)base.GetValue(IsCheckedProperty); set => base.SetValue(IsCheckedProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty TagProperty;

    [MirrorVerified("Release IL")]
    public object Tag { get => base.GetValue(TagProperty); set => base.SetValue(TagProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty ValueChangedProperty;

    [MirrorVerified("Release IL")]
    public bool ValueChanged { get => (bool)base.GetValue(ValueChangedProperty); set => base.SetValue(ValueChangedProperty, value); }
}
