# EquationEditor untouched-lane classification (eq-editor-untouched)

- Owner: eq-editor-untouched dispatched worker (task_01947223ab54)
- Branch: `feature/eqeditor-untouched` (from `main` a4f7309d6)
- Assembly: `Quire.EquationEditor` (managed, `Ptc.EquationEditor.dll` 12.0.1.0)
- Source: every `[MirrorStub]` in `src/Quire.EquationEditor/Generated/Mirror/*.g.cs` (4679 stubs in 297 files, 4362 excluding `const`) cross-checked against the Release IL of the whole assembly in `.mirror-cache/Ptc.EquationEditor/*/Ptc.EquationEditor.il` and against the lane checkpoints in this folder.

## How members were classified

The class is a static scan of the original IL, not a build result; a member marked unblocked has no static blocker and can still fail the Release IL gate on shape.

| Class | Meaning |
|---|---|
| unblocked | no blocker found; batchable |
| closure/lambda/iterator | the member (or its nested compiler type) uses `ldftn`, a display class/struct, or an iterator state machine; Mathcad's ordinals need the parent members first (placeholder stubs allowed per AGENTS.md), and the nested stubs themselves vanish once the parent is written |
| field/method name clash | the IL field name is also a method name (`a`, `b`, …); MirrorGen emits `a_`/`b__`, and the gate compares field names. Applies to the renamed field and to every member that reads or writes it (checkpoint 20261109b, needs a maintainer decision) |
| `.initlocals` header | fat header with no locals, no EH and maxstack ≤ 8; Roslyn emits `.initlocals False` (checkpoint 20261109) |
| ctor stores before base call | constructor assigns fields, runs `newarr` or `newobj` before the base `Object` call; C# cannot express it (checkpoint 20261109) |
| blocked in checkpoint | a checkpoint records the member as blocked; skipped here |
| Settings pairing | the body calls obfuscated `Properties.Settings` members (checkpoint 20261109) |
| XAML | `VisualOperatorDescriptorTable` (`InitializeComponent`, `IComponentConnector.Connect`, `Application.LoadComponent`) |
| needs manual IL match | no counterpart found by name/arity/`.override`: generic methods, indexers, overloads with different arity, `@`-escaped names. Resolve with `reference.py` at batch time; none is a known blocker |
| const (no IL) | `const` fields have no IL; they keep their stub attribute |

No stub is blocked by COM/native or P/Invoke: the assembly has no `pinvokeimpl`/`ComImport`. The `Native.PtcEquationEditor.*` types (the obfuscated root-namespace classes) are ordinary managed classes. Type-map ownership: only `IResultTreeFormatter` and `MathNode` are `exists` in `docs/mirror/type-map.csv`, the rest are `missing` there but already generated, so no type is missing from the project.

## Totals

| Class | Members |
|---|---:|
| unblocked | 1743 |
| closure/lambda/iterator | 609 |
| field/method name clash | 1800 |
| `.initlocals` header | 69 |
| ctor stores before base call | 39 |
| blocked in checkpoint | 38 |
| Settings pairing | 1 |
| XAML | 5 |
| needs manual IL match | 58 |
| const (no IL) | 317 |
| **all** | **4679** |

## Batch candidates (untouched types, most unblocked first)

Untouched = no hand-written partial yet. Check the per-type row for blockers before choosing a batch.

| Type | Unblocked | Stubs |
|---|---:|---:|
| `Equation.EquationModel` | 293 | 676 |
| `Tree.MathNodeDescriptor` | 220 | 225 |
| `SpecTable.Model.SpecTableModel` | 136 | 254 |
| `Tree.MathNode` | 118 | 361 |
| `Native.n` | 48 | 56 |
| `Native.m` | 44 | 52 |
| `SpecTable.Model.SpecTableCalculatableWrapper` | 40 | 58 |
| `Native.l` | 37 | 104 |
| `Tree.Transform.MathNodeToXmlTransform` | 36 | 134 |
| `SpecTable.Model.SpecTableColumn` | 35 | 89 |
| `Tree.Transform.MathNodeToRlTransformer` | 30 | 191 |
| `ScriptingError` | 23 | 24 |
| `Native.PersistentDataObjects.MathTransformer` | 22 | 23 |
| `Tree.InputScriptableWithInputNodeInfo` | 20 | 32 |
| `Tree.RootMathNode` | 19 | 22 |
| `Tree.Transform.MathNodeToSExpressionTransform` | 16 | 28 |
| `Tree.Scoping.TypeoverState` | 16 | 62 |
| `Tree.ComboBoxInputControlData` | 16 | 84 |
| `Native.ae` | 16 | 17 |
| `Tree.ComboBoxControlNodeInfo` | 15 | 22 |
| `RedefinitionWarning` | 14 | 26 |
| `Tree.Scoping.NavigatorSegment` | 12 | 18 |
| `Tree.LimitSideNodeInfo` | 12 | 15 |
| `Native.ab` | 12 | 27 |
| `Tree.Transform.InfixMonomialToSExpressionTransform` | 11 | 37 |
| `Native.ad` | 11 | 26 |
| `Tree.MathNodeToStringTransformer` | 10 | 13 |
| `Native.ao` | 10 | 11 |
| `Native.ac` | 10 | 20 |
| `MathNodeToVisualNodeMap` | 10 | 14 |

## Blocked groups (summary)

- **Field/method name clash** (1800 members across most large types): needs a maintainer decision on how the gate pairs MirrorGen's `a_` fields with Mathcad's `a`; until then it is skipped.
- **Closure/lambda/iterator** (609 stubs, mostly nested compiler types that disappear with their parent): implement the parent members, using unmarked placeholder stubs to hold Mathcad's closure ordinals.
- **`.initlocals`** (69), **ctor-before-base** (39), **checkpoint-blocked** (38), **Settings** (1), **XAML** (5): see the per-type lists; unchanged from earlier checkpoints.
- `Tree.Transform.IResultTreeFormatter` (12): internal members on an interface.

## Per type (nested types are rolled into their top-level type)

`Hand` = a hand-written partial of the type already exists (touched by an earlier lane). Counts exclude nothing; `const` is listed separately.

| Type | Hand | Stubs | unbl | closure | name-clash | initlocals | ctor-pre-base | ckpt | settings | xaml | manual | const |
|---|:-:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| `Equation.EquationModel` | no | 676 | 293 | 100 | 258 | 11 | 4 |  |  |  | 8 | 2 |
| `Tree.MathNodeDescriptor` | no | 225 | 220 |  |  | 1 |  |  |  |  | 4 |  |
| `SpecTable.Model.SpecTableModel` | no | 254 | 136 | 26 | 81 | 6 | 3 |  |  |  | 1 | 1 |
| `Tree.MathNode` | no | 361 | 118 | 49 | 186 | 5 | 1 |  |  |  | 2 |  |
| `Native.n` | no | 56 | 48 | 5 | 2 | 1 |  |  |  |  |  |  |
| `Native.m` | no | 52 | 44 | 7 | 1 |  |  |  |  |  |  |  |
| `SpecTable.Model.SpecTableCalculatableWrapper` | no | 58 | 40 | 6 | 11 | 1 |  |  |  |  |  |  |
| `Native.l` | no | 104 | 37 | 28 | 39 |  |  |  |  |  |  |  |
| `Tree.Transform.MathNodeToXmlTransform` | no | 134 | 36 | 47 | 51 |  |  |  |  |  |  |  |
| `SpecTable.Model.SpecTableColumn` | no | 89 | 35 | 15 | 39 |  |  |  |  |  |  |  |
| `Tree.FormattedString` | yes | 48 | 31 |  | 6 | 6 | 1 |  |  |  |  | 4 |
| `Tree.Transform.MathNodeToRlTransformer` | no | 191 | 30 | 39 | 99 | 1 | 2 |  |  |  | 5 | 15 |
| `ScriptingError` | no | 24 | 23 |  |  |  |  |  |  |  | 1 |  |
| `Native.PersistentDataObjects.MathTransformer` | no | 23 | 22 |  |  |  | 1 |  |  |  |  |  |
| `Tree.InputScriptableWithInputNodeInfo` | no | 32 | 20 | 4 | 6 | 1 | 1 |  |  |  |  |  |
| `Tree.RootMathNode` | no | 22 | 19 |  |  | 3 |  |  |  |  |  |  |
| `Tree.ComboBoxInputControlData` | no | 84 | 16 | 29 | 33 | 4 | 2 |  |  |  |  |  |
| `Tree.Scoping.TypeoverState` | no | 62 | 16 | 25 | 17 |  | 4 |  |  |  |  |  |
| `Tree.Transform.MathNodeToSExpressionTransform` | no | 28 | 16 |  | 8 | 4 |  |  |  |  |  |  |
| `Native.ae` | no | 17 | 16 |  | 1 |  |  |  |  |  |  |  |
| `Tree.ComboBoxControlNodeInfo` | no | 22 | 15 | 5 | 2 |  |  |  |  |  |  |  |
| `RedefinitionWarning` | no | 26 | 14 |  | 12 |  |  |  |  |  |  |  |
| `Tree.Parsing.PanaceaVisitor` | yes | 14 | 14 |  |  |  |  |  |  |  |  |  |
| `Tree.LimitSideNodeInfo` | no | 15 | 12 | 2 | 1 |  |  |  |  |  |  |  |
| `Tree.Scoping.NavigatorSegment` | no | 18 | 12 |  | 5 | 1 |  |  |  |  |  |  |
| `Native.ab` | no | 27 | 12 |  | 7 | 1 |  |  |  |  | 7 |  |
| `Tree.Transform.InfixMonomialToSExpressionTransform` | no | 37 | 11 | 4 | 15 | 1 | 1 |  |  |  |  | 5 |
| `Native.ad` | no | 26 | 11 |  | 8 |  |  |  |  |  | 7 |  |
| `MathNodeToVisualNodeMap` | no | 14 | 10 | 1 | 2 |  |  |  |  |  | 1 |  |
| `Tree.MathNodeToStringTransformer` | no | 13 | 10 |  |  |  |  |  |  |  |  | 3 |
| `Tree.StringToComboBoxMathTransformer` | yes | 17 | 10 |  | 7 |  |  |  |  |  |  |  |
| `Native.ac` | no | 20 | 10 |  | 3 |  |  |  |  |  | 7 |  |
| `Native.ao` | no | 11 | 10 |  | 1 |  |  |  |  |  |  |  |
| `SpecTable.Model.ColumnMemento` | no | 11 | 9 | 1 |  | 1 |  |  |  |  |  |  |
| `Tree.InputControlNodeInfo` | no | 18 | 9 | 4 | 5 |  |  |  |  |  |  |  |
| `Native.a2` | no | 74 | 9 | 23 | 41 |  |  |  |  |  | 1 |  |
| `Native.a4` | no | 19 | 9 |  | 10 |  |  |  |  |  |  |  |
| `Native.aw` | no | 33 | 9 | 6 | 18 |  |  |  |  |  |  |  |
| `EquationPresenter` | no | 68 | 8 | 7 | 52 | 1 |  |  |  |  |  |  |
| `Tree.Navigation.NavigatorState` | yes | 10 | 8 |  |  |  | 2 |  |  |  |  |  |
| `Tree.Transform.XmlNodeToMathNodeTransform` | no | 104 | 8 | 6 | 75 | 1 |  |  |  |  |  | 14 |
| `Native.as` | no | 8 | 8 |  |  |  |  |  |  |  |  |  |
| `Native.bi` | no | 8 | 8 |  |  |  |  |  |  |  |  |  |
| `Native.v` | no | 13 | 8 |  | 4 |  |  |  |  |  | 1 |  |
| `Native.y` | no | 9 | 8 |  |  |  |  |  |  |  | 1 |  |
| `Tree.IdentifierNodeInfo` | yes | 19 | 7 | 3 | 4 | 3 | 1 |  |  |  | 1 |  |
| `Tree.MathNodeDescriptorTable` | no | 42 | 7 | 21 | 14 |  |  |  |  |  |  |  |
| `Tree.MatrixNodeInfo` | yes | 9 | 7 |  | 2 |  |  |  |  |  |  |  |
| `Tree.Transform.SExpressionParser` | no | 24 | 7 | 1 | 15 |  | 1 |  |  |  |  |  |
| `Native.al` | no | 14 | 7 |  | 6 |  | 1 |  |  |  |  |  |
| `Native.bc` | no | 10 | 7 |  | 3 |  |  |  |  |  |  |  |
| `Tree.DefaultDescriptorTable` | no | 101 | 6 | 8 | 2 | 1 |  |  |  |  |  | 84 |
| `Tree.Transform.SExpressionToMathNodeTransform` | no | 17 | 6 |  | 11 |  |  |  |  |  |  |  |
| `Tree.Transform.XmcdToMathNodeTransform` | no | 22 | 6 |  | 15 |  |  |  |  |  |  | 1 |
| `Native.aa` | no | 8 | 6 |  |  |  |  |  |  |  | 2 |  |
| `Native.ak` | no | 71 | 6 |  | 64 | 1 |  |  |  |  |  |  |
| `Native.aq` | no | 7 | 6 |  |  |  | 1 |  |  |  |  |  |
| `Native.az` | no | 6 | 6 |  |  |  |  |  |  |  |  |  |
| `Native.bh` | no | 7 | 6 |  |  | 1 |  |  |  |  |  |  |
| `Native.bk` | no | 11 | 6 | 4 | 1 |  |  |  |  |  |  |  |
| `Native.bl` | no | 10 | 6 |  | 4 |  |  |  |  |  |  |  |
| `Native.bm` | no | 6 | 6 |  |  |  |  |  |  |  |  |  |
| `Native.d` | no | 6 | 6 |  |  |  |  |  |  |  |  |  |
| `Native.j` | no | 14 | 6 | 4 | 3 |  | 1 |  |  |  |  |  |
| `Native.z` | no | 7 | 6 |  |  |  |  |  |  |  | 1 |  |
| `Native.bb` | no | 8 | 5 |  | 2 | 1 |  |  |  |  |  |  |
| `Native.be` | no | 7 | 5 |  | 2 |  |  |  |  |  |  |  |
| `Native.bf` | no | 5 | 5 |  |  |  |  |  |  |  |  |  |
| `Native.bn` | no | 6 | 5 |  |  | 1 |  |  |  |  |  |  |
| `Native.t` | no | 6 | 5 |  |  |  |  |  |  |  | 1 |  |
| `Native.w` | no | 8 | 5 |  | 2 |  |  |  |  |  | 1 |  |
| `Native.x` | no | 8 | 5 |  | 2 |  |  |  |  |  | 1 |  |
| `Tree.Scoping.LinearSegment` | yes | 8 | 4 |  |  |  |  | 2 |  |  |  | 2 |
| `Native.a3` | no | 5 | 4 |  | 1 |  |  |  |  |  |  |  |
| `Native.a9` | no | 15 | 4 | 9 | 2 |  |  |  |  |  |  |  |
| `Native.ah` | no | 54 | 4 |  | 47 |  |  |  |  |  | 1 | 2 |
| `Native.ar` | no | 13 | 4 |  | 9 |  |  |  |  |  |  |  |
| `Native.bg` | no | 8 | 4 |  | 4 |  |  |  |  |  |  |  |
| `Native.bo` | no | 13 | 4 | 2 | 7 |  |  |  |  |  |  |  |
| `Native.u` | no | 5 | 4 |  |  |  |  |  |  |  | 1 |  |
| `Properties.Settings` | no | 4 | 3 |  |  |  |  |  | 1 |  |  |  |
| `SpecTable.Model.SpecTableMemento` | no | 11 | 3 | 7 | 1 |  |  |  |  |  |  |  |
| `Tree.StringToComboBoxMathTransformerForConvertor` | yes | 3 | 3 |  |  |  |  |  |  |  |  |  |
| `Tree.ValueNodeInfo` | yes | 12 | 3 | 4 | 5 |  |  |  |  |  |  |  |
| `Native.a6` | no | 36 | 3 | 8 | 25 |  |  |  |  |  |  |  |
| `Native.a8` | no | 3 | 3 |  |  |  |  |  |  |  |  |  |
| `Native.af` | no | 10 | 3 |  | 5 |  |  |  |  |  |  | 2 |
| `Native.an` | no | 5 | 3 |  | 2 |  |  |  |  |  |  |  |
| `Native.bj` | no | 11 | 3 | 1 | 7 |  |  |  |  |  |  |  |
| `Native.DotfuscatorAttribute` | no | 5 | 2 |  |  | 3 |  |  |  |  |  |  |
| `SpecTable.SpecTableCellMap` | yes | 3 | 2 |  |  |  |  | 1 |  |  |  |  |
| `Tree.InputControlData` | yes | 3 | 2 |  |  |  |  | 1 |  |  |  |  |
| `Tree.InputControlNodeCreator` | no | 2 | 2 |  |  |  |  |  |  |  |  |  |
| `Tree.Transform.ParsedSExpressionToComboBoxInputControlData` | yes | 22 | 2 | 8 | 4 |  |  |  |  |  |  | 8 |
| `Tree.Transform.SExpressionToRlTransform` | yes | 9 | 2 | 3 | 4 |  |  |  |  |  |  |  |
| `Native.a0` | no | 16 | 2 |  | 13 |  | 1 |  |  |  |  |  |
| `Native.a5` | no | 21 | 2 |  | 19 |  |  |  |  |  |  |  |
| `Native.a7` | no | 2 | 2 |  |  |  |  |  |  |  |  |  |
| `Native.am` | no | 6 | 2 |  | 4 |  |  |  |  |  |  |  |
| `Native.au` | no | 3 | 2 |  |  |  | 1 |  |  |  |  |  |
| `Native.av` | no | 7 | 2 |  | 3 |  | 2 |  |  |  |  |  |
| `Native.ax` | no | 75 | 2 | 12 | 61 |  |  |  |  |  |  |  |
| `Native.bd` | no | 9 | 2 |  | 7 |  |  |  |  |  |  |  |
| `Native.br` | no | 4 | 2 |  | 2 |  |  |  |  |  |  |  |
| `Native.bs` | no | 5 | 2 |  | 2 |  |  |  |  |  | 1 |  |
| `Native.o` | no | 2 | 2 |  |  |  |  |  |  |  |  |  |
| `Native.q` | no | 4 | 2 |  | 2 |  |  |  |  |  |  |  |
| `Native.r` | no | 43 | 2 |  | 41 |  |  |  |  |  |  |  |
| `Native.s` | no | 11 | 2 |  | 8 |  | 1 |  |  |  |  |  |
| `SpecTable.Model.SpecTableColumnToRlTransform` | yes | 1 | 1 |  |  |  |  |  |  |  |  |  |
| `SpecTable.SpecTablePresenter` | no | 25 | 1 |  | 24 |  |  |  |  |  |  |  |
| `Tree.ComboBoxMathCreator` | yes | 1 | 1 |  |  |  |  |  |  |  |  |  |
| `Tree.MathNodeDescriptorPiggyback` | yes | 1 | 1 |  |  |  |  |  |  |  |  |  |
| `Tree.MathNodeNames` | no | 99 | 1 |  |  |  |  |  |  |  |  | 98 |
| `Tree.NearestNeighborSelector` | yes | 2 | 1 |  |  |  |  | 1 |  |  |  |  |
| `Tree.Scoping.ScopedNodes` | yes | 9 | 1 | 4 | 4 |  |  |  |  |  |  |  |
| `Tree.Transform.ComboBoxInputControlDataToSExpressionTransform` | no | 15 | 1 | 10 | 4 |  |  |  |  |  |  |  |
| `Tree.Transform.InlineToSExpressionTransform` | yes | 6 | 1 |  |  | 1 |  |  |  |  |  | 4 |
| `Native.a` | no | 3 | 1 |  |  |  |  |  |  |  |  | 2 |
| `Native.ap` | no | 87 | 1 | 24 | 62 |  |  |  |  |  |  |  |
| `Native.ba` | no | 10 | 1 |  | 9 |  |  |  |  |  |  |  |
| `Native.bp` | no | 7 | 1 |  | 4 | 1 | 1 |  |  |  |  |  |
| `Native.f` | yes | 3 | 1 |  | 1 | 1 |  |  |  |  |  |  |
| `Native.k` | no | 9 | 1 |  | 8 |  |  |  |  |  |  |  |
| `EquationErrors` | yes | 1 |  |  |  |  |  |  |  |  |  | 1 |
| `MathNodeSettings` | no | 3 |  |  |  |  |  | 3 |  |  |  |  |
| `Tree.AlgorithmOptions` | yes | 5 |  |  | 2 |  |  |  |  |  |  | 3 |
| `Tree.ButtonScriptableControlData` | yes | 3 |  |  |  |  |  | 1 |  |  |  | 2 |
| `Tree.CheckBoxScriptableControlData` | yes | 3 |  |  |  |  |  | 1 |  |  |  | 2 |
| `Tree.FunctionOperatorNames` | yes | 2 |  |  |  |  |  |  |  |  |  | 2 |
| `Tree.InputControlDataCalculatedValue` | yes | 1 |  |  |  |  |  | 1 |  |  |  |  |
| `Tree.InputScriptableControlData` | yes | 3 |  |  |  |  |  | 3 |  |  |  |  |
| `Tree.InputScriptableControlDataWithInputs` | yes | 1 |  |  |  |  |  | 1 |  |  |  |  |
| `Tree.ListBoxScriptableControlData` | yes | 1 |  |  |  |  |  | 1 |  |  |  |  |
| `Tree.RadioButtonScriptableControlData` | yes | 3 |  |  |  |  |  | 1 |  |  |  | 2 |
| `Tree.Scoping.DetachedTypeoverState` | yes | 1 |  |  |  |  |  | 1 |  |  |  |  |
| `Tree.SliderScriptableControlData` | yes | 4 |  |  |  |  |  | 2 |  |  |  | 2 |
| `Tree.TextBoxScriptableControlData` | yes | 3 |  |  |  |  |  | 1 |  |  |  | 2 |
| `Tree.Transform.IResultTreeFormatter` | yes | 12 |  |  |  |  |  | 12 |  |  |  |  |
| `Tree.Transform.ParsedSExpressionToInlineTransform` | yes | 12 |  | 2 | 8 |  | 1 |  |  |  |  | 1 |
| `Tree.Transform.SExpressionUtility` | yes | 5 |  |  |  |  |  | 5 |  |  |  |  |
| `Tree.TransformMathNodeToComboBoxValues` | yes | 8 |  | 5 | 2 | 1 |  |  |  |  |  |  |
| `VisualOperatorDescriptorTable` | no | 5 |  |  |  |  |  |  |  | 5 |  |  |
| `Native.ag` | no | 32 |  |  |  |  |  |  |  |  |  | 32 |
| `Native.ai` | no | 15 |  |  |  |  |  |  |  |  |  | 15 |
| `Native.aj` | no | 6 |  |  |  |  |  |  |  |  |  | 6 |
| `Native.at` | no | 43 |  | 14 | 27 | 1 | 1 |  |  |  |  |  |
| `Native.ay` | no | 11 |  |  | 9 |  | 1 |  |  |  | 1 |  |
| `Native.bq` | no | 38 |  |  | 34 | 2 | 2 |  |  |  |  |  |
| `Native.g` | no | 78 |  | 16 | 59 | 1 | 1 |  |  |  | 1 |  |

