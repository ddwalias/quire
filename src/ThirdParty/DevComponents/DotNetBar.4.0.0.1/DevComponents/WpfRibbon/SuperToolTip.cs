#nullable disable
using System.Windows;
using System.Windows.Controls;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class SuperToolTip
{
    [MirrorVerified("Release IL")]
    public SuperToolTip() { }

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

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty FooterImageProperty;

    [MirrorVerified("Release IL")]
    public object FooterImage { get => base.GetValue(FooterImageProperty); set => base.SetValue(FooterImageProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty FooterProperty;

    [MirrorVerified("Release IL")]
    public object Footer { get => base.GetValue(FooterProperty); set => base.SetValue(FooterProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty FooterTemplateProperty;

    [MirrorVerified("Release IL")]
    public DataTemplate FooterTemplate { get => (DataTemplate)base.GetValue(FooterTemplateProperty); set => base.SetValue(FooterTemplateProperty, value); }

    [MirrorVerified("Release IL")]
    public DataTemplateSelector FooterTemplateSelector { get => (DataTemplateSelector)base.GetValue(HeaderTemplateSelectorProperty); set => base.SetValue(HeaderTemplateSelectorProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty HasFooterProperty;

    [MirrorVerified("Release IL")]
    public bool HasFooter => (bool)base.GetValue(HasFooterProperty);
}
