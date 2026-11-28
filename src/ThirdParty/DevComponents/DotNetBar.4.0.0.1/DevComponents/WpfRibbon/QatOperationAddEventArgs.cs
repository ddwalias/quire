#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class QatOperationAddEventArgs
{
    [MirrorVerified("Release IL")]
    public QatOperationAddEventArgs(object item) : base(item) { }

    [MirrorVerified("Release IL")]
    public QatOperationAddEventArgs(object item, RoutedEvent e) : base(item, e) { }

    [MirrorVerified("Release IL")]
    public QatOperationAddEventArgs(object item, RoutedEvent e, object originalSource) : base(item, e, originalSource) { }
}