## Members per type

Names are the Quire (generated) names; `name()` is a method, `+Nested.name` a member of a nested type, `×n` overloads or repeats. Obfuscated names (`a`, `b`, `ap`) are the Mathcad names.

### `Native.DotfuscatorAttribute`
- **unblocked** (2): `a`, `c`
- **`.initlocals` header** (3): `A`, `C`, `DotfuscatorAttribute(…)`

### `Equation.EquationModel`
- **unblocked** (293): `+b.a2()`, `+b.a4()`, `+b.a4(…)`, `+b.a5()`, `+b.a6()`, `+b.a7()`, `+b.a8()`, `+b.a9()`, `+b.ae()`, `+b.af()`, `+b.ah()`, `+b.ai()`, `+b.aj()`, `+b.ak()`, `+b.am()`, `+b.an()`, `+b.ao()`, `+b.ap()`, `+b.at()`, `+b.au()`, `+b.aw()`, `+b.ax()`, `+b.ay()`, `+b.az()`, `+b.b_(…)`, `+b.ba()`, `+b.bb()`, `+b.bc()`, `+b.bh()`, `+b.bi()`, `+b.bj()`, `+b.d()`, `+b.e()`, `+b.f()`, `+b.g()`, `+b.j()`, `+b.j(…)`, `+b.l()`, `+b.m()`, `+b.n()`, `+b.o()`, `+b.q()`, `+b.q(…)`, `+b.r()`, `+b.r(…)`, `+b.s()`, `+b.t()`, `+b.u()`, `+b.u(…)`, `+b.v()`, `+b.v(…)`, `+b.w()`, `+b.w(…)`, `+b.x()`, `+b.x(…)`, `+b.y()`, `+b.y(…)`, `+b.z()`, `+b.z(…)`, `+c.b`, `+c.c`, `+c.d`, `+c.e`, `+c.f`, `+c.g`, `+c.h`, `+c.i`, `+c.j`, `+c.k`, `+c.l`, `+c.m`, `+d.b`, `+d.c`, `+d.d`, `+d.e`, `+e.Do()`, `+e.Undo()`, `+e.b()`, `+e.c`, `+e.d`, `+e.e`, `+e.e_(…)`, `+e.f`, `+e.g`, `+e.h`, `AlgorithmChanged`, `AtLeftEdge()`, `AtRightEdge()`, `CalculatingStatus`, `CalculationErrorChanged`, `CanCopy()`, `CanCopy(…)`, `CanCut()`, `CanCut(…)`, `CanInsertKeyword()`, `CanNavigate(…)`, `CanNavigateToOppositeOfScope(…)`, `CanPaste()`, `CanPaste(…)`, `CanPerformColumnOperations(…)`, `CanPerformIdLabelOperations(…)`, `CanPerformLinearNaryOperations(…)`, `CanPerformReplace(…)`, `CanPerformRowOperations(…)`, `CanToggleLineBreakOperator(…)`, `CancelScope()`, `CancelTypeover()`, `ClearError()`, `ClipBoard`, `Clone()`, `CloseParen()`, `ContentChanged`, `ContractScope()`, `ConvertToOperator()`, `Copy()`, `CurrentOperatorMayBeReplaced`, `CurrentSExpression`, `CurrentTypeoverState`, `Cut()`, `DragIntoMatrixHiddenPart(…)`, `EditNode(…)`×2, `EngineResultForSerialization`, `EngineWarnings`, `EnterNode(…)`, `EquationChanged`, `EquationModel(…)`, `EquationSExpression`, `EquationSExpressionWithResolvedLabels`, `EquationType`, `Error`, `ErrorNode`, `EvaluationErrorOccurred`, `ExpandScope()`, `ExpressionValueChanged`, `FindChildWithMatchingId(…)`, `FireAlgorithmChanged()`, `FireContentChanged()`, `FireEquationChanged(…)`, `FireExpressionValueChanged(…)`, `FireNavigatorStateChanged()`, `FireNodeMayBeReplaced(…)`, `FireRequestFocus()`, `FireSynchEvents()`, `FullyContractScope()`, `FullyExpandScope()`, `GetDefinedIdentifierSExpressions(…)`, `GetDefinitionSExpression(…)`, `GetEvaluationSExp(…)`, `GetFirstScopedNode`, `GetLastScopedNode`, `GetRlIndexForNumericResult()`, `HandleInputTagChanged(…)`, `HasNumericEvaluation`, `HookupDescendantsMatrixFormatChanged()`, `HookupDescendantsMatrixFormatChanged(…)`, `InCustomInitialization`, `InCustomInitializationChanged`, `InsertValue(…)`, `IsEmpty`, `IsEvaluatable`, `IsMathFormatHandledLocally`, `IsModified`, `IsReadonly`, `IsReplacingPreviouseOperator`, `IsRestrictedReadOnly`, `IsScriptError`, `IsSuitableForInputTagging()`, `JumpToNode(…)`, `Mode`, `MoveDown()`, `MoveLeft()`, `MoveLeftByTerm()`, `MoveRight()`, `MoveRightByTerm()`, `MoveToCurrentExpressionEnd()`, `MoveToCurrentExpressionExtreme(…)`, `MoveToCurrentExpressionStart()`, `MoveToOppositeSide()`, `MoveToRegionEnd()`, `MoveToRegionExtreme(…)`, `MoveToRegionStart()`, `MoveUp()`, `NavigatorState`, `NavigatorStateChanged`, `NodeMayBeReplaced`, `NumericResultChanged`, `OnCanTraverseBackward(…)`, `OnCanTraverseForward(…)`, `OnTraverseBackward(…)`, `OnTraverseForward(…)`, `OverrideIdLabelByResolvedForDefinitionLhs`, `Paste()`, `PrevStepForMultiKeyOpFailed`, `ProhibitComboBox`, `ProhibitInputControl`, `ReplaceDirectionForMultiKeyOp`, `RequestEvaluation`, `RequestFocus`, `RequestRemoveEquation`, `RequestRestoreInsertionPoint`, `ResolvedLabelOverridesIdLabel(…)`, `RestoreFromHistory(…)`, `RestrictedReadonlyChanged`, `ScopeChanged`, `ScopeLeft()`, `ScopeNode(…)`, `ScopeRight()`, `ScopeSide`, `ScopedNodesByLines`, `ScopedSExpression`, `SelectLeft()`, `SelectRight()`, `SelectTo(…)`, `SelectToEnd()`, `SelectToStart()`, `SerializedContent`, `SetAutoSelectAlgorithm(…)`, `SetCurrentOperatorMayBeReplaced(…)`, `SetDataExchangeModel(…)`, `SetEquationSExpression(…)`, `SetEquationSExpressionWithAutoLabeling(…)`, `SetIsReplacingPreviouseOperator(…)`, `SetResult(…)`, `SwitchLineBreakOperator()`, `System.ICloneable.Clone()`, `ToggleGreek()`, `ToggleIdLabel()`, `ToggleSubscript()`, `ToggleUnitIdLabel()`, `TypeoverChanged`, `TypeoverNodes`, `TypeoverSExpression`, `VisibleLhsOfDefinitionChanged`, `WarningsOccurred`, `a0`, `a1`, `a2`, `a3`, `a4`, `a5`, `a6`, `a7`, `a8`, `a9`, `ac()`, `ae`, `af`, `ag`, `ah`, `ai`, `aj`, `ak`, `al`, `am`, `an`, `ao`, `ap`, `aq`, `ar`, `au`, `av`, `aw`, `ax`, `ay`, `az`, `ba`, `bb`, `bc`, `bd`, `be`, `bf`, `bg`, `d()`, `d(…)`, `f()`, `g()`, `h()`, `i()`, `k()`, `m()`, `n`, `o`, `p`, `q`, `r()`, `s()`, `t()`, `u()`, `v()`, `x`, `y()`, `z()`
- **closure/lambda/iterator** (100): `+a+a.a`, `+a+b.a(…)`, `+a+b.b`, `+a+b.b_()`, `+a+c.c()`, `+a+d.d()`, `+a+e.e()`, `+a.b(…)`, `+aa.aa_()`, `+aa.b`, `+ab.ab_()`, `+ab.c`, `+ac.ac_()`, `+ac.c`, `+ad.ad_()`, `+ae.ae_()`, `+ae.b`, `+af.af_()`, `+af.b`, `+ag.ag_()`, `+ag.b`, `+ag.c`, `+ag.d`, `+ah.ah_()`, `+ah.b`, `+ah.c`, `+ah.d`, `+ah.e`, `+ai.ai_()`, `+ai.b`, `+aj.System.Collections.Generic.IEnumerable<string>.GetEnumerator()`, `+aj.System.Collections.Generic.IEnumerator<string>.Current`, `+aj.System.Collections.IEnumerable.GetEnumerator()`, `+aj.System.Collections.IEnumerator.Current`, `+aj.System.Collections.IEnumerator.MoveNext()`, `+aj.System.Collections.IEnumerator.Reset()`, `+aj.System.IDisposable.Dispose()`, `+aj.aj_(…)`, `+aj.c`, `+aj.e`, `+c+a.a_()`, `+c+a.b`, `+c.c_(…)`, `+f.a(…)`×3, `+f.b(…)`, `+f.c`, `+f.d`, `+f.e`, `+f.f_()`, `+g.b`, `+g.c`, `+g.g_()`, `+h.b`, `+h.h_()`, `+i.b`, `+i.i_()`, `+j.j_()`, `+k.b`, `+k.k_()`, `+l.c`, `+l.l_()`, `+m.m_()`, `+n.b`, `+n.n_()`, `+o.b`, `+o.o_()`, `+p.b`, `+p.p_()`, `+q.b`, `+q.q_()`, `+r.b`, `+r.r_()`, `+s.c`, `+s.s_()`, `+t.b`, `+t.t_()`, `+u.b`, `+u.u_()`, `+v.b`, `+v.v_()`, `+w.b`, `+w.w_()`, `+x.b`, `+x.x_()`, `+y.b`, `+y.c`, `+y.y_()`, `+z.b`, `+z.z_()`, `ClearCells()`, `ClearContent()`, `DeleteColumn()`, `DeleteContents(…)`, `DeleteRow()`, `InsertSymbolicEvaluation()`, `ProcessEqualsKey()`, `SplitLinearNaryLine()`, `UnsetIdLabel()`
- **field/method name clash** (258): `+a+b.a_`, `+a+c.a(…)`, `+a+c.a_`, `+a+d.a(…)`, `+a+d.a_`, `+a+e.a(…)`, `+a+e.a_`, `+aa.a()`, `+aa.a_`, `+ab.a()`, `+ab.a_`, `+ab.b()`, `+ab.b_`, `+ac.a()`, `+ac.a_`, `+ac.b()`, `+ac.b_`, `+ad.a(…)`, `+ad.a_`, `+ae.a()`, `+ae.a_`, `+af.a()`, `+af.a_`, `+ag.a()`, `+ag.a_`, `+ah.a()`, `+ah.a_`, `+ai.a(…)`, `+ai.a_`, `+aj.a_`, `+aj.b_`, `+aj.d_`, `+b.@as()`, `+b.@as(…)`, `+b.a()`, `+b.a(…)`×7, `+b.a0()`, `+b.a1()`, `+b.a3()`, `+b.a3(…)`, `+b.a_`, `+b.aa()`, `+b.aa(…)`, `+b.ab()`, `+b.ab(…)`, `+b.ac()`, `+b.ac(…)`, `+b.ad()`, `+b.ag()`, `+b.ag(…)`, `+b.al()`, `+b.al(…)`, `+b.aq()`, `+b.aq(…)`, `+b.ar()`, `+b.ar(…)`, `+b.av()`, `+b.av(…)`, `+b.b()`, `+b.b(…)`×3, `+b.bd()`, `+b.be()`, `+b.bf()`, `+b.bf(…)`, `+b.bg(…)`, `+b.c()`, `+b.c(…)`×2, `+b.h()`, `+b.i()`, `+b.k()`, `+b.k(…)`, `+b.p()`, `+b.p(…)`, `+c+a.a(…)`, `+c+a.a__`, `+c.a(…)`×4, `+c.a__`, `+d.a()`, `+d.a_`, `+e.Name`, `+e.Redo()`, `+e.a()`, `+e.a(…)`, `+e.a_`, `+e.b_`, `+f.a_`, `+f.b_`, `+g.a()`, `+g.a_`, `+h.a()`, `+h.a_`, `+i.a()`, `+i.a_`, `+j.a(…)`, `+j.a_`, `+k.a(…)`, `+k.a_`, `+l.a()`, `+l.a_`, `+l.b()`, `+l.b_`, `+m.a()`, `+m.a_`, `+m.b()`, `+m.b_`, `+n.a()`, `+n.a_`, `+o.a()`, `+o.a_`, `+p.a()`, `+p.a(…)`, `+p.a_`, `+q.a()`, `+q.a_`, `+r.a()`, `+r.a_`, `+s.a()`, `+s.a_`, `+s.b()`, `+s.b_`, `+t.a()`, `+t.a_`, `+u.a()`, `+u.a_`, `+v.a()`, `+v.a_`, `+w.a()`, `+w.a_`, `+x.a()`, `+x.a_`, `+y.a()`, `+y.a_`, `+z.a()`, `+z.a_`, `AllowExpressionsOnly`, `AtEdge(…)`, `CalculationError`, `CanInsertInputControl(…)`, `CanPerformClearCellsOperation(…)`, `Clear()`, `Clear(…)`, `CurrentModeNavigatorState`, `DataExchangeModel`, `Delete(…)`, `EquationRoot`, `EquationXmlNode`, `Find(…)`, `GetNavigatorStateFromScoperCaretSide()`, `GetRealResult()`, `HandleError(…)`, `HandleScriptingEngineError(…)`, `HasVisualHistoryItems`, `HookupMatrixFormatChanged(…)`, `InsertColumn(…)`, `InsertFunction(…)`, `InsertFunctionWithArgs(…)`, `InsertId(…)`, `InsertInputControl(…)`, `InsertKeyword(…)`, `InsertMatrix(…)`, `InsertModifier(…)`, `InsertOperator(…)`, `InsertRow(…)`, `InsertString(…)`, `InsertUnitMonomial(…)`, `IsExpressionValueModified`, `PreserveExistingScalingWhenFormattingResults`, `Replace(…)`, `ResultMatrixFormat`, `ResultNumberFormat`, `ResultScalingFactor`, `ResultScalingModified`, `ResultScalingSExpression`, `Root`, `ScopeIdentifier(…)`, `ScriptingError`, `SetDefinitionValue(…)`, `SetIdLabel(…)`, `SetIdentifierLabels(…)`, `SetRedefinitionWarnings(…)`, `Settings`, `ToggleVisibleLhsOfDefinition(…)`, `VisibleLhsOfDefinition`, `VisualHistory`, `Warnings`, `a()`, `a(…)`×22, `a__`, `aa()`, `aa__`, `ab()`, `ab__`, `ac__`, `ad__`, `b()`, `b(…)`×5, `b__`, `c()`, `c(…)`, `c__`, `d__`, `e()`, `e(…)`, `g__`, `h__`, `i__`, `j()`, `j__`, `k__`, `l()`, `l__`, `m__`, `r__`, `s__`, `t__`, `u__`, `v__`, `w()`, `w__`, `y__`, `z__`
- **`.initlocals` header** (11): `+d.d_(…)`, `CanInsertFunction(…)`, `CanToggleVisibleLhsOfDefinition()`, `EquationModel()`, `EquationModel(…)`, `EvaluationRoot`, `FindChildWithErrorMatchingId(…)`, `IsMatrixCellClearable(…)`, `IsScriptableControl`, `IsSimpleComboBoxDefinition`, `ScopeResult()`
- **ctor stores before base call** (4): `+e.e_(…)`, `EquationModel(…)`×3
- **needs manual IL match** (8): `+a.a(…)`×6, `as`, `at`
- **const (no IL)** (2)

