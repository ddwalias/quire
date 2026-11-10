# EquationControl lane checkpoint: visualRest group

- Owner: dispatched EquationControl worker (isualRest group)
- Branch: eature/eq-visualrest (created from main at 56ae77eae)
- Files: 83 Controls.Internal.Visual*.g.cs files in src/Quire.Controls.EquationControl/Generated/Mirror/ (excluding VisualMathRegionNode, VisualInputScriptableControlNode*, VisualInputControlEditor)
- Reference: Ptc.Controls.Internal.Visual* in Ptc.Controls.EquationControl, Release IL
- Baseline gate: PASS; 1733 checked (.mirror-cache/release-il-ce4d7b8a4aea/release-il.json)
- Stubs in scope: 1649 total across 83 types (1611 unblocked, 38 blocked)

## Classification Summary

### Blocked Members by Reason

1. **LoadComponent / XAML**: 0 members blocked. No type in visualRest loads XAML or implements IComponentConnector.
2. **dynamic script engine**: 0 members blocked. Handled in VisualInputScriptableControlNode* (excluded).
3. **SymbolDescriptor constructor**: 0 members blocked. Descriptor tables are in separate files.
4. **type-map ownership**: 0 members blocked. All types belong to Ptc.Controls.EquationControl.dll and are mapped.
5. **closure ordinal (iterators and lambdas)**: 38 members blocked.
   - Iterators (<name>d__*): 13 members (CoreElements, CoreElementsTopLine, Convert).
   - Lambdas / closures (<name>b__*): 25 members (15 methods, 10 constructors with compiler-generated delegates).

### Unblocked Members

- **Total unblocked**: 1611 members.
- Events: 10 declarations.
- Fields: 412 instance and static fields.
- Properties: 418 auto-properties and simple get/set accessors.
- Constructors: 84 constructors without closures.
- Methods: 687 straight-line methods (layout, rendering, navigation, hit-testing, event handlers without closures).

## Per-Type Classification

