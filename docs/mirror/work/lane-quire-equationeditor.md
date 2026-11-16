# Quire.EquationEditor lane checkpoint

- Owner: EquationEditor dispatched worker (task_ba9357d5edf4)
- Branch: `feature/equation-editor-stubs-batch7`
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
- Landed commit: `666535f5bae3f2034bb0deb49062dc005e464fe7`
- Report: full `--project Quire.EquationEditor` gate PASS, 878 checked, 0 failures.
- Parked members (exact blockers):
  - `Native.PtcEquationEditor` obfuscated types (`a-z`, `ao`, `ae`, `br`, `q`, `s`, `r`, `bs`) and dependent types/callers (`ComboBoxMathCreator`, `InputControlNodeCreator`, `MatrixNodeInfo`, `RedefinitionWarning`, `InputControlNodeInfo`, `AlgorithmOptions`, `SExpressionUtility`, `NavigatorState.ToString`): Field/method name clash blocker (C# CS0102) and internal helper mismatch.
- Remaining mismatches: 0 in completed types; remaining unbatched/parked stubs in other types.

## Batch 5 — Native.PtcEquationEditor (aq, as), ScriptingError, NearestNeighborSelector

- Members:
  - `Native.PtcEquationEditor.aq`: fields `a`, `b`, `c`, `d = true`, `e = true`, `f`, `.ctor()` (0 stubs remaining).
  - `Native.PtcEquationEditor.as`: all 8 static methods `a(Direction, Fixity)`, `a(Direction, LinkedListNode<MathNodeAnalysisHandle>)`, `b(LinkedListNode<MathNodeAnalysisHandle>)`, `b(Fixity)`, `b(IMathNodeDescriptor)`, `a(IMathNodeDescriptor)`, `a(LinkedListNode<MathNodeAnalysisHandle>)`, `a(Fixity)` (0 stubs remaining).
  - `EquationEditor.ScriptingError`: explicit interface implementation of `IEngineError.Symbol` (0 stubs remaining).
  - `EquationEditor.Tree.NearestNeighborSelector`: `.ctor()` and `SelectNeighbors(MathNode, out bool)` (resolved local variable slot ordering by pre-declaring locals before switch) (0 stubs remaining).
- Landed commit: `1be4b4517`
- Report: full `--project Quire.EquationEditor` gate PASS, 889 checked, 0 failures.
- Parked members (exact blockers):
  - `Native.PtcEquationEditor` obfuscated types (`a-z`, `ao`, `ae`, `br`, `q`, `s`, `r`, `bs`, `a3`, `am`, `an`, `av`, `ba`, `bb`, `be`, `bg`, `bp`) and dependent callers: Field/method name clash blocker (CS0102).
- Remaining mismatches: 0 in completed types; remaining unbatched/parked stubs in other types.

## Batch 6 — Native.PtcEquationEditor result node hierarchy (t, u, z, aa, y)

- Members:
  - `Native.PtcEquationEditor.t`: abstract methods `bo`, `bp`, `Accept`, explicit `ICloneable.Clone()`, `protected .ctor()`, product-only `IResultNode.ValueType` (0 stubs remaining).
  - `Native.PtcEquationEditor.u`: `bo`, `bp`, `Accept`, `.ctor()`, product-only `IResultNode.ValueType` (0 stubs remaining).
  - `Native.PtcEquationEditor.z`: `Value` property and backing field `a`, `bo`, `bp`, `Accept`, `.ctor()`, product-only `IResultNode.ValueType` (0 stubs remaining).
  - `Native.PtcEquationEditor.aa`: `Description` property and backing field `a`, `bo`, `bp`, `Accept`, `.ctor()`, product-only `IFunctionResultNode.Signature`, `IResultNode.ValueType` (0 stubs remaining).
  - `Native.PtcEquationEditor.y`: `RValue` and `IValue` properties and backing fields `a`, `b`, `bo`, `bp`, `Accept`, `.ctor()`, product-only `IResultNode.ValueType` (0 stubs remaining).
- Landed commit: `73ac163c0`
- Report: full `--project Quire.EquationEditor` gate PASS, 915 checked, 0 failures.
- Parked members (exact blockers):
  - `Native.PtcEquationEditor.w`, `x`: static method `a(w)` / `a(x)` vs field `a` clash (CS0102).
  - `Native.PtcEquationEditor` obfuscated types (`a-z`, `ao`, `ae`, `br`, `q`, `s`, `r`, `bs`, `a3`, `am`, `an`, `av`, `ba`, `bb`, `be`, `bg`, `bp`) and dependent callers: Field/method name clash blocker (CS0102).
- Remaining mismatches: 0 in completed types; remaining unbatched/parked stubs in other types.

## Batch 7 — EquationEditor.Tree.MathNode.Utilities (InsertAsParent, ReplaceNode, IsMiddleOfSubscript)

- Members:
  - `EquationEditor.Tree.MathNode`: `InsertAsParent(MathNode, MathNode, int)` ([SkipLocalsInit])
  - `EquationEditor.Tree.MathNode`: `ReplaceNode(MathNode, MathNode)` ([SkipLocalsInit])
  - `EquationEditor.Tree.MathNode`: `IsMiddleOfSubscript(MathNode)` (using `node?.Children?.Count == 2` to match non-virtual `call` instruction)
- Report: full `--project Quire.EquationEditor` gate PASS, 912 checked, 0 failures.
- Parked members (exact blockers):
  - `IResultTreeFormatter`: CS0535 compile error in `ResultFormatter` when methods lose stub defaults.
  - `LimitSideNodeInfo+a`, `MathNodeToStringTransformer`, `DotfuscatorAttribute`, `MathTransformer`, `SExpressionUtility`: Field/method name clashes (CS0102) or obfuscated getter names.
- Remaining mismatches: 0 in completed types; remaining unbatched/parked stubs in other types.
