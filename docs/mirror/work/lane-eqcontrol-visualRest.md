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
| VisualMatrixContents | 2 | 2 | 0 | None |
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

## Batch 2: ComboBox, Exponent, LinearBinaryOperator, BinaryWrapp and Placeholder editors

- Members: 20 members across 5 types:
  - VisualComboBoxControlEditor: 2 constructors ((VisualComboBoxControlNode, VisualMathRegionNode), (VisualComboBoxControlNode, FrameworkElement, VisualMathRegionNode)).
  - VisualLinearBinaryOperatorNode<TOperator>: _operator field, .ctor().
  - VisualBinaryWrappOperatorNodeEditor: _wrappingOperatorNode field, .ctor(), ShowOperatorHighlight, OnRender, OnOperatorSelectedChanged.
  - VisualPlaceholderNodeEditor: _highlighter field, _caret field, .ctor(), MeasureOverride, ArrangeOverride.
  - VisualExponentNode: VerticalAdjustProperty, PAD, .ctor(), .cctor(), MeasureOverride, ArrangeOverride, GetNodeAtPoint.
- Shapes tried:
  - VisualBinaryWrappOperatorNodeEditor.OnRender: idiomatic early guard if (!_showOperatorHighlight) return; matched Mathcad's brtrue.s + immediate ret.
  - VisualExponentNode.GetNodeAtPoint: if (visualSymbolNode != null) { ... } else { ... } instead of ternary matched Mathcad's brfalse.s branching order.
  - VisualExponentNode.MeasureOverride: pre-declaring double height; ahead of ref Point alignmentPoint matched local variable slot indices [2] and [3].
- Release IL: PASS; 1780 checked (.mirror-cache/release-il-0c0d9bf16d0c/release-il.json).

## Batch 3: Operator, TextNodeForEditor, ScaledSymbol, StepRange, Transpose and SymKeywordsStack

- Members: 42 members across 6 types:
  - VisualOperatorNode: _descriptor, _operatorSymbols, _operands, NavigatableParts, this[int index], .ctor(name, operatorSymbols).
  - VisualTextNodeForEditor: Text, FormattedNodeText, CharacterWidths, ScreenCharacterWidths, TextChanged event, .ctor().
  - VisualScaledSymbolNode: _scaleFactor, EffectiveBaselineY, Geometry, .ctor(char), .ctor(string), OnRender, MeasureStaticLayout.
  - VisualStepRangeOperatorNode: _baseline, _effectiveBaseline, _effectiveScriptBaseline, TextBaselineY, .ctor(), MeasureStaticLayout, ArrangeStaticLayout.
  - VisualTransposeNode: _pushFirstOpDown, TextBaselineY, EffectiveBaselineY, EffectiveScriptBaselineY, .ctor(), MeasureOverride, ArrangeOverride, HandleMathFormatChanged.
  - VisualSymKeywordsStackNode: _nodeChildren, _operandsPanel, TextBaselineY, this[int index], .ctor(VisualMathRegionNode), BuildChildren, MeasureOverride, ArrangeOverride.
- Shapes tried:
  - VisualTransposeNode.MeasureOverride: declaring double height; ahead of ref Point alignmentPoint matched local variable slots [2] and [3].
  - VisualSymKeywordsStackNode.set_Item: Enumerable.Count() matched Linq call order.
  - VisualOperatorNode.NavigatableParts: array covariance return matched local variable slot.
- Release IL: PASS; 1808 checked (.mirror-cache/release-il-0b259966dcda/release-il.json).

## Batch 4: Outfix, AtomicTextLeafEditor, LeafOperatorEditor, and MatrixContents

- Members: 32 members across 4 types:
  - VisualOutfixNode: LeftFence, RightFence, NavigatableParts, NodeChildren, DesiredSizeWithoutFences, Horizontal, SetParensVisibility, .ctor.
  - VisualAtomicTextLeafEditor: SelectedProperty, _caretBrush, Selected, .cctor, OnSelectedChanged(d, e), .ctor(editNode, region), OnSelectedChanged(bool), Remove, MeasureOverride, OnRender, OnCaretColorChanged, UpdateCaretBlinking.
  - VisualLeafOperatorEditor: OperatorSelectedProperty, _caretBrush, .cctor, OnOperatorSelectedChanged(d, e), .ctor(editNode, region), OnOperatorSelectedChanged(bool), SelectOperator, Remove, MeasureOverride, OnRender, OnCaretColorChanged, UpdateCaretBlinking.
  - VisualMatrixContents: GeneralRowSpacing, .ctor(), .cctor().
