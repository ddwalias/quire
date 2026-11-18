# Quire.EquationEditor (Half A) lane checkpoint

- Owner: EquationEditor dispatched worker (task_572f7cc4f5ea)
- Branch: `feature/eqeditor-a-20261119`
- Assembly: `Quire.EquationEditor` (managed members, `src/Quire.EquationEditor`, Half A: files <= `Native.PtcEquationEditor.a2.g.cs`)

## Batch 1 — Settings and SExpressionUtility constructors

- Landed members (commit `4a5dac266c`):
  - `Quire.EquationEditor.MathNodeSettings`: `.ctor()`, `.cctor()` (Instance)
  - `Quire.EquationEditor.Properties.Settings`: `.ctor()`, `.cctor()` (field `a`)
  - `Quire.EquationEditor.Tree.Transform.SExpressionUtility`: `.ctor()`
- Report: Release IL match for all changed members (Receipt: `gate-il-1777a7bd4d29.json`).

## Batch 2 — MathNodeNames constants and static constructor

- Implemented members:
  - `Quire.EquationEditor.Tree.MathNodeNames`: 98 `const string` fields and `.cctor()` (`AllLineBreakNodeNamesList` initialization).
  - 99 `[MirrorStub]` declarations removed.
- Report: Release IL match for all changed members (`release-il-64f2f5ee8405`).
- Remaining mismatches: 0 in changed members; remaining types in Half A parked with exact blockers below.

## Parked members (exact blockers)
- `Native.PtcEquationEditor` obfuscated types (`a-z`, `a0`, `a2`, etc.): CS0102 clashes between fields and methods (e.g. `a_` vs `a(...)`).
- `Properties.Settings` and `MathNodeSettings`: `AutoLabelIds` and `Default` property accessors obfuscated in Mathcad (`a()`, `b()`).
- `SExpressionUtility`: `EscapeIdValue` and `CleanWhitespace` blocked by CS0102 clash on field `a` (`a_`) vs method `a(string)`.
- `MathNode`: Remaining stubs touching clashed fields `a__`, `b__`, `c__`, `d__`, `e__`, `h__`, `i__`, `j_` due to CS0102 field/method clashes.
- `RootMathNode.ctor()`: Constructor chaining calls `Native.PtcEquationEditor.r.a.c()`; `r.a` has CS0102 clash.
- `SpecTableColumnToRlTransform.Transform`: References `MathNodeToRlTransformer.e` (CS0102 clash).
- `InputControlNodeCreator`: References `r.a` and `r.d` (CS0102 clash).
- `ValueNodeInfo`, `TransformMathNodeToComboBoxValues`, `ScopedNodes`, `ParsedSExpressionToInlineTransform`, `SExpressionToRlTransform`, `ComboBoxInputControlDataToSExpressionTransform`, `SpecTableMemento`: Compiler-generated closure classes have CS0102 clashes between static fields `a` (`a_`/`a__`) and methods `a`.
- `NavigatorState.ToString()`: Uses C# 12 inline array / span helpers `bs.a` / `bs.b`.
- `MathNodeToVisualNodeMap`, `VisualOperatorDescriptorTable`, `StringToComboBoxMathTransformer`, `AlgorithmOptions`: CS0102 clashes on obfuscated fields/methods.
- `ColumnMemento`: References `Native.PtcEquationEditor.k` which has CS0102 clashes.
- `ComboBoxMathCreator`: References `Native.PtcEquationEditor.r` which has CS0102 clashes.
