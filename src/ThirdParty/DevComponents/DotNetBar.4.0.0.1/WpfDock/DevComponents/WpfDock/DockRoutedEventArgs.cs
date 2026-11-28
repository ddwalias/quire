#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.DevComponents.WpfDock;

public partial class DockRoutedEventArgs
{
    [MirrorVerified("Release IL")]
    public DockRoutedEventArgs(RoutedEvent e) : base(e) { }

    [MirrorVerified("Release IL")]
    public DockRoutedEventArgs(RoutedEvent e, object source) : base(e, source) { }

    [MirrorVerified("Release IL")]
    public readonly object DockControl;

    [MirrorVerified("Release IL")]
    public DockRoutedEventArgs(RoutedEvent e, object source, object dockControl, eEventActionSource actionSource) : base(e, source)
    {
        DockControl = dockControl;
        EventActionSource = actionSource;
    }
}
