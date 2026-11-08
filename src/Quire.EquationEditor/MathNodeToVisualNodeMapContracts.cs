namespace Quire.EquationEditor
{
    public partial interface IMathNodeToVisualNodeMap
    {
        Tree.MathNode RootModelNode { get; }

        View.IVisualMathNode RootVisualNode { get; }

        void Regenerate(Tree.MathNode equationRootNode, global::Quire.Controls.Core.IMathFormat MathFormatToApply, bool overideLabelColor, object getValue);

        Tree.MathNode AssociateNode(View.IVisualMathNode visual);

        View.IVisualMathNode AssociateNode(Tree.MathNode modelNode);

        global::System.Collections.Generic.List<global::System.Collections.Generic.List<View.IVisualMathNode>> AssociateNodesCollection(global::System.Collections.Generic.IList<global::System.Collections.Generic.IList<Tree.MathNode>> nodesToMap);

        global::System.Collections.Generic.IEnumerable<T> AllVisualNodesOfType<T>() where T : View.IVisualMathNode;
    }
}
