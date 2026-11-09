# Equation Editor lane checkpoint (20261109b)

- Owner: EquationEditor dispatched worker (task_ab0a80ddc63d)
- Branch: `mirror/equation-editor-20261109b`
- Assembly: `Quire.EquationEditor` (managed members, `src/Quire.EquationEditor`)

## Batches

- Batch 8: `Tree.Transform.SExpressionUtility.EscapeStringValue` and private `a(string)`; `Tree.SExpressionHelperUtilites` (`a(string)`, `ExtractIdOrFunctionDefinitionName`, `IsIdentifierSExpr`, `IsValidType`, `SplitScalingAndValue`); `Tree.NodeInfoExtensions` (`ValueLength`, `ValueAsString`, `ClearValue`, `SetValue`). Release IL PASS, 324 checked, 0 failures (`.mirror-cache/release-il-055dcfc0a2d1/release-il.json`). Shape that matched: type-pattern `switch` on `INodeInfo` for NodeInfoExtensions (the `if (!(x is T t))` form failed with the wrong block order: `calls differ at #0`, release-il-caa3ad17f3ab).

- Batch 9: `Tree.NearestNeighborSelector.SelectNeighbors(MathNode)` (one-argument overload) and `Tree.Parsing.MathNodeAnalysisHandle` (`Node`, `EffectiveFixity`, `EffectiveAssociativity` fields and both constructors, as a partial struct). Release IL PASS, 327 checked, 0 failures (`.mirror-cache/release-il-d95d46d29e40/release-il.json`).

- Batch 10: `Tree.InputControlDataCreator.Create` (type-pattern-free `switch` over `InputControlTypes` with six scriptable-control constructors and a default `DevAssert`). Release IL PASS, 328 checked, 0 failures (`.mirror-cache/release-il-842b633328f6/release-il.json`).
- Not attempted: `Tree.InputControlNodeCreator.Create` and `CreateInputField` call the obfuscated `Native.PtcEquationEditor.r` (`r.a.a(...)`, `r.d(...)`). Stubs kept until that class's mapping is reviewed.

- Batch 11: `Tree.CheckBoxScriptableControlData.FlowDirection` (field `c` and property); `Tree.TextBoxScriptableControlData` `AcceptsReturn`, `IsReadOnly`, `Border`, `TextAlignment` (fields `c`, `d`, `n`, `o` and properties). Release IL PASS, 338 checked, 0 failures (`.mirror-cache/release-il-d084b066a885/release-il.json`). The CheckBox constructor from batch 5 is kept in the same partial file.
- Not attempted: `RadioButtonScriptableControlData` `Orientation` and `FlowDirection` (`Orientation` has a field initializer, which may need a constructor shape), and the TextBox constructor (it sets `Values = new Collection<string> { ... }`).

- Batch 12: `Tree.RadioButtonScriptableControlData` constructor (`Height = a; Width = b;` after the base call, with the field initializer `Orientation.Vertical` for `c`), `Orientation` and `FlowDirection` properties (fields `c`, `d`). Release IL PASS, 343 checked, 0 failures (`.mirror-cache/release-il-1dc11e8d9302/release-il.json`). The field initializer matched the Roslyn shape as written.
- Still stubbed: `Clone` on all scriptable control data types (object initializer, see the `.initlocals` blocker).

