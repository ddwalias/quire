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

## Batch 3 — `PlotAxesWidget3D`

- Branch: `feature/agy-plots-axes3d-b3`.
- `Quire.Controls.Plots.Plot3D.View.PlotAxesWidget3D`: whole-type Release IL parity (including `PropertyNames`, all properties/methods, closures `<>c__DisplayClass25_0`, `<>c__DisplayClass25_1`, `<>c::__27_0`, `<>c__DisplayClass28_0`, `<>c__DisplayClass38_0`, iterator `<AllAxes>d__36`, and XAML `InitializeComponent`, `_CreateDelegate`, `IComponentConnector.Connect`).
- Release IL report: `Release IL: PASS; 3858 checked` (`python tools/validation/release_il.py --base main --project Quire.Controls.Plots`).
- Remaining in scope: `PlotAxis3D`.

## Batch 4 — `PlotAxis3D`

- Branch: `feature/agy-plots-axes3d-b4`.
- `Quire.Controls.Plots.Plot3D.View.PlotAxis3D`: Release IL parity for all remaining members and closures (`PropertyNames`, `<>c__DisplayClass43_0`, `<RemoveAllLabelsFromViewport>b__66_0`, dependency properties, layout, label visuals, arrowhead and tickmark geometry), except two parked compiler-difference stubs.
- Parked compiler differences (kept at exact Mathcad source slots with `MirrorStub.NotImplemented()`):
  - `GetRelativeDistanceInWorld`: Mathcad emits three unreferenced locals (`[0] Vector3D, [1..2] Point4D`) and allocates the `(targetWorld - origin).Length` temporary in slot 3 before user locals `[4, 5]`; modern Roslyn strips unused locals and orders user locals before expression temporaries.
  - `CreateTickmarkModelVisual3D`: Mathcad evaluates nested `MaterialGroup` and `GeometryModel3D` into locals `[0, 1]` before `dup`ing `ModelVisual3D` for `set_Content`; modern Roslyn emits `dup` immediately after `newobj ModelVisual3D` without locals.
- Release IL report: `Release IL: PASS; 3940 checked` (`python tools/validation/release_il.py --base main --project Quire.Controls.Plots`).
- Remaining in scope: none (all 5 claimed types landed; 2 compiler-difference stubs parked in `PlotAxis3D`).

