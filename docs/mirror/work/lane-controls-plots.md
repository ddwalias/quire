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

## Batch 68 — AxisModel tick-mark range helpers

- Members: `SetRangeToAngularAxis`, `SetTickMarksByEffectiveValues` (the two scale branches join on one `bool` local and one return, as the IL does), `SetDisplayNumberFormat`, `UpdateTickMarkValues`, `SetLinearScale`. Hand-written: placed ahead of `SetRange` in `Mirrored/Controls.Plots.Model.AxisModel.cs`; stubs removed from `AxisModel.g.cs`.
- Ordinal note: removing five stubs from the generated file lowers every later closure ordinal by five. Placing the new members ahead of `SetRange` restores both `SetRange` (170) and `GetNumberOfTracesForEquation` (213). Any further stub removal before those two needs the same compensation.
- Generated layout: changed (five stubs removed). Verified: after a Release build, `emit` regenerates `AxisModel.g.cs` byte-identical.
- Release IL: PASS — 2524 checked, 0 failures.
- Correction to batch 67's "Left" list: `SetLogarithmicScale`, `SetRangeToLinearAxis` and `SetRangeToRadialAxis` are done (batches 64-65). Remaining AxisModel stubs: `AxisCalculatables`, `RegionXmlNode`, `LinearizeStartEnd`, `BuildTickMarksValues`, `FormatTickMarkValues`, `CalculateBetterTickmarks*`, `SetLevels`, `SetXmlNode`, `GetXmlNode`, `AddEquation` (both), `CreateXyDomainType`, `GetDataObject`, `SetDataObject`, `ReadAxisDomainData`, `SetTickMarksPattern` (done in batch 60), and the constructor.

## Batch 69 — AxisModel tick-mark builders (partial)

- Members: `LinearizeStartEnd`, `BuildTickMarksValues`, `CalculateBetterTickmarksWhenZeroIsNotInRange`, `FormatTickMarkValues`. Hand-written: placed ahead of `SetRange` in `Mirrored/Controls.Plots.Model.AxisModel.cs` (keeps the `SetRange` 170 and `GetNumberOfTracesForEquation` 213 ordinals); stubs removed from `AxisModel.g.cs`.
- Release IL: PASS — 2528 checked, 0 failures. Generated layout: four stubs removed; after a Release build, `emit` regenerates `AxisModel.g.cs` byte-identical.
- Blocked, kept as stubs: `CalculateBetterTickmarks` (the Mathcad IL materialises `start < 0 && end > 0` as a bool value and then `brfalse`, with no bool local; Roslyn branches on the condition instead, and a bool local adds a slot Mathcad does not have) and `CalculateBetterTickmarksWithZeroInRange` (Mathcad stores the second `RoundWithOrder` result in a local before `Math.Max`, and the first result stays on the stack; neither the expression form nor the named-local form reproduces it).
- Left in AxisModel: the two blocked helpers above, `SetLevels`, `RegionXmlNode`, `AxisCalculatables`, `SetXmlNode`, `GetXmlNode`, `AddEquation` (both), `CreateXyDomainType`, `GetDataObject`, `SetDataObject`, `ReadAxisDomainData`, and the constructor.
- SetMinWidthHeight (XYPlotStrategy) remains parked on the slot 41 vs 40 mismatch.

## Batches 70-72 — AxisModel XML and data-object members (grouped candidate)

- Members: `AddEquation(IPlotEquation, int)`, `AddEquation(XmlNode)`, `CreateXyDomainType` (protected, object initializer), `GetDataObject` (switch on `AxesType`, `DevAssert` default), `ReadAxisDomainData`, `SetDataObject`, `RegionXmlNode` (property; its get/set call the still-stubbed `GetXmlNode`/`SetXmlNode`), `SetLevels`. Hand-written: placed ahead of `SetRange` (ordinals for `SetRange` 170 and `GetNumberOfTracesForEquation` 213 preserved); stubs removed from `AxisModel.g.cs`. Usings added: `System.Globalization`, `System.Xml`, `Quire.Calculation.Core`, `Quire.PersistentDataObjects`, `Quire.Controls.Core.Serialization`, `Quire.Xml`.
- Shapes: `new NumberFormatSerializer(...).SetDataObject(...)` on a fresh instance gave `call`, Mathcad uses `callvirt`; storing the serializer in a local matched.
- Release IL: PASS — 2537 checked, 0 failures (last run, after batch 72).
- Still stubs: `GetXmlNode`, `SetXmlNode` (large XML writer/reader), `AxisCalculatables` (iterator), `AddEquation` is done, the constructor, and the two blocked tick-mark helpers from batch 69.
- SetMinWidthHeight (XYPlotStrategy) remains parked on the slot 41 vs 40 mismatch.

## Batch 73 — AxisModel.GetXmlNode

- Member: `GetXmlNode(string wsNameSpace, string mathNameSpace)`, the `RegionXmlNode` getter's writer. Hand-written: placed ahead of `SetRange` in `Mirrored/Controls.Plots.Model.AxisModel.cs` (keeps the `SetRange` 170 and `GetNumberOfTracesForEquation` 213 ordinals); stub removed from `AxisModel.g.cs`.
- Generated layout: one stub removed. Verified: after a Release build of `Quire.Controls.Plots`, `emit` leaves `AxisModel.g.cs` as HEAD minus exactly that stub and its `[MirrorStub]` attribute.
- Release IL: PASS — 2538 checked, 0 failures.
- Left in AxisModel: `SetXmlNode` (XML reader), `AxisCalculatables` (iterator), the constructor, and the two blocked tick-mark helpers from batch 69.
- SetMinWidthHeight (XYPlotStrategy) remains parked on the slot 41 vs 40 mismatch.

## Batch 74 — AxisModel.SetXmlNode (blocked, kept as stub)

- Member: `SetXmlNode(XmlNode, XmlNamespaceManager)`, the `RegionXmlNode` setter's reader. Written idiomatically (descriptive locals, no nullable annotations) and checked: every call, constant and branch matches Mathcad after two fixes (`string.Format("ws:{0}", ...)` for the domain path; `SpiritResolver` via `using Quire.Serialization;`). The only remaining difference is at the start: Mathcad stores the `value` parameter in a temp (`ldarg.1; stloc.0; ldloc.0; brtrue.s`, 458 vs 454 entries). Neither a copied local (`XmlNode node = value;`) nor a `switch (value) { case null: ... }` reproduces it; Roslyn drops or does not emit the temp.
- Result: not landed; the member stays a generated stub. The clean draft is kept outside the repo, not committed.
- Left in AxisModel: `AxisCalculatables` (iterator), the constructor, the two blocked tick-mark helpers from batch 69, and `SetXmlNode` (above).
- SetMinWidthHeight (XYPlotStrategy) remains parked on the slot 41 vs 40 mismatch.

## Batch 75 — AxisModel.AxisCalculatables (iterator ordinal)

- Member: `AxisCalculatables` getter (`yield return` of Start, SecondTick and End expressions, each behind a null test). Hand-written: placed at the top of the class body in `Mirrored/Controls.Plots.Model.AxisModel.cs`; stub removed from `AxisModel.g.cs`.
- Iterator ordinal: the state machine is `<get_AxisCalculatables>d__N`. Placed at the top, Quire emitted `d__44` (Mathcad `d__41`); placed after the hand-written members it emitted `d__169`. The three unfinished stubs that follow it in Mathcad order (`CalculateBetterTickmarks`, `CalculateBetterTickmarksWithZeroInRange`, `SetXmlNode`) were still counted ahead of it, so they are declared as unmarked placeholders right after the iterator (Mathcad order) and removed from `AxisModel.g.cs`. The placeholders sit before `SetRange` and `GetNumberOfTracesForEquation`, so those ordinals are unchanged.
- Generated layout: four stubs removed (`AxisCalculatables`, and the three placeholders above). Verified: after a Release build, `emit` regenerates `AxisModel.g.cs` byte-identical to the committed file.
- Release IL: `get_AxisCalculatables` exact; full gate PASS — 2547 checked, 0 failures.
- Left in AxisModel: the constructor, the two blocked tick-mark helpers from batch 69 (placeholders now), and `SetXmlNode` (placeholder; blocked on the Mathcad temp shape, batch 74).
- SetMinWidthHeight (XYPlotStrategy) remains parked on the slot 41 vs 40 mismatch.

## Batch 76 — AxisModel constructor (blocked, kept as stub)

- Member: `AxisModel(AxesType, PlotType, IPlotTitleModel = null)`. A clean idiomatic draft matches every call and field store except the start. Mathcad's constructor stores the field initialisers (`_equations`, `_markers`, `_showNumbers`, `_gridLineColorHex`) before its `call Object::.ctor()`; Roslyn always emits the base call first, so the constructor cannot match with this field-initialiser layout. Declaring the field stores in the body does not change the base-call position either.
- Result: not landed; the constructor stays a generated stub. The draft is kept outside the repo, not committed.
- Left in AxisModel: the constructor (blocked above), the two blocked tick-mark helpers from batch 69 and `SetXmlNode` (both placeholders from batch 75), and the `AxisCalculatables` iterator is done (batch 75).
- SetMinWidthHeight (XYPlotStrategy) remains parked on the slot 41 vs 40 mismatch.

## Batch 77 — TraceModel nested helpers

- Members: `TraceAnalyzedData.HasMoreThanOneColumn` (auto property), `TraceAnalyzedData.SetDefaults`, `PlottingPointsForDisplay` constructor, `ViewPoints`, `GetClosestPlottingCoordinateOnRangeAxis`, `GetGraphPointsForPlottingCoordinateOnRangeAxis`. Hand-written: a nested `partial struct`/`partial class` block at the end of `Mirrored/Controls.Plots.Model.TraceModel.cs`; six stubs removed from `TraceModel.g.cs`.
- Shapes: the idiomatic bodies match, including the chained `RectPoints` NaN assignment and the copied-point loop as written in the original (no rewrite needed).
- Generated layout: six stubs removed. Verified: after a Release build, `emit` regenerates `TraceModel.g.cs` byte-identical to the committed file.
- Release IL: full gate PASS — 2554 checked, 0 failures.
- Left in TraceModel: `MinPositiveX` and `MinPositiveY` (lambda arguments), `RegionXmlNode`, `DataRange`, `Rl`, `SetEquation`, `RecalculateResultOnPlotTypeChange`, `UpdatePlottingPointsForDisplay`, `SetResult`, `AnalyzeResultData`, `CopyMinMaxAndUnitsResults`.

## Batch 78 — TraceModel DataRange and RecalculateResultOnPlotTypeChange

