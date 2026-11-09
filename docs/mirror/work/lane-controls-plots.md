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
