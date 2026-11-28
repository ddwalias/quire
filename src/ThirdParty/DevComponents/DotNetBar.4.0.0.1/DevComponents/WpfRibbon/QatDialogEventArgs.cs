#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class QatDialogEventArgs
{
    [MirrorVerified("Release IL")]
    public QatDialogEventArgs() { }

    [MirrorVerified("Release IL")]
    public QatDialogEventArgs(RoutedEvent e) : base(e) { }

    [MirrorVerified("Release IL")]
    public QatDialogEventArgs(RoutedEvent e, object source) : base(e, source) { }
}
