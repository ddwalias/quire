# Equation Editor lane checkpoint (20261109b)

- Owner: EquationEditor dispatched worker (task_55cd8e4686f2)
- Branch: `mirror/equation-editor-20261109b`
- Assembly: `Quire.EquationEditor` (managed members, `src/Quire.EquationEditor`)

## Batches

- Batch 8: `Tree.Transform.SExpressionUtility.EscapeStringValue` and private `a(string)`; `Tree.SExpressionHelperUtilites` (`a(string)`, `ExtractIdOrFunctionDefinitionName`, `IsIdentifierSExpr`, `IsValidType`, `SplitScalingAndValue`); `Tree.NodeInfoExtensions` (`ValueLength`, `ValueAsString`, `ClearValue`, `SetValue`). Release IL PASS, 324 checked, 0 failures (`.mirror-cache/release-il-055dcfc0a2d1/release-il.json`). Shape that matched: type-pattern `switch` on `INodeInfo` for NodeInfoExtensions (the `if (!(x is T t))` form failed with the wrong block order: `calls differ at #0`, release-il-caa3ad17f3ab).

## Blocked (stubs kept)

- `Tree.Transform.SExpressionUtility.EscapeIdValue`, `CleanWhitespace`, the `a_`/`b` static fields and the cctor. Three independent causes, each seen in the gate (release-il-58c975d22d22):
  - Field name: the IL field is `a` (ILSpy shows `m_a`), but `a` clashes with method `a` in C#, so MirrorGen names it `a_`. The gate compares the field name (`ldsfld ...::a_` vs `::a`).
  - cctor: the constant `char[]` uses `RuntimeHelpers.InitializeArray` with blob type `bs+a`. Dotfuscator renamed `<PrivateImplementationDetails>` to `bs`, and Roslyn emits `<PrivateImplementationDetails>+__StaticArrayInitTypeSize=10`.
  - `CleanWhitespace`: Mathcad calls `Regex.Matches` and keeps the `MatchCollection` on the evaluation stack while it runs `new StringBuilder()`, `start = 0`, then `GetEnumerator()`. A `foreach` over the call with a `StringBuilder` declared first fails with `calls differ at #1`. Needs a maintainer decision.
- Field/method name clash in general (MirrorGen `_` suffix): any Mathcad type with a field and a method of the same obfuscated name (`a`) cannot match until the name compare handles it. Needs a decision.
- Constant array initializers (`InitializeArray` blob named `bs+a`) are blocked for the same reason in any type.

## Remaining

- About 4610 non-const stub-marked members (stub count includes `const` fields, which keep their attribute). Largest: `Equation.EquationModel`, `Tree.MathNode`, `SpecTable.Model.*`, `Tree.Transform.*ToXml/RlTransform`, `Native.PtcEquationEditor.*` (obfuscated classes). Not batched.
- Earlier blocked shapes from the 20261109 checkpoint still apply: `.initlocals` header flag (`ListBoxScriptableControlData.Clone`, `DetachedTypeoverState` ctor), constructors that store fields before the base `Object` call (`LinearSegment()`), and `InputControlDataCalculatedValue.HasCalculatedValue`.
