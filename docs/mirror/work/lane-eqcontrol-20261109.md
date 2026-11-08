# EquationControl lane checkpoint

- Owner: dispatched worker `task_a11ae5e18f43`.
- Branch: `mirror/eqcontrol-20261109`.
- Batch: `Ptc.Controls.InputControls.ComboBoxTableConstants` constants (`MinimumRowsNum`, `MinimumColumnsNum`); the original Release IL declares both as literals with value `2`.
- Shapes tried: hand-written partial constants matching the original declarations; generated stub declarations removed in the same change.
- Release IL: PASS, 595 checked; `.mirror-cache/release-il-451f2a6cbff8/release-il.json`.
- Remaining: 4,506 unfinished members across 327 generated type files; this batch has no mismatches.
