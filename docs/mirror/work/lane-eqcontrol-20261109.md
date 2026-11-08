# EquationControl lane checkpoint

- Owner: dispatched worker `task_a11ae5e18f43`.
- Branch: `mirror/eqcontrol-20261109`.
- Batch: `Ptc.Controls.InputControls.ComboBoxTableConstants` constants (`MinimumRowsNum`, `MinimumColumnsNum`); the original Release IL declares both as literals with value `2`.
- Shapes tried: hand-written partial constants matching the original declarations; generated stub declarations removed in the same change.
- Release IL: PASS, 595 checked; `.mirror-cache/release-il-451f2a6cbff8/release-il.json`.
- Batch: constructors for `Ptc.Controls.Internal.VisualDerivativeOperatorNode` and `Ptc.Controls.Internal.VisualPartialDerivativeOperatorNode`; each uses the corresponding descriptor-table key from the original source.
- Shapes tried: idiomatic constructor initializer with dictionary indexer; both matched without unusual IL-specific shaping.
- Release IL: PASS, 597 checked, 0 failures; `.mirror-cache/release-il-7a877fb8ce53/release-il.json`.
- Remaining: 4,504 unfinished members across 327 generated type files; these batches have no mismatches.
