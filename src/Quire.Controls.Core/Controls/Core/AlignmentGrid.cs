#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class AlignmentGrid
{
    [MirrorVerified("PtcControlsCoreAlignmentGridMechanicalDifferentialTests")]
    public static Rect DefaultGridArea;

    [MirrorVerified("PtcControlsCoreAlignmentGridMechanicalDifferentialTests")]
    public AlignmentGrid() : this(DefaultGridArea) { }

    [MirrorVerified("PtcControlsCoreAlignmentGridMechanicalDifferentialTests")]
    public virtual void Dispose() { }
}
