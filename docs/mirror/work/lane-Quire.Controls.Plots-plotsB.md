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
