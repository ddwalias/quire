#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.DevComponents.WpfDock;

public partial class DockWindowInfo
{
    [MirrorVerified("Release IL")]
    public DockWindowInfo() { }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty ImageProperty;

    [MirrorVerified("Release IL")]
    public object Image { get => base.GetValue(ImageProperty); set => base.SetValue(ImageProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty DescriptionProperty;

    [MirrorVerified("Release IL")]
    public string Description { get => (string)base.GetValue(DescriptionProperty); set => base.SetValue(DescriptionProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty HeaderProperty;

    [MirrorVerified("Release IL")]
    public object Header { get => base.GetValue(HeaderProperty); set => base.SetValue(HeaderProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty DockWindowProperty;

    [MirrorVerified("Release IL")]
    public DockWindow DockWindow { get => (DockWindow)base.GetValue(DockWindowProperty); set => base.SetValue(DockWindowProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsSelectedProperty;

    [MirrorVerified("Release IL")]
    public bool IsSelected { get => (bool)base.GetValue(IsSelectedProperty); set => base.SetValue(IsSelectedProperty, value); }
}