- Shapes tried:
  - VisualAtomicTextLeafEditor.cctor and VisualLeafOperatorEditor.cctor: non-object initializer syntax (`frameworkPropertyMetadata = new FrameworkPropertyMetadata(); frameworkPropertyMetadata.AffectsMeasure = true; ...`) matched Mathcad's sequential `stloc.0` + `ldloc.0` IL rather than `dup`.
  - VisualOutfixNode: omitted explicit `IVisualNode.Loaded` in partial class to let C# compiler synthesize interface bridge methods consistently across base and derived types (`VisualMatrixNode`).
- Release IL: PASS; 1928 checked (.mirror-cache/release-il-48245a53c75e/release-il.json).

## Batch 5: VisualFactory, VisualLimitSideNode, VisualLimitSideNodeEditor, and VisualLayoutManager

- Members: 50 members across 4 types:
  - VisualFactory: _region, .ctor(VisualMathRegionNode), CreateSingleLineBinaryBase, CreateBiLinearBinaryNode, CreateSummationNode, CreateProductNode, CreateArgSeparatorNode, CreateDefinitionNode, CreateIfNode(), CreateIfNode(MathNode), CreateReturnNode, CreateInputControlEditor.
  - VisualLimitSideNode: NodeInfo, Child, IsPlaceholder, EffectiveBaselineY, EffectiveScriptBaselineY, TextBaselineY, CreateVisualChildren, GetNodeAtPoint, OnShowOperatorOnlyWhenActiveChanged, MeasureStaticLayout, ArrangeStaticLayout, .ctor().
  - VisualLimitSideNodeEditor: CaretOpacityProperty, _highlighter, _caret, _limitSideSymbol, OnCaretOpacityPropertyChanged, .cctor, UpdateCaretBlinking, OnCaretColorChanged, OnCaretOpacityChanged, UpdateSymbols, BindToSettings, .ctor(VisualMathNode, VisualMathRegionNode), Remove, MeasureOverride, ArrangeOverride.
  - VisualLayoutManager: _childItems, _operatorPartItems, _baseline, ChildItems, OperatorPartItems, TextBaselineOffset, DesiredSize, .ctor(LayoutItem[], LayoutItem[]), Arrange, FindClosestChild, AlignItemAdjacentTo, ToString.
- Shapes tried:
  - VisualLimitSideNodeEditor.cctor: non-object initializer syntax for FrameworkPropertyMetadata matched Mathcad IL without dup.
  - VisualLimitSideNode: omitted explicit IVisualNode.Loaded in partial declaration to let C# compiler synthesize interface bridge methods consistently.
- Release IL: PASS; 1969 checked (.mirror-cache/release-il-ab025e313e4e/release-il.json).

## Batch 6: VisualLinearUnaryEditor, VisualBinaryOperatorNode, and VisualFractionOperatorNode

- Members: 48 members across 3 types:
  - VisualLinearUnaryEditor: OperatorSelectedProperty, _caretBrush, _operatorGeometry, _operandHighlighter, .cctor, OnOperatorSelectedChanged(d, e), .ctor(editNode, region), OnOperatorSelectedChanged(bool), SelectOperator, Remove, MeasureOverride, OnRender, OnCaretColorChanged, SetCaretType, UpdateCaretBlinking.
  - VisualBinaryOperatorNode: FirstOperandProperty, SecondOperandProperty, this[int index], FirstOperand, SecondOperand, Operator, OperatorElement, FirstMathNode, SecondMathNode, NavigatableParts, OnFirstOperandPropertyChanged, OnSecondOperandPropertyChanged, OnFirstOperandChanged, OnSecondOperandChanged, .ctor().
  - VisualFractionOperatorNode: DEFAULT_SIDE_OVERHANG, DEFAULT_FRACTION_BAR_VERTICAL_PAD, StaticFractionOverhangProperty, StaticFractionBarVerticalPadProperty, _fractionBar, _textBaselineY, Operator, OperatorElement, TextBaselineY, EffectiveBaselineY, EffectiveScriptBaselineY, FractionOverhang, FractionBarVerticalPad, ScreenInkBounds, .ctor(), MeasureStaticLayout, ArrangeStaticLayout, DenominatorY, GetNodeAtPoint.
