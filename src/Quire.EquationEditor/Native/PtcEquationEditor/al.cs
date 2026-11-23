#nullable disable
using Quire.EquationEditor.Tree;
using Quire.Mirror;

namespace Quire.Native.PtcEquationEditor;

internal partial class al
{
    [MirrorVerified("Release IL")]
    public al(MathNode A_0, Direction A_1) : this(A_0, A_0, A_1) { }

    [MirrorVerified("Release IL")]
    public bool e() => SegmentIsSingleNode;
}
