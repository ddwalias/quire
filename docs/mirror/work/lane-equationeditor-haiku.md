# Quire.EquationEditor lane checkpoint (haiku takeover)

- Owner: EquationEditor worker (Claude, Haiku 5.5), taking over the lost Antigravity lane
  `lane-quire-equationeditor` (its uncommitted edits in that worktree were left untouched).
- Branch: `feature/claude-equationeditor-haiku` (worktree `claude-equationeditor-haiku`, created from `main`).
- Assembly: `Quire.EquationEditor` (managed members, `src/Quire.EquationEditor`).
- Commands: `python tools/validation/release_il.py --base main --project Quire.EquationEditor`
  (Release only); landing via `python C:/Projects/Quire/tools/validation/lane_merge.py <branch>`.

## Batch 10 — constant fields (MirrorStub consts moved to hand-written partials)

- Members (constants, values checked against Release IL literals):
  - `EquationErrors.MaxRecursionDepthExceeded` (in `EquationErrors.cs`)
  - `FunctionOperatorNames.Root`, `FunctionOperatorNames.If`
  - `ButtonScriptableControlData.a` = 40, `b` = 70
  - `CheckBoxScriptableControlData.a` = 30, `b` = 100
  - `RadioButtonScriptableControlData.a` = 72, `b` = 80
  - `SliderScriptableControlData.a` = 37, `b` = 100
  - `TextBoxScriptableControlData.a` = 30, `b` = 100
  - `InlineToSExpressionTransform.a_` (string), `b`, `c`, `d` (chars)
- Stubs removed from the matching `Generated/Mirror/*.g.cs` (attribute and declaration only).
- Release IL report: `--project Quire.EquationEditor` PASS, 975 checked, 0 failures.
  The gate does not inventory constant fields, so these 18 const stubs are not counted in "checked".
- Remaining mismatches: 0 in checked members.

## Remaining work (not started in this checkpoint)

- `AlgorithmOptions`: the consts (`MultiStart`, `Optimize`, `OptionSeparator`) and the `a_` field and
  `IsMultiStart` property are still stubs. The existing hand-written partial already holds `IsOptimize`
  (backing `b`), the copy constructor, `ToggleMultiStart`, `ToggleOptimize` and `Clone`, but
  Mathcad's `ICloneable` member is an obfuscated `a()` (parked, see Batch 8 in the older checkpoint).
- Parked (exact blockers, see `lane-quire-equationeditor.md` Batches 1-9): `Native.PtcEquationEditor`
  obfuscated types with CS0102 field/method clashes (e.g. `r.a`, `w.a`, `x.a`, `bs`, `v`);
  `RootMathNode` constructor chain (`r.a.c()`); `NavigatorState.ToString()` (calls `bs.a`/`bs.b`);
  `Properties.Settings` and `MathNodeSettings` (obfuscated getters `a()`/`b()`);
  `ValueNodeInfo`, `ScopedNodes`, `TransformMathNodeToComboBoxValues` closure classes `a`/`b`.
- Remaining stub count: the gate reports 2,769 generated-stub members (unchanged by this batch, since
  constant fields are not inventoried); 4,119 `[MirrorStub]` attributes remain in `Generated/Mirror` after
  this batch (this batch removed 18 const attributes).
