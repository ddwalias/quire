namespace Quire.Controls.Internal;

internal partial class VisualFactory
{

    public Quire.EquationEditor.View.IVisualMathNode CreateEmptyEvalResult()
    {
        return new VisualEmptyResult();
    }

    public Quire.EquationEditor.View.IVisualTextNode CreateTextNode()
    {
        return new VisualTextNode();
    }

    public Quire.EquationEditor.View.IVisualTextNode CreateNumericNode()
    {
        return new VisualNumericNode();
    }

    public Quire.EquationEditor.View.IVisualIdNode CreateIdNode()
    {
        return new VisualIdNode();
    }

    public Quire.EquationEditor.View.IVisualTextNode CreateStringNode()
    {
        return new VisualStringNode();
    }

    public Quire.EquationEditor.View.IVisualLimitSideNode CreateLimitSideNode()
    {
        return new VisualLimitSideNode();
    }

    public Quire.EquationEditor.View.IVisualMathNode CreateComboBoxControl()
    {
        return new VisualComboBoxControlNode();
    }

    public Quire.EquationEditor.View.IVisualMathNode CreateInputControl(Quire.EquationEditor.Tree.MathNode mathNode)
    {
        return new VisualInputScriptableControlNode(mathNode.DescriptorName, mathNode, _region);
    }

    public Quire.EquationEditor.View.IVisualBinaryOperatorNode CreateInlineBinaryNode(char symbol, Quire.EquationEditor.Tree.SymbolMargin margin)
    {
        return new VisualMultiLineAligningBinaryOperatorNode(new VisualSymbolNode(symbol, margin));
    }

    public Quire.EquationEditor.View.IVisualUnaryOperatorNode CreateInlineUnaryNode(char symbol, Quire.EquationEditor.Tree.SymbolMargin margin, bool isPrefix)
    {
        return new VisualLinearUnaryNode(new VisualSymbolNode(symbol, margin), isPrefix);
    }

    public Quire.EquationEditor.View.IVisualBinaryOperatorNode CreateDivNode()
    {
        return new VisualFractionOperatorNode();
    }

    public Quire.EquationEditor.View.IVisualBinaryOperatorNode CreateExponentNode()
    {
        return new VisualExponentNode();
    }

    public Quire.EquationEditor.View.IVisualBinaryOperatorNode CreateSubscriptNode()
    {
        return new VisualSubscriptOperatorNode();
    }

    public Quire.EquationEditor.View.IVisualUnaryOperatorNode CreateTransposeNode()
    {
        return new VisualTransposeNode();
    }

    public Quire.EquationEditor.View.IVisualBinaryOperatorNode CreateMatrixRowNode()
    {
        return new VisualMatrixSuperscriptBinaryNode();
    }

    public Quire.EquationEditor.View.IVisualBinaryOperatorNode CreateMatrixColNode()
    {
        return new VisualMatrixSuperscriptBinaryNode();
    }

    public Quire.EquationEditor.View.IVisualMathNode CreateIntegralNode()
    {
        return new VisualIntegralOperatorNode();
    }

    public Quire.EquationEditor.View.IVisualMathNode CreateDerivativeNode()
    {
        return new VisualDerivativeOperatorNode();
    }

    public Quire.EquationEditor.View.IVisualMathNode CreatePartialDerivativeNode()
    {
        return new VisualPartialDerivativeOperatorNode();
    }

    public Quire.EquationEditor.View.IVisualMathNode CreateStepRangeNode()
    {
        return new VisualStepRangeOperatorNode();
    }

    public Quire.EquationEditor.View.IVisualMathNode CreateVectorizationNode()
    {
        return new VisualVectorizationOperatorNode();
    }

    public Quire.EquationEditor.View.IVisualMathNode CreateConjugateNode()
    {
        return new VisualConjugateOperatorNode();
    }

    public Quire.EquationEditor.View.IVisualMathNode CreateEvaluationNode()
    {
        return new VisualEvalNode();
    }

    public Quire.EquationEditor.View.IVisualMathNode CreateSymbolicEvaluationNode()
    {
        return new VisualSymbolicEvaluationNode();
    }

    public Quire.EquationEditor.View.IVisualMathNode CreateNthRootNode()
    {
        return new VisualNthRootOperatorNode();
    }

    public Quire.EquationEditor.View.IVisualPlaceholderNode CreatePlaceholderNode()
    {
        return new VisualPlaceholderNode();
    }

    public Quire.EquationEditor.View.IVisualApply CreateApply()
    {
        return new VisualApply();
    }

    public Quire.EquationEditor.View.IVisualMatrixNode CreateMatrixNode()
    {
        return new VisualMatrixNode();
    }

    public Quire.EquationEditor.View.IVisualAtomicTextLeafNode CreateCollapsedMatrixNode()
    {
        return new VisualAtomicTextLeafNode();
    }

    public Quire.EquationEditor.View.IVisualUnaryOperatorNode CreateProgramNode()
    {
        return new VisualProgramNode(true);
    }

