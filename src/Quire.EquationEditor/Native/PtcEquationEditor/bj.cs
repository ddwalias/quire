#nullable disable
using Quire.EquationEditor.Tree.Parsing;
using Quire.EquationEditor.Tree.Parsing;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Native.PtcEquationEditor;

internal partial class bj
{
    partial class a_
    {
        [MirrorVerified("Release IL")]
        public a_() { }
    }

    partial class a_
    {
        [MirrorVerified("Release IL")]
        internal void a(MathNodeAnalysisHandle mathNodeAnalysisHandle) => b_.Add(mathNodeAnalysisHandle.Node);
    }
}
