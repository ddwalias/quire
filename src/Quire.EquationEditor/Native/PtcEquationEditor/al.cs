#nullable disable
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Native.PtcEquationEditor;

internal partial class al
{
    [MirrorVerified("Release IL")]
    public al(MathNode mathNode, Direction direction) : this(mathNode, mathNode, direction) { }

    [MirrorVerified("Release IL")]
    public bool e() => SegmentIsSingleNode;

    [MirrorVerified("Release IL")]
    public Direction d() => DirectionOps.Opposite(d_);

    [MirrorVerified("Release IL")]
    public Direction c() => d_;
}
