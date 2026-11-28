#nullable disable
using System.Windows;
using System.Windows.Controls;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class HeaderedPopupControl
{
    [MirrorVerified("Release IL")]
    public HeaderedPopupControl() { }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty HeaderProperty;

    [MirrorVerified("Release IL")]
    public object Header { get => base.GetValue(HeaderProperty); set => base.SetValue(HeaderProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty HeaderTemplateProperty;

    [MirrorVerified("Release IL")]
    public DataTemplate HeaderTemplate { get => (DataTemplate)base.GetValue(HeaderTemplateProperty); set => base.SetValue(HeaderTemplateProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty HeaderTemplateSelectorProperty;

    [MirrorVerified("Release IL")]
    public DataTemplateSelector HeaderTemplateSelector { get => (DataTemplateSelector)base.GetValue(HeaderTemplateSelectorProperty); set => base.SetValue(HeaderTemplateSelectorProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty HasHeaderProperty;

    [MirrorVerified("Release IL")]
    public bool HasHeader => (bool)base.GetValue(HasHeaderProperty);
}