    public Quire.EquationEditor.View.IVisualUnaryOperatorNode CreateBodyNode()
    {
        return new VisualProgramNode(false);
    }

    public Quire.EquationEditor.View.IVisualMathNode CreateForLoopNode()
    {
        return new VisualForLoopNode();
    }

    public Quire.EquationEditor.View.IVisualMathNode CreateTryNode()
    {
        return new VisualTryNode();
    }

    public Quire.EquationEditor.View.IVisualMathNode CreateSymKeywordStackNode()
    {
        return new VisualSymKeywordsStackNode(_region);
    }

    public Quire.EquationEditor.View.IVisualMathNode CreateLimitNode()
    {
        return new VisualLimitOperatorNode();
    }

    public Quire.EquationEditor.View.IVisualMathNode CreateGradientNode()
    {
        return new VisualGradientOperatorNode();
    }

    public Quire.EquationEditor.View.IVisualScoper CreateScopingEditor()
    {
        return new VisualScoper(_region);
    }

    public Quire.EquationEditor.View.IVisualOutfixNode CreateMatrixRowContentNode()
    {
        return VisualSuperscriptOutfixOperatorNode.CreateVerticalBrackets();
    }

    public Quire.EquationEditor.View.IVisualOutfixNode CreateMatrixColContentNode()
    {
        return VisualSuperscriptOutfixOperatorNode.CreateHorizontalBrackets();
    }

    public Quire.EquationEditor.View.IVisualOutfixNode CreateGroupingNode()
    {
        return VisualLinearOutfixNode.CreateParens();
    }

    public Quire.EquationEditor.View.IVisualOutfixNode CreateAbsoluteValueNode()
    {
        return VisualLinearOutfixNode.CreateAbsoluteValue();
    }

    public Quire.EquationEditor.View.IVisualOutfixNode CreateDeterminate()
    {
        return VisualLinearOutfixNode.CreateDeterminate();
    }

    public Quire.EquationEditor.View.IVisualBinaryOperatorNode CreateWhileLoopNode()
    {
        return new VisualLoopNode("while");
    }

    public Quire.EquationEditor.View.IVisualLeafOperatorNode CreateBreakNode()
    {
        return new VisualLeafOperatorNode("break");
    }

    public Quire.EquationEditor.View.IVisualLeafOperatorNode CreateContinueNode()
    {
        return new VisualLeafOperatorNode("continue");
    }

    public Quire.EquationEditor.View.IVisualIdNodeEditor CreateIdentifierEditor(Quire.EquationEditor.View.IVisualIdNode visual)
    {
        return new VisualIdentifierNodeEditor((VisualIdNode)visual, _region);
    }

    public Quire.EquationEditor.View.IVisualTextNodeEditor CreateTextEditor(Quire.EquationEditor.View.IVisualTextNode node)
    {
        return new VisualTextNodeEditor(_region, (VisualTextNodeForEditor)node);
    }

    public Quire.EquationEditor.View.IVisualUnaryEditor CreateProgramEditor(Quire.EquationEditor.View.IVisualMathNode visual)
    {
        return new VisualProgramNodeEditor((VisualProgramNode)visual, _region);
    }

    public Quire.EquationEditor.View.IVisualBinaryOperatorNodeEditor CreateLoopEditor(Quire.EquationEditor.View.IVisualMathNode visual)
    {
        return new VisualLoopNodeEditor((VisualLoopNode)visual, _region);
    }

    public Quire.EquationEditor.View.IVisualOutfixNodeEditor CreateForLoopEditor(Quire.EquationEditor.View.IVisualMathNode visual)
    {
        return new VisualForLoopNodeEditor((VisualForLoopNode)visual, _region);
    }

    public Quire.EquationEditor.View.IVisualLeafOperatorEditor CreateLeafOperatorEditor(Quire.EquationEditor.View.IVisualLeafOperatorNode visual)
    {
        return new VisualLeafOperatorEditor((VisualLeafOperatorNode)visual, _region);
    }

    public Quire.EquationEditor.View.IVisualOutfixNodeEditor CreateTryEditor(Quire.EquationEditor.View.IVisualMathNode visual)
    {
        return new VisualTryNodeEditor((VisualTryNode)visual, _region);
    }

    public Quire.EquationEditor.View.IVisualPairNaryNodeEditor CreatePairNaryEditor(Quire.EquationEditor.View.IVisualPairNaryNode visual)
    {
        return new VisualPairNaryNodeEditor((VisualPairNaryNode)visual, _region);
    }

    public Quire.EquationEditor.View.IVisualPlaceholderNodeEditor CreatePlaceholderEditor(Quire.EquationEditor.View.IVisualPlaceholderNode node)
    {
        return new VisualPlaceholderNodeEditor((VisualMathNode)node, _region);
    }

    public Quire.EquationEditor.View.IVisualVariantLineBinaryOperatorNodeEditor CreateBinaryOperatorEditor(Quire.EquationEditor.View.IVisualBinaryOperatorNode node, bool isEditable)
    {
        return new VisualBinaryOperatorNodeEditor((VisualBinaryOperatorNode)node, _region, isEditable, false);
    }

