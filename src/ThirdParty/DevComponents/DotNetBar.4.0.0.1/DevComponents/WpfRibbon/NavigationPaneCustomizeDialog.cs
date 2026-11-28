#nullable disable
using System.Windows;
using System.Windows.Controls;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class NavigationPaneCustomizeDialog
{
    [MirrorVerified("Release IL")]
    public NavigationPaneCustomizeDialog()
    {
        InitializeComponent();
    }

    [MirrorVerified("Release IL")]
    private bool _DataChanged;

    [MirrorVerified("Release IL")]
    private void CheckedChanged(object sender, RoutedEventArgs e) => _DataChanged = true;

    [MirrorVerified("Release IL")]
    private bool _Reordered;

    [MirrorVerified("Release IL")]
    private void ResetItems(object sender, RoutedEventArgs e)
    {
        Initialize(NavigationPane);
        _Reordered = false;
        _DataChanged = false;
    }

    [MirrorVerified("Release IL")]
    internal ListBox PaneItems;

    [MirrorVerified("Release IL")]
    private PaneItemDescriptionList GetPaneItemDescriptionList() => PaneItems.ItemsSource as PaneItemDescriptionList;

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty NavigationPaneProperty;

    [MirrorVerified("Release IL")]
    public NavigationPane NavigationPane { get => (NavigationPane)base.GetValue(NavigationPaneProperty); set => base.SetValue(NavigationPaneProperty, value); }
}
