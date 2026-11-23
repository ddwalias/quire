# Lane Plots — Quire.Controls.Plots

- Owner: Orca task `task_f28f3845158a` (maintainer-assigned; no WORK.md lane row).
- Branch: `mirror/controls-plots-20261108`; worktree `C:/Users/DuyDang/orca/workspaces/Quire/controls-plots-20261108`.
- Baseline (main `6c22e4dd`): 8917 stub markers in `Generated/Mirror`; Release IL report counts 0 exact / 5243 unfinished before batch 1 (`--project Quire.Controls.Plots`).
- Reference: `python tools/reference/reference.py Ptc.Controls.Plots.<Type> --assembly Ptc.Controls.Plots --il` (full Mathcad type name; run without `PYTHONUTF8=1`).

## Batch 1 — GridLenghtToMargin

- Members: `Convert`, `ConvertBack`, `.ctor`. Hand-written: `src/Quire.Controls.Plots/Mirrored/Controls.Plots.GridLenghtToMargin.cs`; stubs removed from `Generated/Mirror/Controls.Plots.GridLenghtToMargin.g.cs`.
- Shapes: `Convert` as `text switch` expression with `(object)` cast on the first arm → opcode mismatch (Mathcad returns directly from each arm, joining nothing through a shared local). `switch` statement with `return` per case → exact.
- Release IL: PASS — 356 checked, 356 exact, 0 failures, 4887 unfinished.
- Remaining mismatches in this batch: none.

## Batch 2 — BooleanToVisibilityConverter, ColorToSolidColorBrushConverter

- Members: `Instance` (static field initializer), private `.ctor`, `Convert`, `ConvertBack` in each; class attributes `[ValueConversion]` on the hand-written partials. Hand-written: `Mirrored/Controls.Plots.View.{BooleanToVisibilityConverter,ColorToSolidColorBrushConverter}.cs`; stubs removed from the matching `.g.cs`.
- Shapes: idiomatic forms matched first time (`solidColorBrush.Freeze()` without the `(Freezable)` cast; `??` for the result).
- Release IL: PASS — 362 checked, 362 exact, 0 failures, 4883 unfinished.
- Remaining mismatches in this batch: none.

## Batch 3 — MultiGridLengthsToMargin, LimitLabelVisibilityMultiConverter

- Members: `MultiGridLengthsToMargin` (const `ConstMargin`, `Convert`, `ConvertBack`, `.ctor`); `LimitLabelVisibilityMultiConverter` (`Instance`, const `OpacityVisible`, `Convert` x2 incl. protected virtual overload, `ConvertBack`, protected `.ctor`, static `.cctor`). Hand-written: `Mirrored/Controls.Plots.{MultiGridLengthsToMargin,LimitLabelVisibilityMultiConverter}.cs`; stubs removed from the matching `.g.cs`. The `[NotNull]` attribute on `Instance` is not carried (JetBrains annotation, not in the Quire project).
- Shapes: `MultiGridLengthsToMargin.Convert`: `switch` statement with `return` per case → exact; a switch expression fails. The switch subject must be a top-level `string text` local (slot order); a bare `switch ((parameter ?? "")...)` put the `default(Thickness)` temp in the wrong slot (`ldloca.s 1` vs `2`). `LimitLabelVisibilityMultiConverter.Convert`: the negated ternary `(!Convert(values)) ? Collapsed : Visible` emits `brfalse` where Mathcad emits `brtrue`; the positive form `Convert(values) ? Visible : Collapsed` matches.
- Shape check (opcode-level) passed for both types before the gate.
- Release IL: PASS — 368 checked, 368 exact, 0 failures, 4878 unfinished.
- Remaining mismatches in this batch: none.

## Batch 4 — OpacityConverter, SecondTicktLabelVisibilityMultiConverter

- Members: `OpacityConverter` (`Instance`, private `.ctor`, `Convert`, `ConvertBack`); `SecondTicktLabelVisibilityMultiConverter` (`Instance` declared `new` to hide the base field, private `.ctor`, `protected override Convert(object[])`). Hand-written: `Mirrored/Controls.Plots.Plot3D.View.OpacityConverter.cs` and `Mirrored/Controls.Plots.SecondTicktLabelVisibilityMultiConverter.cs`; stubs removed from the matching `.g.cs`.
- Shapes: the `SecondTickt` override matched the opcode-level check first time. `OpacityConverter.Convert` uses the source form (`values.Length != 3 || ...` guard, `DebugTools.FailAssertion(null, Array.Empty<object>())`, local `flag`/`flag2`) and matched the Release gate first time.
- Release IL: PASS — 373 checked, 373 exact, 0 failures, 4875 unfinished.
- Remaining mismatches in this batch: none.
- Not yet done: `BackgroundTypeEToBackgroundConverter` (enum `switch` with a `switch` table; next batch).

## Batch 5 — BackgroundTypeEToBackgroundConverter

- Members: `Instance` (non-readonly static field initializer), `Convert`, `ConvertBack`, `.ctor` (implicit, no hand-written ctor). Hand-written: `Mirrored/Controls.Plots.View.BackgroundTypeEToBackgroundConverter.cs`; stubs removed from the `.g.cs`.
- Shapes: `Convert` as `if (value is BackgroundTypeE backgroundType) switch` with `return` per case → first try failed with "switch targets: Quire 4, Mathcad 5": Mathcad's table covers values 0..4 with `Undefined` (0) sharing the default block. Adding `case BackgroundTypeE.Undefined:` directly above `default:` → exact.
- Release IL: PASS — 376 checked, 376 exact, 0 failures, 4873 unfinished.
- Remaining mismatches in this batch: none.

## Batch 6 — EngineErrorEventArgs, MarkerEventArgs, PlotInvalidResultException

- Members: `EngineErrorEventArgs` (`_engineError` field, `.ctor`, `EngineError` getter); `MarkerEventArgs` (`_marker` field, `.ctor`, `Marker` getter); `PlotInvalidResultException` (three `.ctor`s chaining to `Exception`). Hand-written: `Mirrored/Controls.Plots.{Model.EngineErrorEventArgs,ViewModel.MarkerEventArgs,PlotInvalidResultException}.cs`; stubs removed from the matching `.g.cs`.
- Shapes: explicit `readonly` field assigned in the constructor and expression-bodied getters matched first time; the exception constructors chain to `base(...)` with the original overload set.
- Release IL: PASS — 382 checked, 382 exact, 0 failures.

## Batch 7 — Traces.Symbol, CircleSymbol, RectangleSymbol, FilledRectangleSymbol

- Members: `Symbol` (`IsFilled` virtual getter, static `CreatePathGeometryPointCollection`, protected `.ctor`; abstract `Create` stays in the `.g.cs` as a declaration); `CircleSymbol` (`Create`, `.ctor`); `RectangleSymbol` (`Create`, `.ctor`); `FilledRectangleSymbol` (`IsFilled` override, `.ctor`). Hand-written: `Mirrored/Controls.Plots.Traces.{Symbol,CircleSymbol,RectangleSymbol,FilledRectangleSymbol}.cs`; stubs removed from the matching `.g.cs`.
- Shapes: `RectangleSymbol.Create` reassigns the `point` parameter (`point = new Point(...)`) and passes the `PointCollection` object initializer straight into the helper; that matched. `CreatePathGeometryPointCollection` returns `new PathGeometry(new PathFigureCollection()) { Figures = { figure } }` so the geometry stays on the stack with no local; that matched.
- Release IL: PASS — 387 checked, 387 exact, 0 failures.

## Batch 8 — TriangleSymbol, RhombusSymbol and their filled variants

- Members: `TriangleSymbol` (`Create`, `.ctor`), `FilledTriangleSymbol` (`IsFilled` override, `.ctor`), `RhombusSymbol` (`Create`, `.ctor`), `FilledRhombusSymbol` (`IsFilled` override, `.ctor`). Hand-written: `Mirrored/Controls.Plots.Traces.{TriangleSymbol,FilledTriangleSymbol,RhombusSymbol,FilledRhombusSymbol}.cs`; stubs removed from the matching `.g.cs`.
- Shapes: the operand order of the scalar multiply matters. Rhombus's apex is `point.X + weight * 2.0` (Mathcad multiplies `weight` first), while Rectangle and Triangle use `2.0 * weight`. Writing the wrong order would fail; the source form above matched first time.
- Release IL: PASS — 391 checked, 391 exact, 0 failures, 4858 unfinished.