- Members: `DataRange` (private `Rect` getter; the polar branch uses `-num`, which Roslyn compiles to `neg` as Mathcad does), `RecalculateResultOnPlotTypeChange`. Hand-written at the top of the class in `Mirrored/Controls.Plots.Model.TraceModel.cs`; two stubs removed from `TraceModel.g.cs`.
- Generated layout: two stubs removed. Verified: after a Release build, `emit` regenerates `TraceModel.g.cs` byte-identical.
- Release IL: full gate PASS — 2556 checked, 0 failures.
- Blocked for now: `MinPositiveX` and `MinPositiveY` stay generated stubs. Their lambdas are named `<get_MinPositiveX>b__36_0` / `b__39_0` in Mathcad; the same placement at the top of the class gives `b__52_0` / `b__54_0`, so Quire has 16 more member units ahead of them. Matching needs the remaining `TraceModel` stubs in Mathcad order as placeholders (the approach used for `AxisCalculatables`), which is a larger restructure.
- Also pending on the same closure-ordinal layout: `UpdatePlottingPointsForDisplay` (display class 74) and `AnalyzeResultData` (lambda 172); `SetResult` (display class 166).
- Still stubs in TraceModel: `RegionXmlNode` (XML reader/writer), `Rl` (uses `InlineArray8<object>`), `SetEquation` (no closure; `SetEquationWithoutCommit` and the attach/detach helpers are already in hand-written code), `SetResult`, `AnalyzeResultData`, `CopyMinMaxAndUnitsResults`, `MinPositiveX`, `MinPositiveY`, `UpdatePlottingPointsForDisplay`.

## Batch 79 — TraceModel SetEquation and CopyMinMaxAndUnitsResults (blocked on a shared enum, kept as stubs)

- Members: `SetEquation(AxesIndex, IEquationItem)` and `CopyMinMaxAndUnitsResults(EngineResult)`. Both draft bodies match except one constant: the comparison with `SynchronizationStatus.Synchronized` is `ldc.i4.3` in Mathcad and `ldc.i4.1` in Quire.
- Cause: the enum value differs between the two sides. Mathcad `Ptc.Calculation.Core.SynchronizationStatus` is `Pending = 0, Synchronized = 3` (reference.py). Quire `Quire.Calculation.Core.SynchronizationStatus` (`src/Quire.Calculation.Core/ICalculatable.cs`) is `Pending, Synchronized` (0, 1).
- Result: not landed; both stay generated stubs. The fix is in `Quire.Calculation.Core` (give `Synchronized` the Mathcad value 3), which is outside this lane's assembly. Needs the maintainer or the owning lane. Any other Plots member that compares `SynchronizationStatus` will show the same mismatch until then.

## Batch 80 — TraceModel RegionXmlNode (blocked on the setter, kept as stub)

- Member: `RegionXmlNode` (property; getter and setter in one generated stub). The getter matches Mathcad exactly. The setter has the same leading difference as `AxisModel.SetXmlNode` (batch 74): Mathcad stores the `value` parameter in a temp before `if (value == null) throw new ArgumentNullException("value")` (`ldarg.1; stloc.0; ldloc.0; brtrue.s`), and Roslyn does not emit it.
- Result: not landed. Splitting the property to land only the getter would leave the setter as a placeholder, which AGENTS.md reserves for closure and iterator ordinals, so the whole property stays a generated stub.
- Pattern note: any setter or method with `if (value == null) throw new ArgumentNullException("value")` on an XML node parameter has this shape in Mathcad. Worth checking the same form elsewhere before writing it; the fix is not yet known.

## Batch 81 — PlotItemPlaceholder.GetMinLegendPlaceholderHeight (blocked, kept as stub)

- Member: `GetMinLegendPlaceholderHeight()`. The draft matches Mathcad's conditional-branch count (8) once the `(a is PlotTitle || b is PlotLegend) ? true : false` form is kept. The remaining difference is a materialised bool: Mathcad stores the test in a temp and copies it to `flag` (three more instructions, `stloc/ldloc/stloc`), and Roslyn removes that temp. The same shape blocked batch 69 (`CalculateBetterTickmarks`).
- Result: not landed; the member stays a generated stub.
- Left in PlotItemPlaceholder: `AdjustPadding(Size)` (not started), `GetMinLegendPlaceholderWidth` already exact in hand-written code.

## Batch 82 — PlotGraphAngularAxis overrides

- Members: `GetAxisEffectiveStart`, `GetAxisEffectiveEnd` (casts of `OtherAxis` to `PlotGraphRadialAxis`), `HandleOrientationChanged`, `PositionTickMarkChanged`, `UpdateAxisOriginPositions`, `MouseMoveDraw`, `MoveToValue`, `MoveByValue`, `FindClosestTickValueToZero` (returns true), `ApplyTickNumber`. Hand-written at the end of `Mirrored/Controls.Plots.PlotGraphAngularAxis.cs`; ten stubs removed from `PlotGraphAngularAxis.g.cs`.
- No closures or iterators in the group, so placement does not change any ordinal.
- Generated layout: ten stubs removed. The first `emit` after the Release build also generated two base-call stubs for the nested `Point2ScreenHelperImpl` overrides (`Point2Screen`, `Contains`), which the applied file lacked; the committed file is the emitted version. A second build and `emit` is byte-identical to it, and the gate passes on the committed state.
- Release IL: full gate PASS — 2566 checked, 0 failures.

## Batch 83 — PlotItem.RegisterOrUnregisterFromBorderEvents (exact); Move and OnContentSizeChanged blocked

- Landed member: `RegisterOrUnregisterFromBorderEvents(bool)`. Mathcad starts with `if (_border == null) return;` (early-return form), then `if (register)` with the three `+=` subscriptions and the `-=` else branch. Hand-written at the end of `Mirrored/Controls.Plots.PlotItem.cs`; one stub removed from `PlotItem.g.cs`.
- Generated layout: one stub removed. Verified: after a Release build, `emit` regenerates `PlotItem.g.cs` byte-identical to the committed file.
- Release IL: full gate PASS — 2567 checked, 0 failures.
- Blocked, kept as stubs:
  - `OnContentSizeChanged`: Mathcad pushes `this` before the `newSize = e.NewSize` store (`ldarg.0; ldarg.2; get_NewSize; stloc.0; ldloca.s 0; get_Width; set_Width`). The two-statement form gives the wrong order; `Width = (newSize = e.NewSize).Width` makes Roslyn add a struct temp (28 entries against 26).
  - `Move`: Mathcad stores `this` in a temp (`ldarg.0; stloc.3`) before the `is PlotTitle` / `is PlotLegend` tests. Roslyn does not emit that temp.

## Batch 84 — PlotGraphLinearAxisBase tick and offset helpers

