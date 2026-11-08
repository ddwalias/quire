# Equation Editor lane checkpoint (20261109)

- Owner: EquationEditor dispatched worker (task_794338cc4f09)
- Branch: `mirror/equation-editor-20261109`
- Assembly: `Quire.EquationEditor` (managed members, `src/Quire.EquationEditor`)

## Batches

- Batch 1: `Tree.Parsing.Rule` (fields, 4 ctors, `ApplyToAllSlots`), `Tree.SymbolMargin` (fields, ctor, implicit operator), `Tree.Transform.WorksheetXmlTransformArgs` (ctor, two getters), `Tree.FunctionOperatorNames` (ctor). Release IL PASS, 235 checked, 0 failures (`.mirror-cache/release-il-d4214f673c26/release-il.json`). Shapes: `Left = Right = margin;` in SymbolMargin ctor, `this(...)` chaining in Rule ctors (matched first try).

## Remaining

- About 5000 `MirrorStub` attributes remain in `src/Quire.EquationEditor/Generated/Mirror` across ~160 files; mostly `Equation.EquationModel`, `Tree.MathNode`, `SpecTable.Model`, `Tree.MathNodeDescriptor`, transforms and `Native.PtcEquationEditor.*` classes. Not yet batched beyond batch 1.
- Constants (`const` fields) keep their stub attribute; they have no IL.
- Blocked (from the earlier lane, `Tree.Transform.IResultTreeFormatter`): `internal` members on a public interface cannot be declared abstract in C#; needs a decision on shape.
- Batch 2: `Tree.MathNodeDescriptorPiggyback` (field, ctor, `DescriptorName`), `Tree.ListBoxScriptableControlData` (constants, fields `c`/`d`, `Sort`, `FlowDirection`, ctor). Release IL PASS, 242 checked, 0 failures (`.mirror-cache/release-il-9ad97085833a/release-il.json`). Field names `c`, `d`, `a`, `b` are the obfuscated originals and stay (IL field names compare).
- Not landed: `ListBoxScriptableControlData.Clone` (object initializer, idiomatic). Gate fails only on the method header: Mathcad has `.initlocals True` with no locals (fat header, 185-byte body), while a Roslyn build of a method with no locals has `.initlocals False` (`.mirror-cache/release-il-ff4c3ad13de3/release-il.json`; `il_diff.py` shows only instruction #63). Adding a local changes `.locals`, so neither shape matches. Kept as stub; likely an obfuscator artifact. Needs a decision.
- Batch 3: `Tree.RequestRestoreInsertionPointEventArgs` (field, ctor, two properties), `Tree.Transform.EquationXmlParseErrorException` (field, two ctors, `ErrorCode`), `Tree.StringValueNodeInfo` (three ctors, `new` `Value`, `ValueSExpression`, `bk`, `Clone`). Release IL PASS, 258 checked, 0 failures (`.mirror-cache/release-il-65feafe8d740/release-il.json`). Shapes that matched first try: `new StringValueNodeInfo { Value = Value }` in `Clone`, `base.Value` for the hidden `object` property, and `other.Value.ToString()` (FormattedString override). The inner-exception ctor ignores `errorCode`, as the IL does.
- Batch 4: `VisualOperatorDescriptor` (fields `a`–`e`, five properties, empty ctor), `Tree.ComboBoxMathCreator` (`Default`, ctor, `CreateStringMathNode`, `CreateValueMath`), `Tree.Scoping.DetachedTypeoverState.GetClosestTypeoverState`. Release IL PASS, 273 checked, 0 failures (`.mirror-cache/release-il-bdefe73227ad/release-il.json`).
- Not landed, blocked by the header flag (same class as `ListBoxScriptableControlData.Clone`): `DetachedTypeoverState` ctor. Mathcad's fat header has `.initlocals True` with zero locals (code 0x58 bytes); the Roslyn build of the same shape has `.initlocals False`. Shapes tried: `if (a) return;` (branch sense differs, release-il-7a2e55f10ecd), `if (!a) { ... }` (flag differs only, release-il-d16cf79fe7ee). Stub kept. Likely a systematic property of fat methods without locals in this assembly; needs a decision.
- Not attempted: `ComboBoxMathCreator.CreateUnitMath` (the `??`/identifier-label shape needs a single local that Mathcad reuses; unclear).
- Batch 5: `Tree.FunctionOperatorDescriptor` (fields `a`–`d`, four properties, ctor), `Tree.ButtonScriptableControlData` and `Tree.CheckBoxScriptableControlData` constructors (`Height = a; Width = b;`, consts stay in the generated file). Release IL PASS, 283 checked, 0 failures (`.mirror-cache/release-il-0dd0bd73d661/release-il.json`).
- Not landed: `MathNodeSettings` (`Instance`, `AutoLabelIds`). Mathcad calls the obfuscated `Properties.Settings.b()` and `a()`, Quire calls `get_Default`/`get_AutoLabelIds` (release-il-0f98f872dc79). Needs the Settings mirror mapping decided; stub kept.
- Batch 6: `Tree.InputControlData` properties (`InputControlType`, `OutputFieldSize`, `Background`, `Foreground`, `FontSize`) and `Tree.InputControlDataCalculatedValue` (nine virtual properties with fields `f`–`m`, protected ctor). Release IL PASS, 309 checked, 0 failures (`.mirror-cache/release-il-e3846239cd9a/release-il.json`).
- Not landed, stubs kept: `InputControlDataCalculatedValue.HasCalculatedValue` (Mathcad reads `SelectedCalculatedValue` with a non-virtual `call`; the property must stay virtual because `Controls.EquationControl` overrides `EngineType`; release-il-1b1037c15c54), and `InputControlData` ctor (Mathcad stores fields before the base `Object` call, which C# cannot express).
