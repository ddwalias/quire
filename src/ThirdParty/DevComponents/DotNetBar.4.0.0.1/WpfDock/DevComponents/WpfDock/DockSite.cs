#nullable disable
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.DevComponents.WpfDock;

public partial class DockSite
{
    [MirrorVerified("Release IL")]
    protected override Size MeasureOverride(Size constraint) => MeasureArrange(constraint, false);

    [MirrorVerified("Release IL")]
    private UIElementCollection m_Children;

    [MirrorVerified("Release IL")]
    internal void AddSplitPanel(SplitPanel p) => m_Children.Add(p);

    [MirrorVerified("Release IL")]
    internal void RemoveSplitPanel(SplitPanel p) => m_Children.Remove(p);

    [MirrorVerified("Release IL")]
    protected override Visual GetVisualChild(int index) => m_Children[index];

    [MirrorVerified("Release IL")]
    protected virtual void OnBeforeDockingStarts(DockRoutedEventArgs e) => base.RaiseEvent(e);

    [MirrorVerified("Release IL")]
    private UIElement GetDockHint(eDockHintSide dh) => new DockHint(dh);

    [MirrorVerified("Release IL")]
    private void DockWindowSelectorLostMouseCapture(object sender, MouseEventArgs e) => ReleaseDockSelectorAdorner(false);

    [MirrorVerified("Release IL")]
    private void SaveCurrentDockInfo(DockWindowGroup source) => SaveCurrentDockInfo(source, source, false);

    [MirrorVerified("Release IL")]
    protected virtual void OnAfterDocked(DockRoutedEventArgs dockRoutedEventArgs) => base.RaiseEvent(dockRoutedEventArgs);

    [MirrorVerified("Release IL")]
    protected virtual void OnBeforeDocked(BeforeDockedRoutedEventArgs dockEndsArgs) => base.RaiseEvent(dockEndsArgs);

    [MirrorVerified("Release IL")]
    private void SelectDifferentTab(DockWindowGroup dg) => dg.SelectDifferentTab();

    [MirrorVerified("Release IL")]
    public bool FloatWindow(DockWindow dw) => FloatWindow(dw, eEventActionSource.Code, true);

    [MirrorVerified("Release IL")]
    public bool FloatWindow(DockWindowGroup dg) => FloatWindow(dg, Rect.Empty, eEventActionSource.Code, true);

    [MirrorVerified("Release IL")]
    private List<FloatingWindow> m_FloatingWindows;

    [MirrorVerified("Release IL")]
    internal void FloatingWindowClosed(FloatingWindow floatingWindow) => m_FloatingWindows.Remove(floatingWindow);

    [MirrorVerified("Release IL")]
    public SplitPanel CreateSplitPanel(DockWindowGroup group) => CreateSplitPanel(group, GetDockSize(group));

    [MirrorVerified("Release IL")]
    private SplitPanel CreateSplitPanel(SplitPanel group) => CreateSplitPanel(group, double.NaN);

    [MirrorVerified("Release IL")]
    public void RestoreLastDockPosition(DockWindowGroup dg) => RestoreLastDockPosition(dg, false);

    [MirrorVerified("Release IL")]
    public bool DockWindow(DockWindowGroup group, eDockSide dockSide, bool fullSize) => DockWindow(group, dockSide, fullSize, eEventActionSource.Code, true);

    [MirrorVerified("Release IL")]
    public bool DockWindow(DockWindow dw, eDockSide dockSide) => DockWindow(dw, dockSide, eEventActionSource.Code, true);

    [MirrorVerified("Release IL")]
    public bool DockWindow(DockWindow dw, eDockSide dockSide, eEventActionSource actionSource, bool raiseEvents) => DockWindow(dw, dockSide, false, actionSource, raiseEvents);

    [MirrorVerified("Release IL")]
    public bool DockWindow(DockWindow dw, DockWindowGroup referenceGroup, eDockSide dockSide) => DockWindow(dw, referenceGroup, dockSide, eEventActionSource.Code, true);

    [MirrorVerified("Release IL")]
    public bool DockWindow(DockWindowGroup group, eDockSide dockSide) => DockWindow(group, dockSide, eEventActionSource.Code, true);

    [MirrorVerified("Release IL")]
    public bool DockWindow(DockWindowGroup group, eDockSide dockSide, eEventActionSource actionSource, bool raiseEvents) => DockWindow(group, dockSide, false, actionSource, raiseEvents);

    [MirrorVerified("Release IL")]
    public bool DockWindow(DockWindowGroup group, DockWindowGroup referenceGroup, eDockSide dockSide) => DockWindow(group, referenceGroup, dockSide, eEventActionSource.Code, true);

    [MirrorVerified("Release IL")]
    public bool DockWindow(DockWindowGroup group, DockWindowGroup referenceGroup, eDockSide dockSide, eEventActionSource actionSource, bool raiseEvents) => DockWindowInternal(group, referenceGroup, dockSide, actionSource, raiseEvents);

