#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.DevComponents.WpfDock;

public partial class CancelSourceRoutedEventArgs
{
    [MirrorVerified("Release IL")]
    public CancelSourceRoutedEventArgs() { }

    [MirrorVerified("Release IL")]
    public CancelSourceRoutedEventArgs(RoutedEvent e) : base(e) { }

    [MirrorVerified("Release IL")]
    public CancelSourceRoutedEventArgs(RoutedEvent e, object source) : base(e, source) { }
}
