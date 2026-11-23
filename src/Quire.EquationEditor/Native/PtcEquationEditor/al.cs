#nullable disable
using Quire.EquationEditor.Tree;
using Quire.Mirror;

namespace Quire.Native.PtcEquationEditor;

internal partial class al
{
    [MirrorVerified("Release IL")]
    public al(MathNode mathNode, Direction direction) : this(mathNode, mathNode, direction) { }

    [MirrorVerified("Release IL")]
    public bool e() => SegmentIsSingleNode;
}
