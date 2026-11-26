# Lane Quire.Controls.Plots (agy-whiteboard-3, task_d5c5400c7c44)

- Owner: Orca task `task_d5c5400c7c44` (`agy-whiteboard-3`).
- Branch: `feature/agy-plots-3d-ctrl-vm` (Batch 1), `feature/agy-plots-3d-ctrl` (Batch 2).
- Scope: `PlotViewModel3D`, `PlotControl3D`.

## Batch 1 — `PlotViewModel3D`

- `Quire.Controls.Plots.Plot3D.ViewModel.PlotViewModel3D`: whole-type Release IL parity (including `PropertyNames`, `AddEquationCommand`, `RemoveEquationCommand`, `<SelectedTraces>d__41`, all compiler-generated closures `<>c` / `<>c__DisplayClass*`, properties, commands, and event handlers).
- Release IL report: `Release IL: PASS; 4009 checked` (`python tools/validation/release_il.py --base main --project Quire.Controls.Plots`).
- Remaining in scope: `PlotControl3D`.

## Batch 2 — `PlotControl3D`

- `Quire.Controls.Plots.Plot3D.View.PlotControl3D`: whole-type Release IL parity (all properties, `.cctor`, `.ctor`, command bindings, event/callback handlers, XAML glue, explicit `IPlotControl<XyzEquation>` members, and compiler-generated closures `<>c`, `<>c__DisplayClass80_0`, `<>O`).
- Release IL report: `Release IL: PASS; 4073 checked` (`python tools/validation/release_il.py --base main --project Quire.Controls.Plots`).
- Remaining mismatches: none.
