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

## Batch 8 — AxisMarkerViewModel, BaseMarkerViewModel

- AxisMarkerViewModel exact: `Value`, `EquationProxy`, `IsUserSetValue`, `RealNumberCalculatable`, `CanChangeLineStyle`, `ChangeLineWeight`, `InternalSetIsUserSetValueKeepHiddenFromCalculation`, `InternalSetValue`. Held: `.ctor`, `LineStyle`, `LineWeight`, `ExecuteChangeValueAndIsUserSetFlagCommand` (lambdas in `UndoableCommand`/`RequestShowError`: closure ordinals); `ChangeLineStyle` (one attempt; the `AssertionE.Failed != (AssertionE)DevAssert(...)` branch shape differs).
- BaseMarkerViewModel exact: `Model`, `ShowCallout`, `Calculatable`, `IsActive`, `DisplayNumberFormat`, `Activated`, `Activate` (event invoked through a local, which matched Mathcad's `stloc.0`), `Deactivate`. Held: `.ctor` and `Color` (lambdas); `ChangeColor` (one attempt; the `AssertionE` cast shape differs at the branch). Blocker: the static `ActivateCommand` initializer (`PlotViewModel.ActivateCommand`) lives in the generated field declaration, which I may not edit, so the `.cctor` stays unmatched until MirrorGen emits it.
- Release IL (project gate, changed methods, rebased onto main `6cd4c03b0`): PASS — 1458 checked, 0 failures.

## Batch 9 — XYPlotControl, PlotDataObjectCreator

- XYPlotControl exact: `VerticalMarkersForSerialization`, `HorizontalMarkersForSerialization`, both constructors, `BindCommands`, static `.cctor`, `PostInitializeResizeEventsAfterLoading`, `ReleaseAxisSizes`, `SetGraphSize`, `FixateAxesSizeAtDeserialization`. Held: `EndInit` (lambda in `ExecuteOnLoad`: closure ordinal). The `[DependsOn("FirstPropertyForXamlSerialization")]` attributes are not written because Quire has no `DependsOnAttribute` type; they do not change method IL.
- PlotDataObjectCreator exact (13): `CreatePlotDataObject`, `CreatePolarPlotType`, `GetXLegendPosition`, `GetYLegendPosition`, `GetPlotTitlePosition`, `CreateXyPlotType`, `CreateXyDomainType`, `CreateValueType`, `FillAxisBaseType`, `FillEquations`, `CreateNumberFormat`, `CreateTrace`, `CreateTraceStyle`. The committed `ConvertLineStyle` overloads are kept as they were.
- Held (one attempt each, first write): `ConvertBackgroundType`, `ConvertScaling`, `ConvertAxisPosition`, `ConvertTraceType`, `ConvertSymbol` (C# `switch` expressions lower differently from Mathcad's switch tables), `FillPlotBaseType` (the `for` loop over `plotControl.Traces` differs in its array load order).
- Release IL (project gate, changed methods, rebased onto main): PASS — 1514 checked, 0 failures.

## Batch 10 — PlotGraphRadialAxis

- Exact (14): `Radius`, `TickOffset`, `RadiusWTickOffset`, `ArrowHeadGeometry`, `Position`, `Helper`, static `.cctor` (`StartProperty`/`EndProperty` overrides), `.ctor`, `ArrangeOverride`, `MeasureOverride`, `OnRender`, `Draw`, `GetTickmarksGeometry`, `GetTickMarkPositionByRange`.
- Held (one attempt each, first write): `ClipBounds` (the `0.0 - num` constant ordering differs at the first instruction); `GetTickMarkElementPosition` (its `Point` return and `UIElement` argument order differs).
- Not yet attempted (still generated stubs, not parked): the nested `Point2ScreenHelperImpl` class, `GetTickMarkPositionByIndex`, `GetTickLine`, `AxisLineGeometry`, `DrawNumbers`, and the overrides `GetAxisStartPosition`, `GetAxisEndPosition`, `TickMarksChanged`, `HandleOrientationChanged`, `PositionTickMarkChanged`, `UpdateAxisOriginPositions`, `MouseMoveDraw`, `MoveToValue`, `MoveByValue`, `FindClosestTickValueToZero`, `ApplyTickNumber`.
- Release IL (project gate, changed methods, rebased onto main): PASS — 1529 checked, 0 failures.

## Batch 11 — PlotGraphAngularAxis

- Exact (24): `RadiusRatio`, `Radius`, `TickOffset`, `RadiusWTickOffset`, `Position`, `IsDiscrete`, `Helper`, static `.cctor` (0 to 360 metadata), `.ctor`, `GetTickMarkElementPosition`, `ArrangeOverride`, `SetTicksExpressionsEqnVisibility`, `MeasureOverride`, `OnRender`, `SecondTickExpressionEqn_NavigateOffOfEquation`, `Draw`, `GetTickmarksGeometry`, `GetTickLine`, `AxisCircleGeometry`, `GetTickMarkPositionByAngle`, `GetTickMarkPositionByIndex`, `DrawNumbers`, `ShouldArrangeConvertedEquationsHorizontally`.
- Not yet attempted (still generated stubs, not parked): `HitTestCore` and `CollectElementsForTraversal` (iterator), the nested `Point2ScreenHelperImpl`, `ClipBounds`, `GetAxisEffectiveStart`/`End`, `GetAxisStartPosition`/`EndPosition` overrides, `TickMarksChanged`, `HandleOrientationChanged`, `PositionTickMarkChanged`, `UpdateAxisOriginPositions`, `MouseMoveDraw`, `MoveToValue`, `MoveByValue`, `FindClosestTickValueToZero`, `ApplyTickNumber`.
- Release IL (project gate, changed methods, rebased onto main): PASS — 1553 checked, 0 failures.

## Batch 12 — PlotTrace3D

- Exact (16): static `.cctor` (`_emptyPoint3DCollection`, written as a static constructor because the generated field has no initializer), `BaseLineThickness`, `ShouldShowSymbols`, `ShouldShowLines`, `ViewModel`, `RenderThickness`, `RenderColor`, `Thickness`, `SurfaceFill`, `LineStyle`, `Symbol`, `Visual`, `VisualChanged`, `ViewModelPropertyChanged` (string switch), `UpdateLineWeight`, `UpdateColor`.
- Held (one attempt, first write): `.ctor(IPlotTraceViewModel3D)`: the `_baseLineThickness = 1.0` initializer inside the constructor body and the field order differ from Mathcad's.
- Not yet attempted (still generated stubs): `FireVisualChanged`, `AddOrRemoveFromVisual`, `UpdateLineStyle`, `UpdateSymbol`, `UpdateSurfaceFill`, `CreateSolidMesh`, `UpdateVisualData`. `Dispose` remains its generated `{ }` body.
- Release IL (project gate, changed methods, rebased onto main): PASS — 1575 checked, 0 failures.

## Batch 13 — PlotGraphAxisBase (partial)

- Exact (9 + static `.cctor`): static `.cctor` (type, `NumbersBrush`, `LineColorProperty`, `ShowGridLinesProperty`, `GridLineColorProperty`, `TickMarksProperty` in source order; the generated fields have no initializers, so these are assigned in the static constructor), `LineColor`, `ViewModel`, `StartExpressionEqn`, `SecondTickExpressionEqn`, `EndExpressionEqn`, `ShowGridLines`, `GridLineColor`, `LineColorChanged`.
- Held (one attempt, first write): `.ctor()`: the instance field initializers (`LineBrush`, `_firstLabel`/`_secondLabel`/`_lastLabel`, `GridLineBrush`) are written in the constructor body, and the base-constructor call sequence differs from Mathcad's (`callvirt` in place of Quire's `call FrameworkElement::.ctor`).
- Not yet attempted (still generated stubs): `TickMarks` (lambda in `Dispatch`), `InitializeLimitEquation` (both overloads), `LimitEquationSExpressionChanging`, `LimitEditorLostFocus`, `ShowLimit`, `HideLimit`, the Show/Hide start/second/end methods, `ShowGridLinesChanged`, `GridLineColorChanged`, `TickmarksChanged`, `CreateTickmarksPropertyMetadata`, `Dispatch`, `InitializeLimitLabel`, and `TickMarksChanged`.
- Release IL (project gate, changed methods, rebased onto main): PASS — 1615 checked, 0 failures.

## Batch 14 — PlotGraphLinearAxis (partial)

- Exact: static `.cctor` (`_typeOfThis`, `PositionProperty`), `ColOrRowIndex1`/`ColOrRowIndex2`, `MouseMoveAdorner`, `IsEffectAxis`, `OtherLinearAxis`, `PositionValue`, `SizeFrozen`, `CalcPositionTickMarkNeeded`, the parameterless `.ctor()` (`this(null)`).
- Held (one attempt, first write): `Splitter` setter (the `_splitter` event unhooking order differs from Mathcad) and `.ctor(EquationList, PlotTitle)` (the `_savedLength = double.NaN` field-initializer position and base-call form differ).
- Not written (stubs kept, not parked): `Position` (lambda in `Dispatch`), the nested `MouseMoveRTDataAdorner` (kept as generated stubs; my nested partial collided with the generated `OnRender`), `InitializeColOrRowIndexProperties`, `Splitter_DragCompleted`/`Splitter_DragDelta`, and the remaining overrides and helpers.
- Release IL (project gate, changed methods, rebased onto main): PASS — 1646 checked, 0 failures.

## Batch 15 — PlotGraphControl

- Exact (8): `selectedTrace`, `XAxis`, `YAxis`, `AngularAxis`, `RadialAxis` (setters hook the axis `ShowGridLinesChanged` and `TickMarksChangedEvent`), `TracesUnsorted`, `Count`, and the `OnTraceRemoved`/`OnTraceAdded` callback properties (their generated declarations are removed from the `.g.cs` by hand, with their attributes).
- Not written (stubs kept): the static constructor and the static field initializers (`selectedTraceProperty`, `TraceCountByTypePropertyKey`, and the other registrations; their full text is not yet read), the constructor `PlotGraphControl(PlotType)`, `SelectedTraceChanged`, the trace count/place attached properties, the `Children` collection handlers, `AddTrace`, `RemoveTrace`, `MeasureOverride`, `ArrangeOverride`, `OnRender`, and the gridline drawing helpers.
- Release IL (project gate, changed methods, rebased onto main): PASS — 1662 checked, 0 failures.
