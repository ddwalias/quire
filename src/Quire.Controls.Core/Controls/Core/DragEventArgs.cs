#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class DragEventArgs
{
    public Vector DraggingVector { get; private set; }

    public DragEventArgs(RoutedEvent routedEvent, Vector vector)
        : base(routedEvent)
    {
        DraggingVector = vector;
    }
}
