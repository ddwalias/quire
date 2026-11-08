namespace Quire.EquationEditor.View
{
    public partial interface IVisualFactory
    {
        IVisualMathNode CreateEmptyEvalResult();

        IVisualTextNode CreateTextNode();

        IVisualTextNode CreateNumericNode();

        IVisualIdNode CreateIdNode();

        IVisualTextNode CreateStringNode();

        IVisualMathNode CreateComboBoxControl();

        IVisualMathNode CreateInputControl(global::Quire.EquationEditor.Tree.MathNode mathNode);

        IVisualBinaryOperatorNode CreateInlineBinaryNode(char symbol, global::Quire.EquationEditor.Tree.SymbolMargin margin);

        IVisualBinaryOperatorNode CreateBiLinearBinaryNode(global::Quire.EquationEditor.Tree.MathNode biLinearNode);

        IVisualMathNode CreateSingleLineBinaryBase(global::Quire.EquationEditor.Tree.MathNodeDescriptor descriptor);

        IVisualUnaryOperatorNode CreateInlineUnaryNode(char symbol, global::Quire.EquationEditor.Tree.SymbolMargin margin, bool isPrefix);

        IVisualBinaryOperatorNode CreateDivNode();

        IVisualBinaryOperatorNode CreateExponentNode();

        IVisualBinaryOperatorNode CreateArgSeparatorNode();

        IVisualOutfixNode CreateGroupingNode();

        IVisualOutfixNode CreateAbsoluteValueNode();

        IVisualMathNode CreateNthRootNode();

        IVisualPlaceholderNode CreatePlaceholderNode();

        IVisualMathNode CreateEvaluationNode();

        IVisualMathNode CreateSymbolicEvaluationNode();

        IVisualApply CreateApply();

        IVisualMatrixNode CreateMatrixNode();

        IVisualAtomicTextLeafNode CreateCollapsedMatrixNode();

        IVisualBinaryOperatorNode CreateSubscriptNode();

        IVisualOutfixNode CreateDeterminate();

        IVisualUnaryOperatorNode CreateTransposeNode();

        IVisualBinaryOperatorNode CreateMatrixRowNode();

        IVisualBinaryOperatorNode CreateMatrixColNode();

        IVisualOutfixNode CreateMatrixRowContentNode();

        IVisualOutfixNode CreateMatrixColContentNode();

        IVisualMathNode CreateIntegralNode();

        IVisualMathNode CreateDerivativeNode();

        IVisualMathNode CreatePartialDerivativeNode();

        IVisualMathNode CreateSummationNode();

        IVisualMathNode CreateProductNode();

        IVisualMathNode CreateStepRangeNode();

        IVisualMathNode CreateVectorizationNode();

        IVisualMathNode CreateConjugateNode();

        IVisualUnaryOperatorNode CreateProgramNode();

        IVisualUnaryOperatorNode CreateBodyNode();

        IVisualBinaryOperatorNode CreateWhileLoopNode();

        IVisualMathNode CreateForLoopNode();

        IVisualLeafOperatorNode CreateBreakNode();

        IVisualLeafOperatorNode CreateContinueNode();

        IVisualUnaryOperatorNode CreateReturnNode();

        IVisualPairNaryNode CreateIfNode();

        IVisualMathNode CreateTryNode();

        IVisualPairNaryNode CreateIfNode(global::Quire.EquationEditor.Tree.MathNode node);

        IVisualMathNode CreateSymKeywordStackNode();

        IVisualMathNode CreateLimitNode();

        IVisualLimitSideNode CreateLimitSideNode();

        IVisualMathNode CreateGradientNode();

        IVisualScoper CreateScopingEditor();

        IVisualPlaceholderNodeEditor CreatePlaceholderEditor(IVisualPlaceholderNode node);

        IVisualVariantLineBinaryOperatorNodeEditor CreateBinaryOperatorEditor(IVisualBinaryOperatorNode node, bool isReadonly);

        IVisualOutfixNodeEditor CreateOutfixEditor(IVisualOutfixNode node);

        IVisualOutfixNodeEditor CreateMatrixEditor(IVisualOutfixNode node);

        IVisualInputControlEditor CreateInputControlInputFieldEditor(IVisualMathNode node);

        IVisualAtomicTextLeafEditor CreateAtomicTextEditor(IVisualAtomicTextLeafNode node);

        IVisualOutfixNodeEditor CreateIntegralEditor(IVisualMathNode node);

        IVisualUnaryEditor CreateDerivativeEditor(IVisualMathNode node);

        IVisualOutfixNodeEditor CreateStepRangeEditor(IVisualMathNode node);

        IVisualUnaryEditor CreateUnaryEditor(IVisualUnaryOperatorNode node);

        IVisualUnaryEditor CreateRangedPrefixNodeEditor(IVisualMathNode node);

        IVisualUnaryEditor CreateVectorizationEditor(IVisualMathNode node);

        IVisualUnaryEditor CreateConjugateEditor(IVisualMathNode node);

        IVisualBinaryOperatorNodeEditor CreateNthRootEditor(IVisualMathNode node);

        IVisualSymbolicEvaluationNodeEditor CreateSymbolicEvaluationEditor(IVisualMathNode node);

        IVisualIdNodeEditor CreateIdentifierEditor(IVisualIdNode visual);

        IVisualTextNodeEditor CreateTextEditor(IVisualTextNode node);

        IVisualUnaryEditor CreateProgramEditor(IVisualMathNode visual);

        IVisualBinaryOperatorNodeEditor CreateLoopEditor(IVisualMathNode visual);

        IVisualOutfixNodeEditor CreateForLoopEditor(IVisualMathNode visual);

        IVisualLeafOperatorEditor CreateLeafOperatorEditor(IVisualLeafOperatorNode visual);

        IVisualOutfixNodeEditor CreateTryEditor(IVisualMathNode visual);

        IVisualPairNaryNodeEditor CreatePairNaryEditor(IVisualPairNaryNode visual);

        IVisualMathNodeEditor CreateLimitSideEditor(IVisualLimitSideNode visual);

        IVisualUnaryEditor CreateLimitEditor(IVisualMathNode node);

        IVisualMultiLineWrappingBinaryOperatorNodeEditor CreateBinaryWrappingOperatorEditor(IVisualBinaryOperatorNode node);

        IVisualInputControlEditor CreateInputControlEditor(IVisualInputControlNode visualInputControlNode);

        IVisualUnaryEditor CreateGradientEditor(IVisualMathNode node);

        IVisualMathNode CreateDefinitionNode();
    }
}