### `EquationErrors`
- **const (no IL)** (1)

### `EquationPresenter`
- **unblocked** (8): `EquationPresenter(…)`×2, `WasSuspendedRegenerate`, `d()`, `l()`, `q()`, `t()`, `v()`
- **closure/lambda/iterator** (7): `+b.a`, `+b.b`, `+b.c`, `+c.b`, `+c.c`, `+c.c_()`, `+c.d`
- **field/method name clash** (52): `+c.a()`, `+c.a_`, `FrameworkElementFacade`, `a()`, `a(…)`×17, `a__`, `b()`, `b(…)`×4, `b__`, `c()`, `c__`, `d_`, `e()`, `e_`, `f()`, `f_`, `g()`, `g_`, `h()`, `h_`, `i()`, `i_`, `j()`, `k()`, `m()`, `n()`, `o()`, `p()`, `r()`, `s()`, `u()`, `w()`, `x()`
- **`.initlocals` header** (1): `EquationPresenter(…)`

### `MathNodeSettings`
- **blocked in checkpoint** (3): `AutoLabelIds`, `Instance`, `MathNodeSettings()` — obfuscated Properties.Settings callee pairing (checkpoint 20261109)

### `MathNodeToVisualNodeMap`
- **unblocked** (10): `AssociateNode(…)`×2, `MathNodeToVisualNodeMap(…)`, `RootModelNode`, `RootVisualNode`, `a(…)`, `b`, `c`, `d`, `e`
- **closure/lambda/iterator** (1): `AssociateNodesCollection(…)`
- **field/method name clash** (2): `Regenerate(…)`, `a_`
- **needs manual IL match** (1): `AllVisualNodesOfType<T>()`

### `Properties.Settings`
- **unblocked** (3): `AutoLabelIds`, `Settings()`, `a`
- **Settings pairing** (1): `Default`

### `RedefinitionWarning`
- **unblocked** (14): `CreateMathNodeWarning(…)`, `Label`, `MessageText`, `MsgCode`, `RedefinitionWarning()`, `WarningType`, `b()`, `b(…)`, `c`, `d`, `e`, `f`, `g`, `h`
- **field/method name clash** (12): `Id`, `IsUserDefined`, `OldType`, `Original`, `a()`, `a(…)`×5, `a_`, `b_`

### `ScriptingError`
- **unblocked** (23): `Argument`, `Clone()`, `ErrorCode`, `ErrorMessageArguments`, `ErrorSource`, `FindNextError(…)`, `IsAncestorOf(…)`, `IsEquivalentTo(…)`, `IsRuntime`, `Message`, `NodeHandle`, `OriginalError`, `ResourceString`, `ScriptingError(…)`, `a`, `b`, `c`, `d`, `e`, `f`, `g`, `h`, `i`
- **needs manual IL match** (1): `Quire.Calculation.Core.IEngineError.Symbol`

### `SpecTable.Model.ColumnMemento`
- **unblocked** (9): `Column`, `DataCells`, `HeaderMemento`, `RestoreData()`, `ScalingMemento`, `a`, `b`, `c`, `d`
- **closure/lambda/iterator** (1): `+a.a`
- **`.initlocals` header** (1): `ColumnMemento(…)`

### `SpecTable.Model.SpecTableCalculatableWrapper`
- **unblocked** (40): `CalculatingStatus`, `CanHideFromCalculationContext`, `Commit()`, `ContainsError(…)`, `CurrentError`, `EngineResult`, `Error`, `ErrorCode`, `ErrorCount`, `ErrorSExpression`, `ErrorText`, `GetValueOfIdentifierBefore`, `InterceptAndUpdateResult`, `IsCommitted`, `PropertyChanged`, `RequestEvaluation`, `RequestShowError`, `RestoreResult()`, `Rl`, `SetAutoSelectAlgorithm(…)`, `SetCalculatingStatus(…)`, `SetError(…)`, `SetIdentifierLabels(…)`, `SetRedefinitionWarnings(…)`, `SetResult(…)`, `SetStatusSynchronized()`, `SetSynchronizationStatus(…)`, `ShowError(…)`, `SynchronizationStatus`, `TreatAsUpToDate`, `UseICalculatableToRestoreResult`, `b`, `c`, `d`, `e`, `f`, `g`, `h`, `i`, `j`
- **closure/lambda/iterator** (6): `+a.a(…)`×2, `+a.a_()`, `+a.b`, `+a.c`, `+b.b_()`
- **field/method name clash** (11): `+a.a__`, `+b.a()`, `+b.a_`, `+b.b()`, `+b.b__`, `Column`, `HideFromCalculationContext`, `a()`, `a(…)`×2, `a__`
- **`.initlocals` header** (1): `SpecTableCalculatableWrapper(…)`

### `SpecTable.Model.SpecTableColumn`
- **unblocked** (35): `+a.Do()`, `+a.Name`, `+a.Redo()`, `+a.Undo()`, `+a.a`, `+a.a_(…)`, `+a.b`, `+a.c`, `+b.Do()`, `+b.Name`, `+b.Redo()`, `+b.Undo()`, `+b.a`, `+b.b`, `+b.b_(…)`, `+b.c`, `ClearIsExpressionValueModified()`, `CreateMemento()`, `EquationXmlNode`, `ErrorCell`, `GetSExpression(…)`, `GetScaleSExpression()`, `HandleError(…)`, `RemoveDataCell(…)`, `Rl`, `SExpression`, `ScalingFactor`, `SetRestrictedReadonly(…)`, `SpecTableColumn()`, `System.Collections.IEnumerable.GetEnumerator()`, `b`, `e`, `f`, `g`, `h`
- **closure/lambda/iterator** (15): `+c.a(…)`×2, `+c.b(…)`, `+c.c`, `+c.c_()`, `+c.d`, `+d.d_()`, `+e.System.Collections.Generic.IEnumerator<Quire.EquationEditor.IEquationModel>.Current`, `+e.System.Collections.IEnumerator.Current`, `+e.System.Collections.IEnumerator.MoveNext()`, `+e.System.Collections.IEnumerator.Reset()`, `+e.System.IDisposable.Dispose()`, `+e.d`, `+e.e_(…)`, `SpecTableColumn(…)`
- **field/method name clash** (39): `+c.a_`, `+c.b_`, `+d.a(…)`, `+d.a_`, `+e.a()`, `+e.a_`, `+e.b_`, `+e.c_`, `AddCell()`, `CellCount`, `ColumnCellCountChanged`, `Data`, `DataCellCount`, `Dispose()`, `GetAllValuesScaledSExpression`, `GetCell(…)`, `GetDataCell(…)`, `GetDataSExpression()`, `GetEnumerator()`, `GetHeaderSExpression()`, `Header`, `InCustomInitialization`, `InsertDataCell(…)`, `IsEmpty`, `IsExpressionValueModified`, `RestoreMemento(…)`, `a()`, `a(…)`×7, `a__`, `c()`, `c(…)`, `c__`, `d__`

### `SpecTable.Model.SpecTableColumnToRlTransform`
- **unblocked** (1): `Transform(…)`

### `SpecTable.Model.SpecTableMemento`
- **unblocked** (3): `ColumnMementoList`, `Restore(…)`, `a`
- **closure/lambda/iterator** (7): `+a.a(…)`×2, `+a.a_()`, `+a.b`, `+a.c`, `RestoreData()`, `SpecTableMemento(…)`
- **field/method name clash** (1): `+a.a__`

