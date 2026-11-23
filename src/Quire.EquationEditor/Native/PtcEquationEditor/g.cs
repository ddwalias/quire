#nullable disable
using Quire.EquationEditor;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Navigation;
using Quire.EquationEditor.Tree.Parsing;
using Quire.EquationEditor.View;
using Quire.Mirror;

namespace Quire.Native.PtcEquationEditor;

internal partial class g
{
    [MirrorVerified("Release IL")]
    public void d(IMathNodeToVisualNodeMap A_0, TypeoverChangeEventArgs A_1) => a(A_0, A_1.State);

    [MirrorVerified("Release IL")]
    private static bool e(MathNode A_0) => A_0.CarriesValue;

    [MirrorVerified("Release IL")]
    private static bool d(MathNode A_0) => A_0.IsPlaceholder;

    [MirrorVerified("Release IL")]
    private static bool b(MathNode A_0) => A_0.IsLineSplit;

    [MirrorVerified("Release IL")]
    private static bool a(MathNode A_0) => A_0.NoLogicOnlyVisual;

    partial struct a_
    {
        [MirrorVerified("Release IL")]
        public readonly Predicate<MathNode> a;

        [MirrorVerified("Release IL")]
        public readonly DelegateFunction3<IVisualMathNodeEditor, IVisualMathNode, NavigatorState, IMathNodeToVisualNodeMap> b;

        [MirrorVerified("Release IL")]
        public a_(Predicate<MathNode> A_0, DelegateFunction3<IVisualMathNodeEditor, IVisualMathNode, NavigatorState, IMathNodeToVisualNodeMap> A_1)
        {
            a = A_0;
            b = A_1;
        }
    }

    partial class c_
    {
        [MirrorVerified("Release IL")]
        public c_() { }

        [MirrorVerified("Release IL")]
        internal bool a(MathNode A_0) => A_0.IsReadonly;

        [MirrorVerified("Release IL")]
        internal MathNode a(MathNodeAnalysisHandle A_0) => A_0.Node;
    }

    partial class d_
    {
        [MirrorVerified("Release IL")]
        public d_() { }
    }
}
