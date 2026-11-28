#nullable disable
using System.Windows;
using System.Windows.Input;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class BasePopupControl
{
    [MirrorVerified("Release IL")]
    private void PopupHandlerMenuModeChanged(object sender, EventArgs e) => OnInternalMenuModeChanged(e);

    [MirrorVerified("Release IL")]
    protected override DependencyObject GetContainerForItemOverride() => new ButtonDropDown();

    [MirrorVerified("Release IL")]
    private ParentPopupControlImpl m_PopupHandler;

    [MirrorVerified("Release IL")]
    private void OnSelectedChanged(object sender, RoutedPropertyChangedEventArgs<bool> e) => m_PopupHandler.OnIsSelectedChanged(sender, e);

    [MirrorVerified("Release IL")]
    private static void OnButtonDropDownPreviewClick(object sender, RoutedEventArgs e) => ParentPopupControlImpl.OnButtonDropDownPreviewClick(sender, e);

    [MirrorVerified("Release IL")]
    private static void OnMouseButtonDown(object sender, MouseButtonEventArgs e) => ((BasePopupControl)sender).InternalMouseButtonDown(e);

    [MirrorVerified("Release IL")]
    protected virtual void InternalMouseButtonDown(MouseButtonEventArgs e) => m_PopupHandler.ClosePopupOnMouseEvent(e);

    [MirrorVerified("Release IL")]
    private static void OnMouseButtonUp(object sender, MouseButtonEventArgs e) => ((BasePopupControl)sender).InternalMouseButtonUp(e);

    [MirrorVerified("Release IL")]
    protected virtual void InternalMouseButtonUp(MouseButtonEventArgs e) => m_PopupHandler.ClosePopupOnMouseEvent(e);

    [MirrorVerified("Release IL")]
    private static void OnLostMouseCapture(object sender, MouseEventArgs e) => ParentPopupControlImpl.OnLostMouseCapture(sender, e);

    [MirrorVerified("Release IL")]
    protected internal virtual void OnInternalMenuModeChanged(EventArgs e) { }

    [MirrorVerified("Release IL")]
    internal ButtonDropDown CurrentSelection => m_PopupHandler.CurrentSelection;
}
