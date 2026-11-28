#nullable disable
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class NavigationPane
{
    [MirrorVerified("Release IL")]
    private static void OnLicenseKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((NavigationPane)d).OnLicenseKeyChanged();

    [MirrorVerified("Release IL")]
    private void OnLicenseKeyChanged() => WinApi.ValidateLicenseKey(LicenseKey);

    [MirrorVerified("Release IL")]
    protected virtual void OnExpanded(RoutedEventArgs e) => base.RaiseEvent(e);

    [MirrorVerified("Release IL")]
    protected virtual void OnCollapsed(RoutedEventArgs e) => base.RaiseEvent(e);

    [MirrorVerified("Release IL")]
    protected virtual void OnPopupOpened(RoutedEventArgs e) => base.RaiseEvent(e);

    [MirrorVerified("Release IL")]
    protected virtual void OnPopupClosed(RoutedEventArgs e) => base.RaiseEvent(e);

    [MirrorVerified("Release IL")]
    protected virtual void OnBeforeCustomizeDialogEvent(RoutedEventArgs e) => base.RaiseEvent(e);

    [MirrorVerified("Release IL")]
    public static readonly RoutedEvent PopupOpenedEvent;

    [MirrorVerified("Release IL")]
    private void InternalPopupOpened(object sender, EventArgs e) => OnPopupOpened(new RoutedEventArgs(PopupOpenedEvent, this));

    [MirrorVerified("Release IL")]
    private static void OnLostMouseCapture(object sender, MouseEventArgs e) => ((NavigationPane)sender).SystemLostMouseCapture(e);

    [MirrorVerified("Release IL")]
    private static void MouseDownOutsideCapturedElement(object sender, MouseButtonEventArgs e) => ((NavigationPane)sender).MouseDownOutsideCapturedElement(e);

    [MirrorVerified("Release IL")]
    protected override void OnSelectionChanged(SelectionChangedEventArgs e)
    {
        base.OnSelectionChanged(e);
        UpdateAllSelectedContent();
        CommandManager.InvalidateRequerySuggested();
    }

    [MirrorVerified("Release IL")]
    private void UpdateAllSelectedContent()
    {
        UpdateSelectedHeader();
        UpdateSelectedContent();
    }

    [MirrorVerified("Release IL")]
    internal ContentPresenter GetSelectedContentPresenter() => base.GetTemplateChild("PART_PaneContentHost") as ContentPresenter;

    [MirrorVerified("Release IL")]
    private HeaderedContentControl GetSelectedItem() => base.SelectedItem as HeaderedContentControl;

    [MirrorVerified("Release IL")]
    private static void IsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((NavigationPane)d).IsExpandedChanged(e);

    [MirrorVerified("Release IL")]
    private static void LargeItemsCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((NavigationPane)d).LargeItemsCountChanged(e);

    [MirrorVerified("Release IL")]
    private void LargeItemsCountChanged(DependencyPropertyChangedEventArgs e) => UpdateItemsState();

    [MirrorVerified("Release IL")]
    protected override bool IsItemItsOwnContainerOverride(object item) => item is PaneItem;

    [MirrorVerified("Release IL")]
    protected override DependencyObject GetContainerForItemOverride() => new PaneItem();

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty LicenseKeyProperty;

    [MirrorVerified("Release IL")]
    public string LicenseKey { get => (string)base.GetValue(LicenseKeyProperty); set => base.SetValue(LicenseKeyProperty, value); }

    [MirrorVerified("Release IL")]
    private NavigationPanePanel _ItemsPanel;

    [MirrorVerified("Release IL")]
    public NavigationPanePanel InternalItemsPanel => _ItemsPanel;

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty SelectedPaneItemProperty;

    [MirrorVerified("Release IL")]
    private static readonly DependencyPropertyKey SelectedPaneItemPropertyKey;

    [MirrorVerified("Release IL")]
    public PaneItem SelectedPaneItem { get => (PaneItem)base.GetValue(SelectedPaneItemProperty); internal set => base.SetValue(SelectedPaneItemPropertyKey, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty SelectedContentProperty;

    [MirrorVerified("Release IL")]
    private static readonly DependencyPropertyKey SelectedContentPropertyKey;

    [MirrorVerified("Release IL")]
    public object SelectedContent { get => base.GetValue(SelectedContentProperty); internal set => base.SetValue(SelectedContentPropertyKey, value); }

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
    public static readonly DependencyProperty ExpandedWidthProperty;

    [MirrorVerified("Release IL")]
    public double ExpandedWidth { get => (double)base.GetValue(ExpandedWidthProperty); set => base.SetValue(ExpandedWidthProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty CollapsedWidthProperty;

    [MirrorVerified("Release IL")]
    public double CollapsedWidth { get => (double)base.GetValue(CollapsedWidthProperty); set => base.SetValue(CollapsedWidthProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty LargeItemsCountProperty;

    [MirrorVerified("Release IL")]
    public int LargeItemsCount { get => (int)base.GetValue(LargeItemsCountProperty); set => base.SetValue(LargeItemsCountProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsExpandedProperty;

    [MirrorVerified("Release IL")]
    public bool IsExpanded { get => (bool)base.GetValue(IsExpandedProperty); set => base.SetValue(IsExpandedProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty SystemTextProperty;

    [MirrorVerified("Release IL")]
    private static readonly DependencyPropertyKey SystemTextPropertyKey;

    [MirrorVerified("Release IL")]
    public NavigationPaneLocalization SystemText { get => (NavigationPaneLocalization)base.GetValue(SystemTextProperty); internal set => base.SetValue(SystemTextPropertyKey, value); }
}
