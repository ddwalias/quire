#nullable disable
using System.Windows;
using System.Windows.Input;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class PaneItem
{
    [MirrorVerified("Release IL")]
    private bool _MouseLeftButtonDown;

    [MirrorVerified("Release IL")]
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        _MouseLeftButtonDown = true;
        base.CaptureMouse();
        base.OnMouseLeftButtonDown(e);
    }

    [MirrorVerified("Release IL")]
    protected virtual void OnSelected(RoutedEventArgs e) => RaiseSelectedEvents(true, e);

    [MirrorVerified("Release IL")]
    protected virtual void OnUnselected(RoutedEventArgs e) => RaiseSelectedEvents(false, e);

    [MirrorVerified("Release IL")]
    private void RaiseSelectedEvents(bool newValue, RoutedEventArgs e) => base.RaiseEvent(e);

    [MirrorVerified("Release IL")]
    private static void VisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((PaneItem)d).VisibilityChanged();

    [MirrorVerified("Release IL")]
    private static void ImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((PaneItem)d).ImageSourceChanged();

    [MirrorVerified("Release IL")]
    private static void ImageSmallSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((PaneItem)d).ImageSmallSourceChanged();

    [MirrorVerified("Release IL")]
    private void ImageSmallSourceChanged() => ImageSmall = LoadImage(ImageSmallSource);

    [MirrorVerified("Release IL")]
    private static void SmallStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((PaneItem)d).SmallStateChanged();

    [MirrorVerified("Release IL")]
    private void SmallStateChanged() => UpdateToolTip();

    [MirrorVerified("Release IL")]
    private static void PaneExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((PaneItem)d).PaneExpandedChanged();

    [MirrorVerified("Release IL")]
    private void PaneExpandedChanged() => UpdateToolTip();

    [MirrorVerified("Release IL")]
    public PaneItem() { }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsSelectedProperty;

    [MirrorVerified("Release IL")]
    public bool IsSelected { get => (bool)base.GetValue(IsSelectedProperty); set => base.SetValue(IsSelectedProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty ImageProperty;

    [MirrorVerified("Release IL")]
    public object Image { get => base.GetValue(ImageProperty); set => base.SetValue(ImageProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty ImageSourceProperty;

    [MirrorVerified("Release IL")]
    public string ImageSource { get => (string)base.GetValue(ImageSourceProperty); set => base.SetValue(ImageSourceProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty ImageSmallProperty;

    [MirrorVerified("Release IL")]
    public object ImageSmall { get => base.GetValue(ImageSmallProperty); set => base.SetValue(ImageSmallProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty ImageSmallSourceProperty;

    [MirrorVerified("Release IL")]
    public string ImageSmallSource { get => (string)base.GetValue(ImageSmallSourceProperty); set => base.SetValue(ImageSmallSourceProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty TitleProperty;

    [MirrorVerified("Release IL")]
    public string Title { get => (string)base.GetValue(TitleProperty); set => base.SetValue(TitleProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty SmallStateProperty;

    [MirrorVerified("Release IL")]
    public bool SmallState { get => (bool)base.GetValue(SmallStateProperty); set => base.SetValue(SmallStateProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty PaneExpandedProperty;

    [MirrorVerified("Release IL")]
    public bool PaneExpanded { get => (bool)base.GetValue(PaneExpandedProperty); set => base.SetValue(PaneExpandedProperty, value); }
}