## Batch 9 — PlusSymbol, XSymbol

- Members: `PlusSymbol` (`Create`, private static `CreateVerticalLine`, `CreateGorizontalLine` — Mathcad's spelling kept, `.ctor`); `XSymbol` (`Create`, private static `CreateXSymbolFirstAxe`, `CreateXSymbolSecondAxe`, `.ctor`). Hand-written: `Mirrored/Controls.Plots.Traces.{PlusSymbol,XSymbol}.cs`; stubs removed from the matching `.g.cs`.
- Shapes: `Create` uses the `Figures = { a, b }` object-initializer form on `new PathGeometry(new PathFigureCollection())`, which keeps the geometry on the stack for the return; helpers build `PointCollection` with an initializer, store it in a `PolyLineSegment` via `Points =`, and wrap it with `PathFigure(..., false)`. All matched the opcode check first time.
- Release IL: PASS — 397 checked, 397 exact, 0 failures, 4852 unfinished.

## Batch 10 — StarSymbol, FilledStarSymbol, PointSymbol

- Members: `StarSymbol` (`Create`, `.ctor`); `FilledStarSymbol` (`IsFilled` override, `.ctor`); `PointSymbol` (`IsFilled` override, `Create`, `.ctor`). Hand-written: `Mirrored/Controls.Plots.Traces.{StarSymbol,FilledStarSymbol,PointSymbol}.cs`; stubs removed from the matching `.g.cs`.
- Callee left unfinished: `StarPoints.generateStarPoints` is still a stub. `StarSymbol.Create` calls it with the Mathcad name and the IL matches; the star geometry throws `MirrorStub.NotImplemented` at runtime until `StarPoints` is mirrored.
- Shapes: `PointSymbol.Create` stores `properties.LineWeight` in one local and passes it twice; that matched first time.
- Release IL: PASS — 401 checked, 401 exact, 0 failures, 4848 unfinished.

## Batch 11 — PlotGraph3DWidthHeightConverter, PlotGraph3DContainerParentMarginConverter, TraceLineWeightConverter

- Members: `Instance` static field initializers, private `.ctor`, `Convert` and `ConvertBack` for each; `TraceLineWeightConverter.MinimalWeight` constant. Hand-written: `Mirrored/Controls.Plots.Plot3D.View.{PlotGraph3DWidthHeightConverter,PlotGraph3DContainerParentMarginConverter}.cs` and `Mirrored/Controls.Plots.TraceLineWeightConverter.cs`; stubs removed from the `.g.cs`.
- Shapes: all three matched on the first pass (`Math.Min` boxed through `object`; `new Thickness { ... }` object initializer; `flag ? MinimalWeight : num`).
- Release IL: PASS — 410 checked, 410 exact, 0 failures, 4842 unfinished.
- Not yet done: `PlotGraph3dZEquationMarginConverter` (depends on `WpfUtils.InchesToDips` and `PlotControl3D.AxesWidgetMargin`, a callee in an unfinished type).

## Batch 12 — StarPoints

- Members: all 19 generated stubs: fields `_primaryPoints`, `_secondaryPoints`, `_mergedPoints`, `_rays = 5`, `_radius = 15.0`, `_center = new Point(0,0)`; private `.ctor`; `generateStarPoints` (public static); `getX`, `getY`; `CalculatePoints`, `AdjustToCenter`, `MergePoints`, `FindSecondaryRadius`, `findLineCoefficients`, `findIntersectionPoint`, `BuildPrimaryPoints`, `BuildSecondaryPoints`, `BuildPoints`. Hand-written: `Mirrored/Controls.Plots.StarPoints.cs`; all stubs removed from the `.g.cs`.
- Shapes that failed the opcode check, then matched:
  - `getY` and `findIntersectionPoint`: `0.0 - x` compiles to `ldc.r8 0; sub`, but Mathcad emits `neg`. Source `-radius * ...` and `-(b1 - b2) / ...` match.
  - `primaryPoints[^1]`: Roslyn dups the array reference; Mathcad re-loads the local. `primaryPoints[primaryPoints.Length - 1]` matches.
  - `FindSecondaryRadius` return: `value` / `x` temporaries did not match; the named `Vector val2 = new Vector(val.Value.X, val.Value.Y); return val2.Length;` matches (struct local constructed in place, Point temp in the next slot).
- Runtime smoke test (scratch console app, not committed): `new StarSymbol().Create(new Point(10,10), props)` returns a `PathGeometry` with bounds, so the earlier throw is gone.
- Release IL: PASS — 423 checked, 423 exact, 0 failures, 4829 unfinished.

## Batch 13 — Presenter.AxesTypeToIndex, PlotGraphAxisConsts

- Members: `AxesTypeToIndex` (`Index` static, `.ctor`); `PlotGraphAxisConsts` (six `const` fields: `arrowHeadSize`, `tickLineHalfSize`, `strokethickness`, `Halfstrokethickness`, `TicksTextSize`, `tickTextOffset`). Hand-written: `Mirrored/Controls.Plots.Presenter.AxesTypeToIndex.cs` and `Mirrored/Controls.Plots.PlotGraphAxisConsts.cs`; stubs removed from both `.g.cs`.
- Shapes: `Index` as a `switch` statement with stacked case labels (`case X: case Angular:`) and a `default:` that calls `DebugTools.DevAssert(false, null)` before `return AxesIndex.None` matched first time.
- Release IL: PASS — 424 checked, 424 exact, 0 failures, 4828 unfinished.

## Batch 14 — TraceColorTable, StyleSelectorWithDictionary

- Members: `TraceColorTable` (`Default` static field initializer, private `.ctor` adding seven brushes, static `parseColor`); `StyleSelectorWithDictionary` (`Dictionary` auto-property, `SelectStyle` override, `.ctor`; `[ContentProperty("Dictionary")]` on the hand-written partial). Hand-written: `Mirrored/Controls.Plots.{TraceColorTable,StyleSelectorWithDictionary}.cs`; stubs removed from the `.g.cs`. The JetBrains `[UsedImplicitly]` attribute is not carried (not referenced in the Quire project).
- Shapes: all matched the opcode check first time.
- Release IL: PASS — 430 checked, 430 exact, 0 failures, 4823 unfinished.

## Batch 15 — ColorMapZToBrush, GammaColorMap

- Members: `ColorMapZToBrush` (`Instance` static field, private `.ctor`, `Convert`, `ConvertBack`); `GammaColorMap` (`GetBrush`, `.ctor`). Hand-written: `Mirrored/Controls.Plots.{ColorMapZToBrush,GammaColorMap}.cs`; stubs removed from both `.g.cs`.
- Shapes: `ColorMapZToBrush.Convert` as `return`-per-path (the first try) gave a branch-width mismatch (`ble.s` vs `ble`). Mathcad assigns one `object result` on every path and returns it once; the if/else-assignment form matched. `GammaColorMap.GetBrush` with `new HsvColor { Gamma = (float)value }` matched first time.
- Release IL: PASS — 434 checked, 434 exact, 0 failures, 4820 unfinished.

## Batch 16 — XyEquation, XyzEquation

- Members: `XyEquation` (`_xSExpression`, `_ySExpression` readonly fields, `XsExpression` and `YsExpression` getters, two `.ctor`s with the `this(string.Empty, string.Empty)` chain); `XyzEquation` (`_zSExpression`, `ZsExpression` getter, `.ctor` with `base(...)`, and the default `.ctor` chaining to the three-argument constructor with the trace-expression literal). Hand-written: `Mirrored/Controls.Plots.{XyEquation,XyzEquation}.cs`; stubs removed from both `.g.cs`.
- Shapes: all matched the opcode check first time.
- Release IL: PASS — 441 checked, 441 exact, 0 failures, 4813 unfinished.

## Batch 17 — PointCollectionXml, PlotPointsGenerator

- Members: `PointCollectionXml` (static `Parse`, static `Validate`, `Add(XmlNode, XmlNode)` overload, `.ctor`); `PlotPointsGenerator` (readonly `points` field with its initializer, `Points` getter, `.ctor` with the bottom-tested `for` loop and `break`). The nested `RealFunction` delegate stays in the `.g.cs`. Hand-written: `Mirrored/Controls.Plots.{PointCollectionXml,PlotPointsGenerator}.cs`; stubs removed from both `.g.cs`.
- Shapes: all matched the opcode check first time.
- Release IL: PASS — 446 checked, 446 exact, 0 failures, 4808 unfinished.

## Batch 18 — Presenter.TraceStyleSelector

- Members: `_currentColorIndex` field, `ApplyNextStyle(ITraceModel)` (castclass to `SolidColorBrush` for the colour, enum `none` and `Solid` assignments, `++` with a `>=` wrap to zero), `.ctor`. Hand-written: `Mirrored/Controls.Plots.Presenter.TraceStyleSelector.cs`; stubs removed from the `.g.cs`.
- Shapes: all matched the opcode check first time.
- Release IL: PASS — 447 checked, 447 exact, 0 failures, 4807 unfinished.

## Batch 19 — GradientColorMap

- Members: static fields `TypeOfThis` (private readonly), `SpecialProperty` (readonly), `GradientStopsProperty` (non-readonly) with `DependencyProperty.Register` initializers; `Special` and `GradientStops` dependency-backed properties; `GetBrush`; `.ctor`. Hand-written: `Mirrored/Controls.Plots.GradientColorMap.cs`; stubs removed from the `.g.cs`.
- Shapes that failed, then matched in `GetBrush`:
  - The colour ternary `color = cond ? Special : Color` gives a different arm layout. Mathcad stores the colour in each arm of an if/else.
  - The if/else with `gradientStop2 == null || !(gradientStop2.Offset >= value)` puts the test in the wrong branch order (`bge.s` vs `blt.un.s`). `if (gradientStop2 != null && gradientStop2.Offset >= value) color = gradientStop2.Color; else color = Special;` matches.
- Release IL: PASS — 453 checked, 453 exact, 0 failures, 4802 unfinished.

## Batch 20 attempt — PlotLegendLayoutManager (not landed; reverted)

- Members: `.ctor` x2, `CalcMaxDecimalPerColumn` (override, `newarr`), `Clone` (override), `GetNumberOfColumnsFitWidth`. Left as stubs.
- Shapes tried for `GetNumberOfColumnsFitWidth`:
  - `Size matrixSize` assigned before the first compare: the first `get_MatrixSize` call lands in the wrong position (`calls differ`). Mathcad reads `MatrixSize` inline in the first comparison and keeps local 2 as a struct temporary.
  - Inline `plotLegendLayoutManager.MatrixSize.Width` everywhere: fixes that call order. The `else` branch then differs in the loop shape.
  - `else { while (true) { if (cmp >= 0 || Columns <= 1) break; ... } }`: rejected. Mathcad lays the branch out as a head-tested `while (Compare(newWidth, MatrixSize.Width) < 0 && Columns > 1)` with bottom-tested layout.
  - Head-tested `while` with `int columns` declared inside the body: local slot 4 instead of Mathcad's slot 3 (`Release IL differs`).
  - Same with `int columns;` declared once in the `else` block: still `Release IL differs`.
- Not resolved: the `columns` temporary's slot in the second loop. Next step is an op-level diff of the two bodies, which needs the candidate IL extracted per method (the awk extraction did not isolate the method).

## Batch 21 — LineStyleToDashArrayConverter

- Members: `Instance`, private `.ctor`, `Convert`, `ConvertBack`. Hand-written: `Mirrored/Controls.Plots.LineStyleToDashArrayConverter.cs`; all stubs removed from its `.g.cs`.
- Rejected candidate: `value is LineStyle lineStyle ? lineStyle : LineStyle.None` failed `Convert` at instruction 3 (`brfalse.s 5` vs Mathcad's `brtrue.s 3`). The original-style `value is LineStyle ? (LineStyle)value : LineStyle.None` matches.
- Release IL: PASS — 456 checked, 456 exact, 0 failures, 4794 unfinished.
- Remaining: `PlotLegendLayoutManager.GetNumberOfColumnsFitWidth` remains blocked by the documented local-slot mismatch; other unimplemented stubs remain unclaimed.

## Batch 22 — PlotLegendLayoutManager (unblocked members)

- Members: both constructors, `CalcMaxDecimalPerColumn`, `Clone`. Hand-written: `Mirrored/Controls.Plots.PlotLegendLayoutManager.cs`; these four stubs removed from its `.g.cs`. `GetNumberOfColumnsFitWidth` remains a stub.
- Shapes: constructors chain to the base constructors; `CalcMaxDecimalPerColumn` returns a `MaxMatrixWidth` array sized from `cellsContent.Count`; `Clone` constructs a copy.
- Release IL: PASS — 459 checked, 459 exact, 0 failures.
- Remaining: `GetNumberOfColumnsFitWidth` is blocked by the known local-slot mismatch; other unimplemented stubs remain unclaimed.

## Batch 23 — PlotGraph3dZEquationMarginConverter

- Members: `Instance`, private `.ctor`, `Convert`, `ConvertBack`. Hand-written: `Mirrored/Controls.Plots.Plot3D.View.PlotGraph3dZEquationMarginConverter.cs`; all stubs removed from its `.g.cs`.
- Reference: member-specific extraction omitted the IL file; the method bodies were read from the cached full `Ptc.Controls.Plots.il` Release disassembly.
- Shapes: direct casts for the two `values` elements, a `Thickness` object initializer, conditional right-margin addition, and `NotImplementedException` in `ConvertBack` matched.
- Release IL: PASS — 462 checked, 462 exact, 0 failures.
- Remaining: `PlotLegendLayoutManager.GetNumberOfColumnsFitWidth` remains blocked by the documented local-slot mismatch; other unimplemented stubs remain unclaimed.

## Batch 24 — VectorFormatHelper

- Members: `VectorRl`. Hand-written: `Mirrored/Controls.Plots.Plot3D.Model.VectorFormatHelper.cs`; its stub removed from the `.g.cs`.
- Reference: member-specific extraction omitted the IL file; the method body was read from the cached full `Ptc.Controls.Plots.il` Release disassembly. LINQ projection and invariant numeric formatting matched, including the compiler-generated lambda method.
- Release IL: PASS — 466 checked, 466 exact, 0 failures.
- Remaining: `PlotLegendLayoutManager.GetNumberOfColumnsFitWidth` remains blocked by the documented local-slot mismatch; other unimplemented stubs remain unclaimed.

## Batch 25 — AxisMarkersCollection

- Members: `.ctor`. Hand-written: `Mirrored/Controls.Plots.Model.AxisMarkersCollection.cs`; its stub removed from the `.g.cs`.
- Shape: implicit base initialization through the empty constructor matched.
- Release IL: PASS — 466 checked, 466 exact, 0 failures.
- Remaining: `PlotLegendLayoutManager.GetNumberOfColumnsFitWidth` remains blocked by the documented local-slot mismatch; other unimplemented stubs remain unclaimed.

## Batch 26 — RenderingPropertyNames

- Members: `Stroke`, `DashStyle`, `.ctor`. Hand-written: `Mirrored/Controls.Plots.Contour.View.RenderingPropertyNames.cs`; all stubs removed from its `.g.cs`.
- Shape: the two original string constants and empty constructor matched.
- Release IL: PASS — 466 checked, 466 exact, 0 failures.
- Remaining: `PlotLegendLayoutManager.GetNumberOfColumnsFitWidth` remains blocked by the documented local-slot mismatch; other unimplemented stubs remain unclaimed.

## Batch 27 — CurveVisualData3D

- Members: constructor and `HasNoData`. Hand-written: `Mirrored/Controls.Plots.Plot3D.Model.CurveVisualData3D.cs`; both stubs removed from its `.g.cs`.
- Shapes: constructor passes `TraceType3D.Curve` to the base; `HasNoData` compares `Vertices.Count` to zero.
- Release IL: PASS — 468 checked, 468 exact, 0 failures.
- Remaining: `PlotLegendLayoutManager.GetNumberOfColumnsFitWidth` remains blocked by the documented local-slot mismatch; other unimplemented stubs remain unclaimed.

## Batch 28 — UndefinedVisualData3D

- Members: constructor and `HasNoData`. Hand-written: `Mirrored/Controls.Plots.Plot3D.Model.UndefinedVisualData3D.cs`; both stubs removed from its `.g.cs`.
- Shapes: constructor passes `TraceType3D.Undefined`, `null`, and zero to the base; `HasNoData` returns false.
- Release IL: PASS — 469 checked, 469 exact, 0 failures.
- Remaining: `PlotLegendLayoutManager.GetNumberOfColumnsFitWidth` remains blocked by the documented local-slot mismatch; other unimplemented stubs remain unclaimed.

## Batch 29 — ContourXYAxisViewModel

- Members: `.ctor(AxesType)` (base call with `PlotType.Contour`) and `SetResultRange` override. Hand-written: `Mirrored/Controls.Plots.Contour.ViewModel.ContourXYAxisViewModel.cs`; both stubs removed from its `.g.cs`.
- Shapes: the constructor and override matched the Release IL gate on the first pass.
- Release IL: PASS — 471 checked, 471 exact, 0 failures.
- Rejected: `ContourZAxisViewModel` (ctor, `SetResultRange`, `LimitsExpressionValueChanged` override with the `IsValueValid` `||` chain and `SetRange` early return). Gate failed on `LimitsExpressionValueChanged` ("Release IL differs"); reverted, not landed. Idiomatic `||` short-circuit shape is the first suspect; not investigated further.
- Remaining: `ContourZAxisViewModel` (open); `PlotLegendLayoutManager.GetNumberOfColumnsFitWidth` remains blocked by the documented local-slot mismatch; other unimplemented stubs remain unclaimed.

## Batch 30 — TickScale

- Members: auto-properties `MagnitudeThreashold`, `UseMagnitudeThreashold`, `Show`, `Count`, `Labels`, `Values`, `StartGap`, `Pattern`, `VisabilityPattern`; `.ctor`, `ShouldDrawTick`, `TickMarkPosition`, `FormatValues`, private `GetValuesWithCorrectedZeroForGeneralFormat`, private static `GetValueMagnitude`. Hand-written: `Mirrored/Controls.Plots.Presenter.TickScale.cs`; all stubs (including the property declarations) removed from its `.g.cs`.
- Shapes: auto-properties and the `.ctor` (setter calls) matched first time. `GetValuesWithCorrectedZeroForGeneralFormat` first failed on the `&&` grouping: Mathcad's flag and loop-condition chains are right-grouped (`a && (b && c)`), so separate `ldc.i4.0` exits appear. Left-grouped chains (`(a && b && c)`) share one exit and differ. The right-grouped form matched.
- Release IL: PASS — 495 checked, 495 exact, 0 failures.
- Remaining mismatches in this batch: none.

## Batch 31 — PlotErrorCodes.CreateTooManyPointsForTraceTypeMessage

- Members: `CreateTooManyPointsForTraceTypeMessage`. Hand-written: `Mirrored/Controls.Plots.Model.PlotErrorCodes.cs`; its stub removed from the `.g.cs`.
- Shapes: `string.Format(CultureInfo.CurrentUICulture, "\t{0:N0}", TooManyPointsForPlotType, maximalAllowedNumberOfPoints)` matched first time. The IL formats the key constant, not the count: the count is passed but unused by the `\t{0:N0}` pattern. Kept as Mathcad has it.
- Release IL: PASS — 496 checked, 496 exact, 0 failures.
- Open: `IsLimitsCalculationRelatedCustomError` stays a stub. Its Mathcad IL compares against `"Ptc.Plots.TICKMARKS_HAS_UNITS"` and `"RESULT_NOT_REAL_NUMBER"`, while the Quire constant `TickmarksHasUnits` is `"Quire.Plots.TICKMARKS_HAS_UNITS"`. Using the constant fails the literal match; using the Ptc literal would contradict the name mapping. Needs a decision from the maintainer.

## Batch 32 — ColorSample

- Members: `Color` (Brush, `readonly get`, `internal set`), `Width` (double, `readonly get`, `internal set`). Hand-written: `Mirrored/Controls.Plots.Contour.View.ColorSample.cs`; both property stubs removed from its `.g.cs`.
- Shapes: auto-properties with `readonly` getters (Mathcad marks the getters `IsReadOnly`) matched first time.
- Release IL: PASS — 500 checked, 500 exact, 0 failures.
- Remaining mismatches in this batch: none.

## Batch 33 — Plot3DAxisViewModel

- Members: `.ctor(AxesType)` (base call with `PlotType.Plot3D`, `assembly` access in Mathcad, kept `internal`) and the empty `protected override LimitsExpressionValueChanged`. Hand-written: `Mirrored/Controls.Plots.Plot3D.ViewModel.Plot3DAxisViewModel.cs`; both stubs removed from its `.g.cs`.
- Shapes: the empty override matched first time. The Z-axis override (batch 33 attempt, reverted earlier) has a body and is still open.
- Release IL: PASS — 502 checked, 502 exact, 0 failures.
- Remaining mismatches in this batch: none.

## Batch 34 — TraceModel transform and range helpers

- Members: `CalculateRadialRange` (static, `out` min/max), `CalculateRadialTransformByOffset`, `CalculatePolarTransformPoints`, `CalculateLogTransformPoints`, `TraceTypeIsLinearAndAxesScalingIsLinear`, `Containes`. Hand-written: `Mirrored/Controls.Plots.Model.TraceModel.cs`; their stubs removed from the `.g.cs`.
- Shapes: all six matched first time with plain `foreach` loops and `point.X`/`point.Y` access (ILSpy's `Point current = point` copies are not in the IL). The polar and log helpers use `x`/`y` locals and the `?:` NaN fallback for log.
- Callees left as stubs: none of these helpers depends on an unfinished body beyond the axis properties they already call.
- Release IL: PASS — 508 checked, 508 exact, 0 failures.
- Remaining in TraceModel and the other target types: the rest of the TraceModel stubs (event-heavy and WPF-coupled members) and all PlotTrace, PlotControl and PlotGraphAxis stubs.

## Batch 35 — TraceModel parameter range and RL helpers

- Members: `GetParameterRange` (returns an `Interval` object initializer, or `_angularRange` for angular axes), `GetXAxisLimitEditorValue` (static), `GetxyplotScalingRl`, `GetExpRl`, `InvalidateEngineResult` (`EngineResult = default`). Hand-written: appended to `Mirrored/Controls.Plots.Model.TraceModel.cs`; their stubs removed from the `.g.cs`.
- Shapes: all five matched first time, including the `Interval` initializer (struct temp, `Lower` then `Upper`) and the `!=` check whose else-branch is the `_angularRange` return.
- Release IL: PASS — 513 checked, 513 exact, 0 failures.
- Note: the `.g.cs` edits are byte-preserving (CRLF kept). Batch 34 landed `TraceModel.g.cs` as an LF blob after a text-mode rewrite; the working tree and HEAD agree under autocrlf, so the line-ending churn is in history only.

## Batch 36 — TraceModel event-raising helpers

- Members: `FireIsTraceCommitted`, `FireResultValidChanged`, `FireValidateTraceVsPlotType`, `FireStyleChanged`, `SetIsCommitted` (private), `FireEquationError` (protected), `FireAnalyzedDataChanged`. Hand-written: appended to `Mirrored/Controls.Plots.Model.TraceModel.cs` (adds `using Quire.Calculation.Core;`); their stubs removed from the `.g.cs`.
- Shapes: the `if (X != null) X(...)` form matched for the plain `Fire*` helpers. `FireAnalyzedDataChanged` and `FireEquationError` first used `?.Invoke`, which differs: Mathcad copies the event into a local before the null test (`stloc.0`/`ldloc.0`), so the handler-copy form matched.
- Release IL: PASS — 520 checked, 520 exact, 0 failures.

## Batch 37 — PlotTrace event and selection helpers

- Members: `SelectTrace`, `DeselectTrace`, `fireTraceClicked` (Mathcad's lowercase name kept), `ResetIsTypeDefault`, private `FireTypeChanged`, `FireErrorChanged`, `FireCalculatingStatusChangedEvent`, `FireTraceChanged`. Hand-written: `Mirrored/Controls.Plots.PlotTrace.cs`; their stubs removed from `PlotTrace.g.cs`.
- Shapes: the plain `if (X != null) X(...)` form matched for all but `FireTraceChanged`, which uses the handler-copy form (Mathcad's `?.Invoke` is compiled with a local, the same pattern as TraceModel batch 36).
- Release IL: PASS — 528 checked, 528 exact, 0 failures.

## Batch 38 — TraceModel accessors and calculatable overrides

- Members: `GetAxis`, `GetEquation`, `GetPlottingPointsForDisplay`, `GetRangeAxisType`; overrides `Commit`, `SetError`, `PreprocessEquationSpecificError`, `OnEquationSpecificError`, `BeforeRequestEvaluation`, `OnTraceEquationSExpressionChanged`; private `ClearPoints`. Hand-written: appended to `Mirrored/Controls.Plots.Model.TraceModel.cs`; their stubs (including the `=> base.X(...)` forwarders) removed from the `.g.cs`.
- Shapes: all eleven matched first time, including the `SetIsCommitted(value: false)` named argument and the `_engineResultForSerialization = null` assignment in `SetError`.
- Deliberately left: `OnPlotCalculatableError` (it sets `IsNumOfPointsMutable`, a member whose owner I have not confirmed for TraceModel).
- Release IL: PASS — 539 checked, 539 exact, 0 failures.

## Batch 39 — TraceModel axis-change and type-default helpers

- Members: `AxisModelRangeChanged`, `ResultAxisLimitValueChanged`, `UpdateFilteringAllowed`, `UpdateIsTypeDefault(bool forceUpdate = false)` (the non-short-circuit `ResultValid | forceUpdate`), `SetDefaultType`, `AxesScalingChanged`, `RangeAxisLimitValueChanged`, `EquationControlRequestEvaluation`, `ResetIsTypeDefault(bool? value = null)`. Hand-written: appended to `Mirrored/Controls.Plots.Model.TraceModel.cs`; their stubs removed from the `.g.cs`.
- Shapes: all nine matched on the first gate run after one compile fix (`TraceType` is already in scope through the `Trace2DResults` using; the qualified name does not compile).
- Left: `RecalculateResultOnPlotTypeChange` (calls `ResultManager`, not checked in Quire yet).
- Release IL: PASS — 548 checked, 548 exact, 0 failures.

## Batch 40 — TraceModel plotting-point, rect and equation-attachment helpers

- Members: `MinPlottingPointCoordinate`, `SetEquationWithoutCommit`, `AttachAxisEquationEventHandlers`, `DetachAxisEquationEventHandlers`, `CommitIfEquationsNotEmpty`, `SetPlottingPoints`, `SetTypeImpl` (`callStyleChanged |= _type != type`), `SetRect`, `SetAnalyzedDataForAxis`, `SetAnalyzedDataForResultAxis`, `SetAnalyzedDataForRangeAxis`, `SetPolarPlotRect`. Hand-written: appended to `Mirrored/Controls.Plots.Model.TraceModel.cs` (adds `using System.Linq;`); their stubs removed from the `.g.cs`.
- Shapes: `SetRect` first failed on layout. Mathcad's `if (!I(top) && !I(left) && …) { assign }` compiles as `if (I(top) || I(left) || I(bottom) || I(right)) return;` followed by the assignments; the early-return form matched.
- Blocked: `SetEquation` stays a stub. Its Mathcad IL compares `SynchronizationStatus` against 3 (`Ptc.Calculation.Core.SynchronizationStatus`: `Pending = 0, Synchronized = 3`), while Quire's enum (`src/Quire.Calculation.Core/ICalculatable.cs`) has `Pending, Synchronized` = 0, 1. Fixing the value (`Synchronized = 3`) is a change in `Quire.Calculation.Core`, owned by another lane.
- Release IL: PASS — 560 checked, 560 exact, 0 failures.

## Batch 41 — TraceModel range-axis lookups and image-renderer update

- Members: `GetGraphPointsForPlottingCoordinateOnRangeAxis`, `GetClosestPlottingCoordinateOnRangeAxis` (public forwarders with `DebugTools.AssertNotNull(...).Passed` guards), `OnPlottingPointsChanged`, `UpdateImageRendererModel` (density branch vs `StopRendering`). Hand-written: appended to `Mirrored/Controls.Plots.Model.TraceModel.cs`; their top-level stubs removed from the `.g.cs`. The nested `PlottingPointsForDisplay` stubs stay (they are not in this batch).
- Left: `UpdatePlottingPointsForDisplay` (its `WpfUtils.DoWork` lambda captures `points`, so the closure shape needs its own pass) and the nested class members.
- Release IL: PASS — 564 checked, 564 exact, 0 failures.

## Batch 42 — TraceModel simple properties

- Members: `IsUsingDensityVisualization` (get), `ResultValid` (get/set, fires `FireResultValidChanged`), `IsTypeDefault` (get), `ContainsEmptyEquation` (get), `TopLeft` and `BottomRight` (private-set auto-properties; Mathcad's compiler-generated backing fields), `Color` (get/set via `ImageRendererModel.StyleOwner`), `LineStyle` (get/set), `Type` (get/set). Hand-written: appended to `Mirrored/Controls.Plots.Model.TraceModel.cs`; their top-level property stubs removed from the `.g.cs`.
- Shapes: `LineStyle.set` first failed on layout. Mathcad's `if (value != _lineStyle) { … }` is an early return (`if (value == _lineStyle) return;`); the early-return form matched.
- Release IL: PASS — 579 checked, 579 exact, 0 failures.

## Batch 43 — TraceModel point-count, density and serialization properties

- Members: private `PlottingPointsCount` and `FinalNumberOfDataPointsToRender` (null-guarded counts of the filter), `ForcedToDensityVisualization` (`ControlSettings.Instance.MinPointsForForceDensity`), `EngineResultForSerialization` (get with `CopyMinMaxAndUnitsResults`, private set), `RangeAxisModel` and `ResultAxisModel` (expression-bodied `_axes[0]`/`_axes[1]`), `NumberOfPointsInResult` and `ShouldNotCopyFullResults` (auto-properties), `IsCommitted` (override, expression-bodied). Hand-written: appended to `Mirrored/Controls.Plots.Model.TraceModel.cs` (adds `using Quire.Controls.Core;`); their stubs removed from the `.g.cs`.
- Shapes: all nine matched first time.
- Release IL: PASS — 591 checked, 591 exact, 0 failures.

## Batch 44 — TraceModel style and discrete-axis properties

- Members: `RestrictedToDensityPointsOrSolidLine` (get), `AreDensityPointsSupported` (expression-bodied, `PlotType != PlotType.Polar`), auto-properties `ImageRendererModel` and `PlotType` (private set), `Symbol`, `SymbolWeight`, `SymbolFrequency`, `LineWeight`, `Label`, and `XIsDiscrete`/`YIsDiscrete` (private set). Hand-written: appended to `Mirrored/Controls.Plots.Model.TraceModel.cs`; their top-level stubs removed from the `.g.cs`.
- Left: `MinPositiveX`/`MinPositiveY` (lambda passed to `MinPlottingPointCoordinate`; the compiler-generated closure class and its ordinal need their own pass).
- Shapes: all matched first time, 20 accessors counting getters and setters.
- Release IL: PASS — 611 checked, 611 exact, 0 failures.

## Batch 45 — PlotTrace symbol, line-style and equation helpers

- Members: `ChangeSymbol`, `ChangeLineStyle` (density-restricted pairing of symbol and line style), `SetEquation(AxesType, IPlotEquation)`, `GetEquation(AxesIndex)`, private static `GetMinGap` (sort and minimum positive gap). Hand-written: `Mirrored/Controls.Plots.PlotTrace.cs` (adds `using System;` and `using System.Collections.Generic;`); their stubs removed from `PlotTrace.g.cs`.
- Shapes: all five matched first gate run after one compile fix. PlotTrace's `TraceSymbol` name resolves to the `string TraceSymbol` property, so the enum is written `Presenter.TraceSymbol.none` (Mathcad's IL uses the full enum name).
- Left: `SetAxis` and `MoveFocusToEquation` (WPF event and focus types).
- Release IL: PASS — 616 checked, 616 exact, 0 failures.

## Batch 46 — PlotTrace style-change command and point-count conformance

- Members: `DoStyleChangeCommand`, `UndoStyleChangeCommand` (saved-style round trip with `TraceStyleState?`), private `ConformStyleToPointCount` (saves style and switches symbol or line style for density visualisation, then restores it). Hand-written: appended to `Mirrored/Controls.Plots.PlotTrace.cs`; their stubs removed from `PlotTrace.g.cs`.
- Shapes: all three matched first time. `Presenter.TraceSymbol` is qualified for the same reason as batch 45.
- Left: `SetColorStyle` and `SetMinGap` (WPF `Style`/`Setter` and `DependencyPropertyKey` use).
- Release IL: PASS — 619 checked, 619 exact, 0 failures.

## Batch 47 — TraceModel analysed-data helpers

- Members: `AnalyzeAxisAndTypeData` (range/result rect, units, `IsNumOfPointsMutable` from the parametric result type, equation units), `SetAnalyzedData` (plotting points copy, axis labels, polar or linear rect, `ResultValid = true`). Hand-written: appended to `Mirrored/Controls.Plots.Model.TraceModel.cs`; their stubs removed from the `.g.cs`.
- Shapes: both matched first time.
- Left in TraceModel: `SetResult` (captures `result` in a `WpfUtils.DoWork` lambda; needs the closure pass), `DoSetResult`, `AnalyzeResultData`, `CreateAllPointsByTraceType`, `AnalyzeResult` x2, `AnalyzeVectors`; `Rl` (string-format with an inline array); `UpdatePlottingPointsForDisplay` and `MinPositiveX`/`MinPositiveY` (closures); `CopyMinMaxAndUnitsResults`; nested `PlottingPointsForDisplay` (generated class, not partial).
- Release IL: PASS — 621 checked, 621 exact, 0 failures.

## Batch 48 — TraceModel result-analysis core

- Members: `CreateAllPointsByTraceType` (trace-type switch over the `TraceResultAnalyzer` builders), `AnalyzeResult(IMatrixResultNode)`, `AnalyzeResult(ITrace2DResultNode)`, `AnalyzeVectors` (parametric range check, data-point analysis, engine result). Hand-written: appended to `Mirrored/Controls.Plots.Model.TraceModel.cs`; their stubs removed from the `.g.cs`.
- Shapes: `AnalyzeResult(IMatrixResultNode)` first differed in local-slot order. Mathcad's error code is slot 0 and `plotResult` slot 1, so the error string is declared before the call (`string errorMessageCode;` then `out errorMessageCode`); `out var` in the initializer did not change the order.
- Release IL: PASS — 625 checked, 625 exact, 0 failures.

## Batch 49 — TraceModel DoSetResult

- Members: `DoSetResult` (plotting-filter begin/end around a `CommandScope` over a `CompoundCommand`, result-kind dispatch with `as` casts, `PlotInvalidResultException` error routing). Hand-written: appended to `Mirrored/Controls.Plots.Model.TraceModel.cs` (adds `using Quire.Undo;`); its stub removed from the `.g.cs`.
- Shapes: matched first time.
- Blocked (not in this batch): `AnalyzeResultData` (its `Sum` lambda). Its generated lambda method is named `<AnalyzeResultData>b__207_0` in Quire against Mathcad's `b__172_0`. Roslyn's ordinal counts every member of the type in compile order, and the generated stubs in `Generated/` compile before hand-written members, so the ordinal is out of step until the remaining TraceModel stubs (fields, accessors, nested class) are moved into the hand-written file in Mathcad order as placeholders. The same applies to `SetResult`, `UpdatePlottingPointsForDisplay`, `MinPositiveX`, `MinPositiveY` (all use closures).
- Release IL: PASS — 626 checked, 626 exact, 0 failures.

## Batch 50 — AxisModel simple accessors (owner: Orca task `task_4799da3a9685`, branch `feature/haiku-plots-20261123`)

- Members: `AxisTitleModel`, `Markers`, `Equations`, `CanHaveMarkers`, `XIsString`, `YIsString`, `IsDiscrete`, `LegendWidth` (auto-properties and expression getters), `AxisXmlNodeName`, `DomainXmlNodeName`. Hand-written: `Mirrored/Controls.Plots.Model.AxisModel.cs`; stubs removed from `AxisModel.g.cs`. The backing fields `_markers` and `_equations` stay stubs (constructor not mirrored yet).
- Shapes: the `switch` expression form compiles to an extra local (`stloc.1`), so both name switches are `switch` statements with `return` per case. `DomainXmlNodeName` needs separate `case X:` and `case Y:` returns (Mathcad keeps two blocks); stacked labels did not match.
- Release IL: PASS — 2410 checked, 0 failures.
- Remaining in AxisModel: constructor, setters/logic members, range and tick helpers.

## Batch 51 — AxisModel auto-properties and show flags

- Members: `Type`, `PlotType`, `Position`, `PositionTickMark`, `LegendPositionIndex`, `UseBetterTickMarks`, `StartDefault`, `EndDefault`, `DefaultTickMarks`, `MaxTickmarks`, `IsDisplayNumberFormatLocal`, `MajorTickMarks`, `MinorTickMarks`, `FilteredLabels` (auto-properties, private setters where Mathcad has them); `ShowTicks` (via `MajorTickMarks.Show`), `ShowGridLines`, `ShowNumbers` (guarded `if (x != value)` set with `OnPropertyChanged`). Hand-written: appended to `Mirrored/Controls.Plots.Model.AxisModel.cs`; stubs removed from `AxisModel.g.cs`. The `_showGridLines` and `_showNumbers` fields stay stubs.
- Shapes: all matched first gate run.
- Release IL: PASS — 2444 checked, 0 failures.
- Left in AxisModel: `Start`, `End`, `Scaling`, `GridLineColor`, `DisplayNumberFormat`, the limit-expression properties (event handlers), the constructor, and the range/tick helpers.

## Batch 52 — AxisModel range, rounding and event helpers

- Members: `LinearizedStart`/`LinearizedEnd` (`?? double.MinValue/MaxValue`), `StartDefaultByScaling`, `ExpressionsAreReady`, `Contains`, `IsInStartEndInterval`, `LimitHasErrors`, `HasAxisError`, `IsReady`, `OrderFunction`, `RoundWithOrder`, `CeilingWithOrder`, `FloorWithOrder`, `OnPropertyChanged`, `FireChanged`, `FireRangeChanged`, `FireScalingChanged`, `FireNumberFormatInitialized`, `FireTickMarkLabelsUpdated`. Hand-written: appended to `Mirrored/Controls.Plots.Model.AxisModel.cs` (adds `using System;` and `using System.ComponentModel;`); stubs removed from `AxisModel.g.cs`.
- Shapes: `LimitHasErrors` with `?.HasErrors ?? false` gave `brtrue` where Mathcad has `brfalse`; `axisLimitEditor != null && axisLimitEditor.HasErrors` matched. `FireTickMarkLabelsUpdated` with `?.Invoke()` compiles to `dup`; Mathcad copies the event into a local, so the handler-copy form matched. The `if (X != null) X(...)` form matched for the other `Fire*` helpers.
- Release IL: PASS — 2463 checked, 0 failures.
- Left in AxisModel: `Start`, `End`, `Scaling`, `GridLineColor`, `DisplayNumberFormat`, the limit-expression properties, `StartValueChanged`, `GetRadialPointsStart`, `CreateLogMinorTickmarksPattern`, `AxisCalculatables` (iterator), the constructor, and the tick-mark and XML helpers.

## Batch 53 — AxisModel range, scaling and assert properties

- Members: `Start`, `End` (guarded setters that enable the linearized assert), `Scaling` (early return on equal, then clears and restores `IsLinearizedRangeAssertEnabled` around `UseMagnitudeThreashold` and `FireScalingChanged`), `DisplayNumberFormat` (private set), `IsLinearizedRangeAssertEnabled` (setter that recomputes `LinearizedStart`/`LinearizedEnd`). Hand-written: appended to `Mirrored/Controls.Plots.Model.AxisModel.cs` (adds `using Quire.Controls.Core;`); stubs removed from `AxisModel.g.cs`.
- Shapes: `Scaling` matched with an early return; `if (_scaling != value) { ... }` gave `beq` vs Mathcad's `bne.un`. The assert setter needed a non-short-circuit `&` in the equality test, comparisons via `GetValueOrDefault`/`HasValue` and a local copy of the old field and value for the test only; the store uses `value` directly.
- Release IL: PASS — 2473 checked, 0 failures.
- Left in AxisModel: `GridLineColor` (getter has try/catch regions), `StartExpression`/`SecondTickExpression`/`EndExpression` setters, `StartValueChanged`, `GetRadialPointsStart`, `CreateLogMinorTickmarksPattern`, `AxisCalculatables` (iterator), the constructor, and the tick-mark and XML helpers.

## Batch 54 — AxisModel radial start, limit-editor error and string-label helpers

- Members: `GetRadialPointsStart` (`DevAssert(Radial == Type, null).Failed` guard, log10 for logarithmic scaling), `SetAxisLimitEditorError` (static, sets the custom error when not default), `IsStringLabel` (a two-case string `switch` on `AxisXmlNodeName`, returning `XIsString`/`YIsString`/false). Hand-written: appended to `Mirrored/Controls.Plots.Model.AxisModel.cs` (adds `using Quire;`); stubs removed from `AxisModel.g.cs`.
- Shapes: `IsStringLabel` as an `if` chain or early returns did not match. The two-case string `switch` statement matched because Roslyn emits the `brtrue` string-compare chain Mathcad uses.
- Release IL: PASS — 2476 checked, 0 failures.
- Left in AxisModel: `GridLineColor`, the limit-expression setters, `StartValueChanged`, `CreateLogMinorTickmarksPattern`, `AxisCalculatables` (iterator), `SetDoubleValueIfValid` (dup pattern), the constructor, the legend-position and XML helpers, and the tick-mark helpers.

## Batch 55 — PlotTitle default text

- Members: `GetDefaultTitleText` (static, `switch` on `PlotTitleType` returning the three literals and `string.Empty`). Hand-written: appended to `Mirrored/Controls.Plots.PlotTitle.cs`; stub removed from `PlotTitle.g.cs`.
- Release IL: PASS — 2477 checked, 0 failures.
- Tried and reverted: `HideTitleIfEmpty`. Its single combined `&&` guard, the early-return form and the nested-if form each differ from Mathcad's layout (Mathcad has 30 entries against 28, with a different branch target at the first test). Left as a stub; needs a closer op-level diff.
- Left in PlotTitle: `HandleEditingStarted`, `HandleEditingEnded`, `RegisterToEvents`, `TitleSizeChanged` (WPF event and orientation logic), and the XAML-generated `InitializeComponent`, `_CreateDelegate`, `Connect`.

## Batch 56 — XYPlotStrategy initial placeholder indices

- Members: `GetInitialPlaceholderIndices` (four `out` assignments, `0, 1, 1, 0`). Hand-written: appended to `Mirrored/Controls.Plots.XYPlotStrategy.cs`; stub removed from `XYPlotStrategy.g.cs`.
- Release IL: PASS — 2478 checked, 0 failures.
- Left: `PlotItemPlaceholder.GetMinLegendPlaceholderHeight` (uses `DebugTools.AssertNotNull` and `IsPlotTitleOrientationSwapped`), `PlotItemPlaceholder.AdjustPadding`, the other `XYPlotStrategy` layout methods, and the `PlotItem` WPF handlers.

## Batch 57 — AxisModel log minor tick-mark pattern

- Members: `CreateLogMinorTickmarksPattern` (static; `new double[8]`, bottom-tested-equivalent `for` over 3..9 storing `Log10(n / (n - 1))`, last entry `Log10(2.2222222222222223)`). Hand-written: appended to `Mirrored/Controls.Plots.Model.AxisModel.cs`; stub removed from `AxisModel.g.cs`.
- Release IL: PASS — 2479 checked, 0 failures.

## Blocker — XYPlotStrategy.SetMinWidthHeight (not landed; reverted)

- Implemented `SetMinWidthHeight` (decompiled order, local static `Safe`) and, separately, `BuildEditableAxisRelatedElements` (lambda closure). Both reverted.
- `SetMinWidthHeight` alone: the local function is named `<SetMinWidthHeight>g__Safe|6_0` in Quire vs `g__Safe|5_0` in Mathcad (calls differ at #60; the method also has a "no Mathcad counterpart" row for the Quire local). Moving `GetInitialPlaceholderIndices` after it changed Quire's ordinal to 6 but did not reach 5.
- `BuildEditableAxisRelatedElements` added: its closure is `<>c__DisplayClass5_0` in Quire vs `<>c__DisplayClass4_0` in Mathcad, and the display class has no Mathcad counterpart.
- Mathcad's XYPlotStrategy also declares `InitializeAxes` and a private static `InitializeAxisTitle` (not declared in Quire). Their absence, and the stub declarations in `XYPlotStrategy.g.cs` ordering, appear to drive the ordinals. Needs the maintainer's decision on placeholder stubs (AGENTS.md compile-order device) or the ordinal rule for generated stubs.

## XYPlotStrategy ordinal probe — parked (batch 58 candidate, not landed)

- Probe: moved the mapped `InitializeLegendPositions`, `ApplyPlotTemplate` (throwing placeholders), `BuildEditableAxisRelatedElements` and `SetMinWidthHeight` (implemented) from `XYPlotStrategy.g.cs` into `Mirrored/Controls.Plots.XYPlotStrategy.cs` in Mathcad order after `InitializeAxes`/`InitializeAxisTitle`, with `GetInitialPlaceholderIndices` and the constructor last. The generated file kept only the class declaration.
- Measured ordinal: before (members in `.g.cs` first): local function `g__Safe|6_0` and closure `<>c__DisplayClass5_0`. After the move: `g__Safe|5_0` and `<>c__DisplayClass4_0`, both identical to Mathcad. `BuildEditableAxisRelatedElements` matched.
- Remaining mismatch in `SetMinWidthHeight` only: the `if (PlotLegendPositions != null)` block stores into local slot 41 where Mathcad uses 40, a one-slot difference from an extra double local earlier in Quire's body. Inlining the array temporaries shifted the first difference to instruction #54 (Quire 485 entries vs Mathcad 493), so the named-local form is correct.
- Not landed: `SetMinWidthHeight` remains a stub, so no previously exact method is affected. Patch kept in the scratch folder for resuming the local-slot investigation.
- MirrorGen regeneration of the probe's `.g.cs` was not checked.

## XYPlotStrategy isolated BuildEditableAxisRelatedElements (landed at ae14fd60a)

- Isolated the minimum moves: `InitializeLegendPositions` and `ApplyPlotTemplate` and `SetMinWidthHeight` as unmarked throwing placeholders, `BuildEditableAxisRelatedElements` implemented, and the constructor last, all in `Mirrored/Controls.Plots.XYPlotStrategy.cs` after `InitializeAxisTitle` and before `GetInitialPlaceholderIndices`. Generated `XYPlotStrategy.g.cs` keeps only the class shell.
- Release IL: PASS — 2482 checked, 0 failures (includes the closure `<>c__DisplayClass4_0` identity for `BuildEditableAxisRelatedElements`).
- MirrorGen `emit` reproduces `Controls.Plots.XYPlotStrategy.g.cs` byte-identical to the committed file (checked on the worktree; the emit noise in other projects was reverted).
- `SetMinWidthHeight` stays a placeholder until the extra double local (Quire slot 41 vs Mathcad 40) is resolved. Patch: `scratchpad/xy-isolate-wip.patch`.

## Batch 58 — AxisModel small helpers

- Members: `ValidateAxisLimitExpression` (static, `DevAssert(false, null)` guard), `OnExpressionHasErrorsChanged`, `ClearCustomErrors`, `SetSecondTickPreCalculatedValue`, `SetDoubleValueIfValid` (static, `IsValid()` store), `RemoveEquation`, `StartValueChanged`. Hand-written: appended to `Mirrored/Controls.Plots.Model.AxisModel.cs`; stubs removed from `AxisModel.g.cs`.
- Release IL: PASS — 2489 checked, 0 failures.
- Left in AxisModel: `GridLineColor`, the limit-expression setters, `GetNumberOfTracesForEquation` (LINQ `First`), the legend-position converters, `SetRangeError`, `Linearize`, `DealWithZeroLengthRange`, `UpdateFilteredLabels`, the tick-mark and XML helpers, and the constructor.

## Batch 59 — AxisModel limit-expression properties, Linearize, zero-length range, range error

- Members: `StartExpression`, `SecondTickExpression`, `EndExpression` (validate old, unhook, store, pre-calculate, hook, fire), `Linearize`, `DealWithZeroLengthRange` (the `min == max` test is a `beq` to the body, so the radial test is `Type == Radial ? 0 : -1`), `SetRangeError`. Hand-written: appended to `Mirrored/Controls.Plots.Model.AxisModel.cs`; stubs removed from `AxisModel.g.cs`.
- Shapes: the first `DealWithZeroLengthRange` attempt (`Type != Radial ? -1 : 0`) gave `bne.un` where Mathcad has `beq`; reversing the ternary matched.
- Release IL: PASS — 2498 checked, 0 failures.

## Batch 60 — AxisModel tick-mark pattern and legend-position converters

- Members: `SetTickMarksPattern`, `ConvertLegendPositionEnumToLegendPositionIndex` (`Enum.Parse` on the X/Y legend enum by `AxesTypeToIndex`), `ConvertLegendPositionIndexToLegendPositionEnum`. Hand-written: appended to `Mirrored/Controls.Plots.Model.AxisModel.cs`; stubs removed from `AxisModel.g.cs`.
- Release IL: PASS — 2501 checked, 0 failures.
- Left in AxisModel: `GridLineColor` (try/catch and pattern-matching getter), `GetNumberOfTracesForEquation` (LINQ lambda closure, needs the ordinal check), `SetLevels`, `UpdateFilteredLabels` (needs `Base3DAxisViewModel`-style `FilteredLabels` property), the XML and data-object helpers, and the constructor.

## Batch 61 — PlotTitle event and editing handlers

- Members: `RegisterToEvents` (argument copied to a `FrameworkElement` local before the null test and hook-ups), `TitleSizeChanged` (`ViewModel == null || Visibility == Collapsed` early return; `e.NewSize.Width`/`Height` read inline), `HandleEditingStarted` and `HandleEditingEnded` (`TitleType != YAxis || !IsRotationAllowed` early return; `ViewModel?.IsVertical ?? false`). Hand-written: appended to `Mirrored/Controls.Plots.PlotTitle.cs`; stubs removed from `PlotTitle.g.cs`.
- Shapes: `if (a && b) { ... }` gave a single trailing `ret` where Mathcad has two early returns; the `||` early-return form matched. `Size newSize = e.NewSize` locals gave `stloc` before the `this` push; inline `e.NewSize.Width` matched.
- Release IL: PASS — 2505 checked, 0 failures.
- Left in PlotTitle: `HideTitleIfEmpty` (see batch 55), and the XAML-generated `InitializeComponent`, `_CreateDelegate`, `Connect`.

## Batch 62 — AxisModel grid-line colour and filtered tick labels

- Members: `GridLineColor` (getter with two `try`/`catch` colour lookups; the converted object is a named local before the `is Color` test so the local order matches Mathcad; setter stores the hex string and fires `GridLineColor` when it changes), `UpdateFilteredLabels` (non-short-circuit `flag | flag2 | flag3`, `FilteredLabels` through the property, bottom-tested loop for the middle labels). Hand-written: appended to `Mirrored/Controls.Plots.Model.AxisModel.cs` (adds `using System.Windows.Media;`); stubs removed from `AxisModel.g.cs`.
- Shapes: `ColorConverter.ConvertFromString(...) is Color result` directly gave `stloc.1` at #3 (Quire puts the return local first); the separate object local matched.
- Release IL: PASS — 2508 checked, 0 failures.
- Left in AxisModel: `GetNumberOfTracesForEquation` (LINQ lambda closure, needs the ordinal check), `SetLevels` (uses `AxisModelDefinitionCollection`), the XML and data-object helpers, `SetRangeToRadialAxis`/`SetRangeToLinearAxis`/`SetLogarithmicScale`, and the constructor.

## Batch 63 — PlotTitle.HideTitleIfEmpty

- Member: `HideTitleIfEmpty`. Hand-written: appended to `Mirrored/Controls.Plots.PlotTitle.cs`; stub removed from `PlotTitle.g.cs`. The batch 55 attempt (one combined `&&` guard) is superseded.
- Shapes: `ViewModel == null || Title == null` early return, then a separate `!IsNullOrWhiteSpace` early return, then `if (ViewModel.ShowTitle)` wrapping the body. The combined guard gave a different branch layout.
- Release IL: PASS — 2509 checked, 0 failures.
- Left in PlotTitle: only the XAML-generated `InitializeComponent`, `_CreateDelegate` and `Connect` stubs.

## Batch 64 — AxisModel linear, radial and second-tick range helpers

- Members: `SetRangeToLinearAxis` (conditional `IsValueValid && !IsStringLabel()` picks), `SetRangeToRadialAxis` (`Nullable` `HasValue`/`Value` branches), `ValidateSecondTickValue` (early returns, `try`/`catch OverflowException` with the tick-mark limit). Hand-written: appended to `Mirrored/Controls.Plots.Model.AxisModel.cs`; stubs removed from `AxisModel.g.cs`.
- Shapes: `SetRangeToRadialAxis` gave `stloc.0` where Mathcad uses slot 2; Mathcad's locals are `effectiveStart`, `effectiveEnd`, then `start`, `num` (declaration order matters for slots), so the two effective locals are declared first.
- Release IL: PASS — 2512 checked, 0 failures.

## Batch 65 — AxisModel logarithmic scale

- Member: `SetLogarithmicScale`. Hand-written: appended to `Mirrored/Controls.Plots.Model.AxisModel.cs`; stub removed from `AxisModel.g.cs`.
- Shapes: `AssertionE.Failed == (AssertionE)DebugTools.DevAssert(start <= end, null)` (the enum conversion is what Mathcad's `op_Implicit` call needs); the try/catch with `int` locals declared before the `try` matched without changes.
- Release IL: PASS — 2513 checked, 0 failures.

## Batch 66 — AxisModel.GetNumberOfTracesForEquation (display-class ordinal)

- Member: `GetNumberOfTracesForEquation` (`_equations.First(eq => eq.Equation == equation).Traces.ToList().Count()`). Hand-written: placed in `Mirrored/Controls.Plots.Model.AxisModel.cs` right after the `StartExpression` property; stub removed from `AxisModel.g.cs`.
- Measured ordinal: at the end of the hand-written file the closure was `<>c__DisplayClass233_0` (Mathcad `213_0`). The Roslyn ordinal counts every member before the method in compile order (generated file first, then hand-written), with a property counting as one plus each accessor. Moving the method to a point with exactly 20 member units after it gave `205_0` when placed after `OnExpressionHasErrorsChanged` (28 units after), and `213_0` after `StartExpression` (20 units after, matching Mathcad). The comment in the file records this.
- Generated layout: changed (stub removed). MirrorGen was checked after a Release build of `Quire.Controls.Plots` in the worktree: `emit` regenerates `AxisModel.g.cs` byte-identical to the committed file. (A `emit` without a fresh build re-adds the stubs of members implemented since the last build; always build first.)
- Release IL: PASS — 2516 checked, 0 failures.
- Left: `SetRange` (lambda closure, Mathcad ordinal `170_0`; the same placement rule applies when implemented).

## Batch 67 — AxisModel.SetRange (lambda closure ordinal 170)

- Member: `SetRange(double?, double?)`, with the `labelsUpdated` closure and the `TickMarkLabelsUpdated` hook-up in `try`/`finally`. Hand-written: placed ahead of `LimitHasErrors` in `Mirrored/Controls.Plots.Model.AxisModel.cs`; stub removed from `AxisModel.g.cs`.
- Ordinal: placed at the end of the file, the closure was `233_0`; placed so that exactly 42 member units lie between it and `GetNumberOfTracesForEquation` (which moved from `212_0` to `213_0` when `SetRange` came before it), it is `170_0` as in Mathcad.
- Shapes: the closure is created only after the readiness test, so the body sits inside `if (ExpressionsAreReady)` (a leading `return` gave the closure creation before the check). The `bool flag` local is declared before the delegate local (Mathcad's slot order is flag 1, delegate 2).
- Generated layout: changed (stub removed). Verified: after a Release build of `Quire.Controls.Plots`, `emit` regenerates `AxisModel.g.cs` byte-identical to the file committed here.
- Release IL: PASS — 2519 checked, 0 failures.
- Left in AxisModel: `SetRangeToLinearAxis` and related done; remaining: `SetLevels`, `SetLogarithmicScale` done; the XML helpers (`SetXmlNode`, `GetXmlNode`, `AddEquation`), the data-object helpers, `UpdateTickMarkValues`, `FormatTickMarkValues`, `BuildTickMarksValues`, `SetLinearScale`, `CalculateBetterTickmarks*`, `RegionXmlNode`, `AxisCalculatables`, `SetDisplayNumberFormat`, `LinearizeStartEnd`, `SetTickMarksByEffectiveValues`, `SetRangeToAngularAxis`, `SetDisplayNumberFormat`, and the constructor.
