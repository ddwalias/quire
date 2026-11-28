#nullable disable
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Quire.Mirror;

namespace Quire.DevComponents.WpfDock;

public partial class DockWindowGroup
{
    [MirrorVerified("Release IL")]
    private void ButtonOptionsMouseRightButtonDown(object sender, MouseButtonEventArgs e) => e.Handled = true;

    [MirrorVerified("Release IL")]
    public DockSite GetDockSite() => DockSite.GetDockSite(this);

    [MirrorVerified("Release IL")]
    private void UpdateAllSelectedContent()
    {
        UpdateSelectedHeader();
        UpdateSelectedContent();
    }

    [MirrorVerified("Release IL")]
    internal ContentPresenter GetSelectedContentPresenter() => base.GetTemplateChild("PART_TabContentHost") as ContentPresenter;

    [MirrorVerified("Release IL")]
    protected override void OnSelectionChanged(SelectionChangedEventArgs e)
    {
        base.OnSelectionChanged(e);
        UpdateAllSelectedContent();
        UpdateZOrder();
        UpdateStateProperties();
        CommandManager.InvalidateRequerySuggested();
    }

    [MirrorVerified("Release IL")]
    private HeaderedContentControl GetSelectedItem() => base.SelectedItem as HeaderedContentControl;

    [MirrorVerified("Release IL")]
    protected override bool IsItemItsOwnContainerOverride(object item) => item is DockWindow;

    [MirrorVerified("Release IL")]
    protected override DependencyObject GetContainerForItemOverride() => new DockWindow();

    [MirrorVerified("Release IL")]
    public void AddDockWindow(DockWindow wnd) => base.Items.Add(wnd);

    [MirrorVerified("Release IL")]
    public void RemoveDockWindow(DockWindow wnd) => base.Items.Remove(wnd);

    [MirrorVerified("Release IL")]
    private UIElement m_InnerContent;

    [MirrorVerified("Release IL")]
    internal UIElement InnerContent => m_InnerContent;

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty SelectedDockWindowProperty;

    [MirrorVerified("Release IL")]
    private static readonly DependencyPropertyKey SelectedDockWindowPropertyKey;

