# Lane Plots — Quire.Controls.Plots

- Owner: Orca task `task_f28f3845158a` (maintainer-assigned; no WORK.md lane row).
- Branch: `mirror/controls-plots-20261108`; worktree `C:/Users/DuyDang/orca/workspaces/Quire/controls-plots-20261108`.
- Baseline (main `6c22e4dd`): 8917 stub markers in `Generated/Mirror`; Release IL report counts 0 exact / 5243 unfinished before batch 1 (`--project Quire.Controls.Plots`).
- Reference: `python tools/reference/reference.py Ptc.Controls.Plots.<Type> --assembly Ptc.Controls.Plots --il` (full Mathcad type name; run without `PYTHONUTF8=1`).

## Batch 1 — GridLenghtToMargin

- Members: `Convert`, `ConvertBack`, `.ctor`. Hand-written: `src/Quire.Controls.Plots/Mirrored/Controls.Plots.GridLenghtToMargin.cs`; stubs removed from `Generated/Mirror/Controls.Plots.GridLenghtToMargin.g.cs`.
- Shapes: `Convert` as `text switch` expression with `(object)` cast on the first arm → opcode mismatch (Mathcad returns directly from each arm, joining nothing through a shared local). `switch` statement with `return` per case → exact.
- Release IL: PASS — 356 checked, 356 exact, 0 failures, 4887 unfinished.
- Remaining mismatches in this batch: none.

## Batch 2 — BooleanToVisibilityConverter, ColorToSolidColorBrushConverter

- Members: `Instance` (static field initializer), private `.ctor`, `Convert`, `ConvertBack` in each; class attributes `[ValueConversion]` on the hand-written partials. Hand-written: `Mirrored/Controls.Plots.View.{BooleanToVisibilityConverter,ColorToSolidColorBrushConverter}.cs`; stubs removed from the matching `.g.cs`.
- Shapes: idiomatic forms matched first time (`solidColorBrush.Freeze()` without the `(Freezable)` cast; `??` for the result).
- Release IL: PASS — 362 checked, 0 failures; exact count rises by 6 (2 types + the converter members checked).
- Remaining mismatches in this batch: none.