| Type | Total Stubs | Unblocked | Blocked | Blocked Members & Reasons |
|---|---|---|---|---|
| VisualAtomicTextLeafEditor | 11 | 11 | 0 | None |
| VisualAtomicTextLeafNode | 24 | 24 | 0 | None |
| VisualBinaryOperatorNode | 15 | 15 | 0 | None |
| VisualBinaryOperatorNodeEditor | 22 | 22 | 0 | None |
| VisualBinaryWrappOperatorNodeEditor | 5 | 5 | 0 | None |
| VisualComboBoxControlEditor | 2 | 2 | 0 | None |
| VisualComboBoxControlNode | 24 | 23 | 1 | internal VisualComboBoxControlNode() : base(default(string), default(global::Quire.EquationEditor.Tree.INodeInfo)) (closure ordinal (lambda)) |
| VisualConjugateNodeEditor | 21 | 21 | 0 | None |
| VisualConjugateOperatorNode | 21 | 21 | 0 | None |
| VisualDefinitionNode | 1 | 1 | 0 | None |
| VisualDerivativeNodeEditor | 22 | 22 | 0 | None |
| VisualDerivativeOperatorNodeBase | 28 | 27 | 1 | public override global::System.Collections.Generic.IEnumerable<global::System.Windows.UIElement> CoreElements (closure ordinal (iterator)) |
| VisualDerivativeSymbolNode | 23 | 23 | 0 | None |
| VisualExponentNode | 6 | 6 | 0 | None |
| VisualFactory | 12 | 12 | 0 | None |
| VisualForLoopNode | 28 | 28 | 0 | None |
| VisualForLoopNodeEditor | 23 | 23 | 0 | None |
| VisualFractionOperatorNode | 19 | 19 | 0 | None |
| VisualGradientNodeEditor | 21 | 21 | 0 | None |
| VisualGradientOperatorNode | 18 | 17 | 1 | protected override global::System.Windows.Size MeasureStaticLayout(global::System.Windows.Size availableSize) (closure ordinal (lambda)) |
| VisualIdNode | 19 | 19 | 0 | None |
| VisualIdentifierNodeEditor | 35 | 35 | 0 | None |
| VisualInputControlNode | 25 | 23 | 2 | public VisualInputControlNode(string mathNodeName, global::Quire.EquationEditor.Tree.INodeInfo nodeInfo) (closure ordinal (lambda)), protected override global::System.Windows.Size MeasureStaticLayout(global::System.Windows.Size availableSize) (closure ordinal (lambda)) |
| VisualIntegralNodeEditor | 23 | 23 | 0 | None |
| VisualIntegralOperatorNode | 25 | 25 | 0 | None |
| VisualLayoutManager | 12 | 11 | 1 | public VisualLayoutManager(global::Quire.Controls.Internal.LayoutItem[] childItems, global::Quire.Controls.Internal.LayoutItem[] operatorPartItems) (closure ordinal (lambda)) |
| VisualLeafOperatorEditor | 11 | 11 | 0 | None |
| VisualLeafOperatorNode | 4 | 4 | 0 | None |
| VisualLimitNodeEditor | 22 | 22 | 0 | None |
| VisualLimitOperatorNode | 26 | 25 | 1 | public override global::System.Collections.Generic.IEnumerable<global::System.Windows.UIElement> CoreElements (closure ordinal (iterator)) |
| VisualLimitSideNode | 12 | 12 | 0 | None |
| VisualLimitSideNodeEditor | 14 | 14 | 0 | None |
| VisualLinearBinaryOperatorNode | 18 | 14 | 4 | public override global::System.Collections.Generic.IEnumerable<global::System.Windows.UIElement> CoreElements (closure ordinal (iterator)), public VisualLinearBinaryOperatorNode() (closure ordinal (lambda)), public VisualLinearBinaryOperatorNode(global::Quire.Controls.Internal.VisualSymbolNode opSymbol) (closure ordinal (lambda)), protected override global::System.Windows.Size MeasureStaticLayout(global::System.Windows.Size availableSize) (closure ordinal (lambda)) |
| VisualLinearBinaryOperatorNode | 2 | 2 | 0 | None |
| VisualLinearOutfixNode | 33 | 33 | 0 | None |
| VisualLinearUnaryEditor | 14 | 14 | 0 | None |
| VisualLinearUnaryNode | 19 | 18 | 1 | public override global::System.Collections.Generic.IEnumerable<global::System.Windows.UIElement> CoreElements (closure ordinal (iterator)) |
| VisualMathNode | 54 | 49 | 5 | public virtual global::System.Collections.Generic.IEnumerable<global::System.Windows.UIElement> CoreElements (closure ordinal (iterator)), public virtual global::System.Collections.Generic.IEnumerable<global::System.Windows.UIElement> CoreElementsTopLine (closure ordinal (iterator)), protected VisualMathNode() (closure ordinal (lambda)), protected virtual global::System.Windows.Size MeasureStaticLayout(global::System.Windows.Size availableSize) (closure ordinal (lambda)), protected override void OnRender(global::System.Windows.Media.DrawingContext dc) (closure ordinal (lambda)) |
| VisualMathNodeEditor | 39 | 39 | 0 | None |
| VisualMathNodeHitTestResult | 8 | 8 | 0 | None |
| VisualMatrixContents | 2 | 1 | 1 | public VisualMatrixContents() : base(default(global::Quire.Controls.Core.TableLayoutManager)) (closure ordinal (lambda)) |
| VisualMatrixNode | 69 | 68 | 1 | private void SetupResizeIndicatorBehaviour(global::Quire.Controls.Internal.VisualTextNode resizeIndicator) (closure ordinal (lambda)) |
| VisualMatrixSuperscriptBinaryNode | 4 | 4 | 0 | None |
| VisualMultiLineAligningBinaryOperatorNode | 20 | 20 | 0 | None |
| VisualMultiLineWrappingBinaryOperatorNode | 24 | 23 | 1 | public override global::System.Collections.Generic.IEnumerable<global::System.Windows.UIElement> CoreElementsTopLine (closure ordinal (iterator)) |
| VisualNode | 2 | 2 | 0 | None |
| VisualNthRootNodeEditor | 29 | 28 | 1 | public global::System.Collections.Generic.IEnumerable<global::System.Windows.UIElement> Convert(global::System.Collections.Generic.IEnumerable<global::Quire.EquationEditor.View.IVisualMathNode> unconverted) (closure ordinal (iterator)) |
| VisualNthRootOperatorNode | 29 | 29 | 0 | None |
| VisualOperatorNode | 6 | 6 | 0 | None |
| VisualOutfixNode | 8 | 8 | 0 | None |
| VisualOutfixNodeEditor | 35 | 35 | 0 | None |
| VisualPairNaryNode | 28 | 28 | 0 | None |
| VisualPairNaryNodeEditor | 21 | 21 | 0 | None |
| VisualPlaceholderNode | 22 | 22 | 0 | None |
| VisualPlaceholderNodeEditor | 5 | 5 | 0 | None |
| VisualProgramNode | 22 | 22 | 0 | None |
| VisualProgramNodeEditor | 21 | 21 | 0 | None |
| VisualRangedPrefixNode | 24 | 23 | 1 | public override global::System.Collections.Generic.IEnumerable<global::System.Windows.UIElement> CoreElements (closure ordinal (iterator)) |
| VisualRangedPrefixOperatorNodeEditor | 21 | 21 | 0 | None |
| VisualScaledSymbolNode | 7 | 7 | 0 | None |
| VisualScoper | 26 | 25 | 1 | public static global::System.Collections.Generic.IEnumerable<global::System.Windows.UIElement> Convert(global::System.Collections.Generic.IEnumerable<global::Quire.EquationEditor.View.IVisualMathNode> unconverted, bool useLinearScopingHighlight) (closure ordinal (iterator)) |
| VisualStepRangeOperatorNode | 7 | 7 | 0 | None |
| VisualStepRangeOperatorNodeEditor | 24 | 24 | 0 | None |
| VisualStringNode | 27 | 27 | 0 | None |
| VisualSubscriptOperatorNode | 15 | 14 | 1 | public override global::System.Collections.Generic.IEnumerable<global::System.Windows.UIElement> CoreElements (closure ordinal (iterator)) |
| VisualSuperscriptBinaryNode | 8 | 8 | 0 | None |
| VisualSuperscriptOutfixOperatorNode | 3 | 3 | 0 | None |
| VisualSymKeywordsStackNode | 8 | 8 | 0 | None |
| VisualSymbolNode | 34 | 28 | 6 | public VisualSymbolNode(char symbol) (closure ordinal (lambda)), public VisualSymbolNode(char symbol, global::Quire.EquationEditor.Tree.SymbolMargin horizMargin) (closure ordinal (lambda)), public VisualSymbolNode(string symbol) (closure ordinal (lambda)), public VisualSymbolNode(string symbol, global::Quire.EquationEditor.Tree.SymbolMargin horizMargin) (closure ordinal (lambda)), protected override global::System.Windows.Size MeasureStaticLayout(global::System.Windows.Size availableSize) (closure ordinal (lambda)), protected override global::System.Windows.Size ArrangeStaticLayout(global::System.Windows.Size finalSize) (closure ordinal (lambda)) |
| VisualSymbolicEvaluationNode | 24 | 21 | 3 | public override global::System.Collections.Generic.IEnumerable<global::System.Windows.UIElement> CoreElements (closure ordinal (iterator)), protected override global::System.Windows.Size MeasureStaticLayout(global::System.Windows.Size availableSize) (closure ordinal (lambda)), protected override global::System.Windows.Size ArrangeStaticLayout(global::System.Windows.Size finalSize) (closure ordinal (lambda)) |
| VisualSymbolicEvaluationNodeEditor | 27 | 26 | 1 | public global::System.Collections.Generic.IEnumerable<global::System.Windows.UIElement> Convert(global::System.Collections.Generic.IEnumerable<global::Quire.EquationEditor.View.IVisualMathNode> unconverted) (closure ordinal (iterator)) |
| VisualTextNode | 26 | 24 | 2 | protected override global::System.Windows.Size MeasureStaticLayout(global::System.Windows.Size availableSize) (closure ordinal (lambda)), public VisualTextNode() (closure ordinal (lambda)) |
| VisualTextNodeEditor | 41 | 41 | 0 | None |
| VisualTextNodeForEditor | 6 | 6 | 0 | None |
| VisualTextRun | 18 | 18 | 0 | None |
| VisualTextSpan | 23 | 21 | 2 | protected override global::System.Windows.Size MeasureOverride(global::System.Windows.Size availableSize) (closure ordinal (lambda)), protected virtual void UpdateChildren() (closure ordinal (lambda)) |
| VisualTransposeNode | 8 | 8 | 0 | None |
| VisualTryNode | 26 | 26 | 0 | None |
| VisualTryNodeEditor | 23 | 23 | 0 | None |
| VisualVariantLineBinaryOperatorNode | 70 | 70 | 0 | None |
| VisualVectorizationNodeEditor | 21 | 21 | 0 | None |
| VisualVectorizationOperatorNode | 21 | 21 | 0 | None |
| VisualWrappingSign | 3 | 3 | 0 | None |

