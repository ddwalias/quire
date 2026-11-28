#nullable disable
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Quire.Mirror;

namespace Quire.DevComponents.WpfDock;

public partial class DockWindow
{
    [MirrorVerified("Release IL")]
    private Decorator m_TabBorder;

    [MirrorVerified("Release IL")]
    public override void OnApplyTemplate()
    {
        m_TabBorder = base.GetTemplateChild("TabBorder") as Decorator;
        base.OnApplyTemplate();
    }

    [MirrorVerified("Release IL")]
    public static readonly RoutedEvent TabVisibilityChangedEvent;

    [MirrorVerified("Release IL")]
    internal void InvokeTabVisibilityChangedEvent() => OnTabVisibilityChanged(new RoutedEventArgs(TabVisibilityChangedEvent));

    [MirrorVerified("Release IL")]
    protected virtual void OnTabVisibilityChanged(RoutedEventArgs e) => base.RaiseEvent(e);

    [MirrorVerified("Release IL")]
    private void StartTimer(DispatcherTimer timer)
    {
        timer.Interval = TimeSpan.FromMilliseconds(SystemParameters.MenuShowDelay * 2.5);
        timer.Start();
    }

    [MirrorVerified("Release IL")]
    private static void IsAutoHideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((DockWindow)d).IsAutoHideChanged();

    [MirrorVerified("Release IL")]
    protected virtual void OnAutoHideChanged(RoutedEventArgs e) => base.RaiseEvent(e);

    [MirrorVerified("Release IL")]
    protected virtual void OnActivated(RoutedEventArgs e) => base.RaiseEvent(e);

    [MirrorVerified("Release IL")]
    protected virtual void OnDeactivated(RoutedEventArgs e) => base.RaiseEvent(e);

    [MirrorVerified("Release IL")]
    private static void OnAutoHideOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((DockWindow)d).OnAutoHideOpenChanged();

    [MirrorVerified("Release IL")]
    protected virtual void OnAutoHideOpenChanged(RoutedEventArgs e) => base.RaiseEvent(e);

