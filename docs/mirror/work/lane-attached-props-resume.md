# Attached properties restart (Quire.Controls.Core)

- Owner: Claude worker, Orca task `task_b13d937366a8`; branch `mirror/attached-props-restart-claude-20261117`, based on `main` (`6b6e50ead`).
- Members: `Controls.Core.AttachedProperties` (`.cctor` and `OnCanActivateChanged`, with the field order that fixes its closure ordinal) and `Controls.Core.MathFormatAttachedProperties` (static fields, `.cctor`, `HandleMathFormatChanged`).
- Release IL report: `Quire.Controls.Core`, `release_il.py --base main --project Quire.Controls.Core`: PASS, 3228 checked, 3228 exact, 0 failures, 146 unfinished (`.mirror-cache/release-il-08d84c0cc712/release-il.json`).
- Remaining mismatches (parked, kept as stubs in the hand-written file with `[MirrorStub]`):
  - `AttachedProperties.SetRegisterCommandBindings`, `GetRegisterCommandBindings`, `SetIsBringToView`, `GetIsBringToView`: Mathcad calls `DependencyObject.SetValue`/`GetValue` with `call` on a guarded parameter. Roslyn emits `callvirt` for every instance call on a reference-type parameter, and the earlier worker's two shapes (direct receiver and cast receiver) both failed the same way.
  - `MathFormatAttachedProperties.CoerceFontFamily`: Mathcad calls `Typeface.TryGetGlyphTypeface` with `callvirt` on a new `Typeface`. Roslyn emits `call` for an object-creation receiver, and the natural form failed the gate.
  - `MathFormatAttachedProperties.CoerceFontSize`: Mathcad branches with `blt` (jump when less than the minimum). The `if`-return form I tried compiles to `bge.un`. I tried one idiomatic form only.
- Closure note: `OnCanActivateChanged` needs its captured `newValue` declared at method scope so the display class is created on entry. Members declared after it (`SupportsInputsOutputsProperty` through `WarningDisplayFilterProperty`) sit below it, and that order is what makes its ordinal 32.
- Not done here: the old branch `feature/controls-attached-properties-stubs` (commit `6675f5267`) is untouched and remains a record only.