## Batches Plan

1. **Batch 1 (Small leaf types and types <= 10 stubs)**: VisualDefinitionNode, VisualLinearBinaryOperatorNode_1, VisualNode, VisualSuperscriptOutfixOperatorNode, VisualWrappingSign, VisualLeafOperatorNode, VisualMatrixSuperscriptBinaryNode, VisualBinaryWrappOperatorNodeEditor, VisualPlaceholderNodeEditor, VisualExponentNode, VisualOperatorNode, VisualTextNodeForEditor, VisualScaledSymbolNode, VisualStepRangeOperatorNode, VisualMathNodeHitTestResult, VisualOutfixNode, VisualSuperscriptBinaryNode_1, VisualSymKeywordsStackNode, VisualTransposeNode.
2. **Batch 2 (Leaf / text / symbol editors)**: VisualLeafOperatorEditor, VisualAtomicTextLeafEditor, VisualGradientNodeEditor, VisualConjugateNodeEditor, VisualDerivativeNodeEditor, VisualForLoopNodeEditor, VisualIntegralNodeEditor, VisualLimitNodeEditor, VisualPairNaryNodeEditor, VisualProgramNodeEditor, VisualRangedPrefixOperatorNodeEditor, VisualTryNodeEditor, VisualVectorizationNodeEditor.
3. **Batch 3 (Operator and symbol nodes)**: VisualConjugateOperatorNode, VisualFractionOperatorNode, VisualGradientOperatorNode, VisualIntegralOperatorNode, VisualLimitOperatorNode, VisualPairNaryNode, VisualRangedPrefixNode, VisualTryNode, VisualVectorizationOperatorNode.
4. **Batch 4 (Text and AST nodes)**: VisualAtomicTextLeafNode, VisualIdNode, VisualIdentifierNodeEditor, VisualLinearUnaryNode, VisualLinearUnaryEditor, VisualProgramNode, VisualStringNode, VisualTextNode, VisualTextNodeEditor, VisualTextRun, VisualTextSpan.
5. **Batch 5 (Complex math nodes and layout)**: VisualBinaryOperatorNode, VisualBinaryOperatorNodeEditor, VisualLinearBinaryOperatorNode, VisualLinearOutfixNode, VisualMultiLineAligningBinaryOperatorNode, VisualMultiLineWrappingBinaryOperatorNode, VisualNthRootNodeEditor, VisualNthRootOperatorNode, VisualOutfixNodeEditor, VisualPlaceholderNode, VisualStepRangeOperatorNodeEditor, VisualSymbolNode, VisualSymbolicEvaluationNode, VisualSymbolicEvaluationNodeEditor, VisualVariantLineBinaryOperatorNode, VisualMathNode, VisualMathNodeEditor_1, VisualMatrixNode, VisualLayoutManager, VisualScoper.