### `SpecTable.Model.SpecTableModel`
- **unblocked** (136): `+a.Name`, `+a.Redo()`, `+a.a_(…)`, `+a.b(…)`, `+a.c`, `+a.d`, `+a.e`, `+a.f`, `+a.g`, `+b.Name`, `+b.Undo()`, `+b.a`, `+b.b`, `+b.b_(…)`, `+b.c`, `+b.d`, `+b.e`, `+b.f`, `+c.Do()`, `+c.Name`, `+c.Redo()`, `+c.Undo()`, `+c.a`, `+c.b`, `+c.c`, `+c.c_(…)`, `+c.d`, `+c.e`, `+c.f`, `+c.g`, `+d.Name`, `+d.Redo()`, `+d.Undo()`, `+d.a`, `+d.b`, `+d.c`, `+d.d`, `+d.d_(…)`, `+e.Do()`, `+e.Name`, `+e.Redo()`, `+e.Undo()`, `+e.a`, `+e.b`, `+e.c`, `+e.d`, `+e.e_(…)`, `+f.Do()`, `+f.Name`, `+f.Redo()`, `+f.Undo()`, `+f.a`, `+f.b`, `+f.c`, `+f.d`, `+f.f_(…)`, `+g.Do()`, `+g.Name`, `+g.Redo()`, `+g.Undo()`, `+g.a`, `+g.b`, `+g.c`, `+g.d`, `+g.g_(…)`, `+h.Do()`, `+h.Name`, `+h.Redo()`, `+h.Undo()`, `+h.a`, `+h.b`, `+h.c`, `+h.d`, `+h.e`, `+h.f`, `+h.g`, `+h.h`, `+h.i`, `+i.Do()`, `+i.Name`, `+i.Redo()`, `+i.Undo()`, `+i.a`, `+i.b`, `+i.c`, `+i.d`, `+i.e`, `+i.f`, `+i.g`, `+i.h`, `+i.i_(…)`, `+j.Do()`, `+j.Name`, `+j.Redo()`, `+j.Undo()`, `+j.a`, `+j.b`, `+j.c`, `+j.d`, `+j.e`, `+j.f`, `+j.g`, `+j.h`, `+j.i`, `+j.j`, `AddColumn(…)`×2, `AddColumnAtEnd()`, `AddRow(…)`, `AddRowAtEnd()`, `CreateMatrixFromTableCells(…)`, `CreateMemento()`, `FireRequestUpdateCellsVisibility()`, `GetColumnsRegionPersistentData()`, `HandleClearActiveCell(…)`, `HandleInputTagChanged(…)`, `HandleNavigateToFirstCell()`, `HandleOnDelete(…)`, `HandleOnTraverseBackward(…)`, `HandleRemoveOrClearSelection(…)`, `RemoveColumn(…)`, `RemoveEmptyColumns()`, `RemoveRow(…)`, `RequestAddSelection`, `RequestUpdateCellsVisibility`, `RestoreMemento(…)`, `RestrictedReadonlyChanged`, `SpecTableModel()`, `SubstituteAndCommitInputValue(…)`, `i(…)`, `j`, `k`, `l`, `m`, `n`, `o`
- **closure/lambda/iterator** (26): `+a.b()`, `+b.Do()`, `+k.a`, `+l.a()`, `+l.a(…)`×3, `+l.b()`, `+l.c()`, `+l.d`, `+l.e`, `+l.f`, `+l.g`, `+l.l_()`, `+m.m_()`, `+n.System.Collections.Generic.IEnumerable<Quire.EquationEditor.SpecTable.Model.SpecTableCalculatableWrapper>.GetEnumerator()`, `+n.System.Collections.Generic.IEnumerator<Quire.EquationEditor.SpecTable.Model.SpecTableCalculatableWrapper>.Current`, `+n.System.Collections.IEnumerable.GetEnumerator()`, `+n.System.Collections.IEnumerator.Current`, `+n.System.Collections.IEnumerator.MoveNext()`, `+n.System.Collections.IEnumerator.Reset()`, `+n.System.IDisposable.Dispose()`, `+n.d`, `+n.n_(…)`, `InCustomInitialization`, `SetColumnsRegionPersistentData(…)`
- **field/method name clash** (81): `+a.Do()`, `+a.Undo()`, `+a.a()`, `+a.a(…)`×3, `+a.a__`, `+a.b_`, `+l.a_`, `+l.b_`, `+l.c_`, `+m.a(…)`, `+m.a_`, `+n.a()`, `+n.a_`, `+n.b_`, `+n.c_`, `+n.e_`, `CanNavigationBetweenCells(…)`, `CellAdded`, `CellCount`, `ColumnAdded`, `ColumnCount`, `ColumnRemoved`, `Columns`, `Commit()`, `DefinitionsListToSpecTableModel(…)`, `GetColumn(…)`, `GetColumnWrapper(…)`, `GetWrappedColumns()`, `HandleNavigateToLastCell()`, `HandleNavigationBetweenCells(…)`, `HandleOnTraverseForward(…)`, `IsColumnEmpty(…)`, `IsEmpty`, `IsExtraDataCellOrEmptyScale(…)`, `IsRestrictedReadOnly`, `IsRowEmpty(…)`, `NumberOfDataCells`, `RequestClearSelection`, `RequestRemove`, `TableChanged`, `a()`, `a(…)`×9, `b()`, `b(…)`×4, `b__`, `c()`, `c(…)`×3, `c__`, `d(…)`×3, `d__`, `e(…)`×3, `e__`, `f()`, `f(…)`×3, `f__`, `g(…)`×2, `g__`, `h__`, `i__`
- **`.initlocals` header** (6): `+b.Redo()`, `+d.Do()`, `+h.h_(…)`, `+j.j_(…)`, `SExpression`, `h(…)`
- **ctor stores before base call** (3): `SpecTableModel(…)`×3
- **needs manual IL match** (1): `Quire.EquationEditor.SpecTable.Model.ISpecTableColumnItem`
- **const (no IL)** (1)

### `SpecTable.SpecTableCellMap`
- **unblocked** (2): `a`, `b`
- **blocked in checkpoint** (1): `SpecTableCellMap()` — ctor stores fields before base Object call (checkpoint 20261109b)

### `SpecTable.SpecTablePresenter`
- **unblocked** (1): `SpecTablePresenter(…)`
- **field/method name clash** (24): `FrameworkElementFacade`, `a()`, `a(…)`×7, `a_`, `b()`, `b(…)`×2, `b_`, `c()`, `c(…)`, `c_`, `d()`, `d(…)`, `d_`, `e()`, `e(…)`, `e_`, `f(…)`

### `Tree.AlgorithmOptions`
- **field/method name clash** (2): `IsMultiStart`, `a_`
- **const (no IL)** (3)

### `Tree.ButtonScriptableControlData`
- **blocked in checkpoint** (1): `Clone()` — initlocals header: fat header, no locals (checkpoint 20261109)
- **const (no IL)** (2)

### `Tree.CheckBoxScriptableControlData`
- **blocked in checkpoint** (1): `Clone()` — initlocals header: fat header, no locals (checkpoint 20261109)
- **const (no IL)** (2)

### `Tree.ComboBoxControlNodeInfo`
- **unblocked** (15): `ComboBoxControlNodeInfo()`, `ComboBoxControlNodeInfo(…)`×2, `DelegateRequestRestoreInsertionPoint(…)`, `IsEquivalentTo(…)`×2, `NotifyValueChanged()`, `RequestRestoreInsertionPoint`, `RequestUndoableCommandExecution`, `ValueChanged`, `ValueSExpression`, `a`, `b`, `bk()`, `c`
- **closure/lambda/iterator** (5): `+a.a_()`, `+a.b`, `+a.c`, `Data`, `Quire.EquationEditor.Tree.IComboBoxControlNodeInfo.Value`
- **field/method name clash** (2): `+a.a()`, `+a.a__`

### `Tree.ComboBoxInputControlData`
- **unblocked** (16): `Clone()`, `Cols`, `ComboBoxInputControlData()`, `CreateComboBoxInputControlData(…)`, `IsEquivalentTo(…)`, `IsValid()`, `SelectedValueRow`, `TableTitle`, `Units`, `Values`, `c`, `d`, `e`, `f`, `g`, `h`
- **closure/lambda/iterator** (29): `+a.a(…)`×2, `+a.a_()`, `+a.b(…)`, `+a.c(…)`, `+a.d(…)`, `+a.e(…)`, `+a.f(…)`, `+a.g(…)`, `+a.h(…)`, `+a.i(…)`, `+a.j(…)`, `+a.k(…)`, `+a.l(…)`, `+a.m`, `+a.n`, `+b.System.Collections.Generic.IEnumerable<Quire.EquationEditor.Tree.MathNode>.GetEnumerator()`, `+b.System.Collections.Generic.IEnumerator<Quire.EquationEditor.Tree.MathNode>.Current`, `+b.System.Collections.IEnumerable.GetEnumerator()`, `+b.System.Collections.IEnumerator.Current`, `+b.System.Collections.IEnumerator.MoveNext()`, `+b.System.Collections.IEnumerator.Reset()`, `+b.System.IDisposable.Dispose()`, `+b.b_(…)`, `+b.c`, `+b.e`, `+b.h`, `GroupByRows`, `Quire.EquationEditor.Tree.IComboBoxInputControlDataAdapter.Units`
- **field/method name clash** (33): `+a.a__`, `+a.b_`, `+a.c_`, `+a.d_`, `+a.e_`, `+a.f_`, `+a.g_`, `+a.h_`, `+a.i_`, `+a.j_`, `+a.k_`, `+a.l_`, `+b.a_`, `+b.b__`, `+b.d_`, `+b.f_`, `+b.g_`, `GetMatrixSExprFromSelection(…)`, `GetUnitedValuesForSelectedRowRl()`, `MakeEditable()`, `MakeReadOnly()`, `Rows`, `Titles`, `a()`, `a(…)`×5, `a__`, `b()`, `b(…)`, `b__`
- **`.initlocals` header** (4): `ComboBoxInputControlData(…)`×2, `GetValueByRowCol(…)`, `SelectedRowId`
- **ctor stores before base call** (2): `ComboBoxInputControlData(…)`×2

### `Tree.ComboBoxMathCreator`
- **unblocked** (1): `CreateUnitMath(…)`

### `Tree.DefaultDescriptorTable`
- **unblocked** (6): `ClearSymbolicFunctionOperator`, `Default`, `DefaultDescriptorTable()`, `a()`, `c()`, `d()`
- **closure/lambda/iterator** (8): `+b.a`, `+b.b`, `+b.c`, `+c.b`, `+c.c_()`, `a(…)`, `e()`, `f()`
- **field/method name clash** (2): `+c.a()`, `+c.a_`
- **`.initlocals` header** (1): `b()`
- **const (no IL)** (84)

### `Tree.FormattedString`
- **unblocked** (31): `+a.b()`, `+a.c`, `+a.d`, `Append(…)`×2, `Contains(…)`, `CreateFormattedStringFromSubscriptsList(…)`, `GetSubscriptDepthAtPosition(…)`, `Insert(…)`, `Item`, `MakeEditable()`, `RemoveRange(…)`, `ReplaceStringInFormattedString(…)`, `ReplaceStringInFormattedStringWithSubscripts(…)`, `SearchValueInFormattedStringScopes(…)`, `SelectCharacterAtPosition(…)`, `a(…)`×12, `b(…)`×2, `e`
- **field/method name clash** (6): `+a.a()`, `+a.a(…)`×3, `+a.a__`, `+a.b_`
- **`.initlocals` header** (6): `Clone()`, `MakeReadOnly()`, `Substring(…)`, `a(…)`×3
- **ctor stores before base call** (1): `+a.a_()`
- **const (no IL)** (4)

### `Tree.FunctionOperatorNames`
- **const (no IL)** (2)

### `Tree.IdentifierNodeInfo`
- **unblocked** (7): `Clone()`, `IsEquivalentTo(…)`×2, `Value`, `ValueSExpression`, `b`, `bk()`
- **closure/lambda/iterator** (3): `+a.a__()`, `+a.c`, `IdLabel`
- **field/method name clash** (4): `+a.a()`, `+a.a_`, `+a.b()`, `+a.b_`
- **`.initlocals` header** (3): `IdentifierNodeInfo(…)`×3
- **ctor stores before base call** (1): `IdentifierNodeInfo()`
- **needs manual IL match** (1): `algorithm`

### `Tree.InputControlData`
- **unblocked** (2): `Clone()`, `IsEquivalentTo(…)`
- **blocked in checkpoint** (1): `InputControlData()` — ctor stores fields before base Object call (checkpoint 20261109)

### `Tree.InputControlDataCalculatedValue`
- **blocked in checkpoint** (1): `HasCalculatedValue` — non-virtual call on virtual property (checkpoint 20261109)

### `Tree.InputControlNodeCreator`
- **unblocked** (2): `Create(…)`, `CreateInputField(…)`

### `Tree.InputControlNodeInfo`
- **unblocked** (9): `DelegateRequestRestoreInsertionPoint(…)`, `InputControlNodeInfo()`, `InputControlType`, `IsEquivalentTo(…)`, `NotifyValueChanged()`, `RequestRestoreInsertionPoint`, `ValueChanged`, `b`, `c`
- **closure/lambda/iterator** (4): `+a.a_()`, `+a.b`, `+a.c`, `Quire.EquationEditor.Tree.IInputControlNodeInfo.Value`
- **field/method name clash** (5): `+a.a()`, `+a.a__`, `RequestUndoableCommandExecution`, `a(…)`, `a__`

### `Tree.InputScriptableControlData`
- **blocked in checkpoint** (3): `Script`, `a(…)`, `a_` — field/method name clash (checkpoint 20261109b)

### `Tree.InputScriptableControlDataWithInputs`
- **blocked in checkpoint** (1): `Clone()` — initlocals header: fat header, no locals (checkpoint 20261109)

### `Tree.InputScriptableWithInputNodeInfo`
- **unblocked** (20): `DelegateRequestRestoreInsertionPoint(…)`, `InputControlType`, `InputScriptableWithInputNodeInfo(…)`×4, `InputsToRestore`, `IsInputsAreaShown`, `IsInputsExists`, `NotifyValueChanged()`, `RequestRestoreInsertionPoint`, `RequestUndoableCommandExecution`, `ValueChanged`, `ValueSExpression`, `a()`, `a(…)`, `b`, `c`, `d`, `e`
- **closure/lambda/iterator** (4): `+a.a_()`, `+a.b`, `+b.b`, `+b.b_()`
- **field/method name clash** (6): `+a.a()`, `+a.a__`, `+b.a()`, `+b.a_`, `Value`, `a__`
- **`.initlocals` header** (1): `Clone()`
- **ctor stores before base call** (1): `InputScriptableWithInputNodeInfo()`

### `Tree.LimitSideNodeInfo`
- **unblocked** (12): `IsEquivalentTo(…)`×2, `IsSExpressionOf(…)`, `IsValidInitializerString(…)`, `LimitSideNodeInfo()`, `LimitSideNodeInfo(…)`×2, `Value`, `ValueSExpression`, `b`, `bk()`, `c`
- **closure/lambda/iterator** (2): `+a.a()`, `+a.a__(…)`
- **field/method name clash** (1): `+a.a_`

### `Tree.ListBoxScriptableControlData`
- **blocked in checkpoint** (1): `Clone()` — initlocals header: fat header, no locals (checkpoint 20261109)

