#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class RibbonBar
{
    [MirrorVerified("Release IL")]
    public RibbonBar() { }

    [MirrorVerified("Release IL")]
    internal void InvokeLaunchDialog() => OnLaunchDialog();

    [MirrorVerified("Release IL")]
    protected override void OnInitialized(EventArgs e) => base.OnInitialized(e);

    [MirrorVerified("Release IL")]
    private void LauncherClick(object sender, RoutedEventArgs e)
    {
        OnLaunchDialog();
        e.Handled = true;
    }

    [MirrorVerified("Release IL")]
    internal RibbonBar Copy(bool deepCopy) => CloningMachine.Clone(this, deepCopy);

    [MirrorVerified("Release IL")]
    public static DependencyProperty ResizeOrderIndexProperty;

    [MirrorVerified("Release IL")]
    public int ResizeOrderIndex { get => (int)base.GetValue(ResizeOrderIndexProperty); set => base.SetValue(ResizeOrderIndexProperty, value); }

    [MirrorVerified("Release IL")]
    public static DependencyProperty DialogLauncherVisibleProperty;

    [MirrorVerified("Release IL")]
    public Visibility DialogLauncherVisible { get => (Visibility)base.GetValue(DialogLauncherVisibleProperty); set => base.SetValue(DialogLauncherVisibleProperty, value); }

    [MirrorVerified("Release IL")]
    private bool m_IsCollapsed;

    [MirrorVerified("Release IL")]
    public bool IsCollapsed { get => m_IsCollapsed; internal set => m_IsCollapsed = value; }

    [MirrorVerified("Release IL")]
    public static DependencyProperty CollapsedImageProperty;

    [MirrorVerified("Release IL")]
    public object CollapsedImage { get => base.GetValue(CollapsedImageProperty); set => base.SetValue(CollapsedImageProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty CollapsedHeaderProperty;

    [MirrorVerified("Release IL")]
    public object CollapsedHeader { get => base.GetValue(CollapsedHeaderProperty); set => base.SetValue(CollapsedHeaderProperty, value); }

    [MirrorVerified("Release IL")]
    private bool m_MeasureNeedCollapse;

    [MirrorVerified("Release IL")]
    internal bool MeasureNeedCollapse { get => m_MeasureNeedCollapse; set => m_MeasureNeedCollapse = value; }

    [MirrorVerified("Release IL")]
    private bool m_IsSecondMeasurePass;

    [MirrorVerified("Release IL")]
    internal bool IsSecondMeasurePass { get => m_IsSecondMeasurePass; set => m_IsSecondMeasurePass = value; }

    [MirrorVerified("Release IL")]
    private Size m_LastPanelMeasureSize;

    [MirrorVerified("Release IL")]
    internal Size LastPanelMeasureSize { get => m_LastPanelMeasureSize; set => m_LastPanelMeasureSize = value; }

    [MirrorVerified("Release IL")]
    public static DependencyProperty IsAutoSizeEnabledProperty;

    [MirrorVerified("Release IL")]
    public bool IsAutoSizeEnabled { get => (bool)base.GetValue(IsAutoSizeEnabledProperty); set => base.SetValue(IsAutoSizeEnabledProperty, value); }

    [MirrorVerified("Release IL")]
    private ButtonPanel m_ButtonPanel;

    [MirrorVerified("Release IL")]
    internal ButtonPanel ButtonPanel => m_ButtonPanel;
}