    [MirrorVerified("Release IL")]
    public DockWindow SelectedDockWindow { get => (DockWindow)base.GetValue(SelectedDockWindowProperty); internal set => base.SetValue(SelectedDockWindowPropertyKey, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty SelectedContentTemplateProperty;

    [MirrorVerified("Release IL")]
    private static readonly DependencyPropertyKey SelectedContentTemplatePropertyKey;

    [MirrorVerified("Release IL")]
    public DataTemplate SelectedContentTemplate { get => (DataTemplate)base.GetValue(SelectedContentTemplateProperty); internal set => base.SetValue(SelectedContentTemplatePropertyKey, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty SelectedContentTemplateSelectorProperty;

    [MirrorVerified("Release IL")]
    private static readonly DependencyPropertyKey SelectedContentTemplateSelectorPropertyKey;

    [MirrorVerified("Release IL")]
    public DataTemplateSelector SelectedContentTemplateSelector { get => (DataTemplateSelector)base.GetValue(SelectedContentTemplateSelectorProperty); internal set => base.SetValue(SelectedContentTemplateSelectorPropertyKey, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty SelectedContentHeaderProperty;

    [MirrorVerified("Release IL")]
    private static readonly DependencyPropertyKey SelectedContentHeaderPropertyKey;

    [MirrorVerified("Release IL")]
    public object SelectedContentHeader { get => base.GetValue(SelectedContentHeaderProperty); internal set => base.SetValue(SelectedContentHeaderPropertyKey, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty ContentTemplateProperty;

    [MirrorVerified("Release IL")]
    public DataTemplate ContentTemplate { get => (DataTemplate)base.GetValue(ContentTemplateProperty); set => base.SetValue(ContentTemplateProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty ContentTemplateSelectorProperty;

    [MirrorVerified("Release IL")]
    public DataTemplateSelector ContentTemplateSelector { get => (DataTemplateSelector)base.GetValue(ContentTemplateSelectorProperty); set => base.SetValue(ContentTemplateSelectorProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsFloatingProperty;

    [MirrorVerified("Release IL")]
    private static readonly DependencyPropertyKey IsFloatingPropertyKey;

    [MirrorVerified("Release IL")]
    public bool IsFloating { get => (bool)base.GetValue(IsFloatingProperty); internal set => base.SetValue(IsFloatingPropertyKey, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty TabVisibilityProperty;

    [MirrorVerified("Release IL")]
    private static readonly DependencyPropertyKey TabVisibilityPropertyKey;

    [MirrorVerified("Release IL")]
    public Visibility TabVisibility { get => (Visibility)base.GetValue(TabVisibilityProperty); internal set => base.SetValue(TabVisibilityPropertyKey, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty CanCloseProperty;

    [MirrorVerified("Release IL")]
    private static readonly DependencyPropertyKey CanClosePropertyKey;

    [MirrorVerified("Release IL")]
    public bool CanClose { get => (bool)base.GetValue(CanCloseProperty); internal set => base.SetValue(CanClosePropertyKey, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty CanAutoHideProperty;

    [MirrorVerified("Release IL")]
    private static readonly DependencyPropertyKey CanAutoHidePropertyKey;

    [MirrorVerified("Release IL")]
    public bool CanAutoHide { get => (bool)base.GetValue(CanAutoHideProperty); internal set => base.SetValue(CanAutoHidePropertyKey, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsAutoHideProperty;

    [MirrorVerified("Release IL")]
    private static readonly DependencyPropertyKey IsAutoHidePropertyKey;

    [MirrorVerified("Release IL")]
    public bool IsAutoHide { get => (bool)base.GetValue(IsAutoHideProperty); internal set => base.SetValue(IsAutoHidePropertyKey, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty OptionsMenuProperty;

    [MirrorVerified("Release IL")]
    private static readonly DependencyPropertyKey OptionsMenuPropertyKey;

    [MirrorVerified("Release IL")]
    public Visibility OptionsMenu { get => (Visibility)base.GetValue(OptionsMenuProperty); internal set => base.SetValue(OptionsMenuPropertyKey, value); }

    [MirrorVerified("Release IL")]
    private string m_Id;

    [MirrorVerified("Release IL")]
    public string Id => m_Id;

    [MirrorVerified("Release IL")]
    private eDockSide m_LastDockSide;

    [MirrorVerified("Release IL")]
    internal eDockSide LastDockSide { get => m_LastDockSide; set => m_LastDockSide = value; }

    [MirrorVerified("Release IL")]
    private eDockSide m_LastDockReference;

    [MirrorVerified("Release IL")]
    internal eDockSide LastDockReference { get => m_LastDockReference; set => m_LastDockReference = value; }

    [MirrorVerified("Release IL")]
    private string m_LastDockReferenceId;

    [MirrorVerified("Release IL")]
    internal string LastDockReferenceId { get => m_LastDockReferenceId; set => m_LastDockReferenceId = value; }

    [MirrorVerified("Release IL")]
    private bool m_LastDockIsFullSizeDock;

    [MirrorVerified("Release IL")]
    internal bool LastDockIsFullSizeDock { get => m_LastDockIsFullSizeDock; set => m_LastDockIsFullSizeDock = value; }

    [MirrorVerified("Release IL")]
    private ContextMenu m_ContextMenuOptions;

    [MirrorVerified("Release IL")]
    internal ContextMenu OptionsContextMenu => m_ContextMenuOptions;

    [MirrorVerified("Release IL")]
    public IEnumerable<DockWindow> WindowItems => Enumerable.OfType<DockWindow>(ItemsTabSelector.ViewItems);

    [MirrorVerified("Release IL")]
    public int WindowsCount => base.Items.Count;

    [MirrorVerified("Release IL")]
    public DockWindow SelectedWindow { get => base.SelectedItem as DockWindow; set => base.SelectedItem = value; }

    [MirrorVerified("Release IL")]
    public IList<UIElement> ViewItems => Enumerable.ToList<UIElement>(Enumerable.OfType<UIElement>(base.Items));

    [MirrorVerified("Release IL")]
    public int SelectedViewIndex { get => base.SelectedIndex; set => base.SelectedIndex = value; }
}
