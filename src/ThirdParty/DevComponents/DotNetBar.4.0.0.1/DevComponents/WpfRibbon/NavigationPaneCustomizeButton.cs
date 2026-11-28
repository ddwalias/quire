#nullable disable
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class NavigationPaneCustomizeButton
{
    [MirrorVerified("Release IL")]
    protected override void OnPreviewMouseRightButtonUp(MouseButtonEventArgs e)
    {
        e.Handled = true;
        base.OnPreviewMouseRightButtonUp(e);
    }

    [MirrorVerified("Release IL")]
    private void MenuItemSelectPaneClick(object sender, RoutedEventArgs e) => ((sender as MenuItem).Tag as PaneItem).IsSelected = true;

    [MirrorVerified("Release IL")]
    private void CustomizeMenuOpened(object sender, RoutedEventArgs e) => IsCustomizeMenuOpen = true;

    [MirrorVerified("Release IL")]
    private void ShowNavPaneOptions(object sender, RoutedEventArgs e) => GetNavigationPane().ShowOptionsDialog();

    [MirrorVerified("Release IL")]
    public NavigationPaneCustomizeButton() { }

    [MirrorVerified("Release IL")]
    private static readonly DependencyPropertyKey IsCustomizeMenuOpenPropertyKey;

    [MirrorVerified("Release IL")]
    public static readonly DependencyProperty IsCustomizeMenuOpenProperty;

    [MirrorVerified("Release IL")]
    public bool IsCustomizeMenuOpen { get => (bool)base.GetValue(IsCustomizeMenuOpenProperty); internal set => base.SetValue(IsCustomizeMenuOpenPropertyKey, value); }
}