    [MirrorVerified("Release IL")]
    public bool DockWindow(DockWindowGroup group, SplitPanel sp, eDockSide dockSide) => DockWindow(group, sp, dockSide, eEventActionSource.Code, true);

    [MirrorVerified("Release IL")]
    public bool DockWindow(DockWindowGroup group, SplitPanel sp, eDockSide dockSide, eEventActionSource actionSource, bool raiseEvents) => DockWindowInternal(group, sp, dockSide, actionSource, raiseEvents);

    [MirrorVerified("Release IL")]
    public void UpdateIsDocument(DockWindowGroup group) => UpdateIsDocument(group, IsDockedAsDocument(group));

    [MirrorVerified("Release IL")]
    public void Detach(DockWindowGroup group) => Detach(group, true);

    [MirrorVerified("Release IL")]
    internal void SetAutoHide(DockWindow dw, bool autoHide, eEventActionSource actionSource) => SetAutoHide(dw, autoHide, actionSource, true);

    [MirrorVerified("Release IL")]
    public List<FloatingWindow> GetFloatingWindows() => m_FloatingWindows;

    [MirrorVerified("Release IL")]
    private void GetDockWindows(List<DockWindow> list, IDockWindowGroup parent) => list.AddRange(parent.WindowItems);

    [MirrorVerified("Release IL")]
    internal void OpenAutoHidePopup(DockWindow dw) => OpenAutoHidePopup(dw, false);

    [MirrorVerified("Release IL")]
    private static void DockWindowActivated(object sender, RoutedEventArgs e) => ((DockSite)sender).OnDockWindowActivated(e.OriginalSource as DockWindow);

    [MirrorVerified("Release IL")]
    private void OnDockWindowActivated(DockWindow dockWindow) => ActiveDockWindow = dockWindow;

    [MirrorVerified("Release IL")]
    private static void DockWindowDeactivated(object sender, RoutedEventArgs e) => ((DockSite)sender).OnDockWindowDeactivated(e.OriginalSource as DockWindow);

    [MirrorVerified("Release IL")]
    private ColorFactory GetColorFactory(Color baseColor) => new ColorBlendFactory(baseColor);

    [MirrorVerified("Release IL")]
    private static void OnLicenseKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((DockSite)d).OnLicenseKeyChanged();

    [MirrorVerified("Release IL")]
    private void OnLicenseKeyChanged() => WinApi.ValidateLicenseKey(LicenseKey);

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty DockHintOverlayProperty;

    [MirrorVerified("Release IL")]
    public bool DockHintOverlay { get => (bool)base.GetValue(DockHintOverlayProperty); set => base.SetValue(DockHintOverlayProperty, value); }

    [MirrorVerified("Release IL")]
    private bool _IgnoreLoadLayoutElementNotFoundErrors;

    [MirrorVerified("Release IL")]
    public bool IgnoreLoadLayoutElementNotFoundErrors { get => _IgnoreLoadLayoutElementNotFoundErrors; set => _IgnoreLoadLayoutElementNotFoundErrors = value; }

    [MirrorVerified("Release IL")]
    private DockLocalization m_SystemText;

    [MirrorVerified("Release IL")]
    public DockLocalization SystemText => m_SystemText;

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty SplitPanelsProperty;

    [MirrorVerified("Release IL")]
    public SplitPanelCollection SplitPanels => (SplitPanelCollection)base.GetValue(SplitPanelsProperty);

    [MirrorVerified("Release IL")]
    protected override int VisualChildrenCount => m_Children.Count;

    [MirrorVerified("Release IL")]
    private Rect m_DocumentRect;

    [MirrorVerified("Release IL")]
    public Rect DocumentRect => m_DocumentRect;

    [MirrorVerified("Release IL")]
    private bool m_IsDockingInProgress;

    [MirrorVerified("Release IL")]
    public bool IsDockingInProgress => m_IsDockingInProgress;

    [MirrorVerified("Release IL")]
    private bool IsDesignMode => DesignerProperties.GetIsInDesignMode(this);

    [MirrorVerified("Release IL")]
    public string Layout { get => GetDockSiteLayoutSerializer().SaveLayout(this); set => GetDockSiteLayoutSerializer().LoadLayout(this, value); }

    [MirrorVerified("Release IL")]
    private static readonly DependencyPropertyKey ActiveDockWindowPropertyKey;

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty ActiveDockWindowProperty;

    [MirrorVerified("Release IL")]
    public DockWindow ActiveDockWindow { get => (DockWindow)base.GetValue(ActiveDockWindowProperty); internal set => base.SetValue(ActiveDockWindowPropertyKey, value); }

    [MirrorVerified("Release IL")]
    protected override global::System.Collections.IEnumerator LogicalChildren => m_Children.GetEnumerator();

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty LicenseKeyProperty;

    [MirrorVerified("Release IL")]
    public string LicenseKey { get => (string)base.GetValue(LicenseKeyProperty); set => base.SetValue(LicenseKeyProperty, value); }
}
