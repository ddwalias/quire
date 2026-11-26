# Lane Quire.Controls.Plots (agy-whiteboard-3, task_d5c5400c7c44)

- Owner: Orca task `task_d5c5400c7c44` (`agy-whiteboard-3`).
- Branch: `feature/agy-plots-3d-ctrl-vm` (from `main`).
- Scope: `PlotViewModel3D`, `PlotControl3D`.

## Batch 1 — `PlotViewModel3D`

- `Quire.Controls.Plots.Plot3D.ViewModel.PlotViewModel3D`: whole-type Release IL parity (including `PropertyNames`, `AddEquationCommand`, `RemoveEquationCommand`, `<SelectedTraces>d__41`, all compiler-generated closures `<>c` / `<>c__DisplayClass*`, properties, commands, and event handlers).
- Release IL report: `Release IL: PASS; 4009 checked` (`python tools/validation/release_il.py --base main --project Quire.Controls.Plots`).
- Remaining in scope: `PlotControl3D`.