- Shapes tried:
  - VisualLinearUnaryEditor.OnOperatorSelectedChanged: using `selected ? Visibility.Visible : Visibility.Hidden` instead of `!selected ? ...` matched `brtrue.s` test on `selected`.
  - VisualFractionOperatorNode.DenominatorY: evaluating `double height = _fractionBar.DesiredSize.Height` as a local statement before `visualMathNode.DesiredSize.Height + height` matched stack caching of `visualMathNode` across statement execution.
  - VisualFractionOperatorNode.GetNodeAtPoint: structured `if ... else ...` scoping for `GeneralTransform` variables matched local variable slot indices [1] and [2].
- Release IL: PASS; 2010 checked (.mirror-cache/release-il-0b44db8728c8/release-il.json).

## Batch 7: VisualConjugateOperatorNode and VisualConjugateNodeEditor

- Members: 42 members across 2 types:
  - VisualConjugateOperatorNode: _conjugateDescriptor, _nodeChildren, _operatorSymbol, _layout, this[int index], NodeChildren, OperatorSymbol, ConjugateSymbolTopLeft, TextBaselineY, EffectiveBaselineY, EffectiveScriptBaselineY, Child, OperatorElement, Operator, IVisualUnaryOperatorNode.Child, NavigatableParts, .ctor(), SetVisibilityOfOperatorSymbol, GetNodeAtPoint, MeasureStaticLayout, ArrangeStaticLayout, .cctor().
  - VisualConjugateNodeEditor: OperatorSelectedProperty, CaretOpacityProperty, _operatorSymbol, _operandHighlighters, OperatorSelected, OnOperatorSelectedPropertyChanged, OnCaretOpacityPropertyChanged, .cctor(), OnOperatorSelectedChanged, OnCaretColorChanged, OnCaretOpacityChanged, .ctor(editNode, region), SelectOperator, GetFocus, Remove, MeasureOverride, ArrangeOverride, UpdateOperatorSymbol, BindToSettings, RemoveHighlighters, UpdateCaretBlinking, SetVisibilityOfEditorSymbols.
- Shapes tried:
  - VisualConjugateNodeEditor.cctor: sequential initialization of FrameworkPropertyMetadata matched Mathcad IL without dup.
  - VisualConjugateOperatorNode.MeasureStaticLayout: pre-declaring double width; LayoutItem[] childItems; LayoutItem[] operatorPartItems; ahead of array iteration loop matched Mathcad local variable slots [0], [1], and [2].
- Release IL: PASS; 2050 checked (.mirror-cache/release-il-4510de5e4729/release-il.json).

## Batch 8: VisualVectorizationOperatorNode and VisualVectorizationNodeEditor

- Members: 42 members across 2 types:
  - VisualVectorizationOperatorNode: _vectorizationDescriptor, _nodeChildren, _operatorSymbol, _layout, this[int index], NodeChildren, OperatorSymbol, VectorizationSymbolTopLeft, TextBaselineY, EffectiveBaselineY, EffectiveScriptBaselineY, Child, OperatorElement, Operator, IVisualUnaryOperatorNode.Child, NavigatableParts, .ctor(), SetVisibilityOfOperatorSymbol, GetNodeAtPoint, MeasureStaticLayout, ArrangeStaticLayout, .cctor().
  - VisualVectorizationNodeEditor: OperatorSelectedProperty, CaretOpacityProperty, _operatorSymbol, _operandHighlighters, OperatorSelected, OnOperatorSelectedPropertyChanged, OnCaretOpacityPropertyChanged, .cctor(), OnOperatorSelectedChanged, OnCaretColorChanged, OnCaretOpacityChanged, .ctor(editNode, region), SelectOperator, GetFocus, Remove, MeasureOverride, ArrangeOverride, UpdateOperatorSymbol, BindToSettings, RemoveHighlighters, UpdateCaretBlinking, SetVisibilityOfEditorSymbols.
