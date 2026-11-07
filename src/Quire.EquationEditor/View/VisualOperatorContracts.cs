namespace Quire.EquationEditor.View
{
    public partial interface IVisualSymbolicEvaluationNodeEditor
    {
        void AddNodesHighlight(bool onlyCoreElem, params IVisualMathNode[] nodes);
    }

    public partial interface IVisualPlaceholderNodeEditor
    {
        global::Quire.Direction Direction { get; set; }
    }

    public partial interface IVisualVariantLineBinaryOperatorNodeEditor
    {
        void AddNodesHighlightOnlyCoreElements(global::System.Collections.Generic.List<global::System.Collections.Generic.List<IVisualMathNode>> lines);
    }

    public partial interface IVisualVariantLineBinaryOperatorNode
    {
        void ResetOffsets();
    }
}