- Batch 13: `Tree.MatrixNodeInfo` `IsMatrixComplete` (IL field `b`, Quire name `b`; the generator's `b_` was dropped) and `FullSize` (field `c`), plus `RowsBeforeViewport`, `ColsBeforeViewport`, `RowsAfterViewport`, `ColsAfterViewport`. Release IL PASS, 351 checked, 0 failures (`.mirror-cache/release-il-1dedc0c1aeea/release-il.json`). The first gate (release-il-a29a4ce9bba7, 347 checked) passed with `IsMatrixComplete` and `FullSize` only.
- Not attempted in MatrixNodeInfo: `ViewportSize`, `ViewportOffset`, `ShowIndices`, `CollapseNestedMatrices`, `Origin`, `MatrixFormat`, all of which read the field `a` (IL name `a`), which the generator renames to `a_`. This is the same clash as SExpressionUtility. `Remainder` and `ActualSize` are not attempted.

- Batch 14: `Tree.IdentifierNodeInfo` `ResolvedIdLabel` (field `c`), `HasNoLabel` (`IdLabel.IsNullOrNone()`), `IdLabelForDisplay`. Release IL PASS, 355 checked, 0 failures (`.mirror-cache/release-il-363060715199/release-il.json`). `IdLabel` (undo command buffer) and the nested `a` class are not attempted.

## Blocked (stubs kept)

- `Tree.NearestNeighborSelector.SelectNeighbors(MathNode, out bool)` (virtual, switch on `Fixity` with an `out` parameter). Mathcad's switch temporary is local 3, after the three block locals; C# put it in slot 0 (braced cases, release-il-23d8111cb024) and slot 1 (unbraced `Special` case, release-il-1a7e6d2256e8). Shapes not yet tried: different local declaration order. Stub kept.
- `Tree.Transform.SExpressionUtility.EscapeIdValue`, `CleanWhitespace`, the `a_`/`b` static fields and the cctor. Three independent causes, each seen in the gate (release-il-58c975d22d22):
  - Field name: the IL field is `a` (ILSpy shows `m_a`), but `a` clashes with method `a` in C#, so MirrorGen names it `a_`. The gate compares the field name (`ldsfld ...::a_` vs `::a`).
  - cctor: the constant `char[]` uses `RuntimeHelpers.InitializeArray` with blob type `bs+a`. Dotfuscator renamed `<PrivateImplementationDetails>` to `bs`, and Roslyn emits `<PrivateImplementationDetails>+__StaticArrayInitTypeSize=10`.
  - `CleanWhitespace`: Mathcad calls `Regex.Matches` and keeps the `MatchCollection` on the evaluation stack while it runs `new StringBuilder()`, `start = 0`, then `GetEnumerator()`. A `foreach` over the call with a `StringBuilder` declared first fails with `calls differ at #1`. Needs a maintainer decision.
- Field/method name clash in general (MirrorGen `_` suffix): any Mathcad type with a field and a method of the same obfuscated name (`a`) cannot match until the name compare handles it. Needs a decision.
- Constant array initializers (`InitializeArray` blob named `bs+a`) are blocked for the same reason in any type.

## Remaining

- About 4610 non-const stub-marked members (stub count includes `const` fields, which keep their attribute). Largest: `Equation.EquationModel`, `Tree.MathNode`, `SpecTable.Model.*`, `Tree.Transform.*ToXml/RlTransform`, `Native.PtcEquationEditor.*` (obfuscated classes). Not batched.
- Earlier blocked shapes from the 20261109 checkpoint still apply: `.initlocals` header flag (`ListBoxScriptableControlData.Clone`, `DetachedTypeoverState` ctor), constructors that store fields before the base `Object` call (`LinearSegment()`), and `InputControlDataCalculatedValue.HasCalculatedValue`.

- Batch 15: `Tree.FormattedString.Length`, `IsSimpleString`, `GetHashCode`, `ToString` (fields `g`, `h`, `j`). Release IL PASS, 359 checked, 0 failures (`.mirror-cache/release-il-a61c8b78b70a/release-il.json`). Initial gate `release-il-3361fcff3e22` failed compilation because regeneration needs compiled hand-written metadata; targeted MirrorGen writer regeneration removed the duplicate stubs. Idiomatic `Length` failed only `.initlocals True` versus Mathcad `False` (release-il-a852884b9af7; il_diff instruction #26); getter-level `SkipLocalsInit` matched, with `AllowUnsafeBlocks` enabled for the attribute. No remaining mismatches in this batch.