- Shapes tried:
  - VisualVectorizationNodeEditor.cctor: sequential initialization of FrameworkPropertyMetadata matched Mathcad IL without dup.
  - VisualVectorizationOperatorNode.MeasureStaticLayout: pre-declaring double width; LayoutItem[] childItems; LayoutItem[] operatorPartItems; ahead of array iteration loop matched Mathcad local variable slots [0], [1], and [2].
- Release IL: PASS; 2090 checked (.mirror-cache/release-il-499627bbd9a5/release-il.json).

## Batch 9: VisualTryNode and VisualTryNodeEditor

- Members: 49 members across 2 types:
  - VisualTryNode: BODY_LEFT_PAD, _tryOperator, _onErrorOperator, _operatorRightMinorPart, _verticalOffset, _nodeChildren, _tryOperatorTopLeft, _onErrorOperatorTopLeft, _firstChildTopLeft, _secondChildTopLeft, _operatorRightMinorPartTopLeft, NodeChildren, TryOperator, OnErrorOperator, NavigatableParts, EffectiveBaselineY, EffectiveScriptBaselineY, TextBaselineY, this[int index], TryOperatorTopLeft, OnErrorOperatorTopLeft, .ctor(), MeasureOverride, ArrangeOverride, GetNodeAtPoint, SetVisibilityOfOperatorSymbol.
  - VisualTryNodeEditor: NodeSideProperty, CaretOpacityProperty, _tryOperator, _onErrorOperator, _operandHighlighters, NodeSide, CaretOpacity, OnCaretOpacityPropertyChanged, OnNodeSidePropertyChanged, .cctor(), OnNodeSideChanged, OnCaretColorChanged, OnCaretOpacityChanged, .ctor(editNode, region), GetFocus, Remove, MeasureOverride, ArrangeOverride, BindToSettings, UpdateOperatorSymbols, RemoveHighlighters, GetSymbolNode, UpdateCaretBlinking, SetVisibilityOfEditorSymbols.
- Shapes tried:
  - VisualTryNodeEditor.cctor: sequential initialization of FrameworkPropertyMetadata matched Mathcad IL without dup.
  - VisualTryNode.MeasureOverride: intermediate double locals and Math.Max call chains matched Mathcad's evaluation stack ordering and local slots.
- Release IL: PASS; 2126 checked (.mirror-cache/release-il-31dd0929d65a/release-il.json).

## Batch 10: VisualIntegralOperatorNode and VisualIntegralNodeEditor

- Members: 48 members across 2 types:
  - VisualIntegralOperatorNode: _integralDescriptor, _nodeChildren, _integralSymbol, _variableOfIntegrationPrefix, _layout, AdjacentUpperLimitHeightProperty, AdjacentLowerLimitHeightProperty, AdjacentUpperLimitHeight, AdjacentLowerLimitHeight, this[int index], NodeChildren, IntegralSymbol, VariableOfIntegrationPrefix, TextBaselineY, EffectiveBaselineY, EffectiveScriptBaselineY, OperatorElement, Operator, NavigatableParts, .ctor(), SetVisibilityOfOperatorSymbol, GetNodeAtPoint, MeasureStaticLayout, ArrangeStaticLayout, LimitsAndIntegralSymbolWidth.
  - VisualIntegralNodeEditor: OperatorSelectedProperty, CaretOpacityProperty, _integralSymbol, _variableOfIntegrationPrefix, _operandHighlighters, OperatorSelected, CaretOpacity, OnOperatorSelectedPropertyChanged, OnCaretOpacityPropertyChanged, .cctor(), OnOperatorSelectedChanged, OnCaretColorChanged, OnCaretOpacityChanged, .ctor(editNode, region), SelectOperator, GetFocus, Remove, MeasureOverride, ArrangeOverride, UpdateOperatorSymbols, BindToSettings, RemoveHighlighters, UpdateCaretBlinking, SetVisibilityOfEditorSymbols.
