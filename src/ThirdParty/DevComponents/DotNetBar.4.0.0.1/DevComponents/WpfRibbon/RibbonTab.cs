#nullable disable
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class RibbonTab
{
    [MirrorVerified("Release IL")]
    private RibbonTabBorder m_TabBorder;

    [MirrorVerified("Release IL")]
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        m_TabBorder = base.GetTemplateChild("PART_RibbonTabBorder") as RibbonTabBorder;
        UpdateBorderRenderState();
        UpdateTabBorderColorClass();
    }

    [MirrorVerified("Release IL")]
    private void OnColorChanged(eRibbonTabColor newValue) => UpdateTabBorderColorClass();

    [MirrorVerified("Release IL")]
    protected virtual void OnUnselected(RoutedEventArgs e) => RaiseSelectedEvents(false, e);

    [MirrorVerified("Release IL")]
    private void RaiseSelectedEvents(bool newValue, RoutedEventArgs e) => base.RaiseEvent(e);

    [MirrorVerified("Release IL")]
    protected override void OnMouseEnter(MouseEventArgs e)
    {
        base.OnMouseEnter(e);
        UpdateBorderRenderState();
    }

    [MirrorVerified("Release IL")]
    protected override void OnMouseLeave(MouseEventArgs e)
    {
        base.OnMouseLeave(e);
        UpdateBorderRenderState();
    }

    [MirrorVerified("Release IL")]
    protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnIsKeyboardFocusWithinChanged(e);
        UpdateBorderRenderState();
    }

    [MirrorVerified("Release IL")]
    public RibbonTab() { }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty ContextGroupProperty;

    [MirrorVerified("Release IL")]
    public ContextGroup ContextGroup { get => (ContextGroup)base.GetValue(ContextGroupProperty); set => base.SetValue(ContextGroupProperty, value); }

    [MirrorVerified("Release IL")]
    internal Ribbon ParentRibbon => ItemsControl.ItemsControlFromItemContainer(this) as Ribbon;

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty ColorProperty;

    [MirrorVerified("Release IL")]
    public eRibbonTabColor Color { get => (eRibbonTabColor)base.GetValue(ColorProperty); set => base.SetValue(ColorProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsSelectedProperty;

    [MirrorVerified("Release IL")]
    public bool IsSelected { get => (bool)base.GetValue(IsSelectedProperty); set => base.SetValue(IsSelectedProperty, value); }
}
