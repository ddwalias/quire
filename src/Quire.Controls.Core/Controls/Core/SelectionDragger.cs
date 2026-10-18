#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class SelectionDragger
{
    [MirrorVerified("PtcControlsCoreSelectionDraggerMechanicalDifferentialTests")]
    public event Action<Point> DragDone;
}