- Shapes tried:
  - VisualIntegralNodeEditor.cctor: sequential initialization of FrameworkPropertyMetadata matched Mathcad IL without dup.
  - VisualIntegralOperatorNode.GetNodeAtPoint: ternary condition `(childIndex == 0) ? Direction.Left : Direction.Right` matched Mathcad IL `brfalse.s` branching.
  - VisualIntegralOperatorNode.MeasureStaticLayout: `foreach` loop over `_nodeChildren` followed by assigning intermediate `VisualMathNode` to a local before accessing `DesiredSize` and casting with `as` matched Mathcad IL locals and evaluation stack order.
- Release IL: PASS; 2170 checked (.mirror-cache/release-il-a224b1b4b4b7/release-il.json).

## Batch 11: VisualDerivativeSymbolNode and VisualDerivativeNodeEditor

- Members: 45 members across 2 types:
  - VisualDerivativeSymbolNode: FRACTIONBAR_VERTICAL_PADDING, FRACTIONBAR_SIDE_OVERHANG, _baseline, _fractionBar, _upperDerivativeSymbol, _lowerDerivativeSymbol, _orderSymbol, EffectiveBaselineY, EffectiveScriptBaselineY, TextBaselineY, DerivativeSymbolVerticalOffset, BaseOperandsSize, BaseOperandsOffset, Geometry, .ctor(derivativeSymbol), GetNodeAtPoint, InitializeChildren, SetOrderSymbolFontSize, SetOrderSymbolShowOperatorOnlyWhenActive, MeasureStaticLayout, ArrangeStaticLayout, CopyToOrderSymbol, HandleMathFormatChanged.
  - VisualDerivativeNodeEditor: CaretOpacityProperty, OperatorSelectedProperty, _caretBrush, _operatorGeometry, _operandHighlighters, OperatorSelected, OnOperatorSelectedPropertyChanged, OnCaretOpacityPropertyChanged, .cctor(), OnCaretColorChanged, OnOperatorSelectedChanged, OnCaretOpacityChanged, .ctor(editNode, region), SelectOperator, GetFocus, Remove, UpdateOperatorSymbol, MeasureOverride, OnRender, BindToSettings, RemoveHighlighters, UpdateCaretBlinking, SetVisibilityOfEditorSymbols.
- Shapes tried:
  - VisualDerivativeNodeEditor.cctor: combining delegates with `+=` on `PropertyChangedCallback` matched Mathcad IL `Delegate.Combine` pattern.
  - VisualDerivativeSymbolNode.ctor: ternary `DebugTools.DevAssert(...).Passed ? derivativeSymbol.Value : 'd'` directly matched Mathcad IL `brtrue.s` branching without negation.
- Release IL: PASS; 2208 checked (.mirror-cache/release-il-50f89dbde482/release-il.json).

## Batch 12: VisualProgramNode and VisualProgramNodeEditor

- Members: 43 members across 2 types:
  - VisualProgramNode: _operator, _nodeChildren, _operandsPanel, NodeChildren, Operator, OperatorFence, NavigatableParts, EffectiveBaselineY, EffectiveScriptBaselineY, TextBaselineY, this[int index], .ctor(showFence), MeasureOverride, ArrangeOverride, GetNodeAtPoint, SetVisibilityOfOperatorSymbol, SetOperatorFontColor, SetOperatorOpacity, IVisualMathNode.this[int index].
  - VisualProgramNodeEditor: OperatorSelectedProperty, CaretOpacityProperty, _operatorSymbol, _operandHighlighter, OperatorSelected, CaretOpacity, OnOperatorSelectedPropertyChanged, OnCaretOpacityPropertyChanged, .cctor(), OnOperatorSelectedChanged, OnCaretColorChanged, OnCaretOpacityChanged, .ctor(editNode, region), SelectOperator, GetFocus, Remove, MeasureOverride, ArrangeOverride, UpdateOperatorSymbol, BindToSettings, RemoveHighlighter, UpdateCaretBlinking, SetVisibilityOfEditorSymbols.
- Shapes tried:
  - VisualProgramNode.TextBaselineY: expression-bodied calculation matched Mathcad IL evaluation stack without intermediate local variables.
  - VisualProgramNodeEditor.UpdateOperatorSymbol: passing array covariance directly into `CreateNodeHighlighter` matched Mathcad IL local slot and argument evaluation order.
- Release IL: PASS; 2246 checked (.mirror-cache/release-il-6c495d2e88c1/release-il.json).

