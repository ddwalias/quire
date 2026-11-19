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

## Group 2 — Presenter.PlotPresenter (partial)

- Moved (16 members, hand-written in `src/Quire.Controls.Plots/Mirrored/Controls.Plots.Presenter.PlotPresenter.cs`): `BoundToAxisViewEvents`, the six `View_*` handlers (`YLegendPositionsIndexChanged`, `XLegendPositionsIndexChanged`, `TitlePositionsIndexChanged`, `PlotLegendPositionsIndexChanged`, `PlotLegendWidthChanged`, `BackgroundTypePropertyChanged`), `View_HasOriginPositioningChanged`, `Model_DeserializationStarted`, `Model_DeserializationFinished`, `PrimaryYAxis_EquationAdded`, `xAxis_EquationAdded`, `xAxis_EquationRemoved`, `PrimaryYAxis_EquationRemoved`, `HandleSelectTraces`, `AddTrace`.
- Release IL (`--type Quire.Controls.Plots.Presenter.PlotPresenter`): none of the 16 moved members is in the failure list. The listed failures are the still-stubbed members.
- Remaining stubs (closures or not yet read): `.ctor` (field initializers), `BoundToEvents`, `BoundToAxisModelEvents` (lambdas), `xAxis_EquationContolChange`, `PrimaryYAxis_EquationContolChange` (lambda FirstOrDefault), `HandleEquationAdded`, `HandleEquationRemoved`, `GetViewAxis`, `GetViewTrace`, `SetCommonRange`, `RearrangeTraces`, `CheckTraceVsPlotType`, `AxisModelLimitsValueChanged`, `SetRangeAxisCommonRange`, `SetResultAxisCommonRange`, `MoveFirstTrace`, `MoveAllButFirstTrace`, `RemoveTraceFromEquation`, `RemovePlotLegendItemFromPlotLegend`, `AddTraceToEquation` (two overloads), nested `AddItemCommand`/`RemoveItemCommand` members, and the closure display-class members.
- Not written: the instance field initializers (`_axesMap`, `_tracesMap`, `_traceStyleSelector`) and the constructor body, which stay generated stubs.
