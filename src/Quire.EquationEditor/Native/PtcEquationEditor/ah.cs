#nullable disable
using Quire.EquationEditor.Tree;
using Quire.Mirror;

namespace Quire.Native.PtcEquationEditor;

internal partial class ah
{
    [MirrorVerified("Release IL")]
    public ah() : this(null) { }

    [MirrorVerified("Release IL")]
    public MathNode Format(t result) => result.bo(this);
}
