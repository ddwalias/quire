# Lane: Quire.Controls.EquationControl, half B (eqcontrol-b-haiku)

- Owner: eqcontrol-b-haiku worker (taken over from a Codex worker that made no changes)
- Working branch: `mirror/eqcontrol-b-haiku-20261119`
- Scope: generated stubs from `Controls.Internal.IntegralLayoutManager.g.cs` to the end of
  `src/Quire.Controls.EquationControl/Generated/Mirror`. Parked: `ConjugateLayoutManager`,
  `VectorizationLayoutManager`.

## Landed (Release IL exact for every changed member)
- `05323b754` `Quire.Controls.Internal.NthRootLayoutManager`: 4 getters, ctor, Arrange. Stub: FindClosestChild.
- `e818d8392` `Quire.Controls.Internal.SymbolicEvaluationLayoutManager`: 2 constants, 4 getters, ctor, Arrange. Stub: FindClosestChild.
- LimitLayoutManager: 6 getters, ctor, Arrange. Stubs: FindClosestChild, IsBetween.
- RangedPrefixLayoutManager (landing pending): 3 constants, 4 getters, ctor, Arrange. Stub: FindClosestChild.

## Blockers
- `FindClosestChild` (NthRoot, SymbolicEvaluation, Limit): the body matches opcode for opcode, but Release reports `.maxstack 2` where Mathcad has `3`.

## Remaining
- About 2,100 `MirrorStub` markers in about 42 generated files in this half.
- Next types in order: IntegralLayoutManager, MatrixDisplayHelper, RangedPrefixLayoutManager, RubberBandHighlighter, SingleLine2VariantAddaptor, VerticalFence and the Visual* nodes.