### `Tree.MathNode`
- **unblocked** (118): `+Utilities.AreTreesEqual(…)`, `+Utilities.CopyChildren(…)`, `+Utilities.DisplayIdLabelOfNode(…)`, `+Utilities.FindCommonAncestor(…)`, `+Utilities.FindNearestCommonContainerRoot(…)`, `+Utilities.GetEvaluationNodes(…)`, `+Utilities.HasParent(…)`, `+Utilities.IdLabelOfNode(…)`, `+Utilities.IsDefiniteIntegral(…)`, `+Utilities.IsMiddleOfSubscript(…)`, `+Utilities.MergeWithPreviousProgram(…)`, `+Utilities.MoveChildren(…)`, `+Utilities.NodeValueIsZero(…)`, `+Utilities.NodesToPaths(…)`, `+Utilities.PathsToNodes(…)`, `+Utilities.ToString(…)`, `+Utilities.ValidateSubTree(…)`, `+Utilities.a(…)`×5, `+Utilities.b(…)`×2, `+Utilities.c(…)`, `+a.System.Collections.Generic.IEnumerable<Quire.EquationEditor.Tree.MathNode>.GetEnumerator()`, `+a.a_(…)`, `+a.b`, `ActualMiddleElementDescriptor`, `AdjacentSubexpression(…)`, `AdjacentSubexpressionRoot`, `AlgorithmOptions`, `AlgorithmSExpression`, `AllowIdDescendantsLabelNoneBasedOnContext()`, `AlternateMiddleElementDescriptor`, `AreAllChildrenEmpty()`, `AreAllNonPrimalChildrenEmpty()`, `AutoLabelID()`, `AutoLabelIdDescendants(…)`, `ChildNotPrimal(…)`, `Clone()`, `ConsiderAsPrefixInInsertion`, `ContainerRoot`, `ContainsOrIsComboBox`, `CopyAlgorithm(…)`, `CreateNodeInfo(…)`, `DescendentFromPath(…)`×2, `ElementIndexOfChild(…)`, `ElementsCount`, `EquationRootNode`, `FirstElementDescriptor`, `ForceUnlabeledIdDescendantsToHaveLabelNone()`, `GetBranchIndexInAncestor(…)`, `GetFirstChildIndexOfFollowingRow(…)`, `GetFirstChildIndexOfRow(…)`, `GetMatrixCellIndex(…)`, `GetNearestAncestor(…)`×3, `GetNearestEvaluationAncestor()`, `GetNearestLinearNaryAncestor()`, `GetNearestNaryAncestor()`, `GetNearestVerticalNavigatableAncestor()`, `HasAdjacentSub(…)`, `HasContainerSlot`, `HasLastElement`, `InsertChildren(…)`, `IsAssociativeChildAContainerSlot`, `IsDescendentOf(…)`, `IsEffectiveInfix`, `IsEquationRoot`, `IsEquivalentTo(…)`, `IsInContainerSlot`, `IsInEvaluationResult`, `IsInNumericPartOfEvaluationResult`, `IsInSameFamily(…)`, `IsInSymbolicResult`, `IsInherentChildOfParent`, `IsNonLabeledIdDescendantAllowed`, `IsReadonly`, `IsRooted`, `IsScopeRoot`, `IsValid`, `LastElementDescriptor`, `LeftAdjacentSubexpression(…)`, `LeftNeighbor`, `LeftNeighbors`, `MakeEditable()`, `MakeReadOnly()`, `MathNode(…)`×2, `MergeNode(…)`, `MiddleElementDescriptor`, `NodeFromPath(…)`, `NodeInfoValueAsString`, `NodeInfoValueLength`, `PathFromAncestor(…)`, `PathFromEquationRoot()`, `PrimalChild()`, `RemoveChildren(…)`, `RightAdjacentSubexpression(…)`, `RootNode`, `SExpression`, `ScopeRoot`, `SetRlidForTree()`, `System.ICloneable.Clone()`, `ToString()`, `UpdateChildren(…)`, `c()`, `c(…)`, `d()`, `d(…)`, `e()`, `f`, `g`, `h()`, `i()`
- **closure/lambda/iterator** (49): `+a+a.a__()`, `+a+a.c`, `+a+a.d`, `+a+a.e`, `+a+a.f`, `+a+a.g`, `+a+b.System.Collections.Generic.IEnumerator<Quire.EquationEditor.Tree.MathNode>.Current`, `+a+b.System.Collections.IEnumerator.Current`, `+a+b.System.Collections.IEnumerator.MoveNext()`, `+a+b.System.Collections.IEnumerator.Reset()`, `+a+b.System.IDisposable.Dispose()`, `+a+b.b_(…)`, `+a+b.c`, `+b.b`, `+b.b_()`, `+c.b`, `+c.c`, `+c.c_()`, `+d.d_()`, `+e.e_()`, `+f.c`, `+f.f_()`, `+g.System.Collections.Generic.IEnumerable<Quire.EquationEditor.Tree.MathNode>.GetEnumerator()`, `+g.System.Collections.Generic.IEnumerator<Quire.EquationEditor.Tree.MathNode>.Current`, `+g.System.Collections.IEnumerable.GetEnumerator()`, `+g.System.Collections.IEnumerator.Current`, `+g.System.Collections.IEnumerator.MoveNext()`, `+g.System.Collections.IEnumerator.Reset()`, `+g.System.IDisposable.Dispose()`, `+g.e`, `+g.g_(…)`, `+h.System.Collections.Generic.IEnumerable<Quire.EquationEditor.Tree.MathNode>.GetEnumerator()`, `+h.System.Collections.Generic.IEnumerator<Quire.EquationEditor.Tree.MathNode>.Current`, `+h.System.Collections.IEnumerable.GetEnumerator()`, `+h.System.Collections.IEnumerator.Current`, `+h.System.Collections.IEnumerator.MoveNext()`, `+h.System.Collections.IEnumerator.Reset()`, `+h.System.IDisposable.Dispose()`, `+h.e`, `+h.h_(…)`, `+i.System.Collections.Generic.IEnumerable<Quire.EquationEditor.Tree.MathNode>.GetEnumerator()`, `+i.System.Collections.Generic.IEnumerator<Quire.EquationEditor.Tree.MathNode>.Current`, `+i.System.Collections.IEnumerable.GetEnumerator()`, `+i.System.Collections.IEnumerator.Current`, `+i.System.Collections.IEnumerator.MoveNext()`, `+i.System.Collections.IEnumerator.Reset()`, `+i.System.IDisposable.Dispose()`, `+i.e`, `+i.i_(…)`
- **field/method name clash** (186): `+Utilities.a_`, `+a+a.a()`, `+a+a.a_`, `+a+a.b()`, `+a+a.b_`, `+a+b.a_`, `+a+b.b__`, `+a+b.d_`, `+a.Count`, `+a.GetEnumerator()`, `+a.IndexOf(…)`, `+a.InsertRange(…)`, `+a.RemoveRange(…)`, `+a.ToArray()`, `+a.a(…)`×3, `+a.a___`, `+b.a()`, `+b.a_`, `+c.a()`, `+c.a_`, `+d.a(…)`, `+d.a_`, `+e.a(…)`, `+e.a_`, `+f.a()`, `+f.a_`, `+f.b()`, `+f.b_`, `+g.a()`, `+g.a_`, `+g.b()`, `+g.b_`, `+g.c_`, `+g.d_`, `+g.f_`, `+h.a()`, `+h.a_`, `+h.b()`, `+h.b_`, `+h.c_`, `+h.d_`, `+h.f_`, `+i.a()`, `+i.a_`, `+i.b()`, `+i.b_`, `+i.c_`, `+i.d_`, `+i.f_`, `Algorithm`, `AllChildSlotsAreContainerSlots`, `AllowCaretOn(…)`, `AllowCaretOnLeft`, `AllowCaretOnRight`, `AllowVerticalNavigation`, `AlsoRemoveParent`, `AlternateMiddleElementDescriptorName`, `AlternateOperatorSExpName`, `Alternates`, `Associativity`, `CarriesValue`, `CheckIsMultiline(…)`, `ChildCount`, `Children`, `ChildrenRows`, `ChildrenToRemoveOnDelete`, `ContainerSlots`, `ContainsOrIs(…)`, `DefaultNodeOverrides`, `DeleteParentInsteadOfNode`, `Descriptor`, `DescriptorName`, `DistinctNodeEdges`, `ExplicitValue`, `Family`, `FirstElementDescriptorName`, `Fixity`, `Group`, `HasCaretPositions`, `HasNonLabeledIdDescendants`, `HasVisualRepresentation`, `IgnoreOperandsOnOvertype`, `IndexInParent`, `InfoType`, `InherentChildren`, `IsAlternateFormPairNary`, `IsAncestorTreeAllowed(…)`, `IsBinOctHexModeOn`, `IsChildAllowed(…)`, `IsContainerSlot(…)`, `IsDescendentAllowed(…)`, `IsEvaluatable`, `IsEvaluation`, `IsFullContainer`, `IsIdentifier`, `IsImplicit`, `IsInfix`, `IsLeafOperator`, `IsLineSplit`, `IsLinearNary`, `IsLiteral`, `IsMonoLine`, `IsNary`, `IsNonLabeledId`, `IsPairNary`, `IsPairNarySubElement`, `IsParentAllowed(…)`, `IsPlaceholder`, `IsPrimalChild(…)`, `IsReadonlySlot(…)`, `IsRoot`, `IsScopable`, `IsScopingBoundary`, `IsSkipGenerateVisual`, `IsUnary`, `JumpIntoChildren`, `JumpThrough`, `Label`, `LastElementDescriptorName`, `LeftNeighborSExp`, `LineSpan`, `MatchRegex`, `MediateAncestorRestriction(…)`, `MediateChildInsertionDispute(…)`, `MediateParentRestriction(…)`, `MiddleElementDescriptorName`, `MitigateChildPatternProhibition(…)`, `MustScopeParent`, `NavigablePartCount`, `NavigablePartPositions`, `NavigatablePartRows`, `NoLogicOnlyVisual`, `NodeInfo`, `OperatorSExpName`, `OperatorSymbol`, `OperatorSymbolMargin`, `OperatorText`, `OperatorVisualRepresentation`, `Parent`, `ParentPairNaryDescriptorName`, `PostFenceNavigatable`, `PreFenceNavigatable`, `Precedence`, `PrimalChildPosition`, `PrimalChildTypeoverExtendDirections`, `ProhibitedAutoLabelIdDescendants`, `ProhibitsDescendants`, `RLFormat`, `RLID`, `ReplaceWithChildOnDelete`, `RestrictFromNavigation`, `RestrictsAncestors`, `RestrictsDescendants`, `RowCount`, `SExpFormat`, `SetConstantRlIdDownTheTree(…)`, `ShowOnlyWhenFocused`, `SplitNode(…)`, `SplitOposite`, `Temporal`, `TreeList`, `TreeListWithoutResultNodes`, `ValidValueRegex`, `ValueFixer`, `ValueType`, `WalkSubTree()`, `XMLFormat`, `a(…)`×7, `a__`, `b(…)`×2, `b__`, `c__`, `d__`, `e__`, `h__`, `i__`, `j_`
- **`.initlocals` header** (5): `+Utilities.InsertAsParent(…)`, `+Utilities.ReplaceNode(…)`, `+Utilities.a(…)`, `GetContainerRoot(…)`, `PairNaryNodeCoincideWithMiddleSubOperator(…)`
- **ctor stores before base call** (1): `MathNode(…)`
- **needs manual IL match** (2): `+a.Quire.EquationEditor.Tree.MathNodeItem`, `Quire.EquationEditor.Tree.MathNodeItem`

### `Tree.MathNodeDescriptor`
- **unblocked** (220): `AddDefaultNodeOverride(…)`, `AllChildSlotsAreContainerSlots`, `AllowCaretOn(…)`, `AllowCaretOnLeft`, `AllowCaretOnRight`, `AllowVerticalNavigation`, `AlsoRemoveParent`, `AlternateMiddleElementDescriptorName`, `AlternateOperatorSExpName`, `Alternates`, `Associativity`, `BinOctHexModeOn`, `CarriesValue`, `ChildCount`, `ChildrenRows`, `ChildrenToRemoveOnDelete`, `ContainerSlots`, `DefaultNodeOverrides`, `DeleteParentInsteadOfNode`, `DescriptorName`, `DistinctNodeEdges`, `ExplicitValue`, `Family`, `FirstElementDescriptorName`, `Fixity`, `Group`, `HasCaretPositions`, `HasContainerSlot`, `HasPanaceaux`, `HasVisualRepresentation`, `IgnoreOperandsOnOvertype`, `InfoType`, `InherentChildren`, `InitialChildPosition`, `IsAncestorTreeAllowed(…)`, `IsBinOctHexModeOn`, `IsChildAllowed(…)`, `IsContainerSlot(…)`, `IsDescendentAllowed(…)`, `IsEvaluatable`, `IsEvaluation`, `IsFullContainer`, `IsIdentifier`, `IsImplicit`, `IsInfix`, `IsLeafOperator`, `IsLinearNary`, `IsLiteral`, `IsNary`, `IsNonLabeledIdChildAllowed(…)`, `IsNonLabeledIdDescendantAllowed(…)`, `IsPairNary`, `IsPairNarySubElement`, `IsParentAllowed(…)`, `IsPlaceholder`, `IsReadonlySlot(…)`, `IsScopable`, `IsScopingBoundary`, `IsSkipGenerateVisual`, `IsUnary`, `JumpIntoChildren`, `JumpThrough`, `LastElementDescriptorName`, `LineSpan`, `MatchRegex`, `MediateAncestorRestriction(…)`, `MediateChildInsertionDispute(…)`, `MediateNonLabeledIdChildProhibition(…)`, `MediateNonLabeledIdDescendantProhibition(…)`, `MediateParentRestriction(…)`, `MiddleElementDescriptorName`, `MitigateChildPatternProhibition(…)`, `MustScopeParent`, `NavigablePartCount`, `NavigablePartPositions`, `NavigatablePartRows`, `NoLogicOnlyVisual`, `OperatorSExpName`, `OperatorSymbol`, `OperatorSymbolMargin`, `OperatorText`, `OperatorVisualRepresentation`, `ParentPairNaryDescriptorName`, `PostFenceNavigatable`, `PreFenceNavigatable`, `Precedence`, `PrimalChildPosition`, `PrimalChildTypeoverExtendDirections`, `ProhibitChild(…)`×2, `ProhibitChildren(…)`, `ProhibitDescendant(…)`×2, `ProhibitNonLabeledIdChild(…)`, `ProhibitNonLabeledIdDescendant(…)`, `ProhibitParent(…)`×2, `ProhibitedAutoLabelIdDescendants`, `ProhibitsDescendants`, `RLFormat`, `ReadonlySlots`, `ReplaceWithChildOnDelete`, `RestrictAncestor(…)`×2, `RestrictChild(…)`×2, `RestrictDescendant(…)`, `RestrictFromNavigation`, `RestrictParent(…)`×2, `RestrictsAncestors`, `RestrictsDescendants`, `RowCount`, `SExpFormat`, `SetAlternates(…)`, `SetChildrenRows(…)`, `SetChildrenToRemoveOnDelete(…)`, `SetContainerSlots(…)`, `SetInherentChildren(…)`, `SetNavigablePartPositions(…)`, `SetNavigatablePartRows(…)`, `SetPrimalChildTypeoverExtendDirections(…)`, `SetProhibitedAutoLabelIdDescendants(…)`, `SetReadonlySlots(…)`, `ShowOnlyWhenFocused`, `SplitOposite`, `Temporal`, `ToString()`, `ValidValueRegex`, `ValueFixer`, `ValueType`, `XMLFormat`, `a`, `a0`, `a1`, `a2`, `a3`, `a4`, `a5`, `a6`, `a7`, `a8`, `a9`, `aa`, `ab`, `ac`, `ad`, `ae`, `af`, `ag`, `ah`, `ai`, `aj`, `ak`, `al`, `am`, `an`, `ao`, `ap`, `aq`, `ar`, `au`, `av`, `aw`, `ax`, `ay`, `az`, `b`, `b0`, `b1`, `b2`, `ba`, `bb`, `bc`, `bd`, `be`, `bf`, `bg`, `bh`, `bi`, `bj`, `bk`, `bl`, `bm`, `bn`, `bo`, `bp`, `bq`, `bs`, `bt`, `bu`, `bv`, `bw`, `bx`, `by`, `bz`, `c`, `d`, `e`, `f`, `g`, `h`, `i`, `j`, `k`, `l`, `m`, `n`, `o`, `p`, `q`, `r`, `s`, `t`, `u`, `v`, `w`, `x`, `y`, `z`
- **`.initlocals` header** (1): `MathNodeDescriptor(…)`
- **needs manual IL match** (4): `SetValueFixer<T>()`, `as`, `at`, `br`

### `Tree.MathNodeDescriptorPiggyback`
- **unblocked** (1): `b4`

### `Tree.MathNodeDescriptorTable`
- **unblocked** (7): `Add(…)`, `FindMatchingValueDescriptor(…)`×2, `FunctionOperatorsList`, `IDLabelMapDictionary`, `IsMatchingDescriptor(…)`, `MathNodeDescriptorTable()`
- **closure/lambda/iterator** (21): `+a.a__(…)`, `+a.b`, `+b.System.Collections.Generic.IEnumerable<Quire.EquationEditor.Tree.IMathNodeDescriptor>.GetEnumerator()`, `+b.System.Collections.Generic.IEnumerator<Quire.EquationEditor.Tree.IMathNodeDescriptor>.Current`, `+b.System.Collections.IEnumerable.GetEnumerator()`, `+b.System.Collections.IEnumerator.Current`, `+b.System.Collections.IEnumerator.MoveNext()`, `+b.System.Collections.IEnumerator.Reset()`, `+b.System.IDisposable.Dispose()`, `+b.d`, `+c.System.Collections.Generic.IEnumerable<Quire.EquationEditor.Tree.IMathNodeDescriptor>.GetEnumerator()`, `+c.System.Collections.Generic.IEnumerator<Quire.EquationEditor.Tree.IMathNodeDescriptor>.Current`, `+c.System.Collections.IEnumerable.GetEnumerator()`, `+c.System.Collections.IEnumerator.Current`, `+c.System.Collections.IEnumerator.MoveNext()`, `+c.System.Collections.IEnumerator.Reset()`, `+c.System.IDisposable.Dispose()`, `+c.d`, `FormattedStringValueDescriptors`, `GetFunctionOperatorDescriptor(…)`, `ValueDescriptors`
- **field/method name clash** (14): `+a.a()`, `+a.a_`, `+b.a()`, `+b.a_`, `+b.b(…)`, `+b.b_`, `+b.c_`, `+b.e_`, `+c.a()`, `+c.a_`, `+c.b_`, `+c.c(…)`, `+c.c_`, `+c.e_`

### `Tree.MathNodeNames`
- **unblocked** (1): `AllLineBreakNodeNamesList`
- **const (no IL)** (98)

### `Tree.MathNodeToStringTransformer`
- **unblocked** (10): `TransformToTitleViewString(…)`, `TransformToUnitViewString(…)`, `TransformToValueViewString(…)`, `TransformToViewStringByType(…)`, `a(…)`×5, `b(…)`
- **const (no IL)** (3)

### `Tree.MatrixNodeInfo`
- **unblocked** (7): `CollapseNestedMatrices`, `Origin`, `ShowIndices`, `System.ICloneable.Clone()`, `ViewportOffset`, `ViewportSize`, `a(…)`
- **field/method name clash** (2): `MatrixFormat`, `a_`

### `Tree.Navigation.NavigatorState`
- **unblocked** (8): `CaretInsertionType`, `CaretSide`, `Current`, `EnterSide`, `NodeSide`, `OperatorPartIndex`, `ToString()`, `ValueRange`
- **ctor stores before base call** (2): `NavigatorState(…)`×2