## Batch 13: VisualPairNaryNode and VisualPairNaryNodeEditor

- Members: 49 members across 2 types:
  - VisualPairNaryNode: OPERATOR_PAD, BODY_LEFT_PAD, _operatorRightMinorPart, _operatorRightMinorPartTopLeft, _verticalOffset, _nodeChildren, _operatorParts, _operatorPartsTopLeft, _operandsTopLeft, _firstOperatorPartSymbol, _middleOperatorPartsSymbol, _lastOperatorPartSymbol, _hasLastSymbol, EffectiveBaselineY, EffectiveScriptBaselineY, TextBaselineY, NodeChildren, OperatorParts, OperatorPartstopLeft, this[int index], NavigatableParts, .ctor(firstOperatorPartSymbol, middleOperatorPartsSymbol, lastOperatorPartSymbol), BuildChildren, PrepareVisualChildren, MeasureOverride, ArrangeOverride, GetNodeAtPoint, SetVisibilityOfOperatorSymbol.
  - VisualPairNaryNodeEditor: OperatorPartIndexProperty, CaretOpacityProperty, _operatorParts, _operandHighlighters, OperatorPartIndex, CaretOpacity, OnCaretOpacityPropertyChanged, OnOperatorPartIndexPropertyChanged, .cctor(), OnOperatorPartIndexChanged, OnCaretColorChanged, OnCaretOpacityChanged, .ctor(editNode, region), GetFocus, Remove, MeasureOverride, ArrangeOverride, BindToSettings, UpdateOperatorSymbols, RemoveHighlighters, UpdateCaretBlinking, SetVisibilityOfEditorSymbols.
- Shapes tried:
  - VisualPairNaryNode.PrepareVisualChildren: declaring `VisualSymbolNode visualSymbolNode` ahead of loop counter `i` matched Mathcad IL local slots [1] and [2].
  - VisualPairNaryNode.MeasureOverride: declaring intermediate layout locals at the top of the method and initializing `contentSize = new Size(0.0, 0.0)` after child Measure loops matched Mathcad IL local variable table [0..15] and evaluation stack order.
  - VisualPairNaryNode.GetNodeAtPoint: structured `if (i == 0) ... else if (i == num2 - 1 && _hasLastSymbol) ... else ...` matched Mathcad IL branching without inverted condition branches.
- Release IL: PASS; 2281 checked (.mirror-cache/release-il-ee817411bd25/release-il.json).

## Batch 14: VisualForLoopNode and VisualForLoopNodeEditor

- Members: 51 members across 2 types:
  - VisualForLoopNode: OPERATOR_PAD, BODY_LEFT_PAD, _forOperator, _inOperator, _operatorRightMinorPart, _verticalOffset, _nodeChildren, _forOperatorTopLeft, _inOperatorTopLeft, _firstChildTopLeft, _secondChildTopLeft, _thirdChildTopLeft, _operatorRightMinorPartTopLeft, NodeChildren, ForOperator, InOperator, NavigatableParts, EffectiveBaselineY, EffectiveScriptBaselineY, TextBaselineY, this[int index], ForOperatorTopLeft, InOperatorTopLeft, .ctor(), MeasureOverride, ArrangeOverride, GetNodeAtPoint, SetVisibilityOfOperatorSymbol.
  - VisualForLoopNodeEditor: NodeSideProperty, CaretOpacityProperty, _forOperator, _inOperator, _operandHighlighters, NodeSide, CaretOpacity, OnCaretOpacityPropertyChanged, OnNodeSidePropertyChanged, .cctor(), OnNodeSideChanged, OnCaretColorChanged, OnCaretOpacityChanged, .ctor(editNode, region), GetFocus, Remove, MeasureOverride, ArrangeOverride, BindToSettings, UpdateOperatorSymbols, RemoveHighlighters, GetVisualSymbolNode, UpdateCaretBlinking, SetVisibilityOfEditorSymbols.
- Shapes tried:
  - VisualForLoopNodeEditor.cctor: non-object initializer syntax for FrameworkPropertyMetadata with `+=` on `PropertyChangedCallback` matched Mathcad IL delegate combination.
  - VisualForLoopNode.MeasureOverride: assigning `_operatorRightMinorPart.ContentSize.Width` and `0.0` to local coordinates `num4` and `num5` before constructing `Point` matched Mathcad IL evaluation pattern.
