# Lane: Quire.Controls.EquationControl, half B (eqcontrol-b-haiku)

- Owner: eqcontrol-b-haiku worker (taken over from a Codex worker that made no changes)
- Working branch: `mirror/eqcontrol-b-haiku-20261119`
- Scope: generated stubs from `Controls.Internal.IntegralLayoutManager.g.cs` to the end of
  `src/Quire.Controls.EquationControl/Generated/Mirror`. Parked: `ConjugateLayoutManager`,
  `VectorizationLayoutManager`.

## Landed (Release IL exact for every changed member; remaining stubs listed per type)
- `05323b754` NthRootLayoutManager: getters, ctor, Arrange. Stub: FindClosestChild.
- `e818d8392` SymbolicEvaluationLayoutManager: constants, getters, ctor, Arrange. Stub: FindClosestChild.
- `34afd2829` LimitLayoutManager: getters, ctor, Arrange. Stubs: FindClosestChild, IsBetween.
- `dfb278760` RangedPrefixLayoutManager: constants, getters, ctor, Arrange. Stub: FindClosestChild.
- `2bbed1666` IntegralLayoutManager: constants, fields, getters, both ctors, Arrange. Stubs: FindClosestChild, IsBetween.
- `96bfb65f2` MatrixDisplayHelper: DisplaySize, MatrixNodeIsInResult, ShowChildAsIs, ShowEmptyCell, predicates. Stubs: constructor (int32 comparison locals), ShowDownRightEllipsis, ShowVerticalEllipsis, ShowHorizontalEllipsis.
- VisualLinearBinaryOperatorNode (landing pending): fields, both ctors, Operator, OperatorElement, ScreenInkBounds, IsMonoLine. Stubs: CoreElements (iterator), MeasureStaticLayout, ArrangeStaticLayout, GetNodeAtPoint, EffectiveBaselineY, EffectiveScriptBaselineY.

## Blockers
- `FindClosestChild` (NthRoot, SymbolicEvaluation, Limit, RangedPrefix, Integral): the body matches opcode for opcode, but Release reports `.maxstack 2` where Mathcad has `3`.
- `ShowDownRightEllipsis`, `ShowVerticalEllipsis`, `ShowHorizontalEllipsis`: Mathcad shares return blocks across the `||` chain with a different instruction count; if-chain and single-expression forms both differ.
- MatrixDisplayHelper constructor: Mathcad keeps comparison results in `int32` locals (`cgt`), which Roslyn does not emit for bool-to-int.
- Iterator members (`CoreElements`) need the state-machine closure ordinal.

## Remaining
- About 1,200 member-level `MirrorStub` attributes across about 40 generated files in this half (as of this checkpoint).
- Next: remaining VisualLinearBinaryOperatorNode members, then the other Visual* nodes, RubberBandHighlighter, VerticalFence, SingleLine2VariantAddaptor, SpecTable views.
