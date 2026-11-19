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

## Batch 2 — PlotItem

- Exact (23): `_typeOfThis`/`DragBorder`/`_dragStarted`/`_border`, both DependencyProperty fields, `IsWidthExplicitlySet`, `MoveDraggerVisibility`, `IsBorderVisible`, `Header`, `RightDraggerVisibility`, `Border`, `VisualBrushRectangle`, `DragStarted`/`DragEnded` events, `OnIsMoveableChanged`, `FireDragStarted`, `FireDragEnded`, `OnApplyTemplate`, `BorderMouseUp`, `RequestMoveAndResize`, `BorderPreviewMouseMove`, static `.cctor`, `.ctor`.
- Held stubs (one attempt each): `RegisterOrUnregisterFromBorderEvents` (Mathcad tests `register` first and checks `_border` once; two shapes tried, field-access order still differs at `_border`); `OnContentSizeChanged` (operand order of `!IsWidthExplicitlySet && e.WidthChanged` tried both ways; the `SizeChangedEventArgs.WidthChanged` call target differs); `Move` (its `PlotLegend.ViewModel`/`PlotTitle.ViewModel` call signature differs from Quire's stub types, not yet resolved).
- Release IL (project gate, changed methods): PASS — 1330 checked, 0 failures.

## Batch 3 — PlotLegendItem

- Exact (9 incl. fields): `_legendLabel`, `DefaultMiniTraceWidth`/`MiniTraceDefaultHeight` consts, `MiniTraceDefaultMargin`, `MiniTraceDefaultPoints`, `MiniTraceWidthProperty`, `LegendItemStackPanel`/`MiniTraceLegend`/`LegendItemLabelPlaceholder`, `_contentLoaded`, `SourceTrace`, `LegendLabel`, `MiniTraceWidth` accessors, `OnLegendLabelLostFocus` (null guard as an early return; the `&&` form differed), `GetDataObject`, `SetDataObject`.
- Held stubs (blockers): `.ctor(IPlotTrace)` (WPF lambdas via `WpfUtils.ExecuteOnLoad`; closure ordinals depend on the whole type); `OnActivateExecute` and `AddTrace` (lambdas: closure ordinals); `GetRegionXmlNode` and `SetRegionXmlNode` (pattern matching on `IRegionType`/`textType` and XmlDocument calls, not attempted); `InitializeComponent` and `IComponentConnector.Connect` (XAML-generated bodies, not reproducible by hand here).
- Release IL (project gate, changed methods): PASS — 1340 checked, 0 failures.

## Batch 4 — PlotTitle

- Exact: `_isEditingYAxisTitle`, `TitleTypeProperty` (static initializer), `PART_TitleHost`, `ViewModel`/`IsRotationAllowed`/`Title` accessors, `TitleType` accessors, `.ctor(PlotTitleType)` (calls the held `InitializeComponent`), `SetWidthFromContent`, `ResetWidthConstraints`, `OnElementOnGotFocus`, `OnElementOnLostFocus`, `ApplyTemporaryOrientation`, `InitializeYAxisOrientationSupport`, `ToggleOrientation`.
- Held stubs (one attempt each): `GetDefaultTitleText` (switch expression lowers to a switch table with a different case layout than Mathcad's 2,4,6); `RegisterToEvents` (null-guard form at `if (title != null)` differs at the first branch); `TitleSizeChanged` (`ViewModel != null && Visibility != Collapsed` branch order differs); `HandleEditingStarted` (`IsVertical` call resolves to a different getter than Mathcad's); `HandleEditingEnded` (the `bne.un` shape of the three-part condition differs); `HideTitleIfEmpty` (the `ViewModel != null && ...` chain shape differs). Each needs its own closer look at the Mathcad IL.
- Held XAML members (blocker): `InitializeComponent`, `_CreateDelegate`, `IComponentConnector.Connect`, plus the `_contentLoaded` field they use.
- Release IL (project gate, changed methods): PASS — 1357 checked, 0 failures.

## Batch 5 — PlotLegend

- Exact: `DefaultTraceLabelFormat`, `PlotTypeProperty` and the static constructor (the `WidthProperty` override uses the held `PlotLegendWidthChanged` stub by method group), `_legendWidthChanged`, `_pendingLegendWidth`, `PlotLegendGrid`, `ViewModel`, `TextStyleChangedHandler`, `InitialTextFormat`, `PlotType`, `MaxItemWidth`, `PlotLegendListEnumerable`, `PlotLegendItemsCount`, the explicit `IPlotLegend.LegendWidthChanged` event, `.ctor`, `SetMinMaxWidth()`, `RemoveTraceFromPlotLegend`.
- Held stubs (one attempt each unless noted): `Columns` setter (the `!=` guard lowers to `beq.s` where Mathcad uses `bne.un.s`); `SetLegendWidthWhenLayoutReady` (branch shape at the `HasValue` check); `PlotLegendSizeChanged` (argument load order in `legendWidthChanged(val.Width)`); `PlotLegendWidthChanged` (`dObj as PlotLegend` null-check shape); `AddNewItemToPlotLegend` and `UpdateDefaultLegendLabelsAfterIndex` (the `$"Trace {…}"` interpolation lowers to `DefaultInterpolatedStringHandler` calls that differ from Mathcad's `String.Format`-style call); `PlotLegendItemSizeChanged` (argument order at `e.PreviousSize`/`e.NewSize`).
- Held lambda members (blocker, closure ordinal): `IPlotLegend.LegendWidth` (lambda in `ExecuteOnLoad`), `SetMinMaxWidth(double, double)` (lambda in `Max`), `FindByTrace` (lambda in `FirstOrDefault`).
- Held XAML members (blocker): `InitializeComponent`, `_CreateDelegate`, `IComponentConnector.Connect`, `_contentLoaded`, `PlotLegendGrid` field's XAML connect.
- Release IL (project gate, changed methods): PASS — 1374 checked, 0 failures.

## Batch 6 — PlotEquationBase

- Exact: `IsCommitted`, `ViewModel` (virtual get/internal set with `DevAssert(false)`), `MathFormat`, `MathFormatForSerialize`, `CurrentMathFormat`, `InCustomInitialization`, `DeserializeMathFormat` and `DeserializeLabeledIdFormat` (setters written with an early `return` on null; the `if (value != null)` nesting differed), the two events, `.ctor`, `ResetBeforeAdding`, `ResetIsCommitted`, `OnSubEquationExpressionChanged`, `FireExpressionValueChanged`, `AttachSExpressionValueChanged`, `DetachSExpressionValueChanged`, `HandleMathFormatChanged`, `FirePropertyChanged`, and the six command handlers (`OnChangeHighlightColorPreview` … `CanClearMathFormat`).
- Abstract members (`ContainedEquationControls`, `Expression`, `ScalingEquation`) stay as generated declarations.
- Release IL (project gate, changed methods, rebased onto main `07bd98389`): PASS — 1414 checked, 0 failures.

## Batch 7 — PlotGraphLinearAxisBase

- Exact: `_typeOfThis`, `ShowArrowHeadProperty`, `EdgeTickMarkScale`, `MainTicksRangeScale`, `ScaleEdgeEvenlyProperty`, `ShowArrowHead`, `ScaleEdgeEvenly`, `.ctor`, `GetAxisStartPosition`, `GetAxisEndPosition`, `GetMinorTickPositionByTickNumber`, `GetMinorTickmarks`, `HandleOrientationChanged`, `GetTickmarksGeometry`, `GetTickPosition`, `GetTickRelativePosition(TickScale, int)`, `GetTickPositionByTickNumber`, `GetTickRelativePosition(int)`, `Draw`.
- Held stubs (one attempt each, first write): `ArrowHeadGeometry` (`PathFigure` initializer's `Point` construction differs); `GetTickLine` (constants differ at the first `Point`); `GetTickValueTextAndPositionByTickNumber` (out-`start` local shape differs); `Length` (local slot order at `desiredSize`/`num`); `DrawNumbers` (`out var start` lowering differs in the loop).
- Not yet attempted (still generated stubs, not parked): `GetRealOffset`, `GetRealRelaviveOffset`, `ArrangeOverride`, `GetPositions`, `MeasureOverride`, `OnRender`.
- Release IL (project gate, changed methods, rebased onto main `c02f47e40`): PASS — 1436 checked, 0 failures.