### `Tree.NearestNeighborSelector`
- **unblocked** (1): `NearestNeighborSelector()`
- **blocked in checkpoint** (1): `SelectNeighbors(…)` — switch temp local slot ordering (checkpoint 20261109b)

### `Tree.Parsing.PanaceaVisitor`
- **unblocked** (14): `AutoLabelIdChildHandler`, `AutoLabelIdDescendantHandler`, `BinaryMergeValueChildrenHandler`, `InsertChildrenIntoParentHandler`, `PanaceaHandler`, `PullOutAdjacentDescendantFromChildHandler`, `ReparentChildHandler`, `ReparentNodeAndPullOutChildHandler`, `ReparentNodeHandler`, `ReplaceChildDescriptorHandler`, `ReplaceWithChildHandler`, `ReplaceWithDefaultNodeHandler`, `ReplaceWithHandler`, `TransformToHandler`

### `Tree.RadioButtonScriptableControlData`
- **blocked in checkpoint** (1): `Clone()` — initlocals header: fat header, no locals (checkpoint 20261109)
- **const (no IL)** (2)

### `Tree.RootMathNode`
- **unblocked** (19): `DelegateRequestRestoreInsertionPoint(…)`, `DispatchRequestUndoableCommandExecution(…)`, `FindDescendant(…)`, `FireMatrixDescendantAdded(…)`, `MatrixDescendantAdded`, `RequestNotifySubTreeNodeValueChanged(…)`, `RequestRestoreInsertionPoint`, `RequestUndoableCommandExecution`, `RootMathNode()`, `SExpression`, `ScopeRoot`, `SubTreeNodeValueChanged`, `a(…)`, `b(…)`, `k`, `l`, `m`, `n`, `o`
- **`.initlocals` header** (3): `RootMathNode(…)`×2, `TryFindDescendant(…)`

### `Tree.Scoping.DetachedTypeoverState`
- **blocked in checkpoint** (1): `DetachedTypeoverState(…)` — initlocals header: fat header, no locals (checkpoint 20261109)

### `Tree.Scoping.LinearSegment`
- **unblocked** (4): `IsInSegment(…)`, `a()`, `a(…)`, `c`
- **blocked in checkpoint** (2): `LinearSegment()`, `LinearSegment(…)` — ctor stores fields before base Object call (checkpoint 20261109)
- **const (no IL)** (2)

### `Tree.Scoping.NavigatorSegment`
- **unblocked** (12): `GetPartsOfOperatorInSegment(…)`, `HasNodes`, `IsInSegment(…)`, `LeftOperatorPart`, `Linearizable`, `NavigatorSegment(…)`, `RightNavigatorState`, `RightOperatorPart`, `a(…)`×3, `b`
- **field/method name clash** (5): `LeftNavigatorState`, `LeftNode`, `NavigatorSegmentFromLinearSegment(…)`, `RightNode`, `a_`
- **`.initlocals` header** (1): `SegmentIsSingleNode`

### `Tree.Scoping.ScopedNodes`
- **unblocked** (1): `FirstOrDefault(…)`
- **closure/lambda/iterator** (4): `+a.a_()`, `+b.b_()`, `All(…)`, `Any(…)`
- **field/method name clash** (4): `+a.a(…)`, `+a.a__`, `+b.a(…)`, `+b.a_`

### `Tree.Scoping.TypeoverState`
- **unblocked** (16): `AreMatrixCellsInTypeover()`, `Clone()`, `CloneForNewTree(…)`, `ExcludedNodes`, `ExcludedOperands`, `IsMatrixInTypeover()`, `MatrixCellsInTypeover`, `NavState`, `SExpression`, `System.ICloneable.Clone()`, `TypeoverNodes`, `TypeoverState(…)`×4, `d()`
- **closure/lambda/iterator** (25): `+a.a_()`, `+a.b`, `+b.b_()`, `+c.System.Collections.Generic.IEnumerable<Quire.EquationEditor.Tree.MathNode>.GetEnumerator()`, `+c.System.Collections.Generic.IEnumerator<Quire.EquationEditor.Tree.MathNode>.Current`, `+c.System.Collections.IEnumerable.GetEnumerator()`, `+c.System.Collections.IEnumerator.Current`, `+c.System.Collections.IEnumerator.MoveNext()`, `+c.System.Collections.IEnumerator.Reset()`, `+c.System.IDisposable.Dispose()`, `+c.c_(…)`, `+c.e`, `+d.System.Collections.Generic.IEnumerable<Quire.EquationEditor.Tree.MathNode>.GetEnumerator()`, `+d.System.Collections.Generic.IEnumerator<Quire.EquationEditor.Tree.MathNode>.Current`, `+d.System.Collections.IEnumerable.GetEnumerator()`, `+d.System.Collections.IEnumerator.Current`, `+d.System.Collections.IEnumerator.MoveNext()`, `+d.System.Collections.IEnumerator.Reset()`, `+d.System.IDisposable.Dispose()`, `+d.c`, `+d.d_(…)`, `+d.e`, `a()`, `b()`, `c()`
- **field/method name clash** (17): `+a.a(…)`, `+a.a__`, `+b.a(…)`, `+b.a_`, `+c.a()`, `+c.a_`, `+c.b()`, `+c.b_`, `+c.c__`, `+c.d_`, `+c.f_`, `+d.a_`, `+d.b_`, `+d.d__`, `+d.f_`, `IncludePrimalChildren`, `c__`
- **ctor stores before base call** (4): `TypeoverState(…)`×4

### `Tree.SliderScriptableControlData`
- **blocked in checkpoint** (2): `Clone()`, `SliderScriptableControlData(…)` — field-before-base / header blocker (checkpoint 20261109b); initlocals header: fat header, no locals (checkpoint 20261109)
- **const (no IL)** (2)

### `Tree.StringToComboBoxMathTransformer`
- **unblocked** (10): `b`, `c`, `d`, `e`, `f`, `g`, `h`, `i`, `j`, `k`
- **field/method name clash** (7): `SignExpr`, `TransformStringMathNode(…)`, `TransformStringToNumericMathTree(…)`, `a(…)`, `a_`, `bm(…)`, `bn(…)`

### `Tree.StringToComboBoxMathTransformerForConvertor`
- **unblocked** (3): `SignExpr`, `bm(…)`, `bn(…)`

### `Tree.TextBoxScriptableControlData`
- **blocked in checkpoint** (1): `Clone()` — initlocals header: fat header, no locals (checkpoint 20261109)
- **const (no IL)** (2)

### `Tree.Transform.ComboBoxInputControlDataToSExpressionTransform`
- **unblocked** (1): `a(…)`
- **closure/lambda/iterator** (10): `+a.a(…)`×2, `+a.a_()`, `+a.b(…)`, `+a.c`, `+a.d`, `+b.b()`, `Transform(…)`×2, `a(…)`
- **field/method name clash** (4): `+a.a__`, `+a.b_`, `+b.a(…)`, `+b.a_`

### `Tree.Transform.IResultTreeFormatter`
- **blocked in checkpoint** (12): `DisplayNumberFormat`, `Format(…)`, `FormatResult(…)`×8, `FormatUnitMonomial(…)`, `ResultMatrixFormat` — internal interface members cannot be abstract in C# (checkpoint 20261108)

### `Tree.Transform.InfixMonomialToSExpressionTransform`
- **unblocked** (11): `GetInstance(…)`, `b(…)`, `c(…)`, `d(…)`, `e(…)`, `f(…)`, `g(…)`, `l`, `m`, `n`, `o`
- **closure/lambda/iterator** (4): `+a.a(…)`, `+a.a_()`, `+a.b(…)`, `+a.c`
- **field/method name clash** (15): `+a.a__`, `+a.b_`, `Transform(…)`, `a()`, `a(…)`×2, `d_`, `e_`, `f_`, `h(…)`, `i(…)`, `i_`, `j_`, `k(…)`, `k_`
- **`.initlocals` header** (1): `j(…)`
- **ctor stores before base call** (1): `InfixMonomialToSExpressionTransform(…)`
- **const (no IL)** (5)

### `Tree.Transform.InlineToSExpressionTransform`
- **unblocked** (1): `e`
- **`.initlocals` header** (1): `InlineToSExpressionTransform()`
- **const (no IL)** (4)

### `Tree.Transform.MathNodeToRlTransformer`
- **unblocked** (30): `+PositionsToLabel.Label`, `+PositionsToLabel.Positions`, `+PositionsToLabel.PositionsToLabel(…)`, `+a.b(…)`, `Default`, `GetLabelRLSuffix(…)`×2, `GetRlCount(…)`, `GetRlIndexForEvaluationNode(…)`, `IsNumeric(…)`, `IsValidRl(…)`, `SetRlidAndTransform(…)`, `UnescapeIdentifier(…)`, `UnescapeLiteral(…)`, `ab`, `ac`, `ad`, `c(…)`×3, `d(…)`×3, `g(…)`, `i(…)`, `q(…)`, `t(…)`, `u(…)`, `y(…)`, `z(…)`
- **closure/lambda/iterator** (39): `+MathNodeToRlTransformerParams.Handle`, `+MathNodeToRlTransformerParams.HasError`, `+a+a.a__()`, `+a+a.b`, `+a+a.c`, `+a+a.d`, `+a+b.b_()`, `+b.a(…)`×2, `+b.b(…)`×2, `+b.b_()`, `+b.c`, `+b.d`, `+b.e`, `+c.b`, `+c.c_()`, `+d.d_()`, `+e.e_()`, `+f.f_()`, `+g.g_()`, `+h.h_()`, `+i.b`, `+i.c`, `+i.i_()`, `+j.b`, `+j.j_()`, `+k.k_()`, `+l.System.Collections.Generic.IEnumerable<Quire.EquationEditor.Tree.MathNode>.GetEnumerator()`, `+l.System.Collections.Generic.IEnumerator<Quire.EquationEditor.Tree.MathNode>.Current`, `+l.System.Collections.IEnumerable.GetEnumerator()`, `+l.System.Collections.IEnumerator.Current`, `+l.System.Collections.IEnumerator.MoveNext()`, `+l.System.Collections.IEnumerator.Reset()`, `+l.System.IDisposable.Dispose()`, `+l.d`, `+l.f`, `+l.l_(…)`, `ApplyRlParams(…)`
- **field/method name clash** (99): `+a+a.a(…)`, `+a+a.a_`, `+a+b.a(…)`, `+a+b.a_`, `+b.a_`, `+b.b__`, `+c.a(…)`, `+c.a_`, `+d.a(…)`, `+d.a_`, `+e.a(…)`, `+e.a_`, `+e.b(…)`, `+e.b_`, `+f.a(…)`, `+f.a_`, `+f.b(…)`, `+f.b_`, `+g.a(…)`, `+g.a_`, `+h.a(…)`, `+h.a_`, `+i.a()`, `+i.a_`, `+j.a()`, `+j.a_`, `+k.a(…)`, `+k.a_`, `+k.b(…)`, `+l.a()`, `+l.a_`, `+l.b_`, `+l.c_`, `+l.e_`, `EscapeIdentifier(…)`, `GetEvaluationNodeForRlIndex(…)`, `LabelForNoLabel`, `SetRlidAndTransformWithResultScalingFactor(…)`, `Transform(…)`×2, `TransformForAggregateWithReplacementScalingFactor(…)`, `TransformLhsOfDef(…)`, `a(…)`×18, `a__`, `aa_`, `b()`, `b(…)`×3, `b__`, `c__`, `d__`, `e(…)`, `e__`, `f(…)`, `f__`, `g__`, `h(…)`, `h__`, `i__`, `j(…)`, `j__`, `k(…)`, `k__`, `l(…)`, `l__`, `m(…)`, `n(…)`, `o(…)`, `p(…)`, `r(…)`, `s(…)`, `t_`, `u_`, `v(…)`, `v_`, `w(…)`, `w_`, `x(…)`, `x_`, `y_`, `z_`
- **`.initlocals` header** (1): `aa(…)`
- **ctor stores before base call** (2): `MathNodeToRlTransformer()`, `MathNodeToRlTransformer(…)`
- **needs manual IL match** (5): `+a.a(…)`×5
- **const (no IL)** (15)

### `Tree.Transform.MathNodeToSExpressionTransform`
- **unblocked** (16): `AlgoSexp`, `AlgoSexpFormat`, `Default`, `Genfit`, `IdLabelToken`, `IntegralSexp`, `MathNodeToSExpressionTransform()`, `MathNodeToSExpressionTransform(…)`, `Numol`, `Polyroots`, `ScaleSexpFormat`, `a()`, `a(…)`×2, `b(…)`, `e(…)`
- **field/method name clash** (8): `ShowResolvedIdLabels`, `Transform(…)`×2, `a_`, `b_`, `c(…)`, `c_`, `d(…)`
- **`.initlocals` header** (4): `a(…)`×2, `b(…)`×2

### `Tree.Transform.MathNodeToXmlTransform`
- **unblocked** (36): `+TranformNodeException.TranformNodeException(…)`, `+TranformNodeException.mathNode`, `+TranformNodeException.xmlDocument`, `AppendChildNodes(…)`, `ConvertChildNodes(…)`, `Default`, `MathNodeToXmlTransform()`, `MathNodeToXmlTransform(…)`, `TransformArgumentSeparator(…)`, `TransformArgumentsList(…)`, `TransformChildNodes(…)`, `TransformDefaultNode(…)`, `TransformDerivativeNode(…)`, `TransformInlineDivision(…)`, `TransformIntegralNode(…)`, `TransformLimitNode(…)`, `TransformLineBreakAddition(…)`, `TransformLineBreakGeneral(…)`, `TransformLineBreakInlineDivision(…)`, `TransformLineBreakMultiplication(…)`, `TransformLineBreakSubtraction(…)`, `TransformMatrix(…)`, `TransformNegationNode(…)`, `TransformOperatorNode(…)`, `TransformPlaceholderNode(…)`, `TransformRScaleNode(…)`, `TransformScaleNode(…)`, `TransformStepRange(…)`, `TransformStr(…)`, `TransformSymbolicEvaluation(…)`, `c(…)`, `d(…)`, `e(…)`, `f(…)`, `g(…)`, `h(…)`
- **closure/lambda/iterator** (47): `+a.a`, `+a.aa`, `+a.ab`, `+a.b`, `+a.c`, `+a.d`, `+a.e`, `+a.f`, `+a.g`, `+a.h`, `+a.i`, `+a.j`, `+a.k`, `+a.l`, `+a.m`, `+a.n`, `+a.o`, `+a.p`, `+a.q`, `+a.r`, `+a.s`, `+a.t`, `+a.u`, `+a.v`, `+a.w`, `+a.x`, `+a.y`, `+a.z`, `+b.b_()`, `+c.c_()`, `+d.d_()`, `+e.e_()`, `+f.a`, `+f.f_()`, `+g.b`, `+g.g_()`, `+h.b`, `+h.h_()`, `+i.a`, `+i.b`, `+i.i()`, `+j.b`, `+j.j()`, `+k.b`, `+k.k()`, `+l.b`, `+l.l()`
- **field/method name clash** (51): `+b.a(…)`, `+b.a_`, `+c.a(…)`, `+c.a_`, `+d.a(…)`, `+d.a_`, `+e.a(…)`, `+e.a_`, `+g.a(…)`, `+g.a_`, `+h.a(…)`, `+h.a_`, `+j.a(…)`, `+j.a_`, `+k.a(…)`, `+k.a_`, `+l.a(…)`, `+l.a_`, `IsBaseNumericNode(…)`, `IsCapitalBaseNumericNode(…)`, `Transform(…)`×2, `TransformEvaluation(…)`, `a()`, `a(…)`×20, `a__`, `b(…)`×4, `b__`, `c__`

### `Tree.Transform.ParsedSExpressionToComboBoxInputControlData`
- **unblocked** (2): `Transform(…)`×2
- **closure/lambda/iterator** (8): `+b.a(…)`, `+b.b()`, `+b.b__(…)`, `+b.c(…)`, `+b.d(…)`, `+b.e`, `a(…)`×2
- **field/method name clash** (4): `+b.a_`, `+b.b_`, `+b.c_`, `+b.d_`
- **const (no IL)** (8)

### `Tree.Transform.ParsedSExpressionToInlineTransform`
- **closure/lambda/iterator** (2): `+a.a_()`, `+a.f`
- **field/method name clash** (8): `+a.a__`, `+a.b_`, `+a.c_`, `+a.d_`, `+a.e_`, `a()`, `a(…)`, `a__`
- **ctor stores before base call** (1): `ParsedSExpressionToInlineTransform()`
- **const (no IL)** (1)

