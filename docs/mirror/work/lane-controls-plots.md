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
