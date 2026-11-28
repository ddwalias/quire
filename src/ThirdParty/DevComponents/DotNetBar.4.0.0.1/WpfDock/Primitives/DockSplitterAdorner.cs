#nullable disable
using System.Windows;
using System.Windows.Media;
using Quire.Mirror;

namespace Quire.DevComponents.WpfDock.Primitives;

internal partial class DockSplitterAdorner
{
    [MirrorVerified("Release IL")]
    public DockSplitterAdorner(DockSplitter adornedElement) : base(adornedElement) { }

    [MirrorVerified("Release IL")]
    private Brush m_PreviewBrush;

    [MirrorVerified("Release IL")]
    public Brush PreviewBrush { get => m_PreviewBrush; set => m_PreviewBrush = value; }
}