### `Tree.Transform.SExpressionParser`
- **unblocked** (7): `UnParse(…)`, `b()`, `c()`, `d()`, `f`, `g`, `h`
- **closure/lambda/iterator** (1): `+a.a_()`
- **field/method name clash** (15): `+a.a(…)`, `+a.a__`, `IsValid`, `Parsed`, `ReasonInvalid`, `SExpression`, `a()`, `a(…)`×2, `a__`, `b_`, `c_`, `d_`, `e()`, `e_`
- **ctor stores before base call** (1): `SExpressionParser()`

### `Tree.Transform.SExpressionToMathNodeTransform`
- **unblocked** (6): `Default`, `SExpressionToMathNodeTransform(…)`, `SafeTransform(…)`, `Transform(…)`, `e(…)`, `f(…)`
- **field/method name clash** (11): `a()`, `a(…)`×5, `a_`, `b(…)`, `b_`, `c(…)`, `d(…)`

### `Tree.Transform.SExpressionToRlTransform`
- **unblocked** (2): `SExpressionToRlTransform(…)`, `b`
- **closure/lambda/iterator** (3): `+a.a(…)`, `+a.a_()`, `+a.b`
- **field/method name clash** (4): `+a.a__`, `Transform(…)`, `a(…)`, `a__`

### `Tree.Transform.SExpressionUtility`
- **blocked in checkpoint** (5): `CleanWhitespace(…)`, `EscapeIdValue(…)`, `SExpressionUtility()`, `a_`, `b` — field/method name clash, InitializeArray blob name, CleanWhitespace stack shape (checkpoint 20261109b)

### `Tree.Transform.XmcdToMathNodeTransform`
- **unblocked** (6): `Default`, `Transform(…)`×2, `XmcdToMathNodeTransform()`, `XmcdToMathNodeTransform(…)`, `b(…)`
- **field/method name clash** (15): `BuildUnitMonomial(…)`, `a(…)`×7, `a_`, `b_`, `c(…)`, `d(…)`, `e(…)`, `f(…)`, `g(…)`
- **const (no IL)** (1)

### `Tree.Transform.XmlNodeToMathNodeTransform`
- **unblocked** (8): `Default`, `IsMC14Document(…)`, `NumberSetStrings`, `RemoveMinusSignFromString(…)`, `ScrapWrappingParenthesesFromExpression(…)`, `TransformToEditable(…)`, `o`, `q`
- **closure/lambda/iterator** (6): `+b.a`, `+b.b`, `+c.a(…)`, `+c.b`, `+c.c_()`, `+d.d_()`
- **field/method name clash** (75): `+c.a_`, `+d.a(…)`, `+d.a_`, `Transform(…)`, `a()`, `a(…)`×24, `a__`, `b(…)`×7, `c(…)`×5, `d(…)`×3, `d__`, `e(…)`×3, `e_`, `f(…)`×3, `f_`, `g(…)`×3, `g_`, `h(…)`×3, `i(…)`×3, `j(…)`×3, `k(…)`×3, `l(…)`, `m(…)`, `m_`, `n(…)`, `n_`
- **`.initlocals` header** (1): `XmlNodeToMathNodeTransform(…)`
- **const (no IL)** (14)

### `Tree.TransformMathNodeToComboBoxValues`
- **closure/lambda/iterator** (5): `+a.a()`, `+a.a__(…)`, `+a.b(…)`, `+a.c`, `CanTransform(…)`
- **field/method name clash** (2): `+a.a_`, `+a.b_`
- **`.initlocals` header** (1): `TransformToViewStrings(…)`

### `Tree.ValueNodeInfo`
- **unblocked** (3): `ValueSExpression`, `a`, `bk()`
- **closure/lambda/iterator** (4): `+a.a_()`, `+a.b`, `+b.b()`, `Value`
- **field/method name clash** (5): `+a.a()`, `+a.a__`, `+b.a()`, `+b.a_`, `+b.b_`

### `VisualOperatorDescriptorTable`
- **XAML** (5): `InitializeComponent()`, `System.Windows.Markup.IComponentConnector.Connect(…)`, `VisualOperatorDescriptorTable()`, `a(…)`, `a_`

### `Native.PersistentDataObjects.MathTransformer`
- **unblocked** (22): `HasUnitOverride`, `Math50Xml`, `MathNode`, `ResultNode`, `ResultRefId`, `ResultRefIdSpecified`, `SExpression`, `a`, `b`, `c`, `d`, `e`, `f`, `g`, `h`, `i`, `j`, `k`, `l`, `m`, `n`, `o`
- **ctor stores before base call** (1): `MathTransformer(…)`

### `Native.a`
- **unblocked** (1): `a()`
- **const (no IL)** (2)

### `Native.a0`
- **unblocked** (2): `br()`, `bs()`
- **field/method name clash** (13): `a()`, `a(…)`×2, `a_`, `b()`, `b_`, `c()`, `c_`, `d()`, `e()`, `f()`, `g()`, `h()`
- **ctor stores before base call** (1): `a0(…)`

### `Native.a2`
- **unblocked** (9): `+b.a()`, `+b.b_(…)`, `+b.e`, `+b.f`, `+b.g`, `+b.h`, `+b.i`, `+b.j`, `a2(…)`
- **closure/lambda/iterator** (23): `+c.a`, `+c.b`, `+c.c`, `+c.d`, `+d.d_()`, `+e.System.Collections.Generic.IEnumerable<Quire.Native.PtcEquationEditor.a2.a_>.GetEnumerator()`, `+e.System.Collections.Generic.IEnumerator<Quire.Native.PtcEquationEditor.a2.a_>.Current`, `+e.System.Collections.IEnumerable.GetEnumerator()`, `+e.System.Collections.IEnumerator.Current`, `+e.System.Collections.IEnumerator.MoveNext()`, `+e.System.Collections.IEnumerator.Reset()`, `+e.System.IDisposable.Dispose()`, `+e.c`, `+e.e_(…)`, `+f.System.Collections.Generic.IEnumerable<Quire.Native.PtcEquationEditor.a2.a_>.GetEnumerator()`, `+f.System.Collections.Generic.IEnumerator<Quire.Native.PtcEquationEditor.a2.a_>.Current`, `+f.System.Collections.IEnumerable.GetEnumerator()`, `+f.System.Collections.IEnumerator.Current`, `+f.System.Collections.IEnumerator.MoveNext()`, `+f.System.Collections.IEnumerator.Reset()`, `+f.System.IDisposable.Dispose()`, `+f.c`, `+f.f_(…)`
- **field/method name clash** (41): `+b.a_`, `+b.b__`, `+b.c()`, `+b.c_`, `+b.d()`, `+b.d_`, `+d.a(…)`, `+d.a_`, `+e.a_`, `+e.b_`, `+e.d_`, `+f.a_`, `+f.b_`, `+f.d_`, `a()`, `a(…)`×6, `a__`, `b()`, `b(…)`, `b__`, `c(…)`, `c__`, `d(…)`, `e(…)`, `f(…)`, `g(…)`, `h(…)`, `i(…)`, `j(…)`, `k(…)`, `l(…)`, `m(…)`, `n(…)`, `o(…)`, `p(…)`, `q(…)`
- **needs manual IL match** (1): `+b.b()`

### `Native.a3`
- **unblocked** (4): `a(…)`×2, `a3(…)`, `b`
- **field/method name clash** (1): `a_`

### `Native.a4`
- **unblocked** (9): `a4()`, `a4(…)`, `b(…)`, `c(…)`, `d(…)`, `e(…)`, `f(…)`, `g(…)`, `h(…)`
- **field/method name clash** (10): `a()`, `a(…)`×5, `a_`, `b_`, `c_`, `d_`

### `Native.a5`
- **unblocked** (2): `a5()`, `g`
- **field/method name clash** (19): `a()`, `a(…)`×6, `a_`, `b()`, `b(…)`, `b_`, `c()`, `c_`, `d()`, `d_`, `e()`, `e_`, `f()`, `f_`

### `Native.a6`
- **unblocked** (3): `a6(…)`, `b(…)`, `c`
- **closure/lambda/iterator** (8): `+a.a_()`, `+a.b`, `+a.c`, `+a.d`, `+a.e`, `+a.f`, `+a.g`, `+a.h`
- **field/method name clash** (25): `+a.a(…)`×13, `+a.a__`, `a(…)`×9, `a__`, `b_`

### `Native.a7`
- **unblocked** (2): `CleanValue(…)`, `a7()`

### `Native.a8`
- **unblocked** (3): `a`, `a8(…)`, `bq()`

### `Native.a9`
- **unblocked** (4): `a`, `a9(…)`, `b`, `c`
- **closure/lambda/iterator** (9): `+a.a_()`, `+a.b`, `+a.c`, `+a.d`, `+a.e`, `+a.f`, `+a.g`, `+a.h`, `bq()`
- **field/method name clash** (2): `+a.a(…)`, `+a.a__`

### `Native.aa`
- **unblocked** (6): `Accept(…)`, `Description`, `a`, `aa()`, `bo(…)`, `bp()`
- **needs manual IL match** (2): `Quire.Calculation.Core.IFunctionResultNode.Signature`, `Quire.Calculation.Core.IResultNode.ValueType`

### `Native.ab`
- **unblocked** (12): `Accept(…)`, `Columns`, `Count`, `Quire.Calculation.Core.IMatrixResultNode.Item`, `Size`, `Viewport`, `ViewportOffset`, `bo(…)`, `bp()`, `c`, `d`, `e`
- **field/method name clash** (7): `Rows`, `a(…)`×3, `a_`, `b(…)`, `b_`
- **`.initlocals` header** (1): `ab(…)`
- **needs manual IL match** (7): `Quire.Calculation.Core.IMatrixResultNode.Cols`, `Quire.Calculation.Core.IMatrixResultNode.DisplayValues`, `Quire.Calculation.Core.IMatrixResultNode.ElementUnits`, `Quire.Calculation.Core.IMatrixResultNode.IsRangeTable`, `Quire.Calculation.Core.IMatrixResultNode.Units`, `Quire.Calculation.Core.IResultNode.ValueType`, `Quire.Native.PtcEquationEditor.tItem`

### `Native.ac`
- **unblocked** (10): `Accept(…)`, `Columns`, `Quire.Calculation.Core.IMatrixResultNode.Item`, `Rows`, `SetElementAt(…)`, `Size`, `ac(…)`, `b`, `bo(…)`, `bp()`
- **field/method name clash** (3): `Count`, `a(…)`, `a_`
- **needs manual IL match** (7): `Quire.Calculation.Core.IMatrixResultNode.Cols`, `Quire.Calculation.Core.IMatrixResultNode.DisplayValues`, `Quire.Calculation.Core.IMatrixResultNode.ElementUnits`, `Quire.Calculation.Core.IMatrixResultNode.IsRangeTable`, `Quire.Calculation.Core.IMatrixResultNode.Units`, `Quire.Calculation.Core.IResultNode.ValueType`, `Quire.Native.PtcEquationEditor.tItem`

### `Native.ad`
- **unblocked** (11): `Accept(…)`, `Count`, `MatrixProxy`, `Quire.Calculation.Core.IMatrixResultNode.Item`, `Size`, `TransformValueToResult`, `ad(…)`, `bo(…)`, `bp()`, `c`, `d`
- **field/method name clash** (8): `Columns`, `Rows`, `a(…)`×3, `a_`, `b(…)`, `b_`
- **needs manual IL match** (7): `Quire.Calculation.Core.IMatrixResultNode.Cols`, `Quire.Calculation.Core.IMatrixResultNode.DisplayValues`, `Quire.Calculation.Core.IMatrixResultNode.ElementUnits`, `Quire.Calculation.Core.IMatrixResultNode.IsRangeTable`, `Quire.Calculation.Core.IMatrixResultNode.Units`, `Quire.Calculation.Core.IResultNode.ValueType`, `Quire.Native.PtcEquationEditor.tItem`

### `Native.ae`
- **unblocked** (16): `CreateComplexResultNode()`, `CreateComplexResultNode(…)`, `CreateDisplayMatrixResultNode(…)`, `CreateEagerMatrixResultNode(…)`, `CreateEmptyResultNode()`, `CreateFunctionResultNode(…)`, `CreateImaginaryResultNode()`, `CreateImaginaryResultNode(…)`, `CreateLazyMatrixResultNode(…)`, `CreateRealResultNode()`, `CreateRealResultNode(…)`, `CreateStringResultNode(…)`, `CreateSymbolicResultNode(…)`, `CreateUnitedValueResultNode()`, `a()`, `ae()`
- **field/method name clash** (1): `a_`

### `Native.af`
- **unblocked** (3): `af(…)`, `b()`, `b(…)`
- **field/method name clash** (5): `a()`, `a(…)`×3, `a_`
- **const (no IL)** (2)

### `Native.ag`
- **const (no IL)** (32)

### `Native.ah`
- **unblocked** (4): `Format(…)`, `System.ICloneable.Clone()`, `ah()`, `ah(…)`
- **field/method name clash** (47): `DisplayNumberFormat`, `FormatResult(…)`×8, `FormatUnitMonomial(…)`, `ResultMatrixFormat`, `a()`, `a(…)`×15, `b(…)`×6, `c(…)`×4, `c_`, `d(…)`×2, `d_`, `e(…)`×2, `e_`, `f(…)`×2, `f_`
- **needs manual IL match** (1): `Quire.EquationEditor.Tree.Transform.IResultTreeFormatter.Format(…)`
- **const (no IL)** (2)

### `Native.ai`
- **const (no IL)** (15)

### `Native.aj`
- **const (no IL)** (6)

### `Native.ak`
- **unblocked** (6): `+a.a_(…)`, `+a.b`, `+a.c`, `f()`, `g()`, `h()`
- **field/method name clash** (64): `+a.a(…)`, `+a.a__`, `a()`, `a(…)`×15, `a__`, `b()`, `b(…)`×6, `b_`, `c()`, `c(…)`×4, `c_`, `d()`, `d(…)`×3, `d_`, `e()`, `e(…)`×3, `e_`, `f_`, `g_`, `h_`, `i()`, `i_`, `j()`, `j_`, `k()`, `l()`, `m()`, `n()`, `o()`, `p()`, `q()`, `r()`, `s()`, `t()`, `u()`, `v()`, `w()`, `x()`
- **`.initlocals` header** (1): `ak()`

### `Native.al`
- **unblocked** (7): `Equals(…)`, `GetHashCode()`, `al(…)`×2, `b()`, `b(…)`, `e()`
- **field/method name clash** (6): `a()`, `a(…)`×2, `c()`, `d()`, `d_`
- **ctor stores before base call** (1): `al(…)`

### `Native.am`
- **unblocked** (2): `am(…)`, `c`
- **field/method name clash** (4): `a()`, `a_`, `b()`, `b_`

### `Native.an`
- **unblocked** (3): `an(…)`, `b(…)`, `c(…)`
- **field/method name clash** (2): `a(…)`, `a_`

### `Native.ao`
- **unblocked** (10): `a()`, `a(…)`, `ao()`, `b`, `c`, `d`, `e`, `f`, `g`, `h`
- **field/method name clash** (1): `a_`

### `Native.ap`
- **unblocked** (1): `ap(…)`
- **closure/lambda/iterator** (24): `+b.a`, `+b.b`, `+b.c`, `+b.d`, `+b.e`, `+b.f`, `+b.g`, `+b.h`, `+b.i`, `+b.j`, `+b.k`, `+c.b`, `+c.c_()`, `+d.b`, `+d.d_()`, `+e.System.Collections.Generic.IEnumerable<Quire.Native.PtcEquationEditor.ap.a_>.GetEnumerator()`, `+e.System.Collections.Generic.IEnumerator<Quire.Native.PtcEquationEditor.ap.a_>.Current`, `+e.System.Collections.IEnumerable.GetEnumerator()`, `+e.System.Collections.IEnumerator.Current`, `+e.System.Collections.IEnumerator.MoveNext()`, `+e.System.Collections.IEnumerator.Reset()`, `+e.System.IDisposable.Dispose()`, `+e.c`, `+e.e_(…)`
- **field/method name clash** (62): `+c.a(…)`, `+c.a_`, `+d.a(…)`, `+d.a_`, `+e.a_`, `+e.b_`, `+e.d_`, `a()`, `a(…)`×8, `a__`, `aa(…)`, `ab(…)`, `ac(…)`, `ad(…)`, `ae(…)`, `af(…)`, `ag(…)`, `ah(…)`, `ai(…)`, `aj(…)`, `ak(…)`, `al(…)`, `am(…)`, `an(…)`, `b()`, `b(…)`×3, `b__`, `c()`, `c(…)`, `c__`, `d(…)`, `e(…)`, `f(…)`, `g(…)`, `h(…)`, `i(…)`, `j(…)`, `k(…)`, `l(…)`, `m(…)`, `n(…)`, `o(…)`, `p(…)`, `q(…)`, `r(…)`, `s(…)`, `t(…)`, `u(…)`, `v(…)`, `w(…)`, `x(…)`, `y(…)`, `z(…)`

