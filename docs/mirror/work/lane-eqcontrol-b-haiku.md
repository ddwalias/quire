# Lane: Quire.Controls.EquationControl, half B (eqcontrol-b-haiku)

- Owner: eqcontrol-b-haiku worker (taken over from a Codex worker that made no changes)
- Working branch: `mirror/eqcontrol-b-haiku-20261119` (from main 457672a0c; no landings)
- Scope: generated stubs from `Controls.Internal.IntegralLayoutManager.g.cs` to the end of
  `src/Quire.Controls.EquationControl/Generated/Mirror`. Parked: `ConjugateLayoutManager`,
  `VectorizationLayoutManager`.

## Landed
- None.

## Held (committed on local branch, not landed)
- `hold/nthrootlayoutmanager-eqcB2` (158c9e1dd): `Quire.Controls.Internal.NthRootLayoutManager`.
  Exact in Release IL: 4 getters, constructor, `Arrange`. Parked: `FindClosestChild`, which
  matches opcode for opcode but reports `.maxstack 2` where Mathcad has `3`. The type gate
  (`--type`) fails on this member, so the type is held.

## Attempted and not yet written
- `Controls.Internal.SymbolicEvaluationLayoutManager`: read IL only. Its `Arrange` is 819 bytes
  with `Point` temporaries; not attempted.

## Remaining
- About 2,117 `MirrorStub` markers in 44 generated files in this half (counted by occurrence,
  attributes and `NotImplemented` references together). Largest: `Controls.SpecTableView`,
  `Controls.SpecTableControl`, `Controls.Internal.VisualMatrixNode`, `VisualVariantLineBinaryOperatorNode`,
  `VisualMathNode`, `VisualTextNodeEditor`.
- Iterator-bearing members (`VisualSubscriptOperatorNode.CoreElements`, and similar) need the
  closure/iterator ordinal layout and were not started.
