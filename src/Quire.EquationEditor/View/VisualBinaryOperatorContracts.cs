namespace Quire.EquationEditor.View
{
    public partial interface IVisualBinaryOperatorNodeEditor
    {
        void SetNodeHighlights(params IVisualMathNode[] nodes);

        void AddNodesHighlight(params IVisualMathNode[] nodes);

        void RemoveNodeHighlights();

        void ShowOperatorHighlight(bool show);

        void SelectOperator(bool selected);
    }

    public partial interface IVisualBinaryOperatorNode
    {
        IVisualMathNode FirstOperand { get; set; }

        IVisualMathNode SecondOperand { get; set; }

        IVisualMathNode Operator { get; }
    }
}
