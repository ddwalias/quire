#nullable disable
using Quire.EquationEditor.Tree.Parsing;
using Quire.Mirror;

namespace Quire.Native.PtcEquationEditor;

internal partial class a4
{
    [MirrorVerified("Release IL")]
    private void g(ref LinkedListNode<MathNodeAnalysisHandle> linkedListNode) => throw new ApplicationException("Bad right operand");

    [MirrorVerified("Release IL")]
    private void f(ref LinkedListNode<MathNodeAnalysisHandle> linkedListNode) => throw new ApplicationException("Bad left operand");
}
