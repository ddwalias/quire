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

## Group 3 — Presenter.PlotPresenter (continued)

- Moved (9 more members): `RemovePlotLegendItemFromPlotLegend`, both `AddTraceToEquation` overloads, `RemoveTraceFromEquation`, `RearrangeTraces` (switch with `default` first), `MoveFirstTrace`, `MoveAllButFirstTrace` (the `IEnumerable<T>` cast in the foreach is needed to match Mathcad), `GetViewAxis`, `GetViewTrace`, and the four-argument static `SetCommonRange`.
- Release IL (`--type Quire.Controls.Plots.Presenter.PlotPresenter`): none of the nine moved members is in the failure list.
- Parked: `CheckTraceVsPlotType` (two shapes tried: the `!(a ^ b) && !(c ^ d)` early return and the positive `(a ^ b) || (c ^ d)` block). Both differ at the first null-test branch (instruction 9: Quire `beq.s 24`, Mathcad `beq.s 25`, Mathcad has one more instruction in the preamble). Stub restored; no hand-written body.
- Still stubs: the closure members listed in group 2, and `SetCommonRange(ITraceModel)` (lambdas).

## Group 4 — View.PlotControlBase (passes 1-2)

- Partial `src/Quire.Controls.Plots/Mirrored/Controls.Plots.View.PlotControlBase.cs` holds every movable method and property in Mathcad order. Unfinished members are unmarked placeholders (`=> throw NotImplemented()`), so closure ordinals line up. Events, abstract members and fields stay generated.
- Implemented and passing the type gate: 120 members (plain properties, DependencyProperty accessors, focus/origin/error/grid/traversal handlers, text-format and math-format members).
- Parked after one attempt (placeholder, blocker recorded): `CurrentDisplayIdLabel` setter (Mathcad `brfalse` at the null test, Quire `brtrue`), `CurrentError` getter (Mathcad `stloc.0` temp, Quire `dup`), `ChangeBackgroundCommandExecute` (`SetRegionPropertyCommand.Execute` is `call`, Mathcad `callvirt`), `BindChangeOriginPositioningCommand` (delegate cache `<>O` ordinal differs), `HandleGotFocus` (`as` shape at the first cast differs).
- Closure, LINQ and iterator members (`CalculatableChildren`, `DelegateFocus`, `BindCommands`, `ToggleGridLines`, `ToggleTicks`, `GetShowTicks`, `GetShowNumbers`, `UpdateMathFormat`, `SortRegionsByLocation`, `GetISearchableEnumerator`, `GetSearchableRegions`, `FindOncePrivate`, `DeserializeMathFormat`, `DeserializeLabeledIdFormat`, `InCustomInitialization`, `.ctor`, `.cctor`, and others) remain placeholders.
- Remaining passes: the rest of the placeholder set, then `ExecuteAxesSetPropertyCommand`, `ShouldSerializeProperty`, `GetErrorMessage`, and the closure members in pass 3.

## Group 4 — View.PlotControlBase (passes 3, final state)

- Implemented and gated in total: 122 members (120 from passes 1-2 plus `ExecuteAxesSetPropertyCommand<T>` and `OnSetNumberFormatPropertyPreview`). Both pass the type gate.
- Placeholders remaining (30), with named blockers:
  - Closure ordinals: `BindCommands`, `DelegateFocus`, `FindOncePrivate`, `DeserializeMathFormat`, `DeserializeLabeledIdFormat`, `InCustomInitialization`, `GetShowGridLines`, `ToggleGridLines`, `ChangeGridLineColor`, `ToggleTicks`, `GetShowTicks`, `GetShowNumbers`, `ToggleNumbered`, `UpdateCurrentNumberFormat`, `CanClearResultFormat`, `UpdateMathFormat`. Each one's lambda is numbered `b__<n>_0` from its position in the whole class, so these need the earlier closure members (`BindCommands`, `DelegateFocus`, `FindOncePrivate`, and the search helpers) in place first. Ten were tried, all failed on the lambda name, and were reverted.
  - Iterators: `CalculatableChildren`, `GetISearchableEnumerator`, `GetSearchableRegions`, `SortRegionsByLocation` (LINQ closures).
  - Interface/XAML-bound: `IHasChildArrangeInfoProvider.ChildArrangeInfoProvider` (explicit implementation, not yet attempted), `CurrentDisplayIdLabel` setter, `CurrentError` getter (shape differs, see group 4 pass 2).
  - Shape differences (one or two attempts each): `NavigateIntoItem` (Mathcad keeps the result in a local), `GetErrorMessage`, `HandleApplyTextStyleCommand`, `ChangeBackgroundCommandExecute` (`callvirt` on `SetRegionPropertyCommand.Execute`), `HandleGotFocus`.
  - Blocked by a missing Quire type: `ChangeOriginPositioningCommandExecute` (`ButtonCommandParameter` has no Quire counterpart in `type-map.csv`), and its caller `BindChangeOriginPositioningCommand` (delegate cache ordinal `<2>` vs Mathcad `<12>`).
- Not attempted: the rest of the closure members listed above, because their lambda numbers depend on the parked ones.