- Members: `GetTickLine` (`-size` for the negative side, as Mathcad's `neg`), `Length` (the `double` local is declared before the `Size` local, matching the Mathcad slot order), `GetRealOffset`, `GetRealRelaviveOffset`. Hand-written at the end of `Mirrored/Controls.Plots.PlotGraphLinearAxisBase.cs`; four stubs removed from `PlotGraphLinearAxisBase.g.cs`.
- Generated layout: four stubs removed. Verified: after a Release build, `emit` regenerates `PlotGraphLinearAxisBase.g.cs` byte-identical to the committed file.
- Release IL: full gate PASS — 2571 checked, 0 failures.
- Still stubs in this class: `ArrowHeadGeometry`, `GetTickValueTextAndPositionByTickNumber`, `DrawNumbers`, `GetPositions`.

## Batch 85 — PlotGraphLinearAxisBase.GetTickValueTextAndPositionByTickNumber and DrawNumbers

- Members: `GetTickValueTextAndPositionByTickNumber` (`-width` for the negative offset, as Mathcad's `neg`) and `DrawNumbers` (the `out` variable is declared before the text variable to match Mathcad's slot order). Hand-written at the end of `Mirrored/Controls.Plots.PlotGraphLinearAxisBase.cs`; two stubs removed from `PlotGraphLinearAxisBase.g.cs`.
- Blocked, kept as stub: `ArrowHeadGeometry`. Mathcad calls `Point..ctor` on an address (`call`) for the struct values, where Roslyn uses `newobj` for `new Point(...)` in these positions (123 against 124 entries).
- Generated layout: two stubs removed. Verified: after a Release build, `emit` regenerates `PlotGraphLinearAxisBase.g.cs` byte-identical.
- Release IL: full gate PASS — 2573 checked, 0 failures.
- Still stubs in this class: `ArrowHeadGeometry` (blocked above) and `GetPositions`.

## Batch 86 — PlotGraphLinearAxisBase.GetPositions (blocked, kept as stub)

- Member: `GetPositions(out ×6)`, no closures. Two drafts were checked: a faithful two-statement form with `double` temps (extra locals, 416 entries against 374), and an inline form that assigns `desiredSize` and `transform` inside the `new Point(...)` arguments to keep Mathcad's evaluation order (422 entries).
- Cause: Mathcad stores each value once (`stloc.3; ldloca.s 3`) after reading `start.X` into the argument stack. Roslyn compiles `(desiredSize = X).Width` as `dup; stloc.s N; ldloca.s N` with a new temp slot. A separate statement fixes the temp but changes the argument order, so no C# form found reproduces the IL.
- Result: not landed; `GetPositions` stays a generated stub.

## Batch 87 — PlotLegendItem.OnActivateExecute (blocked on closure ordinal, kept as stub)

- Member: `OnActivateExecute` (one `WpfUtils.ExecuteOnLoad` lambda). The body matches except its closure name. Mathcad: `<OnActivateExecute>b__17_0`; Mathcad's constructor carries ordinal 16 (`<>9__16_1`), `AddTrace` is 19.
- Measured Quire names: at the top of the class `b__6_0` (only the six generated units count ahead of it); after the hand-written `SourceTrace`, `LegendLabel` and `MiniTraceWidth` properties `b__26_0`. The properties contribute more than their accessors, probably because their own lambdas count, so the simple unit count from batch 58 does not apply here.
- Result: not landed; `OnActivateExecute` stays a generated stub. Needs the placeholder layout for the type (Mathcad order, lambdas included) before the ordinal can be set.
- `AddTrace` (lambda ordinal 19, many object-initialiser bindings) is not started.

## Batch 88 — PlotGraphRadialAxis overrides

- Members: `HandleOrientationChanged`, `PositionTickMarkChanged`, `UpdateAxisOriginPositions`, `MouseMoveDraw`, `MoveToValue`, `MoveByValue`, `FindClosestTickValueToZero` (returns true), `ApplyTickNumber`. Mathcad bodies are empty (same as the angular axis in batch 82). Hand-written at the top of the class in `Mirrored/Controls.Plots.PlotGraphRadialAxis.cs`, so the unit count ahead of any existing hand-written member does not change; eight stubs removed.
- Generated layout: eight stubs removed. The first `emit` after the build also generated two base-call stubs for the nested `Point2ScreenHelperImpl` overrides (`Point2Screen`, `Contains`), as in batch 82; the committed file is the emitted version and the gate passes on it.
- Release IL: full gate PASS — 2581 checked, 0 failures.
- Still stubs in this class: `GetTickMarkElementPosition`, `GetTickMarkPositionByIndex`, `GetTickLine`, `AxisLineGeometry`, `DrawNumbers`.

## Batch 90 — PlotGraphRadialAxis tick helpers

- Members: `GetTickMarkElementPosition` (the element's `DesiredSize.Width` is read inline, so Mathcad's `this` load comes first), `GetTickMarkPositionByIndex` (public), `GetTickLine`, `AxisLineGeometry`, `DrawNumbers` (the `DevAssert` with format arguments and the `i < lastTick` loop as written). Hand-written at the top of the class in `Mirrored/Controls.Plots.PlotGraphRadialAxis.cs`; five stubs removed.
- A first draft with a local `Size desiredSize` failed on the load order at instruction 0; the inline width form matches.
- Generated layout: five stubs removed. Verified: after a Release build, `emit` leaves no new lines against HEAD, only the five removals, so the emitted file equals the applied one.
- Release IL: full gate PASS — 2586 checked, 0 failures.
- `PlotGraphRadialAxis.g.cs` now has no `NotImplemented` stubs left.

## Batch 91 — PlotTraceViewModel3D (8 members)

- Owner: haiku-plots-20261123. Branch: `prep/haiku-plots-next-3`, to land as `feature/haiku-plots-20261123`.
- Members: `FirePropertyChanged`, `ChangeSymbol`, `UserChangedStyle`, `IsOwnModel`, `ModelPropertyChanged`, `ChangeLineStyle`, `DoStyleChangeCommand`, `UndoStyleChangeCommand`. No closures. Hand-written at the top of the class in `Mirrored/Controls.Plots.Plot3D.ViewModel.PlotTraceViewModel3D.cs`; eight stubs removed. Usings added: `System.ComponentModel`, `System.Windows.Input`.
- Generated layout: after a Release build, `emit` also regenerated the nested `PropertyNames` struct (ten `[MirrorStub]` const fields). That block was missing from HEAD, so the committed `.g.cs` is the emitted version. Emit noise in other projects was reverted.
- Release IL: full gate PASS — 2594 checked, 0 failures, 8 verdict changes. The 2 unverified items are `PlotControlBase` members, not in this change.
- Still stubs in this class: none from the eight above.

## Batch 92 — BaseTrackball helpers (9 members)

- Owner: haiku-plots-20261123. Branch: `feature/haiku-plots-20261123` (local commit only; not landed, no slot yet).
- Members: `GetPositionRelativeToViewport`, `FireMouseUp`, `FireMouseWheel`, `HandleMouseWheel`, `ScaleFromTicks`, `AddScale`, `SetScale`, `IsTrackingDifferentButton`, `CoercePosition`. No closures. Hand-written in `Mirrored/Controls.Plots.Plot3D.Tools.BaseTrackball.cs`; nine stubs removed from the generated file.
- Shapes Mathcad needs: `FireMouseUp`/`FireMouseWheel` copy the event into a local before the null test (`stloc.0; ldloc.0; brfalse`), so the code uses local handler variables instead of `?.Invoke`. `HandleMouseWheel` uses an early return on `IsTracking`, not `if (!IsTracking)`.
- Release IL: first draft had three mismatches (the three shapes above); after the change the full gate PASS — 2603 checked, 0 failures.
- MirrorGen emit after the build changed only the nine removals in the target file; other emit noise reverted.
- Still stubs in this class: `Attach`, `Reset`, `CloneAndFreeze`, `Detach`, `UpdateCurrentTrackingInfo`, the `OnMouse*` handlers, `HandleMouseDown`/`HandleMouseUp`/`HandleMouseMove`, `TrackStart`/`Track`/`TrackEnd` and `InitializeTransform`. Not attempted in this batch.

## Batch 93 — BaseTrackball mouse handlers (7 members)

- Owner: haiku-plots-20261123. Branch: `feature/haiku-plots-20261123` (local commit; not landed, slot requested).
- Members: `OnMouseDown`, `HandleMouseDown`, `OnMouseUp`, `HandleMouseUp`, `OnMouseMove`, `HandleMouseMove`, `OnMouseWheel`. No closures. Hand-written in `Mirrored/Controls.Plots.Plot3D.Tools.BaseTrackball.cs` (new `using Quire.Wpf;` for `WpfUtils`); seven stubs removed from the generated file.
- Shapes Mathcad needs: `HandleMouseDown`, `HandleMouseUp` and `HandleMouseMove` return early on their guard conditions (`if (IsTrackingDifferentButton(...)) return;`, `if (!IsTracking || IsTrackingDifferentButton(...)) return;`, `if (!IsTracking) return;`), not nested `if` blocks. A first draft with nested `if` blocks failed on the first branch.
- Release IL: after the early-return change, the full gate PASS — 2610 checked, 0 failures.
- MirrorGen emit after the build changed only the seven removals; other emit noise reverted.
- Still stubs in this class: `UpdateCurrentTrackingInfo` (needs the `DocumentAttachedProperties.GetZoom` helper and `DoubleComparer`), `Attach`, `Reset`, `CloneAndFreeze`, `Detach`, `TrackStart`/`Track`/`TrackEnd`, `InitializeTransform`.

## Batch 94 — BaseTrackball.UpdateCurrentTrackingInfo

- Owner: haiku-plots-20261123. Branch: `feature/haiku-plots-20261123` (local commit; slot to be requested).
- Member: `UpdateCurrentTrackingInfo` (protected virtual, no closures). Hand-written in `Mirrored/Controls.Plots.Plot3D.Tools.BaseTrackball.cs` (new `using Quire.Controls.Core;`); one stub removed from the generated file.
- Shapes Mathcad needs: `CurrentTrackingInfo` is re-read through the property at each use (no local cache), the `HasSignificantMove` guard is an early return, and the offset is computed inside the setter expression so the receiver is loaded first (`stloc` + `ldloca` for `Vector.Length`).
- Release IL: first draft (local cache, `if (!...)` block, separate `offset` statement) failed on receiver/load order; the final form PASS — 2611 checked, 0 failures.
- MirrorGen emit after the build changed nothing beyond the staged stub removal; other emit noise reverted.
- Still stubs in this class: `Attach`, `Reset`, `CloneAndFreeze`, `Detach`, `TrackStart`/`Track`/`TrackEnd` (abstract, not needed), `InitializeTransform` (abstract).

## Batch 95 — BaseTrackball attach/reset/detach cluster (4 methods, 10 properties/fields)

- Owner: haiku-plots-20261123. Branch: `feature/haiku-plots-20261123` (local commit; slot to be requested).
- Members: `Attach`, `Reset`, `CloneAndFreeze<T>`, `Detach` (methods); `ScaleTransform`, `BaseTransform`, `IsTracking`, `CurrentScale`, `CameraTransform`, `TrackingHelper`, `Viewport`, `MouseEventsSource` (properties); fields `_transformOwner`, `_trackingHelper`. Hand-written in `Mirrored/Controls.Plots.Plot3D.Tools.BaseTrackball.cs`; 14 stubs removed from the generated file (property stubs with their accessors, and the fields).
- Shapes Mathcad needs: `Reset` has no local for `transform3DGroup.Children` (read the property at each use) and assigns the scale transform inside the argument (`Children.Add(ScaleTransform = new MatrixTransform3D(matrix))`), so the initializer temp is slot 4 and the dup temp slot 5, as in Mathcad. A declared `value2` local shifted the slots and failed.
- Release IL: after the change, the full gate PASS — 2628 checked, 0 failures.
- MirrorGen emit: 28 deletions (14 stubs + attributes), equal to the applied edit; other emit noise reverted.
- Still stubs in this class: `Model3DBoundingBox`, `MouseUp`/`MouseWheel` event accessors (generated), the constructor and `InitializeTransform`/`TrackStart`/`Track`/`TrackEnd` (abstract, unchanged).

## Batch 96 — BaseTrackball.Model3DBoundingBox

- Owner: haiku-plots-20261123. Branch: `feature/haiku-plots-20261123` (local commit; slot to be requested).
- Member: `Model3DBoundingBox` (public read-only property, `=> PlotCommonConstants.Plot3DBoundingBox`). Hand-written in `Mirrored/Controls.Plots.Plot3D.Tools.BaseTrackball.cs`; the stub and its attribute removed from the generated file.
- Release IL: full gate PASS — 2629 checked, 0 failures.
- MirrorGen emit: 2 deletions, equal to the applied edit.
- Remaining in this class: the protected constructor (`{ }` stub-marked, Mathcad body is `base()` only) and the abstract `InitializeTransform`/`TrackStart`/`Track`/`TrackEnd` (abstract, no body).

## Batch 97 — PlotDataObjectCreator enum converters (5 members)

- Owner: haiku-plots-20261123. Branch: `feature/haiku-plots-20261123` (local commit; slot to be requested).
- Members: `ConvertBackgroundType`, `ConvertScaling`, `ConvertAxisPosition`, `ConvertTraceType`, `ConvertSymbol`. Each is a plain enum-to-enum `switch` with an `ArgumentOutOfRangeException` default, written in the same style as the already-exact `ConvertLineStyle` overloads. Hand-written in `Mirrored/PlotDataObjects.PlotDataObjectCreator.cs`; five stubs removed from the generated file.
- Parameter type: `ConvertTraceType` uses the fully qualified `Quire.Calculation.Core.Trace2DResults.TraceType` to match the generated signature (`using` alone does not bring the `Trace2DResults` name into scope).
- Release IL: full gate PASS — 2634 checked, 0 failures; all five are listed exact.
- MirrorGen emit: 10 deletions (5 stubs + attributes), equal to the applied edit.
- Still a stub: `FillPlotBaseType` (larger body, reads the plot control's axes and trace collections).

## Batch 98 — PlotTrace3D visual members (5 of 6)

- Owner: haiku-plots-20261123. Branch: `feature/haiku-plots-20261123` (local commit; slot to be requested).
- Members landed exact: `FireVisualChanged`, `UpdateLineStyle`, `UpdateSymbol`, `CreateSolidMesh`, `UpdateVisualData`. Hand-written in `Mirrored/Controls.Plots.Plot3D.View.PlotTrace3D.cs` (usings added: `System.Linq`, `Quire.Controls.Plots.Plot3D.Model`, `Quire.Media3D`, `Quire.Media3D.Wires`); five stubs removed from the generated file.
- Shapes Mathcad needs: guards in `UpdateLineStyle`/`UpdateSymbol` are early returns (`if (Visual == null || X == null) return;`); `UpdateVisualData` declares `wireBase` before `surfaceFillVisual` (local slot order).
- Still a stub: `UpdateSurfaceFill`. It is not exact. Early-return guards and the local order match; the remaining difference is the `geometryModel3D.BackMaterial = material` pair. Mathcad loads the `geometryModel3D` receiver twice and stores the material with a single `dup; stloc` before `set_BackMaterial`, then `set_Material`. Roslyn adds an extra `dup; stloc.3` for the outer assignment in each expression form tried (two statements, `Material material;` declared first, and the chained `geometryModel3D.Material = material = geometryModel3D.BackMaterial = ...`). Left as a stub rather than landing a non-exact body.
- Runtime note: `UpdateVisualData` (exact) calls `UpdateSurfaceFill`, which still throws `NotImplemented`. The constructor's path through `UpdateVisualData` will throw until `UpdateSurfaceFill` is done. It threw before this batch too, so the change does not add a new failure path, but the batch is not end-to-end functional.
- Release IL: full gate PASS — 2639 checked, 0 failures.
- MirrorGen emit: deletions only (five members with their attributes), equal to the applied edit.

## Batch 99 — PlotTrace3D.UpdateSurfaceFill (completes batch 98)

- Owner: haiku-plots-20261123. Branch: `feature/haiku-plots-20261123` (local commit; slot to be requested).
- Member: `UpdateSurfaceFill`. Hand-written in `Mirrored/Controls.Plots.Plot3D.View.PlotTrace3D.cs`; its stub removed from the generated file.
- Shape Mathcad needs: `geometryModel3D.Material = geometryModel3D.BackMaterial = Tools3D.MaterialForColor(SurfaceFill.Value);`, a single chained statement. Roslyn lowers `recv.Material = (recv.BackMaterial = X)` to `recv; recv; X; dup; stloc tmp; set_BackMaterial; ldloc tmp; set_Material`, which is exactly the Mathcad sequence (the temp is slot 3). The earlier `Material material` variants declared a real local, which shifted the slots and failed. The same pattern is already exact in `PlotGraphAngularAxis`.
- Release IL: full gate PASS — 2640 checked, 0 failures.
- MirrorGen emit: removes only the `UpdateSurfaceFill` stub and attribute, equal to the applied edit.
- `PlotTrace3D` now has no `NotImplemented` stubs for the visual path; the constructor's `UpdateVisualData` call no longer throws.

## Batch 100 — Base3DAxisViewModel event helpers (5 members)

- Owner: haiku-plots-20261123. Branch: `feature/haiku-plots-20261123` (local commit; slot to be requested).
- Members: `FireRangeChanged`, `InitializeLimitExpression`, `FireCalculatableAdded`, `HandleTraverse`, `CanTraverse`. No closures; each is a null-checked event or field invocation. New hand-written partial `Mirrored/Controls.Plots.Base3D.ViewModel.Base3DAxisViewModel.cs`; five stubs removed from the generated file.
- Release IL: full gate PASS — 2645 checked, 0 failures; first try.
- MirrorGen emit: deletions only (five stubs with their attributes), equal to the applied edit.
- Still stubs in this class: `UpdateLineColor`, `HookupView`, `GetTickInterval`, `StartExpressionValueChanged` (depend on the `IsActive`/`IsHighlighted`/`LineColor`/`PlotAxisModel` property stubs), the constructor, and the property/event accessors.

## Batch 101 — Base3DAxisViewModel LineColor and colour fields

- Owner: haiku-plots-20261123. Branch: `feature/haiku-plots-20261123` (local commit; slot to be requested).
- Members: the `LineColor` property (private setter with the `Equals` guard and `FirePropertyChanged`), and the fields `_lineColor`, `_defaultLineColor`, `_higlightedLineColor`, `_activeLineColor`. Also the `_isActive`/`_isHighlighted` backing fields. Hand-written in `Mirrored/Controls.Plots.Base3D.ViewModel.Base3DAxisViewModel.cs`; the matching stubs removed from the generated file.
- Release IL: full gate PASS — 2648 checked, 0 failures.
- Still stubs, deliberately: `IsActive`, `IsHighlighted` and `UpdateLineColor`. `UpdateLineColor` is `LineColor = <cond>` with the receiver loaded first and a `brtrue` layout into the active arm. Roslyn normalizes `!a ? x : y` into `a ? y : x`, which puts the active arm first, so every expression form I tried mismatched at the field-access order. The stub is kept so the `IsActive`/`IsHighlighted` setters are not hand-written against a non-exact body.
- MirrorGen emit: the target file changes match the applied removals; other emit noise reverted.

## Batch 102 — PlotGraphAxisBase limit-equation helpers (5 members)

- Owner: haiku-plots-20261123. Branch: `feature/haiku-plots-20261123` (local commit; slot to be requested).
- Members: `InitializeLimitEquation(EquationControl)`, `LimitEquationSExpressionChanging`, `LimitEditorLostFocus`, `ShowLimit`, `HideLimit`. No closures; method-group handlers only. Added to the existing hand-written partial `Mirrored/Controls.Plots.PlotGraphAxisBase.cs` (new `using Quire.Controls;`); five stubs removed.
- Release IL: full gate PASS — 2653 checked, 0 failures; first try.
- MirrorGen emit: deletions only (five stubs with attributes), equal to the applied edit.
- Still stubs: the no-argument `InitializeLimitEquation()` (uses `CommandScope`/`CompoundCommand` and `EquationControlWithTransform`), `ShowStartExpression`/`HideStartExpression` and siblings, `TickMarksChanged`/`TickmarksChanged`, `Dispatch`, `InitializeLimitLabel`, and the `DependencyProperty` change handlers.

## Batch 103 — PlotGraphAxisBase show/hide expression wrappers (7 members)

- Owner: haiku-plots-20261123. Branch: `feature/haiku-plots-20261123` (local commit; slot to be requested).
- Members: `ShowStartExpression`, `ShowSecondTickExpression`, `ShowEndExpression`, `HideStartExpression`, `HideSecondTickExpression`, `HideEndExpression` (one-line wrappers over the exact `ShowLimit`/`HideLimit`), and `TickMarksChanged` (protected virtual, Mathcad body `throw new NotImplementedException()`). Hand-written in the existing `Mirrored/Controls.Plots.PlotGraphAxisBase.cs`; seven stubs removed.
- Release IL: full gate PASS — 2660 checked, 0 failures; first attempt with the wrong attribute spelling failed to compile and was reverted before the real attempt.
- MirrorGen emit: deletions only (seven stubs with attributes), equal to the applied edit.
- Still stubs: the no-argument `InitializeLimitEquation()`, `ShowGridLinesChanged`, `GridLineColorChanged`, `TickmarksChanged`, `CreateTickmarksPropertyMetadata`, `Dispatch`, `InitializeLimitLabel`.

## Batch 104 — PlotGraphAxisBase InitializeLimitLabel

- Owner: haiku-plots-20261123. Branch: `feature/haiku-plots-20261123` (local commit; slot to be requested).
- Members: `InitializeLimitLabel(Control, MouseButtonEventHandler)` (protected; adds the label to `VisualChildren`, sets font size/family/foreground/padding, hooks the mouse handler). Added to the existing hand-written partial `Mirrored/Controls.Plots.PlotGraphAxisBase.cs` (new `using System.Windows.Input;`); one stub removed from the generated file.
- Release IL: full gate PASS — 2661 checked, 0 failures, 2661 exact.
- MirrorGen emit: deletions only (the one stub with its attribute), equal to the applied edit. The first emit restored the stub because `bin/Release` was stale; rebuilding the Plots Release project before emit kept the removal.
- Still stubs: the no-argument `InitializeLimitEquation()`, `ShowGridLinesChanged`, `GridLineColorChanged`, `TickmarksChanged`, `CreateTickmarksPropertyMetadata`, `Dispatch`.

## Batch 105 — PlotGraphAxisBase tick-mark metadata and Dispatch

- Owner: haiku-plots-20261123. Branch: `feature/haiku-plots-20261123` (local commit; slot to be requested).
- Members: `CreateTickmarksPropertyMetadata(int)` (a single `FrameworkPropertyMetadata` return with the method group `TickmarksChanged`; Roslyn emits the same static delegate cache as Mathcad) and `Dispatch(Action)` (`Dispatcher.CheckAccess()` then `Dispatcher.Invoke(DispatcherPriority.Normal, action)`). Added to the existing hand-written partial `Mirrored/Controls.Plots.PlotGraphAxisBase.cs` (new `using System.Windows.Threading;`); two stubs removed.
- Release IL: full gate PASS — 2663 checked, 0 failures, 2663 exact.
- MirrorGen emit: deletions only (the two stubs with attributes), equal to the applied edit. The Plots Release DLL was rebuilt before emit; other emit noise reverted.
- Still stubs: the no-argument `InitializeLimitEquation()`, `ShowGridLinesChanged`, `GridLineColorChanged`, `TickmarksChanged`.

## Batch 106 — PlotGraphAxisBase grid-line change handlers

- Owner: haiku-plots-20261123. Branch: `feature/haiku-plots-20261123` (local commit; slot to be requested).
- Members: `ShowGridLinesChanged` and `GridLineColorChanged` (private static `DependencyProperty` callbacks). They call the generated stubs `PlotGraphAxis.FireShowGridLinesChanged` and `GridLineColorPropertyChanged`, which stay stubs until the `PlotGraphAxis` batch. Added to the existing hand-written partial `Mirrored/Controls.Plots.PlotGraphAxisBase.cs`, after `LineColorChanged`; two stubs removed.
- Release IL: full gate PASS — 2665 checked, 0 failures, 2665 exact.
- MirrorGen emit: deletions only (the two stubs with attributes), equal to the applied edit. Other emit noise reverted.
- Still stubs: the no-argument `InitializeLimitEquation()` and `TickmarksChanged` (needs `PlotGraphAxis.TickMarksChanged`/`FireTickMarksChanged`).

## Batch 107 — PlotGraphAxisBase no-argument InitializeLimitEquation

- Owner: haiku-plots-20261123. Branch: `feature/haiku-plots-20261123` (local commit; slot to be requested).
- Members: `InitializeLimitEquation()` (protected). Creates the three `EquationControlWithTransform(8.0)` limit editors inside `using (new CommandScope(new CompoundCommand()))`, then calls the one-argument helper for each. Added to the existing hand-written partial `Mirrored/Controls.Plots.PlotGraphAxisBase.cs` (new `using Quire.Undo;`); one stub removed.
- Release IL: full gate PASS — 2666 checked, 0 failures, 2666 exact; first try.
- MirrorGen emit: deletions only (the one stub with its attribute), equal to the applied edit. Other emit noise reverted.
- Still stubs: `TickmarksChanged` (needs `PlotGraphAxis.TickMarksChanged`/`FireTickMarksChanged`), and the stubs in `PlotGraphAxis` that `ShowGridLinesChanged`/`GridLineColorChanged` now call.

## Batch 108 — SynchronizationStatus follow-up (PlotCalculatable, comparison cleanup)

- Owner: haiku-plots-20261123. Branch: `feature/haiku-plots-20261123` (local commit; slot to be requested).
- Members: `PlotCalculatable.SynchronizationStatus` (backing field initialised to `Synchronized`, getter, private setter with the `!=` guard and `FirePropertyChanged`), `SetSynchronizationStatus`, and `SetStatusSynchronized` (copies `TreatAsUpToDate` to a local before invoking; the `?.Invoke` form mismatched the Mathcad IL). New hand-written partial `Mirrored/Controls.Plots.Model.PlotCalculatable.cs`; four generated stubs removed (field, property, two methods).
- Cleanup now that the enum is correct: `BaseMarker` and `PlotEquationModel3D` compare against `SynchronizationStatus.Synchronized` instead of the literal `(SynchronizationStatus)3`. The Release IL is unchanged.
- Release IL: full gate PASS — 2670 checked, 0 failures, 2670 exact.
- MirrorGen emit: deletions only in `PlotCalculatable.g.cs` (eight lines, equal to the applied removals). Other emit noise reverted.
- Still blocked/stubbed, not changed here: `PlotModel3D.ComputeTotalTracesSynchronizationStatus`, the `SynchronizationStatus` properties on `AxisLimitEditor`, `RealNumberCalculatable`, `PlotViewModel3D`, `PlotEquation`, and `SetSynchronizationStatus` on `AxisLimitEditor`/`RealNumberCalculatable`.

## Batch 109 — SynchronizationStatus members on RealNumberCalculatable, PlotEquation, PlotViewModel3D

- Owner: haiku-plots-20261123. Branch: `feature/haiku-plots-20261123` (local commit; slot to be requested).
- Members: `RealNumberCalculatable.SynchronizationStatus` (getter), `RealNumberCalculatable.SetSynchronizationStatus` (`!=` guard then `OnPropertyChanged("SynchronizationStatus")`), and its backing field (new partial `Mirrored/Controls.Plots.Model.RealNumberCalculatable.cs`); `PlotEquation.SynchronizationStatus` (combined `_exp`/`_scaling` status; added to the existing `Mirrored/Controls.Plots.PlotEquation.cs`); `PlotViewModel3D.SynchronizationStatus` (`Model.SynchronizationStatus`; added to the existing `Mirrored/Controls.Plots.Plot3D.ViewModel.PlotViewModel3D.cs`). Four stubs removed in each generated file set (six for RealNumberCalculatable's field, property, method; two each for the other two).
- Release IL: full gate PASS — 2674 checked, 0 failures, 2674 exact.
- MirrorGen emit: deletions only, equal to the applied removals. Other emit noise reverted.
- Still stubbed, not touched: `PlotModel3D.ComputeTotalTracesSynchronizationStatus`, `AxisLimitEditor.SynchronizationStatus` and `SetSynchronizationStatus`. `Controls.Mathboard.AggregateMathboardControl` in another lane still compares against the literal `3`; not in this assembly.

## Batch 110 — PlotGraphAxisBase TickmarksChanged

- Owner: haiku-plots-20261123. Branch: `feature/haiku-plots-20261123` (local commit; slot to be requested).
- Members: `TickmarksChanged(DependencyObject, DependencyPropertyChangedEventArgs)` (protected static; casts to `PlotGraphAxis`, null-checks, calls `TickMarksChanged()` and `FireTickMarksChanged()`). Added to the existing hand-written partial `Mirrored/Controls.Plots.PlotGraphAxisBase.cs`; one stub removed.
- Release IL: full gate PASS — 2675 checked, 0 failures, 2675 exact.
- MirrorGen emit: deletions only (the one stub with its attribute). Other emit noise reverted.
- Blocked, not committed: the `TickMarks` property. Its setter wraps `SetValue` in a lambda passed to `Dispatch`; Roslyn names the display class `<>c__DisplayClass60_0` where Mathcad has `58_0`. The member ordinal in the type is two higher in Quire, and I found no member to remove or reorder inside this assembly. The property stub stays. The getter alone cannot be declared without the setter stub.
- Still stubs: `TickMarks`, `InitializeComponent`/`Connect`/`_CreateDelegate` (XAML-generated, skipped), and the other members listed for this type.

## Batch 111 — AxisLimitEditor SynchronizationStatus members (retry on current main)

- Owner: haiku-plots-20261123. Branch: `feature/haiku-plots-20261123` (local commit; slot to be requested).
- Members: `AxisLimitEditor.SynchronizationStatus` (forwards to `_eq.SynchronizationStatus`) and `SetSynchronizationStatus` (forwards to `_eq.SetSynchronizationStatus`). New hand-written partial `Mirrored/Controls.Plots.AxisLimitEditor.cs`; two stubs removed.
- Release IL: full gate PASS — 2677 checked, 0 failures, 2677 exact. The earlier blocker (IEquationControl not inheriting ICalculatableInfo) is resolved on current main; no compile failure.
- MirrorGen emit: deletions only (the two stubs with attributes). Other emit noise reverted.
- Still stubs: the `SynchronizationStatus == Pending` consumer inside the other AxisLimitEditor members (not in this batch).

## Batch 112 — PlotDataObjectCreator.FillPlotBaseType

- Owner: haiku-plots-20261123. Branch: `feature/haiku-plots-20261123` (local commit; slot to be requested).
- Members: `FillPlotBaseType(plotBaseType, PlotControl)`, the save path for a plot's legend, graph size and traces (called by `CreatePlotDataObject`). Added to the existing hand-written partial `Mirrored/PlotDataObjects.PlotDataObjectCreator.cs`, after `CreateNumberFormat`; one stub removed.
- Shape: the graph size is an object initializer on `new SizeType`. Declaring a `SizeType` local and a separate `Size` local compiled to a different IL layout (extra locals and an extra `newobj` position); the initializer matches Mathcad.
- Release IL: full gate PASS — 2678 checked, 0 failures, 2678 exact.
- MirrorGen emit: deletions only (the one stub with its attribute). Other emit noise reverted.
- Still stubs in this type: none listed as non-XAML. `CreateNumberFormat` and other members remain as in earlier checkpoints.

## Batch 113 — PlotGraphAxisBase TickMarks (closure ordinal resolved by member placement)

- Owner: haiku-plots-20261123. Branch: `feature/haiku-plots-20261123` (local commit; slot to be requested).
- Members: `TickMarks` property (getter `GetValue`, setter forwards `SetValue` inside a `Dispatch` lambda). Added to the existing hand-written partial `Mirrored/Controls.Plots.PlotGraphAxisBase.cs`; one stub removed.
- Closure ordinal: the setter's display class ordinal counts members declared earlier in the type (one per member). Placed after `HideSecondTickExpression` and before `HideEndExpression`, the setter's display class is `<>c__DisplayClass58_0`, matching Mathcad. The earlier placements (before `LineColorChanged`: 44, before `TickMarksChanged`: 60) did not match. This is a compile-order device, commented in the file.
- Release IL: full gate PASS — 2682 checked, 0 failures, 2682 exact.
- MirrorGen emit: deletions only (the one stub with its attribute). Other emit noise reverted.
- Blocked: `PlotModel3D.ComputeTotalTracesSynchronizationStatus` still needs the same kind of placement test (its lambda is `b__96_0` against Mathcad's `b__67_0`).

## Batch 114 — AxisLimitEditor members and events

- Owner: agy-whiteboard-2. Branch: `feature/agy-plots`.
- Members:
  - Properties & simple members: `Rl`, `CanHideFromCalculationContext`, `CurrentError`, `EngineResult` (getter/setter), `InterceptAndUpdateResult`, `UseICalculatableToRestoreResult`, `CalculatingStatus`, `ErrorCount`, `ErrorSExpression`, `ErrorCode`, `ErrorText`, `IsCommitted`, `Error`, `RestoreResult`
  - Methods: `SetIdentifierLabels`, `SetAutoSelectAlgorithm`, `SetRedefinitionWarnings`, `SetStatusSynchronized`, `SetCalculatingStatus`, `ContainsError`, `ShowError`, `FireValueChanged`, `FireHasErrorsChanged`, `FirePropertyChanged`
  - Events: `ValueChanged`, `HasErrorsChanged`, `RequestEvaluation`, `GetValueOfIdentifierBefore`, `TreatAsUpToDate`, `PropertyChanged`
- Hand-written: `Mirrored/Controls.Plots.AxisLimitEditor.cs`; corresponding generated stubs removed from `Generated/Mirror/Controls.Plots.AxisLimitEditor.g.cs`.
- Release IL: PASS — 2707 checked in Plots, 0 failures.
## Batch 115 — AxisLimitEditor remainder

- Owner: agy-whiteboard-2. Branch: `feature/agy-plots`.
- Members:
  - Properties: `IsWaitingForEngineResponse` (get/set), `CalculatedValue` (get/set), `PreCalculatedValue` (get/set), `IsValueValid`, `IsDefaultValue` (get/set), `HasErrors` (get/set), `RegionXmlNode` (get/set)
  - Constructor & lifecycle: `AxisLimitEditor(.ctor)`, `HookupEquationChangedEvents`, `UnhookEquationChangedEvents`
  - Calculation & formatting: `SetCalculatedValueField`, `SetIsDefaultValueInternal`, `ResetCalculatedValue`, `SetPreCalculatedValue`, `EquationChanged`, `EquationSExpressionChanging`, `CreateNewSExpIfIsDefaultValueIsTrue`
  - Errors: `SetNonCalculationRelatedCustomError`, `SetCalculationRelatedCustomError`, `SetCustomError`, `ClearNonCalculationRelatedCustomErrors`, `ClearAllCustomErrors`, `SetResult`, `SetError`
  - Evaluation & hide: `EqRequestEvaluation`, `Commit`, `SetHideFromCalculationContext`, `UpdateHideFromCalculationContextToDefaultValue`
  - Serialization: `GetDataObject`, `SetEquationExpressionXml`, `SetDataObject`
- Closure ordinal: `HideFromCalculationContext` property setter uses `DisplayClass79_0` in Mathcad; kept as a generated stub in `AxisLimitEditor.g.cs` pending ordinal alignment. All other 31 members in `AxisLimitEditor` mirrored and verified exact.
- Shapes:
  - `SetIsDefaultValueInternal`: early return on `if (_isDefaultValue == newValue) return;` emits `bne.un.s`.
  - `CreateNewSExpIfIsDefaultValueIsTrue`: `NumberFormat numberFormat = (_currentNumberFormatProperty == null) ? null : _currentNumberFormatProperty.CurrentNumberFormat;` emits `brfalse.s` branching to `ldnull`.
  - `GetDataObject`: `valueType.InnerXml = ((resultManager == null) ? null : (flag ? ... : null));` and `valueType.EquationSExpression = ((resultManager != null) ? null : (flag ? ... : null));` match Mathcad's `brfalse.s` / `brtrue.s` branching to `ldnull`.
  - `SetEquationExpressionXml`: `XmlNode xmlNode = value.OwnerDocument.CreateElement("math", "ws");` types local 0 as `XmlNode`.
- Release IL: PASS — 2744 checked in Plots, 0 failures.
- Remaining mismatches in this batch: none.

## Batch 116 — BaseMarkerViewModel, PlotItem, PlotItemPlaceholder, XYPlotControl

- Owner: agy-whiteboard-2. Branch: `feature/agy-plots`.
- Members:
  - `BaseMarkerViewModel`: `ActivateCommand` static field (`PlotViewModel.ActivateCommand`)
  - `PlotItem`: `Move`, `OnContentSizeChanged`
  - `PlotItemPlaceholder`: `AdjustPadding(Size)`, `GetMinLegendPlaceholderHeight()`
  - `XYPlotControl`: `_xAxis`, `_yAxis` fields, `EndInit()`
- Hand-written:
  - `Mirrored/Controls.Plots.ViewModel.BaseMarkerViewModel.cs`
  - `Mirrored/Controls.Plots.PlotItem.cs`
  - `Mirrored/Controls.Plots.PlotItemPlaceholder.cs`
  - `Mirrored/Controls.Plots.XYPlotControl.cs`
- Shapes:
  - `PlotItem.Move`: `this switch { ... }` with positive ternaries `ShowTitle ? Visible : Collapsed`.
  - `PlotItem.OnContentSizeChanged`: `if (IsWidthExplicitlySet) return; if (e.WidthChanged && e.NewSize.Width != Width) Width = e.NewSize.Width;` matches Mathcad's early return and setter.
  - `PlotItemPlaceholder.AdjustPadding`: Local slots 0 and 1 assigned by declaring `double num; double num2;` before `frameworkElement`. Positive ternary `flag ? Height : Width` matches Mathcad branch order.
  - `PlotItemPlaceholder.GetMinLegendPlaceholderHeight`: Local variable order (`FrameworkElement frameworkElement`, `double height`, `bool flag`, `bool flag2`, `double num`), with struct temporaries `Thickness` and `Size` using slots 5 and 6 via direct `GridCellSize.Height`.
  - `XYPlotControl.EndInit`: Positive null-check guard `if (Model != null && _xAxis != null && _yAxis != null)` wraps the body, jumping to final `ret` on null without early ret. Null-conditional `?.` expressions `_xAxis.EquationList?.Visibility == Visibility.Visible` match Mathcad's `dup; brtrue.s ...; pop; ldc.i4.0; ... ceq`.
- Release IL: PASS — 2751 checked in Plots, 0 failures.
- Remaining mismatches in this batch: none.

## Batch 117 — MarkersPanel, PlotLegendItem

- Owner: agy-whiteboard-2. Branch: `feature/agy-plots`.
- Members:
  - `MarkersPanel`: `HorizontalRangeHelperProperty`, `VerticalRangeHelperProperty`, `VerticalValueProperty`, `HorizontalValueProperty`, `EquationControlTransformProperty` (and `.cctor`)
  - `PlotLegendItem`: `.ctor(IPlotTrace)`, `OnActivateExecute`, `AddTrace`, `GetRegionXmlNode`, `SetRegionXmlNode`, `InitializeComponent`, `IComponentConnector.Connect`
- Hand-written:
  - `Mirrored/Controls.Plots.MarkersPanel.cs`
  - `Mirrored/Controls.Plots.PlotLegendItem.cs`
- Shapes:
  - `MarkersPanel`: Static field initializers for attached properties with `new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)` emit `ldc.i4 264` matching Mathcad IL.
  - `PlotLegendItem`:
    - Moved XAML-generated fields (`LegendItemStackPanel`, `MiniTraceLegend`, `LegendItemLabelPlaceholder`, `_contentLoaded`) and stubs (`InitializeComponent`, `Connect`) to the bottom of the partial class, restoring exact member ordinals for closures (`<.ctor>b__16_0`, `<OnActivateExecute>b__17_0`, `<AddTrace>b__19_0`).
    - Guard branches in `OnActivateExecute`, `OnLegendLabelLostFocus`, `SetRegionXmlNode`, and `.ctor` lambda use early return on null (`if (LegendLabel == null) return;`), matching Mathcad's `brtrue.s` skip patterns.
- Release IL: PASS — 2763 checked in Plots, 0 failures.
- Remaining mismatches in this batch: none.

## Batch 118 — PlotTitle and TraceRenderer remaining members

- Owner: agy-whiteboard-2. Branch: `feature/agy-plots`.
- Members:
  - `PlotTitle`: `_contentLoaded`, `InitializeComponent`, `_CreateDelegate`, `IComponentConnector.Connect`
  - `TraceRenderer`: `_GeometryTransformationDictionary`, `_TraceLineDictionary`, `_ownerTrace`, `StartNewCollectionAfter`, `OnRender`, `DrawSymbolGeometry`, `BreakPolylineIfItIsTooLong`
- Hand-written:
  - `Mirrored/Controls.Plots.PlotTitle.cs`
  - `Mirrored/Controls.Plots.Traces.TraceRenderer.cs`
- Shapes:
  - `PlotTitle`:
    - Moved auto-properties (`ViewModel`, `IsRotationAllowed`, `Title`) before `_isEditingYAxisTitle` to match Mathcad's field backing order.
    - Implemented `InitializeComponent` with early exit guard on `_contentLoaded`.
    - Implemented `_CreateDelegate` and `Connect` with exact BAML connector branching.
  - `TraceRenderer`:
    - Maintained exact member declaration order from `members.tsv` (`.ctor`, `Render`, `OnRender`, `DrawGeometry`, `CreateSymbols`, `DrawSymbolGeometry`, `CreateSymbolsGeometryDictionary`, `CreateTraceLineDictionary`, properties, `AddNewScreenPointCollection`, `SetScreenPoints`, `BreakPolylineIfItIsTooLong`, `TotalPointCount`).
    - Exact closure parity: `DrawSymbolGeometry` lambda aligns to `<>c__DisplayClass10_0::<DrawSymbolGeometry>b__0` and `BreakPolylineIfItIsTooLong` lambda aligns to `<>c::<BreakPolylineIfItIsTooLong>b__31_0`.
- Release IL: PASS — 2774 checked in Plots, 0 failures.
- Remaining mismatches in this batch: none.

## Batch 119 — BaseTrackball, PlotTrace3D, and PlotTraceViewModel3D remaining members

- Owner: agy-whiteboard-2. Branch: `feature/agy-plots`.
- Members:
  - `BaseTrackball`: `_previousTransform`, `MouseUp` event, `MouseWheel` event, `InitializeTransform`, `TrackStart`, `Track`, `TrackEnd`, `.ctor`
  - `PlotTrace3D`: `_baseLineThickness`, `_visual`, `_linesTraceVisual`, `_symbolTraceVisual`, `_surfaceFillVisual`, `_emptyPoint3DCollection`, `.ctor(IPlotTraceViewModel3D)`, `Dispose`
  - `PlotTraceViewModel3D`: `PropertyNames` (10 const strings), `_model`, `PropertyChanged` event, `.ctor(IPlotTraceModel3D)`
- Hand-written:
  - `Mirrored/Controls.Plots.Plot3D.Tools.BaseTrackball.cs`
  - `Mirrored/Controls.Plots.Plot3D.View.PlotTrace3D.cs`
  - `Mirrored/Controls.Plots.Plot3D.ViewModel.PlotTraceViewModel3D.cs`
- Shapes:
  - `BaseTrackball`:
    - Arranged fields and properties (`_transformOwner`, `MouseUp`, `MouseWheel`, `ScaleTransform`, `BaseTransform`, `CurrentTrackingInfo`, `_trackingHelper`, `_previousTransform`, `Viewport`, `MouseEventsSource`) to match Mathcad IL field layout.
    - Declared abstract methods and protected constructor in matching token sequence.
  - `PlotTrace3D`:
    - Declared `VisualChanged`, `_baseLineThickness = 1.0;`, `ViewModel`, `_visual`, `_linesTraceVisual`, `_symbolTraceVisual`, `_surfaceFillVisual`, `_emptyPoint3DCollection` to match exact field layout.
    - Used static inline initialization for `_emptyPoint3DCollection` without explicit static constructor, emitting `beforefieldinit` to match Mathcad IL.
    - Implemented constructor wiring and empty `Dispose()` method.
  - `PlotTraceViewModel3D`:
    - Declared partial struct `PropertyNames` with 10 const string fields.
    - Implemented `_model`, `PropertyChanged` event, and constructor subscription.
- Release IL: PASS — 2791 checked in Plots, 0 failures.
- Remaining mismatches in this batch: none.

## Batch 120 — PlotEquationBase properties and PlotLegend members

- Owner: agy-whiteboard-2. Branch: `feature/agy-plots-120`.
- Members:
  - `PlotEquationBase`: `ContainedEquationControls`, `Expression`, `ScalingEquation` (3 abstract properties)
  - `PlotLegend`: `Columns`, `IPlotLegend.LegendWidth` (unmarked placeholder stub), `SetLegendWidthWhenLayoutReady`, `PlotLegendSizeChanged`, `PlotLegendWidthChanged`, `SetMinMaxWidth(double, double)`, `AddNewItemToPlotLegend`, `PlotLegendItemSizeChanged`, `RemoveTraceFromPlotLegend`, `FindByTrace`, `UpdateDefaultLegendLabelsAfterIndex`, `InitializeComponent`, `_CreateDelegate`, `IComponentConnector.Connect`
- Hand-written:
  - `Mirrored/Controls.Plots.PlotEquationBase.cs`
  - `Mirrored/Controls.Plots.PlotLegend.cs`
- Shapes:
  - `PlotEquationBase`:
    - Declared abstract properties `ContainedEquationControls`, `Expression`, `ScalingEquation` in matching order.
  - `PlotLegend`:
    - Placed XAML-generated fields `PlotLegendGrid` and `_contentLoaded` at bottom of partial class alongside `InitializeComponent`, `_CreateDelegate`, and `Connect` to align closure ordinals `<SetMinMaxWidth>b__37_0` and `<>c__DisplayClass41_0::<FindByTrace>b__0`.
    - Declared `IPlotLegend.LegendWidth` setter as unmarked placeholder stub (`set => throw MirrorStub.NotImplemented();`) to avoid unmapped explicit interface namespace prefix in compiler-generated closure name.
    - Used early-return guard `if (e.PreviousSize.Width.Equals(e.NewSize.Width)) return;` in `PlotLegendSizeChanged` matching Mathcad's `brfalse.s / ret` pattern.
- Release IL: PASS — 2821 checked in Plots, 0 failures.
- Remaining mismatches in this batch: none.

## Batch 121 — PlotEquationWOUnit remaining members

- Owner: agy-whiteboard-2. Branch: `feature/agy-plots`.
- Members:
  - `PlotEquationWOUnit`: `_plotCalculatableError`, `RequestRemove`, `RequestEvaluation`, `_scaling`, `_typeOfThis`, `CalcIsDisabledProperty`, `DisplayNumberFormatProperty`, `.ctor`, `static .cctor`, `OnTraversedFocusEvent`, `BindCommands`, `OnPreviewGotKeyboardFocus`, `HandleNavigateToEquationStart`, `HandleNavigateToEquationEnd`, `HandleBackspace`, `HandleDelete`, `OnRequestEvaluation`, `EquationSExpression`, `SetRegionXmlNode`, `SetExpressionXmlNode`, `SetExpressionSExpression`, `GetRegionXmlNode`, `GetDataObject`, `SetDataObject`, `NavigateRegionEnd`, `NavigateRegionStart`, `Commit()`, `Commit(bool fireEvent)`, `Units`, `ScalingEquation`, `RegionErrorCount`, `FireRequestEvaluation`, `FireRequestRemove`, `DelegateFocus`, `ScalingRl`, `PlotCalculatableError`, `ContainedEquationControls`, `InitializeComponent`, `_CreateDelegate`, `IComponentConnector.Connect`, `IPlotEquation.ActualWidth`, `IPlotEquation.ActualHeight`
- Hand-written:
  - `Mirrored/Controls.Plots.PlotEquationWOUnit.cs`
- Shapes:
  - `PlotEquationWOUnit`:
    - Arranged fields and properties in exact Mathcad order (`_plotCalculatableError`, `RequestRemove`, `RequestEvaluation`, `_scaling`, `_typeOfThis`, `CalcIsDisabledProperty`, `DisplayNumberFormatProperty`, `ActiveEquation`, `_errorFrame`, `_exp`, `_contentLoaded`).
    - Positioned `_errorFrame` before `ContainedEquationControls` to achieve exact compiler-generated iterator ordinal `<get_ContainedEquationControls>d__85`.
    - Used ternary guard `if (!(value == null ? false : value.Count < 1))` in `set_Units` to match Mathcad's `brfalse.s / clt / brtrue.s` instruction sequence.
    - Used non-early-return wrapper `if (_plotCalculatableError != value)` in `set_PlotCalculatableError` to match Mathcad's single exit `ret`.
    - Used `if (AssertionE.Failed == (AssertionE)DebugTools.DevAssert(equationData.math.Length > 0, null)) return;` in `SetDataObject` matching `ldc.i4.1 / cgt.un / bne.un.s / ret`.
    - Implemented explicit interface properties `IPlotEquation.ActualWidth` and `ActualHeight`.
- Release IL: PASS — 2867 checked in Plots, 0 failures.
- Remaining mismatches in this batch: none.

## Batch 122 — EvaluativeEquationControl remaining members

- Owner: agy-whiteboard-2. Branch: `feature/agy-plots`.
- Members:
  - `EvaluativeEquationControl`:
    - 41 exact members: `_expressionTransform`, `_resultTransform`, `_formatter`, `IsUserSetValueProperty`, `EquationProxyProperty`, `RealResultValueProperty`, `IsActiveProperty`, `RequestRemoveEvent`, `ErrorProperty`, `DisplayNumberFormatProperty`, `_equationControl`, `_formattedResult`, `_contentLoaded`, `.cctor`, `.ctor`, `FixCollapsedEqOnLoaded`, `EquationRequestEvaluation`, `EditingEquationChanged`, `FormattedResultChanged`, `add_RequestEvaluation`, `remove_RequestEvaluation`, `EquationXmlNode`, `EquationSExpression`, `UpdateError`, `ResultSExpression`, `Rl`, `IsUserSetValue`, `SetNonUserSetSExpressionWithoutNotification`, `IsCleared`, `IsCommitted`, `OnGotFocus`, `ShouldSerializeProperty`, `EquationProxy`, `RealResultValue`, `UpdateEquationVisibility`, `IsActive`, `Error`, `DisplayNumberFormatChanged(DependencyObject, DependencyPropertyChangedEventArgs)`, `DisplayNumberFormatChanged(string, object)`, `CreateFormatter`, `DisplayNumberFormat`, `FormatResult`, `EquationControlRequestFocus`, `EquationControlRequestRemove`, `InitializeComponent`, `IComponentConnector.Connect`.
    - 5 parked stubs: `IsUserSetValueChanged`, `RealResultValueChanged`, `IsActiveChanged`, `ErrorChanged`, `FontSizeChanged` (parked due to Roslyn 10 dup vs legacy stloc peephole stack optimization).
- Hand-written:
  - `Mirrored/Controls.Plots.EvaluativeEquationControl.cs`
- Shapes:
  - `EvaluativeEquationControl`:
    - Arranged fields and properties in exact Mathcad order.
    - Used early guard `if (AssertionE.Failed == (AssertionE)DebugTools.DevAssert(uIElement != null, null)) return;` in `FixCollapsedEqOnLoaded` and `FormatResult` matching `ldc.i4.1 / cgt.un / bne.un.s / ret`.
    - Used `flag ? Visibility.Visible : Visibility.Collapsed` in `UpdateEquationVisibility` to match Mathcad's `brfalse.s / ldc.i4.0 / br.s / ldc.i4.2` pattern.
    - Used early exit `if (_contentLoaded) return; _contentLoaded = true;` in `InitializeComponent`.
    - Used explicit `string.Format("(eval {0})", _equationControl.Rl)` to prevent C# 10 `DefaultInterpolatedStringHandler` lowering.
- Release IL: PASS — 2908 checked in Plots, 0 failures.
- Remaining mismatches in this batch: none.

## Batch 123 — RealNumberCalculatable remaining members

- Owner: agy-whiteboard-2. Branch: `feature/agy-plots`.
- Members:
  - `RealNumberCalculatable`:
    - `PropertyNames` (`Value`, `ErrorCount`, `ErrorCode`, `ErrorText`, `Error`)
    - Fields: `_equationProxy`, `_value`, `_calculatingStatus`, `_engineError`, `_customErrors`, `_error`, `_hideFromCalculationContext`, `_overrideHideFromCalculation`
    - Properties & Methods: `EquationProxy`, `ProxyRequestEvaluation`, `Value`, `RequestEvaluation`, `GetValueOfIdentifierBefore`, `TreatAsUpToDate`, `Rl`, `Commit`, `SetStatusSynchronized`, `SetCalculatingStatus`, `SetResult`, `FirstCustomError`, `UpdateError`, `SetError`, `ClearEngineError`, `SetIdentifierLabels`, `SetAutoSelectAlgorithm`, `SetRedefinitionWarnings`, `SetCustomError`, `ClearCustomErrors`, `EngineResult`, `InterceptAndUpdateResult`, `UseICalculatableToRestoreResult`, `RestoreResult`, `PropertyChanged`, `OnPropertyChanged`, `CalculatingStatus`, `ErrorCount`, `ErrorSExpression`, `ErrorCode`, `ErrorText`, `IsCommitted`, `Error`, `CanHideFromCalculationContext`, `HideFromCalculationContext`, `ContainsError`, `RequestShowError`, `ShowError`, `CurrentError`, `OverrideHideFromCalculation`, `.ctor`.
- Hand-written:
  - `Mirrored/Controls.Plots.Model.RealNumberCalculatable.cs`
- Shapes:
  - `RealNumberCalculatable`:
    - Mapped `CalculatingStatus.NotComputing` to `CalculatingStatus.Idle` (2) per D20 enum mapping.
    - Preserved exact field ordering and constructor initializers (`_calculatingStatus`, `_customErrors`, `_overrideHideFromCalculation`).
    - Handled `ContainsError` delegating to `Equals(error.ErrorSource)` matching `callvirt Object::Equals(object)`.
    - Formatted `SetResult` branches with early returns matching Mathcad IL `ret` targets.
    - Used `flag |= _customErrors.Remove(item)` in `ClearCustomErrors` foreach loop matching bitwise-OR sequence.
- Release IL: PASS — 2956 checked in Plots, 0 failures.
- Remaining mismatches in this batch: none.

## Batch 124 — PlotCalculatable remaining members

- Owner: agy-whiteboard-2. Branch: `feature/agy-plots`.
- Members:
  - `PlotCalculatable`:
    - `PropertyNames` (`CalculatingStatus`, `Error`, `HideFromCalculationContext`, `SynchronizationStatus`, `IsNumOfPointsMutable`, `NumberOfPoints`, `.ctor`)
    - Fields: `_synchronizationStatus`, `_calculatingStatus`, `_error`, `_hideFromCalculationContext`, `_nodeHandleMapper`, `_numberOfPoints`, `_isNumOfPointsMutable`
    - Events: `PropertyChanged`, `RequestEvaluation`, `GetValueOfIdentifierBefore`, `TreatAsUpToDate`, `PlotCalculatableError`, `Changed`
    - Properties & Methods: `FireRequestEvaluation`, `BeforeRequestEvaluation`, `Rl`, `Commit`, `SetResult`, `SetIdentifierLabels`, `SetAutoSelectAlgorithm`, `SetRedefinitionWarnings`, `CurrentError`, `SynchronizationStatus`, `CalculatingStatus`, `EngineResult`, `Error`, `Result`, `EngineWarnings`, `ErrorCode`, `ErrorText`, `ErrorSExpression`, `CanHideFromCalculationContext`, `HideFromCalculationContext`, `InterceptAndUpdateResult`, `EquationWithError`, `UseICalculatableToRestoreResult`, `ErrorCount`, `IsCommitted`, `NumberOfPoints`, `IsNumOfPointsMutable`, `FirePropertyChanged`, `OnChanged`, `ContainsError`, `SetHideFromCalculationContext`, `RestoreResult`, `SetCalculatingStatus`, `SetError`, `OnTraceEquationSExpressionChanged`, `CleanErrors`, `ClearEquationError`, `PreprocessEquationSpecificError`, `OnEquationSpecificError`, `FindChildWithMatchingId`, `OnPlotCalculatableError`, `IsApplicationCustomError`, `SetStatusSynchronized`, `SetSynchronizationStatus`, `ShowError`, `ClearNodeHandleMapper`, `GetReplacedRl(IEquationControl)`, `GetReplacedRl(IEquationControl, string)`, `AttachOnTraceEquationSExpressionChanged`, `DetachOnTraceEquationSExpressionChanged`, `.ctor`, closure `<>c__DisplayClass56_0`.
- Hand-written:
  - `Mirrored/Controls.Plots.Model.PlotCalculatable.cs`
- Shapes:
  - `PlotCalculatable`:
    - Mapped `CalculatingStatus.NotComputing` to `CalculatingStatus.Idle` (2) per D20 enum mapping.
    - Repositioned uninitialized fields `_numberOfPoints` and `_isNumOfPointsMutable` after `IsCommitted` and before `NumberOfPoints`, aligning syntax ordinal 56 for `HideFromCalculationContext` (`DisplayClass56_0`).
    - Used `Action<ICalculatable> treatAsUpToDate = TreatAsUpToDate; if (treatAsUpToDate != null) treatAsUpToDate(this);` in `SetStatusSynchronized` to emit `stloc.0 / ldloc.0` instead of `dup`.
    - Formatted `OnPlotCalculatableError`, `SetIdentifierLabels`, `SetError`, and `ClearEquationError` matching exact Mathcad Release IL branch structures.
- Release IL: PASS — 3017 checked in Plots, 0 failures.
- Remaining mismatches in this batch: none.

## Batch 125 — PlotEquation remaining members

- Owner: agy-whiteboard-2. Branch: `feature/agy-plots`.
- Members:
  - `PlotEquation`:
    - Fields: `_navigateToEquationStart`, `_errorFrame`, `_viewModel`, `_propertyChangedNotificationHandler`, `_scalingHeaderVisibilityPropertyKey`, `ScalingHeaderVisibilityProperty`, `_typeOfThis`, `CalcIsDisabledProperty`, `DisplayNumberFormatProperty`, `EmptySExpression`, `_exp`, `_scaling`, `_contentLoaded`
    - Events: `RequestRemove`, `RequestEvaluation`, `ScalingChanged`
    - Constructors: `.ctor` (with lambda `<.ctor>b__12_0`), `.cctor`
    - Properties & Methods: `ComputeBaseLine`, `MeasureOverride`, `ContainedEquationControls` (iterator `<get_ContainedEquationControls>d__15`), `OnSubEquationExpressionChanged` (display class `<>c__DisplayClass16_0` and `<OnSubEquationExpressionChanged>b__16_1`), `PlotCalculatableErrorChanged`, `OnTraversedFocusEvent`, `ScalingHeaderVisibility`, `BindCommands`, `CanNavigateRight`, `CanNavigateLeft`, `OnNavigateRight`, `OnNavigateLeft`, `OnPreviewGotKeyboardFocus`, `HandleNavigateToEquationStart`, `HandleNavigateToEquationEnd`, `HandleBackspace`, `HandleDelete`, `CanNavigate`, `NavigateToExp`, `NavigateToScaling`, `ShowOrHideScaling`, `ShowScaling`, `HideScaling`, `ShowScalingHeader`, `HideScalingHeader`, `OnRequestEvaluation`, `FormatPlotEquationSExpression`, `ScalingRl`, `EquationSExpression`, `Exp`, `Expression`, `Scaling`, `ScalingItem`, `IsScalingEmpty`, `IsExpEmpty`, `IsPlotEquationEmpty`, `ScalingChangedByUser`, `UserScalingExpression`, `EmptyPlotEquationSExp`, `SetDataObject`, `SetRegionXmlNode`, `SetScalingXmlNode`, `SetScalingSExpression`, `SetExpressionXmlNode`, `SetExpressionSExpression`, `GetRegionXmlNode`, `GetUserScalingExpression`, `GetExpXml`, `AtLeftEdge`, `RegionErrorSExp`, `RegionErrorCode`, `ViewModel`, `PlotCalculatableError`, `TextBaselineOffset`, `ActiveEquation`, `FocusedEquation`, `SynchronizationStatus`, `CalculatingStatus`, `ScalingEquation`, `FireScalingChanged`, `Units`, `RegionErrorCount`, `ExpressionItem`, `DisplayNumberFormat`, `Commit`, `Commit(bool)`, `CommitEquationAvoidingEvents`, `FireRequestEvaluation`, `FireRequestRemove`, `DelegateFocus`, `OnRequestFocus`, `MergeScalingExpression`, `MergeUnitMonomial`, `GetDataObject`, `NavigateRegionEnd`, `NavigateRegionStart`, `InitializeComponent`, `_CreateDelegate`, `IComponentConnector.Connect`, `IPlotEquation.ActualWidth`, `IPlotEquation.ActualHeight`.
- Hand-written:
  - `Mirrored/Controls.Plots.PlotEquation.cs`
- Shapes:
  - `PlotEquation`:
    - Aligned member syntax ordinals to match Mathcad's `<.ctor>b__12_0`, `<get_ContainedEquationControls>d__15`, and `<>c__DisplayClass16_0` by placing the 10 fields and `ComputeBaseLine`/`MeasureOverride` before `.ctor`, and moving events `RequestRemove`/`RequestEvaluation` out of the class header.
    - Used pattern matching `(new Regex(...) switch { var r => r }).Match(value)` in `set_EquationSExpression` to preserve `callvirt` on `Regex.Match` without allocating a local slot.
    - Used `if (equationData.math.Length < 2) return;` in `SetDataObject` to match Mathcad's inverted branch `bge.s`.
    - Used local variable assignment `IEquationControl result` in `GetUserScalingExpression` to match Mathcad's `stloc.0 / ldloc.0 / ret` shape.
    - Used `if (IsVisible) return;` and explicit `PlotControl plotControl = ...; if (plotControl != null)` in `OnRequestFocus` to match Mathcad's `stloc.0 / ldloc.0` and branch structure.
- Release IL: PASS — 3109 checked in Plots, 0 failures.
- Remaining mismatches in this batch: none.

## Batch 126 — ContourPlotControl, PlotLegendItem, AxisLimitEditor, AxisModel remaining members

- Owner: agy-contour. Branch: `feature/agy-contour`.
- Members:
  - `ContourPlotControl`:
    - Fields/constants & static `.cctor`: `_typeOfThis`, `LabelFormatterProperty`, `_engineResults`, `_changedEvent`, `TraceLineStyleProperty`, `TraceLineWeightProperty`, `TraceColorProperty`, `ColorSchemeProperty`, `ShowContourValuesProperty`, `ToggleShowContourValuesCommand`, `SetColorSchemeCommand`, `NumberOfPoints_Min`, `NumberOfPoints_Max`, `_content`, `_background`, `plotGrid`, `_contourLines`, `PART_ySplitter`, `PART_xSplitter`, `_yAxis`, `_yEquation`, `_contourPlot`, `plotLegend`, `_xAxis`, `_xEquation`, `_zAxis`, `_zEquation`, `_errorFrame`, `_contentLoaded`.
    - Constructors & properties: `.ctor` (with `<.ctor>b__13_0`, `<.ctor>b__13_1`), `ViewModel`, `EngineResults`, `LabelFormatter`, `ChangedEvent`, `LastChildItem`, `ColorScheme`, `TraceLineStyle`, `TraceLineWeight`, `TraceColor`, `ShowContourValues`, `StateForXamlSerialization`, `HideFromCalculationContext`, `XAxis`, `YAxis`, `ZAxis`, `SubElementToResizeData`, `PreserveAspectRatio`, `ErrorFrameShouldBeVisible`, `NumberOfPointsMin`, `NumberOfPointsMax`, `CalculatableInfosWithErrors`, and explicit `IPlotControl<XyzEquation>` properties (`Equation`, `Plot`, `ErrorFrame`, `PlotGrid`).
    - Methods & events: `Dispose`, `CanvasMeasureOverride`, `CanvasArrangeOverride`, `OnGridIsResizingChanged`, `OnPropertyChanged`, `AxisIsKeyboardFocusWithinChanged`, `ZAxisNumberFormatChanged`, `ViewModelPropertyChanged`, `ReadDataOnDeserializationEnd`, `ViewModelModelChanged`, `SetBindingsForVisualProperties`, `EndInit`, `AddEquations`, `GetPlotEquations` (`<GetPlotEquations>d__49`), `DelegateFocus`, `BindCommands`, `OnBackgroundTypeChanged`, `ChangeOriginPositioning`, `ShowContourValuesExecute`, `ZEquation_OnIsKeyboardFocusWithinChanged`, `HandleLostFocus`, `IsActiveRegionChanged`, `BuildEquationList`, `Commit`, `SetRegionPersistentData`, `GetRegionPersistentData`, `ZEquation_ScalingChanged`, `InitializeComponent`, `_CreateDelegate`, `IComponentConnector.Connect`.
  - `PlotLegendItem`: `InitializeComponent`, `IComponentConnector.Connect`.
  - `AxisLimitEditor`: `HideFromCalculationContext` (`<>c__DisplayClass79_0`, exact member order in `Mirrored/Controls.Plots.AxisLimitEditor.cs`).
  - `AxisModel`: `PropertyNames`, fields & events (`LogarithmicStartDefault`, `LogarithmicAxisStep`, `TickMarkNodeName`, `_equations`, `_markers`, `_startExpression`, `_secondTickExpression`, `_endExpression`, `_scaling`, `_start`, `_end`, `_showNumbers`, `ErrorIndicator`, `_showGridLines`, `_gridLineColorHex`, `_displayNumberFormat`, `_isLinearizedRangeAssertEnabled`, `RequestAddEquation`, `Changed`, `RangeChanged`, `ScalingChanged`, `NumberFormatInitialized`, `PropertyChanged`, `TickMarkLabelsUpdated`), `.ctor`, `AxisCalculatables` (`<get_AxisCalculatables>d__41`), `CalculateBetterTickmarks`, `CalculateBetterTickmarksWithZeroInRange`.
- Hand-written:
  - `Mirrored/Controls.Plots.Contour.View.ContourPlotControl.cs`
  - `Mirrored/Controls.Plots.PlotLegendItem.cs`
  - `Mirrored/Controls.Plots.AxisLimitEditor.cs`
  - `Mirrored/Controls.Plots.Model.AxisModel.cs`
- Shapes:
  - `ContourPlotControl`: placed `Dispose`, `CalculatableInfosWithErrors`, `CanvasMeasureOverride`, `CanvasArrangeOverride`, and explicit `IPlotControl<XyzEquation>` properties before `.ctor` so `.ctor` lambdas take `<.ctor>b__13_*` and `GetPlotEquations` emits `<GetPlotEquations>d__49`; used tuple deconstruction `(zBaselineOffset, zEquationSize) = (...)` in `CanvasMeasureOverride` to match slot order and inline branch structure.
  - `AxisLimitEditor`: ordered all fields/events/properties/methods in Mathcad order so `HideFromCalculationContext` closure emits `<>c__DisplayClass79_0`.
  - `AxisModel`: used `(start < 0.0 && end > 0.0) is true` in `CalculateBetterTickmarks` to materialize bool before `brfalse.s` without adding a bool local; kept `SetXmlNode` as an unmarked placeholder stub after `CalculateBetterTickmarksWithZeroInRange` (Roslyn 10 `dup` vs legacy `stloc` on `domainNode`) to preserve `AxisCalculatables` `d__41`, `SetRange` 170, and `GetNumberOfTracesForEquation` 213 ordinals.
  - `EvaluativeEquationControl`: 5 dependency-property callbacks remain parked in `EvaluativeEquationControl.g.cs` per Batch 122 (Roslyn 10 `dup` vs legacy `stloc`).
- Release IL: PASS; 4302 checked; 0 failures.
- Remaining parked stubs in assigned types: `AxisModel.SetXmlNode`, `EvaluativeEquationControl.{IsUserSetValueChanged,RealResultValueChanged,IsActiveChanged,ErrorChanged,FontSizeChanged}`.

## Batch 127 — PlotGraphAxis (haiku worker, branch `feature/haiku-plot-axis2`)

- Owner: Orca task `task_4099d2e1085e`. Hand-written: `Mirrored/Controls.Plots.PlotGraphAxis.cs` (whole class in Mathcad IL order); `Point2ScreenHelper.Contains` stub removed from `Generated/Mirror/Controls.Plots.PlotGraphAxis.g.cs`.
- Members (about 50): static `.cctor` (all DependencyProperty registrations); `.ctor`; DP-backed accessors (`Orientation`, `Start`, `End`, `Labels`, `TickMarksValues`, `IsDiscrete`, `DisplayNumberFormat`, `Offset` setter, `Scaling`, `PositionTickMark`, `PositionValue`, `HasOriginPositioning`); static and instance callbacks (`PositionTickMarkChanged`, `ShowNumbersPropertyChanged` x2, `CreateOrientationPropertyMetadata`, `RegisterHandleTickmarksFormattingChanged`); `LinearizeStartEnd`, `GetAxisEndPosition`, `CreateDoubleAnimation`, `ClearDisplayNumberFormatWithCommand`, `PositionToTickNumber`, `CoerceScreenValue`, `GetPlotAxis`, `SetPlotAxis`, `HandleAncestorLostFocus`, `GetVisibilityBinding`, `LimitExpressionEqnPreviewGotKeyboardFocus`, `BindCommands`, `HookToViewModel`, `UnhookFromViewModel`, `ActivateAxisExpression`, `ActivateSecondOrBoundaryTick`, the three `*NavigateOffOfEquation` methods, `GetVisualChild`, `VisualChildrenCount`, `MeasureOverride`, `this[int]`, `OtherAxis`, `AxisTitle` getter, `IsLinearizedRangeAssertEnabled` getter, `CollectElementsForTraversal` (iterator), `LegendWidthChanged` add/remove.
- Shapes:
  - Closure ordinals: `_typeOfThis` must be declared after the `set_ShowNumbers` setter (its earlier position shifted the display classes of `set_ShowTicks`..`set_ShowNumbers` by one). `_pendingWidth` must sit between `CollectElementsForTraversal` and `set_PositionTickMark`.
  - Static constructor placed at token `0x0600047c` (after `get_LinearizedEnd`). Method-group caches use `<>O` and match once `.ctor` binds `LimitExpressionEqnPreviewGotKeyboardFocus`.
  - `GetPlotAxis` calls `HasLocalValue` on `axisView as DependencyObject`; the `is ... ? ... : null` cast form emitted an extra `castclass`.
- Release IL (`release_il.py --base main --project Quire.Controls.Plots --type Quire.Controls.Plots.PlotGraphAxis`): 4494 checked; no new failures against main; 53 base failures fixed; 5 remaining in `PlotGraphAxis`, all unchanged placeholders.
- Remaining mismatches / parked:
  - `IPlotAxisView.LegendActualWidth` setter: closure name is `<Quire.Controls.Plots.Presenter.IPlotAxisView.set_LegendActualWidth>b__0`; Mathcad's is `<Ptc.Controls.Plots...>`. Gate name mapping does not rewrite lambda names inside explicit-interface members. Parked (placeholder).
  - `AxisTitle` setter and `IsLinearizedRangeAssertEnabled` setter: Mathcad uses `bne.un.s` over a `br.s` for the reference and nullable `!=`; Roslyn emits `beq.s`/`brtrue.s`, and `if (a == b) { } else { ... }` gives the same IL. Parked (placeholders).
  - `GetMajorTickmaks`: the `||` condition chain is 6 bytes shorter than Mathcad's. Parked (placeholder).
  - Auto-property placeholders (`MirrorBlocked`) are unchanged.
