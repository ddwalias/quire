#nullable disable
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class Gallery
{
    [MirrorVerified("Release IL")]
    private void MenuItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => UpdateMenuItems();

    [MirrorVerified("Release IL")]
    private static void OnCategoryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }

    [MirrorVerified("Release IL")]
    private bool _ImmediateLivePreview;

    [MirrorVerified("Release IL")]
    protected override void OnMouseLeave(MouseEventArgs e)
    {
        _ImmediateLivePreview = false;
        base.OnMouseLeave(e);
    }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty MenuItemsProperty;

    [MirrorVerified("Release IL")]
    public ObservableCollection<UIElement> MenuItems => (ObservableCollection<UIElement>)base.GetValue(MenuItemsProperty);

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsCollapsibleProperty;

    [MirrorVerified("Release IL")]
    public bool IsCollapsible { get => (bool)base.GetValue(IsCollapsibleProperty); set => base.SetValue(IsCollapsibleProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsResizeEnabledProperty;

    [MirrorVerified("Release IL")]
    public bool IsResizeEnabled { get => (bool)base.GetValue(IsResizeEnabledProperty); set => base.SetValue(IsResizeEnabledProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsCollapsedProperty;

    [MirrorVerified("Release IL")]
    public bool IsCollapsed { get => (bool)base.GetValue(IsCollapsedProperty); set => base.SetValue(IsCollapsedProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty MenuItemsVisibilityProperty;

    [MirrorVerified("Release IL")]
    private static readonly DependencyPropertyKey MenuItemsVisibilityPropertyKey;

    [MirrorVerified("Release IL")]
    public Visibility MenuItemsVisibility { get => (Visibility)base.GetValue(MenuItemsVisibilityProperty); internal set => base.SetValue(MenuItemsVisibilityPropertyKey, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty SuggestedContainerWidthProperty;

    [MirrorVerified("Release IL")]
    public double SuggestedContainerWidth { get => (double)base.GetValue(SuggestedContainerWidthProperty); set => base.SetValue(SuggestedContainerWidthProperty, value); }

    [MirrorVerified("Release IL")]
    private Panel m_ItemsPanel;

    [MirrorVerified("Release IL")]
    internal Panel InternalItemsPanel => m_ItemsPanel;

    [MirrorVerified("Release IL")]
    private GalleryScrollViewer m_Panel;

    [MirrorVerified("Release IL")]
    public GalleryScrollViewer GalleryScrollViewer => m_Panel;

    [MirrorVerified("Release IL")]
    internal bool ImmediateLivePreview { get => _ImmediateLivePreview; set => _ImmediateLivePreview = value; }
}
