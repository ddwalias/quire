#nullable disable
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class Ribbon
{
    [MirrorVerified("Release IL")]
    private void Popup_Opened(object sender, EventArgs e) { }

    [MirrorVerified("Release IL")]
    private void UpdatePanelBorderSelectedTabRect() => UpdatePanelBorderSelectedTabRect(true);

    [MirrorVerified("Release IL")]
    private object InvokeUpdateBorderAfterRender(object args)
    {
        UpdatePanelBorderSelectedTabRect(false);
        return null;
    }

    [MirrorVerified("Release IL")]
    internal ContentPresenter GetSelectedContentPresenter() => base.GetTemplateChild("PART_TabContentHost") as ContentPresenter;

    [MirrorVerified("Release IL")]
    protected override void OnSelectionChanged(SelectionChangedEventArgs e)
    {
        base.OnSelectionChanged(e);
        UpdateSelectedContent();
        CommandManager.InvalidateRequerySuggested();
    }

    [MirrorVerified("Release IL")]
    protected override void OnVisualParentChanged(DependencyObject oldParent)
    {
        SetupRibbonWindow();
        base.OnVisualParentChanged(oldParent);
    }

    [MirrorVerified("Release IL")]
    private RibbonTab GetSelectedTabItem() => base.SelectedItem as RibbonTab;

    [MirrorVerified("Release IL")]
    private static void OnLicenseKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((Ribbon)d).OnLicenseKeyChanged();

    [MirrorVerified("Release IL")]
    private void OnLicenseKeyChanged() => WinApi.ValidateLicenseKey(LicenseKey);

    [MirrorVerified("Release IL")]
    public static void InitLK(string lk) => WinApi.ValidateLicenseKey(lk);

    [MirrorVerified("Release IL")]
    private void OnIsMinimizedChanged(bool oldValue, bool newValue) => UpdateMinimizedContentState();

    [MirrorVerified("Release IL")]
    private static void KeyTipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }

    [MirrorVerified("Release IL")]
    private static void IsCustomizableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }

    [MirrorVerified("Release IL")]
    protected override void OnContextMenuClosing(ContextMenuEventArgs e)
    {
        HandleContextMenuClosing(e);
        base.OnContextMenuClosing(e);
    }

    [MirrorVerified("Release IL")]
    private void ContextMenuMinMaxRibbonClick(object sender, RoutedEventArgs e) => ToggleRibbonMinimized();

    [MirrorVerified("Release IL")]
    private void ContextMenuQatChangePlacementClick(object sender, RoutedEventArgs e) => ChangeQatPlacement();

    [MirrorVerified("Release IL")]
    private void ContextMenuCustomizeQatClick(object sender, RoutedEventArgs e) => OnQatDialogCustomize();

    [MirrorVerified("Release IL")]
    internal void ToggleRibbonMinimized() => IsMinimized = !IsMinimized;

    [MirrorVerified("Release IL")]
    private void ToggleRibbonMinimizeExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        ToggleRibbonMinimized();
        e.Handled = true;
    }

    [MirrorVerified("Release IL")]
    private void ToggleRibbonMinimizeCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = AutoExpand;
        e.Handled = true;
        e.ContinueRouting = false;
    }

    [MirrorVerified("Release IL")]
    protected virtual void OnAfterCustomizeQatDialog(QatDialogEventArgs e) => base.RaiseEvent(e);

    [MirrorVerified("Release IL")]
    private void OnBeforeCustomizeQatDialog(QatDialogEventArgs e) => base.RaiseEvent(e);

    [MirrorVerified("Release IL")]
    internal void ChangeQatPlacement() => IsQuickAccessToolbarBelow = !IsQuickAccessToolbarBelow;

    [MirrorVerified("Release IL")]
    protected virtual void OnAfterAddToQat(QatOperationAddEventArgs qe) => base.RaiseEvent(qe);

    [MirrorVerified("Release IL")]
    protected virtual void OnBeforeAddToQat(QatOperationAddEventArgs qe) => base.RaiseEvent(qe);

    [MirrorVerified("Release IL")]
    protected virtual void OnAfterRemoveFromQat(QatOperationEventArgs oe) => base.RaiseEvent(oe);

    [MirrorVerified("Release IL")]
    protected virtual void OnBeforeRemoveFromQat(QatOperationEventArgs oe) => base.RaiseEvent(oe);

    [MirrorVerified("Release IL")]
    public ResourceDictionary CreateColorScheme(eRibbonVisualStyle baseColorTable, Color baseColor) => CreateColorScheme(GetColorTable(baseColorTable), baseColor);

    [MirrorVerified("Release IL")]
    private static ColorFactory GetColorFactory(Color baseColor) => new ColorBlendFactory(baseColor);

    [MirrorVerified("Release IL")]
    private static void OnButtonDropDownUnCheckedClick(object sender, RoutedEventArgs e) => ((Ribbon)sender).SyncChecked(e.Source as ButtonDropDown);

    [MirrorVerified("Release IL")]
    private ParentPopupControlImpl m_PopupHandler;

    [MirrorVerified("Release IL")]
    private void OnSelectedChanged(object sender, RoutedPropertyChangedEventArgs<bool> e) => m_PopupHandler.OnIsSelectedChanged(sender, e);

    [MirrorVerified("Release IL")]
    private static void OnButtonDropDownPreviewClick(object sender, RoutedEventArgs e) => ParentPopupControlImpl.OnButtonDropDownPreviewClick(sender, e);

    [MirrorVerified("Release IL")]
    private static void OnMouseButtonDown(object sender, MouseButtonEventArgs e) => ((Ribbon)sender).InternalMouseButtonDown(e);

    [MirrorVerified("Release IL")]
    private static void OnPreviewMouseButtonDown(object sender, MouseButtonEventArgs e) => ((Ribbon)sender).InternalPreviewMouseButtonDown(e);

    [MirrorVerified("Release IL")]
    private static void OnMouseButtonUp(object sender, MouseButtonEventArgs e) => ((Ribbon)sender).InternalMouseButtonUp(e);

    [MirrorVerified("Release IL")]
    protected virtual void InternalMouseButtonUp(MouseButtonEventArgs e) => m_PopupHandler.ClosePopupOnMouseEvent(e);

    [MirrorVerified("Release IL")]
    private static void OnLostMouseCapture(object sender, MouseEventArgs e) => ParentPopupControlImpl.OnLostMouseCapture(sender, e);

    [MirrorVerified("Release IL")]
    private bool IsDesignMode => DesignerProperties.GetIsInDesignMode(this);

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty SystemButtonsProperty;

    [MirrorVerified("Release IL")]
    public bool SystemButtons { get => (bool)base.GetValue(SystemButtonsProperty); set => base.SetValue(SystemButtonsProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty LicenseKeyProperty;

    [MirrorVerified("Release IL")]
    public string LicenseKey { get => (string)base.GetValue(LicenseKeyProperty); set => base.SetValue(LicenseKeyProperty, value); }

    [MirrorVerified("Release IL")]
    public bool IsActive { get => (bool)base.GetValue(RibbonBackgroundChrome.IsActiveProperty); set => base.SetValue(RibbonBackgroundChrome.IsActiveProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty ApplicationMenuProperty;

    [MirrorVerified("Release IL")]
    public UIElement ApplicationMenu { get => (UIElement)base.GetValue(ApplicationMenuProperty); set => base.SetValue(ApplicationMenuProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty QuickAccessToolbarProperty;

    [MirrorVerified("Release IL")]
    public UIElement QuickAccessToolbar { get => (UIElement)base.GetValue(QuickAccessToolbarProperty); set => base.SetValue(QuickAccessToolbarProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsQuickAccessToolbarBelowProperty;

    [MirrorVerified("Release IL")]
    public bool IsQuickAccessToolbarBelow { get => (bool)base.GetValue(IsQuickAccessToolbarBelowProperty); set => base.SetValue(IsQuickAccessToolbarBelowProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty WindowBorderThicknessProperty;

    [MirrorVerified("Release IL")]
    public Thickness WindowBorderThickness { get => (Thickness)base.GetValue(WindowBorderThicknessProperty); set => base.SetValue(WindowBorderThicknessProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty HorizontalWindowBorderThicknessProperty;

    [MirrorVerified("Release IL")]
    private static readonly DependencyPropertyKey HorizontalWindowBorderThicknessPropertyKey;

    [MirrorVerified("Release IL")]
    public Thickness HorizontalWindowBorderThickness { get => (Thickness)base.GetValue(HorizontalWindowBorderThicknessProperty); internal set => base.SetValue(HorizontalWindowBorderThicknessPropertyKey, value); }

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
    public static readonly DependencyProperty ContentTemplateProperty;

    [MirrorVerified("Release IL")]
    public DataTemplate ContentTemplate { get => (DataTemplate)base.GetValue(ContentTemplateProperty); set => base.SetValue(ContentTemplateProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty ContentTemplateSelectorProperty;

    [MirrorVerified("Release IL")]
    public DataTemplateSelector ContentTemplateSelector { get => (DataTemplateSelector)base.GetValue(ContentTemplateSelectorProperty); set => base.SetValue(ContentTemplateSelectorProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty ContextGroupsProperty;

    [MirrorVerified("Release IL")]
    public ContextGroupCollection ContextGroups => (ContextGroupCollection)base.GetValue(ContextGroupsProperty);

    [MirrorVerified("Release IL")]
    private RibbonLocalization m_SystemText;

    [MirrorVerified("Release IL")]
    public RibbonLocalization SystemText => m_SystemText;

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty AutoExpandProperty;

    [MirrorVerified("Release IL")]
    public bool AutoExpand { get => (bool)base.GetValue(AutoExpandProperty); set => base.SetValue(AutoExpandProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty CanDialogCustomizeQatProperty;

    [MirrorVerified("Release IL")]
    public bool CanDialogCustomizeQat { get => (bool)base.GetValue(CanDialogCustomizeQatProperty); set => base.SetValue(CanDialogCustomizeQatProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty CanChangeQatPlacementProperty;

    [MirrorVerified("Release IL")]
    public bool CanChangeQatPlacement { get => (bool)base.GetValue(CanChangeQatPlacementProperty); set => base.SetValue(CanChangeQatPlacementProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty AutoHideRibbonContentProperty;

    [MirrorVerified("Release IL")]
    public bool AutoHideRibbonContent { get => (bool)base.GetValue(AutoHideRibbonContentProperty); set => base.SetValue(AutoHideRibbonContentProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty AutoHideTriggerSizeProperty;

    [MirrorVerified("Release IL")]
    public Size AutoHideTriggerSize { get => (Size)base.GetValue(AutoHideTriggerSizeProperty); set => base.SetValue(AutoHideTriggerSizeProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty UseSpecKeyTipPositioningProperty;

    [MirrorVerified("Release IL")]
    public bool UseSpecKeyTipPositioning { get => (bool)base.GetValue(UseSpecKeyTipPositioningProperty); set => base.SetValue(UseSpecKeyTipPositioningProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsRibbonMenuOpenProperty;

    [MirrorVerified("Release IL")]
    public bool IsRibbonMenuOpen { get => (bool)base.GetValue(IsRibbonMenuOpenProperty); set => base.SetValue(IsRibbonMenuOpenProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsMinimizedProperty;

    [MirrorVerified("Release IL")]
    public bool IsMinimized { get => (bool)base.GetValue(IsMinimizedProperty); set => base.SetValue(IsMinimizedProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsUsingGlassProperty;

    [MirrorVerified("Release IL")]
    private static readonly DependencyPropertyKey IsUsingGlassPropertyKey;

    [MirrorVerified("Release IL")]
    public bool IsUsingGlass { get => (bool)base.GetValue(IsUsingGlassProperty); internal set => base.SetValue(IsUsingGlassPropertyKey, value); }

    [MirrorVerified("Release IL")]
    public string QuickAccessToolbarLayout { get => GetQatLayoutDescription(); set => SetQatLayoutDescription(value); }

    [MirrorVerified("Release IL")]
    private static bool _IsLivePreviewActive;

    [MirrorVerified("Release IL")]
    internal static bool IsLivePreviewActive => _IsLivePreviewActive;
}
