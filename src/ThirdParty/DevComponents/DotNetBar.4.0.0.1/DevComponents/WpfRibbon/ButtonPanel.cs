#nullable disable
using System.ComponentModel;
using System.Windows;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class ButtonPanel
{
    [MirrorVerified("Release IL")]
    public virtual ButtonPanel Copy(bool deepCopy) => CloningMachine.Clone(this, deepCopy);

    [MirrorVerified("Release IL")]
    private Size MeasureDefault(Size availableSize) => MeasureDefault(availableSize, true);

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty OrientationProperty;

    [MirrorVerified("Release IL")]
    public eButtonPanelOrientation Orientation { get => (eButtonPanelOrientation)base.GetValue(OrientationProperty); set => base.SetValue(OrientationProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty FixedWidthProperty;

    [MirrorVerified("Release IL")]
    public double FixedWidth { get => (double)base.GetValue(FixedWidthProperty); set => base.SetValue(FixedWidthProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty FixedHeightProperty;

    [MirrorVerified("Release IL")]
    public double FixedHeight { get => (double)base.GetValue(FixedHeightProperty); set => base.SetValue(FixedHeightProperty, value); }

    [MirrorVerified("Release IL")]
    private bool IsDesignMode => DesignerProperties.GetIsInDesignMode(this);

    [MirrorVerified("Release IL")]
    private List<Gallery> m_Galleries;

    [MirrorVerified("Release IL")]
    private bool HasGallery => m_Galleries.Count > 0;
}
