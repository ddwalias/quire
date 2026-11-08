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
