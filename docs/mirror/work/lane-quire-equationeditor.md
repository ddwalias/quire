# Quire.EquationEditor lane checkpoint

- Owner: EquationEditor dispatched worker (task_4a851705d6ac)
- Branch: `feature/equation-editor-stubs-batch4`
- Assembly: `Quire.EquationEditor` (managed members, `src/Quire.EquationEditor`)

## Batch 1 — Native.PtcEquationEditor members (a7, o, bf, au, az, d, a)

- Landed members:
  - `Native.PtcEquationEditor.a7`: `CleanValue(string)`, `.ctor()` (0 stubs remaining).
  - `Native.PtcEquationEditor.o`: `a(this AlgorithmOptions, params string[])`, `a(this MathNode, string)` (0 stubs remaining).
  - `Native.PtcEquationEditor.bf`: `.ctor(a5)`, `b()`, `a()`, `br()`, `bq()` (0 stubs remaining).
  - `Native.PtcEquationEditor.au`: `au(MathNode, Panacea)`, fields `a`, `b` (0 stubs remaining).
  - `Native.PtcEquationEditor.az`: `az()`, `az(IEnumerable<Rule>)`, `c(string, int)`, `a(int)`, `b(string, int)`, `a(string, int)` (0 stubs remaining).
  - `Native.PtcEquationEditor.d`: `a(IMathNodeToVisualNodeMap, IVisualMathNode, Direction, Direction, int?, double, double)`, `a(MathNode)`, `a(IVisualTextNode, MathNode, double)`, `a(IVisualIdNode, MathNode, double)`, `a(IList<double>, double)`, `.ctor()` (0 stubs remaining).
  - `Native.PtcEquationEditor.a`: `.ctor()` (const fields retain stubs per D15/rule).
- Landed commit: `fa69d61d3fcdaa6010477a4842011fdf35f8628b`
- Report: full `--project Quire.EquationEditor` gate PASS, 862 checked, 0 failures.
- Remaining mismatches: 0 in completed types; remaining unbatched stubs in other types.

## Batch 2 — Native.PtcEquationEditor.f and Tree ScriptableControlData types (Clone and .ctor)

- Members:
  - `Native.PtcEquationEditor.f`: `.ctor()` (b initialized, Greek character mapping, [SkipLocalsInit])
  - `EquationEditor.Tree.ButtonScriptableControlData`: `Clone()` (base calls, [SkipLocalsInit])
  - `EquationEditor.Tree.CheckBoxScriptableControlData`: `Clone()` (base calls, FlowDirection, [SkipLocalsInit])
  - `EquationEditor.Tree.InputScriptableControlDataWithInputs`: `Clone()` (base and instance calls, [SkipLocalsInit])
  - `EquationEditor.Tree.ListBoxScriptableControlData`: `Clone()` (base calls, Sort, FlowDirection, [SkipLocalsInit])
  - `EquationEditor.Tree.RadioButtonScriptableControlData`: `Clone()` (base calls, Orientation, FlowDirection, [SkipLocalsInit])
  - `EquationEditor.Tree.SliderScriptableControlData`: `.ctor()` (field initializers, [SkipLocalsInit]), `Clone()` (base calls, Orientation, TickPlacement, etc., [SkipLocalsInit])
  - `EquationEditor.Tree.TextBoxScriptableControlData`: `Clone()` (base calls, AcceptsReturn, IsReadOnly, Border, TextAlignment, [SkipLocalsInit])
- Landed commit: `fd411405927fc1c722571e760570b092e36c2c1c`
- Report: full `--project Quire.EquationEditor` gate PASS, 871 checked, 0 failures.
- Remaining mismatches: 0 in completed types; remaining unbatched stubs in other types.

## Batch 3 — InputControlData, InputControlDataCalculatedValue, DetachedTypeoverState

- Members:
  - `EquationEditor.Tree.Scoping.DetachedTypeoverState`: `.ctor(TypeoverState)` ([SkipLocalsInit])
  - `EquationEditor.Tree.InputControlDataCalculatedValue`: `HasCalculatedValue`, unvirtualized property implementations for non-virtual call dispatch matching Mathcad IL
  - `EquationEditor.Tree.InputControlData`: `protected InputControlData()` constructor with color/size initializers and abstract `Clone()`, `IsEquivalentTo(IInputControlData)`
- Landed commit: `254b737790a754ab67e318dea246f60e7ca51ea6`
- Report: full `--project Quire.EquationEditor` gate PASS, 874 checked, 0 failures.
- Parked members (exact blockers):
  - `Native.PtcEquationEditor` obfuscated types (`a-z`, `ao`, `ae`, `br`, `q`, `s`, `r`) and dependent callers (`ComboBoxMathCreator.CreateUnitMath`, `InputControlNodeCreator`): Field/method name clash blocker. MirrorGen emits `_` suffix on fields that share names with methods/properties in the same type to satisfy C# CS0102, causing token mismatch in MethodAudit IL comparison.
  - `NearestNeighborSelector.SelectNeighbors`: Switch temporary local allocated at slot 0 in Roslyn vs slot 3 in Mathcad IL; parked pending maintainer local-ordering solution.
- Remaining mismatches: 0 in completed types; remaining unbatched/parked stubs in other types.

## Batch 4 — MathNodeDescriptorPiggyback, SpecTableCellMap, InlineToSExpressionTransform, NavigatorState

- Members:
  - `EquationEditor.Tree.MathNodeDescriptorPiggyback`: `b4` field (0 stubs remaining).
  - `EquationEditor.SpecTable.SpecTableCellMap`: `a`, `b` fields, `.ctor()` (0 stubs remaining).
  - `EquationEditor.Tree.Transform.InlineToSExpressionTransform`: `e` dictionary field, `.ctor()` with `[SkipLocalsInit]`.
  - `EquationEditor.Tree.Navigation.NavigatorState`: `Current`, `CaretSide`, `NodeSide`, `OperatorPartIndex`, `EnterSide`, `ValueRange`, `CaretInsertionType` fields, and the 2 `.ctor` overloads (with `valueRange` branch matching Mathcad IL).
- Report: full `--project Quire.EquationEditor` gate PASS, 878 checked, 0 failures.
- Parked members (exact blockers):
  - `Native.PtcEquationEditor` obfuscated types (`a-z`, `ao`, `ae`, `br`, `q`, `s`, `r`, `bs`) and dependent types/callers (`ComboBoxMathCreator`, `InputControlNodeCreator`, `MatrixNodeInfo`, `RedefinitionWarning`, `InputControlNodeInfo`, `AlgorithmOptions`, `SExpressionUtility`, `NavigatorState.ToString`): Field/method name clash blocker (C# CS0102) and internal helper mismatch.
- Remaining mismatches: 0 in completed types; remaining unbatched/parked stubs in other types.
