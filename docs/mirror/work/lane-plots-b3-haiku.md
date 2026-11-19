# Lane Quire.Controls.Plots, b3 (haiku)

- Owner: Orca task `task_78c2af8d288d`.
- Branch: `mirror/controls-plots-b3-haiku-20261119` from `main` (ddef5484d); worktree `C:/Users/DuyDang/orca/workspaces/Quire/plots-b3-haiku`.
- Files: stub files under `src/Quire.Controls.Plots/Generated/Mirror` from `Controls.Plots.Presenter.PlotPresenter.g.cs` through `Controls.Plots.View.PlotControlBase.g.cs`.

## Group 1 — Traces.TraceRenderer

- Moved (15 members, hand-written in `src/Quire.Controls.Plots/Mirrored/Controls.Plots.Traces.TraceRenderer.cs`): `.ctor(IPlotTrace)`, `Render`, `DrawGeometry`, `CreateSymbols`, `CreateSymbolsGeometryDictionary`, `CreateTraceLineDictionary`, `FillBrush`, `LinePen`, `SymbolPen`, `ScreenPoints`, `HasPoints`, `LastScreenPointCollection`, `TotalPointCount`, `AddNewScreenPointCollection`, `SetScreenPoints`.
- Release IL (`--type Quire.Controls.Plots.Traces.TraceRenderer`): the 18 non-abstract members are exact; the failures list only the two stubs below, which are unchanged. `DrawGeometry` and `CreateSymbols` needed the positive `!= null` shape (Mathcad uses `brfalse` to skip the body).
- Remaining (stubs kept):
  - `DrawSymbolGeometry`: lambda closure `TraceRenderer+<>c__DisplayClass10_0` (closure; stays a stub).
  - `BreakPolylineIfItIsTooLong`: `Sum` lambda cached in `TraceRenderer+<>c` (closure; stays a stub).
  - `OnRender`: abstract declaration, no body.
- Not written: the static field initialisation (`.cctor`, no stub in the generated file; static constructors stay out of scope for this lane).
