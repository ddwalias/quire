#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class GalleryScrollViewer
{
    [MirrorVerified("Release IL")]
    private void LineDown() => Animate(VerticalOffset - GetSingleLineOffset());

    [MirrorVerified("Release IL")]
    private void LineUp() => Animate(Math.Min(0.0, VerticalOffset + GetSingleLineOffset()));

    [MirrorVerified("Release IL")]
    public void BringIntoView(double Top) => Animate(-Top);

    [MirrorVerified("Release IL")]
    private bool m_Animation;

    [MirrorVerified("Release IL")]
    internal bool Animation { get => m_Animation; set => m_Animation = value; }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty VerticalOffsetProperty;

    [MirrorVerified("Release IL")]
    public double VerticalOffset { get => (double)base.GetValue(VerticalOffsetProperty); set => base.SetValue(VerticalOffsetProperty, value); }
}
