#nullable disable
using System.Collections.Generic;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Parsing;
using Quire.Mirror;

namespace Quire.Native.PtcEquationEditor;

internal partial class aw
{
    [MirrorVerified("Release IL")]
    public aw() { }

    [MirrorVerified("Release IL")]
    public LinkedList<MathNodeAnalysisHandle> b(MathNode node, params MathNode[] nodes) => a(node, (IEnumerable<MathNode>)nodes);

    [MirrorVerified("Release IL")]
    public static Fixity a(MathNode node, params MathNode[] nodes) => a(node, (ICollection<MathNode>)nodes);

    partial class a_
    {
        [MirrorVerified("Release IL")]
        public a_() { }
    }

    partial class b_
    {
        [MirrorVerified("Release IL")]
        public b_() { }
    }
}
