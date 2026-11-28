#nullable disable
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.DevComponents.WpfDock;

public partial class SplitPanel
{
    [MirrorVerified("Release IL")]
    protected override Size MeasureOverride(Size availableSize) => MeasureArrange(availableSize, false);

    [MirrorVerified("Release IL")]
    private int GetSplitterHeight() => 4;

    [MirrorVerified("Release IL")]
    private double GetSplitterWidth() => 4.0;

    [MirrorVerified("Release IL")]
    protected override Size ArrangeOverride(Size finalSize) => MeasureArrange(finalSize, true);

    [MirrorVerified("Release IL")]
    protected virtual SplitPanelChildrenCollection CreateUIElementCollection() => new SplitPanelChildrenCollection(this);

    [MirrorVerified("Release IL")]
    public DockSite GetDockSite() => DockSite.GetDockSite(this);

    [MirrorVerified("Release IL")]
    protected virtual bool IsDocument => DockSite.GetIsDocument(this);

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty OrientationProperty;

    [MirrorVerified("Release IL")]
    public Orientation Orientation { get => (Orientation)base.GetValue(OrientationProperty); set => base.SetValue(OrientationProperty, value); }

    [MirrorVerified("Release IL")]
    private List<DockSplitter> m_SplittersIndexed;

    [MirrorVerified("Release IL")]
    public List<DockSplitter> Splitters => m_SplittersIndexed;

    [MirrorVerified("Release IL")]
    private Dictionary<UIElement, DockSplitter> m_Splitters;

    [MirrorVerified("Release IL")]
    protected override int VisualChildrenCount => Children.Count + m_Splitters.Count;

    [MirrorVerified("Release IL")]
    public SplitPanelChildrenCollection Children => InternalChildren;

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty BackgroundProperty;

    [MirrorVerified("Release IL")]
    public Brush Background { get => (Brush)base.GetValue(BackgroundProperty); set => base.SetValue(BackgroundProperty, value); }

    [MirrorVerified("Release IL")]
    private string m_Id;

    [MirrorVerified("Release IL")]
    public string Id => m_Id;

    [MirrorVerified("Release IL")]
    public event EventHandler SplittersUpdating;

    [MirrorVerified("Release IL")]
    public event EventHandler SplittersRecreated;
}
