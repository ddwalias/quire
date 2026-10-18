#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class DragEventArgs
{
    [MirrorVerified("PtcControlsCoreDragEventArgsMechanicalDifferentialTests")]
    public Vector DraggingVector { get; private set; }
}
