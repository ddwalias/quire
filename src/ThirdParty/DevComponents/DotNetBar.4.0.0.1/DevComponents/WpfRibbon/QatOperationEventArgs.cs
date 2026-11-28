#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class QatOperationEventArgs
{
    [MirrorVerified("Release IL")]
    public readonly object Item;

    [MirrorVerified("Release IL")]
    public QatOperationEventArgs(object item)
    {
        Item = item;
    }

    [MirrorVerified("Release IL")]
    public QatOperationEventArgs(object item, RoutedEvent e) : base(e)
    {
        Item = item;
    }

    [MirrorVerified("Release IL")]
    public QatOperationEventArgs(object item, RoutedEvent e, object originalSource) : base(e, originalSource)
    {
        Item = item;
    }
}
