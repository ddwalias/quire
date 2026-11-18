# Lane Quire.Controls.Plots, half B

- Owner: Orca task `task_cd3ae4f060ec` (half B: generated files after `Controls.Plots.Plot3D.View.PlotAxesWidget3D.g.cs`).
- Branch: `mirror/controls-plots-b-20261119`; worktree `C:/Users/DuyDang/orca/workspaces/Quire/plots-b`.

## Batch 1 — PlotItemPlaceholder (partial), PlotEquationViewModelBase`1

- PlotItemPlaceholder: exact — `SnapToGrid`, `GridCellSize`, `Update`, `MeasureOverride`, `AdjustPadding()`, `IsPlotTitleOrientationSwapped`, `GetMinLegendPlaceholderWidth`, `GetHalfPaddingValue`, `.ctor`, `.cctor`. Stubs held: `AdjustPadding(Size)` (one attempt: the `if (SnapToGrid)` then-block is 6 bytes longer than Mathcad's, first branch to the else differs); `GetMinLegendPlaceholderHeight` (one hoisted-local attempt: the `bool flag2 = (x is PlotTitle || x is PlotLegend) ? true : false` branch shape at `if (flag)` differs from Mathcad's materialised `isinst`/`stloc.3` form).
- PlotEquationViewModelBase`1: exact — `Model`, `PlotCalculatableError` get/set, `.ctor`, `PropertyNames.PlotCalculatableError`. The constructor uses `Model.PropertyChanged +=` directly (a `TModel model` local differs).
- Release IL (project gate, changed methods): PASS — 1309 checked, 0 failures.
- Type gate with `--type` on PlotItemPlaceholder still fails on the two held stubs (expected).

## Remaining mismatches / blockers (this lane)

- PlotItemPlaceholder.AdjustPadding(Size), GetMinLegendPlaceholderHeight: stubs, blockers above.
- Other half-B types: not started.