## Batch 1: Small leaf visual nodes and hit test results

- Members: 25 members across 7 types:
  - VisualDefinitionNode: constructor VisualDefinitionNode(VisualSymbolNode opSymbol).
  - VisualNode: .ctor(), .ctor(SerializationInfo, StreamingContext).
  - VisualWrappingSign: _yoffset field, .ctor(), Text getter, YOffset getter.
  - VisualLeafOperatorNode: .ctor(string symbol), NavigatableParts getter, OnVisualParentChanged, GetNodeAtPoint.
  - VisualMathNodeHitTestResult: NodeHit, CaretSideHit, NodeSideHit, OperatorPartIndexHit, IsOperatorHit fields, and all 3 constructor overloads.
  - VisualMatrixSuperscriptBinaryNode: .ctor(), MeasureOverride, ArrangeOverride, GetNodeAtPoint.
  - VisualSuperscriptOutfixOperatorNode: .ctor(left, right), .ctor(horizontal, left, right), GetNodeAtPoint.
- Shapes tried:
  - VisualMatrixSuperscriptBinaryNode.MeasureOverride: natural double height; declaration ahead of 
ef Point alignmentPoint matched Mathcad local variable index [2].
  - VisualMatrixSuperscriptBinaryNode.GetNodeAtPoint: declaring both GeneralTransform locals before branching matched Mathcad local indexing.
  - VisualSuperscriptOutfixOperatorNode.GetNodeAtPoint: caching (LeftFence as Fence).ActualWidth into a local before comparison matched the call order.
- Release IL: PASS, 1750 checked (.mirror-cache/release-il-9b6fc1cf3171/release-il.json).
