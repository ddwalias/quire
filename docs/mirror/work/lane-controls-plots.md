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
