#nullable disable
using Quire.Calculation.Core;
using Quire.Mirror;

namespace Quire.Native.PtcEquationEditor;

internal partial class x
{
    [MirrorVerified("Release IL")]
    public override void Accept(IResultNodeVisitor visitor) => visitor.Visit(this);

    [MirrorVerified("Release IL")]
    public x() { }
}
