#nullable disable
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Quire.Mirror;

namespace Quire.DevComponents.WpfDock;

public partial class DockWindowSelector
{
    [MirrorVerified("Release IL")]
    protected override void OnSelectionChanged(SelectionChangedEventArgs e)
    {
        base.OnSelectionChanged(e);
        UpdateAllSelectedContent();
        CommandManager.InvalidateRequerySuggested();
    }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty ToolWindowsProperty;

    [MirrorVerified("Release IL")]
    private static readonly DependencyPropertyKey ToolWindowsPropertyKey;

    [MirrorVerified("Release IL")]
    public ObservableCollection<DockWindowInfo> ToolWindows { get => (ObservableCollection<DockWindowInfo>)base.GetValue(ToolWindowsProperty); internal set => base.SetValue(ToolWindowsPropertyKey, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty SelectedImageProperty;

    [MirrorVerified("Release IL")]
    private static readonly DependencyPropertyKey SelectedImagePropertyKey;

    [MirrorVerified("Release IL")]
    public object SelectedImage { get => base.GetValue(SelectedImageProperty); internal set => base.SetValue(SelectedImagePropertyKey, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty SelectedHeaderProperty;

    [MirrorVerified("Release IL")]
    private static readonly DependencyPropertyKey SelectedHeaderPropertyKey;

    [MirrorVerified("Release IL")]
    public object SelectedHeader { get => base.GetValue(SelectedHeaderProperty); internal set => base.SetValue(SelectedHeaderPropertyKey, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty SelectedDescriptionProperty;

    [MirrorVerified("Release IL")]
    private static readonly DependencyPropertyKey SelectedDescriptionPropertyKey;

    [MirrorVerified("Release IL")]
    public string SelectedDescription { get => (string)base.GetValue(SelectedDescriptionProperty); internal set => base.SetValue(SelectedDescriptionPropertyKey, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty SelectedDockWindowProperty;

    [MirrorVerified("Release IL")]
    private static readonly DependencyPropertyKey SelectedDockWindowPropertyKey;

    [MirrorVerified("Release IL")]
    public DockWindow SelectedDockWindow { get => (DockWindow)base.GetValue(SelectedDockWindowProperty); internal set => base.SetValue(SelectedDockWindowPropertyKey, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty ActiveFilesLabelProperty;

    [MirrorVerified("Release IL")]
    public string ActiveFilesLabel { get => (string)base.GetValue(ActiveFilesLabelProperty); set => base.SetValue(ActiveFilesLabelProperty, value); }

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty ActiveToolWindowsLabelProperty;

    [MirrorVerified("Release IL")]
    public string ActiveToolWindowsLabel { get => (string)base.GetValue(ActiveToolWindowsLabelProperty); set => base.SetValue(ActiveToolWindowsLabelProperty, value); }
}
