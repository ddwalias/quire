namespace Quire.EquationEditor.View
{
    public partial interface IMathViewTransformer
    {
        IVisualMathNode Transform(
            global::Quire.EquationEditor.Tree.MathNode subTreeRoot,
            ref global::System.Collections.Generic.IDictionary<global::Quire.EquationEditor.Tree.MathNode, IVisualMathNode> modelToViewMap,
            global::Quire.Controls.Core.IMathFormat mathFormatToApply,
            bool overrideLabelColor,
            object labelIdFormatDictionary);
    }

    public partial interface IHighlightRectProvider
    {
        global::System.Windows.Rect HighlightRect();
    }

    public partial interface IVisualAtomicTextLeafEditor
    {
        bool Selected { get; set; }
    }

    public partial interface IVisualInputControlEditor
    {
        CaretStateValues CaretState { get; set; }
    }

    public partial interface IVisualUnaryEditor
    {
        void SelectOperator(bool selected);
    }

    public partial interface IVisualPairNaryNodeEditor
    {
        int? OperatorPartIndex { set; }
    }

    public partial interface IVisualOutfixNodeEditor
    {
        global::Quire.Direction NodeSide { set; }
    }
}