- Release IL: PASS; 2317 checked (.mirror-cache/release-il-494a62467c98/release-il.json).

## Batch 15: VisualLimitOperatorNode and VisualLimitNodeEditor

- Members: 36 members across 2 types:
  - VisualLimitOperatorNode (17 members): _limitDescriptor, _nodeChildren, _limitSymbol, _limitArrow, OperatorPartCount, _layout, LimitVariableOperandIndex, LimitVariableExpressionOperandIndex, LimitSideOperandIndex, LimitExpressionOperandIndex, this[int index], NavigatableParts, LimitSymbol, LimitArrow, NodeChildren, LimitSymbolTopLeft, LimitArrowTopLeft, TextBaselineY, EffectiveBaselineY, EffectiveScriptBaselineY, .ctor(), ModifyChildMathFormat, ArrangeStaticLayout, GetNodeAtPoint. (Blocked: CoreElements iterator).
  - VisualLimitNodeEditor (19 members): CaretOpacityProperty, OperatorSelectedProperty, _operandHighlighters, _limitSymbol, _limitArrow, OperatorSelected, OnOperatorSelectedPropertyChanged, OnCaretOpacityPropertyChanged, .cctor(), OnCaretColorChanged, OnOperatorSelectedChanged, OnCaretOpacityChanged, .ctor(editNode, region), SelectOperator, GetFocus, Remove, UpdateOperatorSymbol, MeasureOverride, ArrangeOverride, BindToSettings, RemoveHighlighters, UpdateCaretBlinking, SetVisibilityOfEditorSymbols.
- Shapes tried:
  - VisualLimitOperatorNode.set_Item: using `string.Format("Index cannot be greater than {0}...", Length - 1)` matched Mathcad IL string.Format call. Storing `scaledFontSize` into a local variable before `DevAssert` preserved local variable slots [0..3].
  - VisualLimitOperatorNode.GetNodeAtPoint: declaring `Direction caretSideHit` ahead of `if (childType == LayoutChildType.Child)` and testing `childSideHit == Direction.Left ? Direction.Left : Direction.Indeterminate` matched Mathcad IL local slot 3 and `brfalse.s` branch shape.
- Release IL: PASS; 2353 checked (.mirror-cache/release-il-02b9711384f4/release-il.json).

## Batch 16: VisualGradientOperatorNode and VisualGradientNodeEditor

- Members: 35 members across 2 types:
  - VisualGradientOperatorNode (14 members): GradientDescriptor, _layout, VariableListOperandIndex, FunctionOperandIndex, GradientSymbol, NodeChildren, NavigatableParts, CoreElements, TextBaselineY, EffectiveBaselineY, EffectiveScriptBaselineY, this[int index], .ctor(), ModifyChildMathFormat, SetVisibilityOfOperatorSymbol, ArrangeStaticLayout, GetNodeAtPoint. (Blocked: MeasureStaticLayout closure lambda).
  - VisualGradientNodeEditor (21 members): OperatorSelectedProperty, CaretOpacityProperty, OperatorSymbol, _operandHighlighters, OperatorSelected, OnOperatorSelectedPropertyChanged, OnCaretOpacityPropertyChanged, .cctor(), OnOperatorSelectedChanged, OnCaretColorChanged, OnCaretOpacityChanged, .ctor(editNode, region), SelectOperator, GetFocus, Remove, MeasureOverride, ArrangeOverride, UpdateOperatorSymbol, BindToSettings, RemoveHighlighters, UpdateCaretBlinking, SetVisibilityOfEditorSymbols.
- Shapes tried:
  - VisualGradientOperatorNode.ctor: assigning ternary result `opSymbol` to a local `char` variable before calling `new VisualSymbolNode(opSymbol)` matched Mathcad IL evaluation order (ternary evaluated before `ldarg.0` for field store).
  - VisualGradientOperatorNode.GetNodeAtPoint: declaring `HitTestResult result` before branching and returning single exit variable matched local slot 3 and `br.s` control flow.
- Release IL: PASS; 2388 checked (.mirror-cache/release-il-d489fdd7c024/release-il.json).