### `Native.aq`
- **unblocked** (6): `a`, `b`, `c`, `d`, `e`, `f`
- **ctor stores before base call** (1): `aq()`

### `Native.ar`
- **unblocked** (4): `ar()`, `ar(…)`×3
- **field/method name clash** (9): `a()`, `a_`, `b()`, `b_`, `c()`, `c_`, `d()`, `d_`, `e()`

### `Native.as`
- **unblocked** (8): `a(…)`×5, `b(…)`×3

### `Native.at`
- **closure/lambda/iterator** (14): `+a.a`, `+a.b`, `+b.b`, `+b.b_()`, `+c.Quire.Native.PtcEquationEditor.aq>>.Current`, `+c.Quire.Native.PtcEquationEditor.aq>>.GetEnumerator()`, `+c.System.Collections.IEnumerable.GetEnumerator()`, `+c.System.Collections.IEnumerator.Current`, `+c.System.Collections.IEnumerator.MoveNext()`, `+c.System.Collections.IEnumerator.Reset()`, `+c.System.IDisposable.Dispose()`, `+c.c`, `+c.c_(…)`, `+c.e`
- **field/method name clash** (27): `+b.a(…)`, `+b.a_`, `+c.a_`, `+c.b_`, `+c.d_`, `+c.f_`, `a(…)`×9, `a__`, `b(…)`, `b__`, `c(…)`, `c__`, `d(…)`, `d_`, `e(…)`, `f(…)`, `g(…)`, `h(…)`, `i(…)`
- **`.initlocals` header** (1): `j(…)`
- **ctor stores before base call** (1): `at(…)`

### `Native.au`
- **unblocked** (2): `a`, `b`
- **ctor stores before base call** (1): `au(…)`

### `Native.av`
- **unblocked** (2): `b`, `c`
- **field/method name clash** (3): `a(…)`×2, `a_`
- **ctor stores before base call** (2): `av(…)`×2

### `Native.aw`
- **unblocked** (9): `aw()`, `b(…)`×3, `c(…)`×2, `d(…)`, `e(…)`, `f(…)`
- **closure/lambda/iterator** (6): `+a.a(…)`, `+a.a_()`, `+a.b`, `+b.b`, `+b.b_()`, `+b.c`
- **field/method name clash** (18): `+a.a__`, `+b.a()`, `+b.a_`, `a(…)`×14, `a__`

### `Native.ax`
- **unblocked** (2): `ax(…)`, `c(…)`
- **closure/lambda/iterator** (12): `+a.a_()`, `+a.b`, `+b.b`, `+b.b_()`, `+c.b`, `+c.c_()`, `+d.b`, `+d.d()`, `+e.b`, `+e.e()`, `+f.b`, `+f.f()`
- **field/method name clash** (61): `+a.a()`, `+a.a__`, `+b.a()`, `+b.a_`, `+c.a()`, `+c.a_`, `+d.a()`, `+d.a_`, `+e.a()`, `+e.a_`, `+f.a()`, `+f.a_`, `a()`, `a(…)`×37, `a__`, `b(…)`×9, `b__`

### `Native.ay`
- **field/method name clash** (9): `a()`, `a(…)`×4, `a_`, `b()`, `c()`, `d()`
- **ctor stores before base call** (1): `ay()`
- **needs manual IL match** (1): `Quire.EquationEditor.Tree.Navigation.NavigatorStateItem`

### `Native.az`
- **unblocked** (6): `a(…)`×2, `az()`, `az(…)`, `b(…)`, `c(…)`

### `Native.ba`
- **unblocked** (1): `ba(…)`
- **field/method name clash** (9): `a()`, `a(…)`×2, `a_`, `b()`, `b_`, `bq()`, `c()`, `c_`

### `Native.bb`
- **unblocked** (5): `a()`, `a(…)`, `b`, `c`, `d`
- **field/method name clash** (2): `a_`, `bq()`
- **`.initlocals` header** (1): `bb(…)`

### `Native.bc`
- **unblocked** (7): `a()`, `a(…)`×2, `b()`, `bc(…)`, `c`, `d`
- **field/method name clash** (3): `a_`, `b_`, `bq()`

### `Native.bd`
- **unblocked** (2): `bd(…)`, `c`
- **field/method name clash** (7): `a()`, `a(…)`×2, `a_`, `b()`, `b_`, `bq()`

### `Native.be`
- **unblocked** (5): `a()`, `a(…)`, `b`, `be(…)`, `c`
- **field/method name clash** (2): `a_`, `bq()`

### `Native.bf`
- **unblocked** (5): `a()`, `b()`, `bf(…)`, `bq()`, `br()`

### `Native.bg`
- **unblocked** (4): `b`, `bg(…)`, `c`, `d`
- **field/method name clash** (4): `a()`, `a(…)`, `a_`, `bq()`

### `Native.bh`
- **unblocked** (6): `br()`, `bs()`, `d`, `e`, `f`, `g`
- **`.initlocals` header** (1): `bh(…)`

### `Native.bi`
- **unblocked** (8): `a`, `b`, `bi(…)`×2, `bq()`, `c`, `d`, `e`

### `Native.bj`
- **unblocked** (3): `b`, `bj(…)`, `c`
- **closure/lambda/iterator** (1): `+a.a_()`
- **field/method name clash** (7): `+a.a(…)`, `+a.a__`, `+a.b(…)`, `+a.b_`, `a(…)`, `a__`, `bq()`

### `Native.bk`
- **unblocked** (6): `a`, `b`, `bk(…)`, `c`, `d`, `e`
- **closure/lambda/iterator** (4): `+a.a(…)`, `+a.a_()`, `+a.b`, `bq()`
- **field/method name clash** (1): `+a.a__`

### `Native.bl`
- **unblocked** (6): `a(…)`×2, `b(…)`, `bl(…)`, `c(…)`, `d`
- **field/method name clash** (4): `a_`, `b_`, `bq()`, `c_`

### `Native.bm`
- **unblocked** (6): `a`, `b`, `bm(…)`, `bq()`, `c`, `d`

### `Native.bn`
- **unblocked** (5): `a`, `b`, `bq()`, `c`, `d`
- **`.initlocals` header** (1): `bn(…)`

### `Native.bo`
- **unblocked** (4): `b`, `bo(…)`, `c`, `d`
- **closure/lambda/iterator** (2): `+a.a_()`, `+a.c`
- **field/method name clash** (7): `+a.a()`, `+a.a__`, `+a.b()`, `+a.b_`, `a()`, `a__`, `bq()`

### `Native.bp`
- **unblocked** (1): `b(…)`
- **field/method name clash** (4): `a(…)`×3, `a_`
- **`.initlocals` header** (1): `bp(…)`
- **ctor stores before base call** (1): `bp(…)`

### `Native.bq`
- **field/method name clash** (34): `a()`, `a(…)`×8, `a_`, `b()`, `b(…)`×6, `b_`, `c()`, `c(…)`×6, `d()`, `d(…)`×4, `e()`, `e(…)`, `f()`, `f(…)`
- **`.initlocals` header** (2): `bq(…)`×2
- **ctor stores before base call** (2): `bq(…)`×2

### `Native.br`
- **unblocked** (2): `b`, `br(…)`
- **field/method name clash** (2): `a()`, `a_`

### `Native.bs`
- **unblocked** (2): `b>(…)`×2
- **field/method name clash** (2): `a__`, `b__`
- **needs manual IL match** (1): `bs()`

### `Native.d`
- **unblocked** (6): `a(…)`×5, `d()`

### `Native.f`
- **unblocked** (1): `b`
- **field/method name clash** (1): `a_`
- **`.initlocals` header** (1): `f()`

### `Native.g`
- **closure/lambda/iterator** (16): `+a.a`, `+a.b`, `+b.a`, `+b.b`, `+b.c`, `+b.d`, `+b.e`, `+b.f`, `+b.g`, `+c.b`, `+c.c`, `+c.c_()`, `+c.d`, `+c.e`, `+c.f`, `+d.d_()`
- **field/method name clash** (59): `+c.a(…)`×5, `+c.a_`, `+d.a(…)`, `+d.a_`, `a(…)`×12, `a__`, `aa(…)`, `ab(…)`, `ac(…)`, `ad(…)`, `b(…)`×3, `b__`, `c(…)`×3, `d(…)`×3, `e(…)`×2, `f(…)`×2, `g(…)`, `h(…)`, `i(…)`, `j(…)`, `k(…)`, `l(…)`, `m(…)`, `n(…)`, `o(…)`, `p(…)`, `q(…)`, `r(…)`, `s(…)`, `t(…)`, `u(…)`, `v(…)`, `w(…)`, `x(…)`, `y(…)`, `z(…)`
- **`.initlocals` header** (1): `g_(…)`
- **ctor stores before base call** (1): `+a.a_(…)`
- **needs manual IL match** (1): `g_(…)`

### `Native.j`
- **unblocked** (6): `Transform(…)`, `b`, `c`, `d`, `e`, `f`
- **closure/lambda/iterator** (4): `+a.a`, `+a.b`, `j(…)`×2
- **field/method name clash** (3): `a()`, `a(…)`, `a__`
- **ctor stores before base call** (1): `+a.a_(…)`

### `Native.k`
- **unblocked** (1): `k()`
- **field/method name clash** (8): `a()`, `a(…)`×2, `a_`, `b()`, `b(…)`, `b_`, `c()`

### `Native.l`
- **unblocked** (37): `@as(…)`, `ab(…)`, `ac(…)`, `ad()`, `ae()`, `af()`, `ag(…)`, `ah()`, `ai()`, `aj()`, `ak()`, `aq(…)`, `ar(…)`, `at()`, `au()`, `av(…)`, `aw()`, `b(…)`×2, `e()`, `f()`, `g()`, `h()`, `i()`, `j(…)`, `k(…)`, `m()`, `n()`, `o()`, `p(…)`, `q(…)`, `r(…)`, `s()`, `t()`, `u(…)`, `v(…)`, `w(…)`
- **closure/lambda/iterator** (28): `+a.a_()`, `+a.b`, `+b.b_()`, `+c.b`, `+c.c()`, `+d.d()`, `+e.b`, `+e.e_()`, `+f.b`, `+f.f_()`, `+g.b`, `+g.c`, `+g.d`, `+g.g_()`, `+h.b`, `+h.c`, `+h.d`, `+h.e`, `+h.h_()`, `+i.b`, `+i.c`, `+i.d`, `+i.e`, `+i.i_()`, `am()`, `an()`, `ao()`, `ap()`
- **field/method name clash** (39): `+a.a()`, `+a.a__`, `+b.a(…)`, `+b.a_`, `+c.a()`, `+c.a_`, `+d.a()`, `+d.a_`, `+d.b()`, `+d.b_`, `+e.a()`, `+e.a_`, `+f.a()`, `+f.a_`, `+g.a()`, `+g.a_`, `+h.a()`, `+h.a(…)`, `+h.a_`, `+i.a(…)`, `+i.a_`, `a()`, `a(…)`×9, `aa(…)`, `al(…)`, `b__`, `l(…)`×2, `x(…)`, `y(…)`, `z(…)`

### `Native.m`
- **unblocked** (44): `@as(…)`, `a()`, `a(…)`×3, `a0()`, `a1()`, `a2()`, `a3(…)`, `a5()`, `a7()`, `a8()`, `a9()`, `ad()`, `ae()`, `af()`, `ag(…)`, `al(…)`, `ao()`, `ap()`, `aq(…)`, `ar(…)`, `at()`, `au()`, `aw()`, `ax()`, `ay()`, `az()`, `e()`, `f()`, `g()`, `i()`, `j(…)`, `k(…)`, `m(…)`, `n()`, `q(…)`, `r(…)`, `s()`, `t()`, `u(…)`, `v(…)`, `w(…)`, `x(…)`
- **closure/lambda/iterator** (7): `+a.a(…)`, `+a.a_()`, `+a.b`, `a4(…)`, `a6()`, `am()`, `an()`
- **field/method name clash** (1): `+a.a__`

### `Native.n`
- **unblocked** (48): `@as(…)`, `a()`, `a(…)`×2, `a0()`, `a1()`, `a5()`, `a6()`, `a7()`, `a9()`, `ad()`, `ae()`, `af()`, `ag(…)`, `al(…)`, `am()`, `an()`, `ao()`, `ap()`, `ar(…)`, `at()`, `av(…)`, `b()`, `ba()`, `bb()`, `bc()`, `bd()`, `be()`, `bf(…)`, `bg(…)`, `bh()`, `bi()`, `bj()`, `c()`, `e()`, `f()`, `g()`, `j(…)`, `k(…)`, `n(…)`, `q(…)`, `r(…)`, `s()`, `t()`, `u(…)`, `v(…)`, `w(…)`, `x(…)`
- **closure/lambda/iterator** (5): `+a.a_()`, `+a.b`, `+a.c`, `+a.d`, `aa(…)`
- **field/method name clash** (2): `+a.a()`, `+a.a__`
- **`.initlocals` header** (1): `a(…)`

### `Native.o`
- **unblocked** (2): `a(…)`×2

### `Native.q`
- **unblocked** (2): `q(…)`×2
- **field/method name clash** (2): `a()`, `a_`

### `Native.r`
- **unblocked** (2): `e(…)`, `r(…)`
- **field/method name clash** (41): `a()`, `a(…)`×26, `a_`, `b()`, `b(…)`×4, `b_`, `c()`, `c(…)`×2, `c_`, `d()`, `d(…)`, `d_`

### `Native.s`
- **unblocked** (2): `c`, `s(…)`
- **field/method name clash** (8): `a()`, `a(…)`×3, `a_`, `b()`, `b(…)`, `b_`
- **ctor stores before base call** (1): `s(…)`

### `Native.t`
- **unblocked** (5): `Accept(…)`, `System.ICloneable.Clone()`, `bo(…)`, `bp()`, `t()`
- **needs manual IL match** (1): `Quire.Calculation.Core.IResultNode.ValueType`

### `Native.u`
- **unblocked** (4): `Accept(…)`, `bo(…)`, `bp()`, `u()`
- **needs manual IL match** (1): `Quire.Calculation.Core.IResultNode.ValueType`

### `Native.v`
- **unblocked** (8): `Accept(…)`, `AddUnit(…)`, `Quire.Calculation.Core.IUnitedValueResultNode.Value`, `Units`, `b`, `bo(…)`, `bp()`, `v()`
- **field/method name clash** (4): `a()`, `a(…)`×2, `a_`
- **needs manual IL match** (1): `Quire.Calculation.Core.IResultNode.ValueType`

### `Native.w`
- **unblocked** (5): `Accept(…)`, `a(…)`, `bo(…)`, `bp()`, `w()`
- **field/method name clash** (2): `Value`, `a_`
- **needs manual IL match** (1): `Quire.Calculation.Core.IResultNode.ValueType`

### `Native.x`
- **unblocked** (5): `Accept(…)`, `a(…)`, `bo(…)`, `bp()`, `x()`
- **field/method name clash** (2): `Value`, `a_`
- **needs manual IL match** (1): `Quire.Calculation.Core.IResultNode.ValueType`

### `Native.y`
- **unblocked** (8): `Accept(…)`, `IValue`, `RValue`, `a`, `b`, `bo(…)`, `bp()`, `y()`
- **needs manual IL match** (1): `Quire.Calculation.Core.IResultNode.ValueType`

### `Native.z`
- **unblocked** (6): `Accept(…)`, `Value`, `a`, `bo(…)`, `bp()`, `z()`
- **needs manual IL match** (1): `Quire.Calculation.Core.IResultNode.ValueType`