    public Quire.EquationEditor.View.IVisualMultiLineWrappingBinaryOperatorNodeEditor CreateBinaryWrappingOperatorEditor(Quire.EquationEditor.View.IVisualBinaryOperatorNode node)
    {
        return new VisualBinaryWrappOperatorNodeEditor((VisualMultiLineWrappingBinaryOperatorNode)node, _region);
    }

    public Quire.EquationEditor.View.IVisualOutfixNodeEditor CreateOutfixEditor(Quire.EquationEditor.View.IVisualOutfixNode node)
    {
        return new VisualOutfixNodeEditor((VisualOutfixNode)node, _region);
    }

    public Quire.EquationEditor.View.IVisualOutfixNodeEditor CreateMatrixEditor(Quire.EquationEditor.View.IVisualOutfixNode node)
    {
        return new VisualMatrixNodeEditor((VisualOutfixNode)node, _region);
    }

    public Quire.EquationEditor.View.IVisualInputControlEditor CreateInputControlInputFieldEditor(Quire.EquationEditor.View.IVisualMathNode node)
    {
        return new VisualInputScriptableControlNodeEditor((VisualMathNode)node, _region);
    }

    public Quire.EquationEditor.View.IVisualAtomicTextLeafEditor CreateAtomicTextEditor(Quire.EquationEditor.View.IVisualAtomicTextLeafNode node)
    {
        return new VisualAtomicTextLeafEditor((VisualAtomicTextLeafNode)node, _region);
    }

    public Quire.EquationEditor.View.IVisualUnaryEditor CreateRangedPrefixNodeEditor(Quire.EquationEditor.View.IVisualMathNode node)
    {
        return new VisualRangedPrefixOperatorNodeEditor((VisualRangedPrefixNode)node, _region);
    }

    public Quire.EquationEditor.View.IVisualSymbolicEvaluationNodeEditor CreateSymbolicEvaluationEditor(Quire.EquationEditor.View.IVisualMathNode node)
    {
        return new VisualSymbolicEvaluationNodeEditor((VisualSymbolicEvaluationNode)node, _region);
    }

    public Quire.EquationEditor.View.IVisualUnaryEditor CreateUnaryEditor(Quire.EquationEditor.View.IVisualUnaryOperatorNode node)
    {
        return new VisualLinearUnaryEditor((VisualUnaryOperatorNodeForEditor)node, _region);
    }

    public Quire.EquationEditor.View.IVisualOutfixNodeEditor CreateIntegralEditor(Quire.EquationEditor.View.IVisualMathNode node)
    {
        return new VisualIntegralNodeEditor((VisualIntegralOperatorNode)node, _region);
    }

    public Quire.EquationEditor.View.IVisualUnaryEditor CreateDerivativeEditor(Quire.EquationEditor.View.IVisualMathNode node)
    {
        return new VisualDerivativeNodeEditor((VisualDerivativeOperatorNodeBase)node, _region);
    }

    public Quire.EquationEditor.View.IVisualOutfixNodeEditor CreateStepRangeEditor(Quire.EquationEditor.View.IVisualMathNode node)
    {
        return new VisualStepRangeOperatorNodeEditor((VisualStepRangeOperatorNode)node, _region);
    }

    public Quire.EquationEditor.View.IVisualUnaryEditor CreateVectorizationEditor(Quire.EquationEditor.View.IVisualMathNode node)
    {
        return new VisualVectorizationNodeEditor((VisualVectorizationOperatorNode)node, _region);
    }

    public Quire.EquationEditor.View.IVisualUnaryEditor CreateConjugateEditor(Quire.EquationEditor.View.IVisualMathNode node)
    {
        return new VisualConjugateNodeEditor((VisualConjugateOperatorNode)node, _region);
    }

    public Quire.EquationEditor.View.IVisualBinaryOperatorNodeEditor CreateNthRootEditor(Quire.EquationEditor.View.IVisualMathNode node)
    {
        return new VisualNthRootNodeEditor((VisualNthRootOperatorNode)node, _region);
    }

    public Quire.EquationEditor.View.IVisualMathNodeEditor CreateLimitSideEditor(Quire.EquationEditor.View.IVisualLimitSideNode visual)
    {
        return new VisualLimitSideNodeEditor((VisualLimitSideNode)visual, _region);
    }

    public Quire.EquationEditor.View.IVisualUnaryEditor CreateLimitEditor(Quire.EquationEditor.View.IVisualMathNode node)
    {
        return new VisualLimitNodeEditor((VisualLimitOperatorNode)node, _region);
    }

    public Quire.EquationEditor.View.IVisualUnaryEditor CreateGradientEditor(Quire.EquationEditor.View.IVisualMathNode node)
    {
        return new VisualGradientNodeEditor((VisualGradientOperatorNode)node, _region);
    }
}
