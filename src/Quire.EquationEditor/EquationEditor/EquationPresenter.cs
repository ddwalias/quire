#nullable disable
using System.Windows;
using Quire.Controls.Core;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Navigation;
using Quire.EquationEditor.View;
using Quire.Mirror;

namespace Quire.EquationEditor;

public partial class EquationPresenter
{
    [MirrorVerified("Release IL")]
    public EquationPresenter(IEquationModel model, IVisualMathRegionNode view) : this(model, view, null, null, null) { }

    [MirrorVerified("Release IL")]
    public EquationPresenter(IEquationModel model, IVisualMathRegionNode view, IMathNodeToVisualNodeMap map) : this(model, view, map, null, null) { }

    [MirrorVerified("Release IL")]
    private static bool v() => false;

    [MirrorVerified("Release IL")]
    private static bool a(FindReplaceQuery findReplaceQuery) => false;

    [MirrorVerified("Release IL")]
    private void t() { }

    [MirrorVerified("Release IL")]
    private void q() => p();

    [MirrorVerified("Release IL")]
    private void d() => c();

    [MirrorVerified("Release IL")]
    private void a(DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs) => c();

    [MirrorVerified("Release IL")]
    private void a(MathNode mathNode) => b(mathNode);

    partial class c_
    {
        [MirrorVerified("Release IL")]
        public c_() { }
    }
}
