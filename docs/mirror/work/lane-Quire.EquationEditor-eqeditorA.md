# Quire.EquationEditor (Half A) lane checkpoint

- Owner: EquationEditor dispatched worker (task_572f7cc4f5ea)
- Branch: `feature/eqeditor-a-20261119`
- Assembly: `Quire.EquationEditor` (managed members, `src/Quire.EquationEditor`, Half A: files <= `Native.PtcEquationEditor.a2.g.cs`)

## Batch 1 — Settings and SExpressionUtility constructors

- Landed members:
  - `Quire.EquationEditor.MathNodeSettings`: `.ctor()`, `.cctor()` (Instance)
  - `Quire.EquationEditor.Properties.Settings`: `.ctor()`, `.cctor()` (field `a`)
  - `Quire.EquationEditor.Tree.Transform.SExpressionUtility`: `.ctor()`
- Report: Release IL match for all changed members.
- Parked members (exact blockers):
  - `Native.PtcEquationEditor` obfuscated types (`a-z`, `a0`, `a2`, etc.): CS0102 clashes between fields and methods (e.g. `a_` vs `a(...)`).
  - `Properties.Settings` and `MathNodeSettings`: `AutoLabelIds` and `Default` property accessors obfuscated in Mathcad (`a()`, `b()`).
  - `SExpressionUtility`: `EscapeIdValue` blocked by CS0102 clash on field `a_` vs method `a(string)`.
  - `MathNode`: Remaining stubs touching clashed fields `a__`, `b__`, `c__`, `d__`, `e__`, `h__`, `i__`, `j_` due to CS0102 field/method clashes.
  - `RootMathNode.ctor()`: Constructor chaining calls `Native.PtcEquationEditor.r.a.c()`; `r.a` has CS0102 clash.
  - `SpecTableColumnToRlTransform.Transform`: References `MathNodeToRlTransformer.e` (CS0102 clash).
  - `InputControlNodeCreator`: References `r.a` and `r.d` (CS0102 clash).
  - `ValueNodeInfo`: Compiler-generated closure classes have CS0102 clashes.
  - `TransformMathNodeToComboBoxValues`: Closure class CS0102 clash.
  - `NavigatorState.ToString()`: Uses C# 12 inline array / span helpers `bs.a` / `bs.b`.
  - `MathNodeToVisualNodeMap`, `VisualOperatorDescriptorTable`, `StringToComboBoxMathTransformer`: CS0102 clashes.
  - `ScopedNodes`: Closure classes CS0102 clashes.
  - `MathNodeNames`: Constants without method bodies.
