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
    public void d(IMathNodeToVisualNodeMap mathNodeToVisualNodeMap, TypeoverChangeEventArgs typeoverChangeEventArgs) => a(mathNodeToVisualNodeMap, typeoverChangeEventArgs.State);

    [MirrorVerified("Release IL")]
    private static bool e(MathNode mathNode) => mathNode.CarriesValue;

    [MirrorVerified("Release IL")]
    private static bool d(MathNode mathNode) => mathNode.IsPlaceholder;

    [MirrorVerified("Release IL")]
    private static bool b(MathNode mathNode) => mathNode.IsLineSplit;

    [MirrorVerified("Release IL")]
    private static bool a(MathNode mathNode) => mathNode.NoLogicOnlyVisual;

    partial struct a_
    {
        [MirrorVerified("Release IL")]
        public readonly Predicate<MathNode> a;

        [MirrorVerified("Release IL")]
        public readonly DelegateFunction3<IVisualMathNodeEditor, IVisualMathNode, NavigatorState, IMathNodeToVisualNodeMap> b;

        [MirrorVerified("Release IL")]
        public a_(Predicate<MathNode> predicate, DelegateFunction3<IVisualMathNodeEditor, IVisualMathNode, NavigatorState, IMathNodeToVisualNodeMap> callback)
        {
            a = predicate;
            b = callback;
        }
    }

    partial class c_
    {
        [MirrorVerified("Release IL")]
        public c_() { }

        [MirrorVerified("Release IL")]
        internal bool a(MathNode mathNode) => mathNode.IsReadonly;

        [MirrorVerified("Release IL")]
        internal MathNode a(MathNodeAnalysisHandle mathNodeAnalysisHandle) => mathNodeAnalysisHandle.Node;
    }

    partial class d_
    {
        [MirrorVerified("Release IL")]
        public d_() { }
    }
}