    [MirrorVerified("Release IL")]
    private static void UpdateParentGroupState(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((DockWindow)d).UpdateParentGroupState();

    [MirrorVerified("Release IL")]
    public DockSite GetDockSite() => DockSite.GetDockSite(this);

    [MirrorVerified("Release IL")]
    protected virtual void OnUnselected(RoutedEventArgs e) => RaiseSelectedEvents(false, e);

    [MirrorVerified("Release IL")]
    private void RaiseSelectedEvents(bool newValue, RoutedEventArgs e) => base.RaiseEvent(e);

    [MirrorVerified("Release IL")]
    private static void ImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((DockWindow)d).ImageSourceChanged();

    [MirrorVerified("Release IL")]
    public bool Close() => Close(eEventActionSource.Code);

    [MirrorVerified("Release IL")]
    protected virtual void OnClosed(RoutedEventArgs e) => base.RaiseEvent(e);

    [MirrorVerified("Release IL")]
    protected virtual void OnClosing(CancelSourceRoutedEventArgs e) => base.RaiseEvent(e);

    [MirrorVerified("Release IL")]
    internal void InvokeDockParentChanged() => OnDockParentChanged(new EventArgs());

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty TabToolTipProperty;

    [MirrorVerified("Release IL")]
    public object TabToolTip { get => base.GetValue(TabToolTipProperty); set => base.SetValue(TabToolTipProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsSelectedProperty;

    [MirrorVerified("Release IL")]
    public bool IsSelected { get => (bool)base.GetValue(IsSelectedProperty); set => base.SetValue(IsSelectedProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty CanAutoHideProperty;

    [MirrorVerified("Release IL")]
    public bool CanAutoHide { get => (bool)base.GetValue(CanAutoHideProperty); set => base.SetValue(CanAutoHideProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty CanCloseProperty;

    [MirrorVerified("Release IL")]
    public bool CanClose { get => (bool)base.GetValue(CanCloseProperty); set => base.SetValue(CanCloseProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty CanDockAsDocumentProperty;

    [MirrorVerified("Release IL")]
    public bool CanDockAsDocument { get => (bool)base.GetValue(CanDockAsDocumentProperty); set => base.SetValue(CanDockAsDocumentProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty CanDockBottomProperty;

    [MirrorVerified("Release IL")]
    public bool CanDockBottom { get => (bool)base.GetValue(CanDockBottomProperty); set => base.SetValue(CanDockBottomProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty CanDockLeftProperty;

    [MirrorVerified("Release IL")]
    public bool CanDockLeft { get => (bool)base.GetValue(CanDockLeftProperty); set => base.SetValue(CanDockLeftProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty CanDockRightProperty;

    [MirrorVerified("Release IL")]
    public bool CanDockRight { get => (bool)base.GetValue(CanDockRightProperty); set => base.SetValue(CanDockRightProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty CanDockTopProperty;

    [MirrorVerified("Release IL")]
    public bool CanDockTop { get => (bool)base.GetValue(CanDockTopProperty); set => base.SetValue(CanDockTopProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty CanFloatProperty;

    [MirrorVerified("Release IL")]
    public bool CanFloat { get => (bool)base.GetValue(CanFloatProperty); set => base.SetValue(CanFloatProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty CanTearOffProperty;

    [MirrorVerified("Release IL")]
    public bool CanTearOff { get => (bool)base.GetValue(CanTearOffProperty); set => base.SetValue(CanTearOffProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsAutoHideProperty;

    [MirrorVerified("Release IL")]
    public bool IsAutoHide { get => (bool)base.GetValue(IsAutoHideProperty); set => base.SetValue(IsAutoHideProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty AutoHideOpenProperty;

    [MirrorVerified("Release IL")]
    public bool AutoHideOpen { get => (bool)base.GetValue(AutoHideOpenProperty); set => base.SetValue(AutoHideOpenProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty OptionsMenuProperty;

    [MirrorVerified("Release IL")]
    public bool OptionsMenu { get => (bool)base.GetValue(OptionsMenuProperty); set => base.SetValue(OptionsMenuProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty CustomOptionsMenuProperty;

    [MirrorVerified("Release IL")]
    public ContextMenu CustomOptionsMenu { get => (ContextMenu)base.GetValue(CustomOptionsMenuProperty); set => base.SetValue(CustomOptionsMenuProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty ImageProperty;

    [MirrorVerified("Release IL")]
    public object Image { get => base.GetValue(ImageProperty); set => base.SetValue(ImageProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty ImageSourceProperty;

    [MirrorVerified("Release IL")]
    public string ImageSource { get => (string)base.GetValue(ImageSourceProperty); set => base.SetValue(ImageSourceProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty DescriptionProperty;

    [MirrorVerified("Release IL")]
    public string Description { get => (string)base.GetValue(DescriptionProperty); set => base.SetValue(DescriptionProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty FloatingRectProperty;

    [MirrorVerified("Release IL")]
    public Rect FloatingRect { get => (Rect)base.GetValue(FloatingRectProperty); set => base.SetValue(FloatingRectProperty, value); }

    [MirrorVerified("Release IL")]
    private string m_LastDockWindowGroupId;

    [MirrorVerified("Release IL")]
    internal string LastDockWindowGroupId { get => m_LastDockWindowGroupId; set => m_LastDockWindowGroupId = value; }

    [MirrorVerified("Release IL")]
    private Dock m_LastDock;

    [MirrorVerified("Release IL")]
    internal Dock LastDock { get => m_LastDock; set => m_LastDock = value; }

    [MirrorVerified("Release IL")]
    private DockWindowGroup m_LastDockWindowGroup;

    [MirrorVerified("Release IL")]
    internal DockWindowGroup LastDockWindowGroup { get => m_LastDockWindowGroup; set => m_LastDockWindowGroup = value; }

    [MirrorVerified("Release IL")]
    private Size m_LastDockedSize;

    [MirrorVerified("Release IL")]
    internal Size LastDockedSize { get => m_LastDockedSize; set => m_LastDockedSize = value; }

    [MirrorVerified("Release IL")]
    private Size m_LastAutoHideSize;

    [MirrorVerified("Release IL")]
    internal Size LastAutoHideSize { get => m_LastAutoHideSize; set => m_LastAutoHideSize = value; }

    [MirrorVerified("Release IL")]
    public event EventHandler DockParentChanged;

    partial class DockInfoStart
    {
        [MirrorVerified("Release IL")]
        public DockWindowGroup Group;

        [MirrorVerified("Release IL")]
        public bool DragSelectedTab;

        [MirrorVerified("Release IL")]
        public eEventActionSource Source;

        [MirrorVerified("Release IL")]
        public DockInfoStart(DockWindowGroup group, bool dragSelectedTab, eEventActionSource source)
        {
            Group = group;
            DragSelectedTab = dragSelectedTab;
            Source = source;
        }
    }
}
