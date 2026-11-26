# Lane Quire.Controls.Plots (agy-whiteboard-3)

- Owner: Orca task `task_c7ae56469c29` (`agy-whiteboard-3`).
- Branch: `feature/agy-plots-axes3d` (from `main`).
- Scope: `Base3DAxisViewModel`, `PlotGraph3D`, `PlotGraphControl`, `PlotAxesWidget3D`, `PlotAxis3D`.

## Batch 1 — `Base3DAxisViewModel` + `PlotGraph3D`

- `Quire.Controls.Plots.Base3D.ViewModel.Base3DAxisViewModel`: whole-type Release IL parity (all remaining generated stubs implemented, including `get_ClippingInterval`, `LimitsExpressionValueChanged`, `.ctor`, `HookupView`, event accessors, properties).
- `Quire.Controls.Plots.Plot3D.View.PlotGraph3D`: whole-type Release IL parity (all remaining generated stubs and compiler-generated closures `<>c__DisplayClass7_0`, `<>c__DisplayClass34_0`, `<>c__DisplayClass54_0`, plus XAML glue `InitializeComponent`, `_CreateDelegate`, `IComponentConnector.Connect`).
- Release IL report: `Release IL: PASS; 3750 checked` (`python tools/validation/release_il.py --base main --project Quire.Controls.Plots`).
- Remaining in scope: `PlotAxesWidget3D`, `PlotAxis3D`.

## Batch 2 — `PlotGraphControl`

- Branch: `feature/agy-plots-axes3d-b2`.
- `Quire.Controls.Plots.PlotGraphControl`: whole-type Release IL parity (all remaining generated stubs, `.cctor`, closure `<>c__DisplayClass62_0`, gridline and layout methods).
- Release IL report: `Release IL: PASS; 3786 checked` (`python tools/validation/release_il.py --base main --project Quire.Controls.Plots`).
- Remaining in scope: `PlotAxesWidget3D`, `PlotAxis3D`.
