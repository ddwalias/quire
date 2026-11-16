# Lane M — Quire.Calculation.Core, Quire.Media3D

- Owner: Lane M (dispatch task_c76312c4c851). Branch `mirror/m-calc-core-20261116-f7` from `main`.
- Earlier batches 1–5 (branch `mirror/m-calc-core-20261108-e`) are landed and remain exact.
- Batch 6 members:
  - Stubs moved to hand-written partials: `SimpleResultNode.Accept`, `CalculatableWrapper._wrappedCalculatable`, `XmcdToIResultNodeTransform._resultVectorUnescapingXslt` and `ErrorAttributeConversionToEnum`.
  - `ResultNodeToXmcdTransform.WriteDisplayMatrixXml`, now exact: the negated `Equals` is a local, because the IL computes it as a value before branching.
  - The 8 Mathcad interface members no Quire member paired with: `ICalculatableContext.UnitSystemUpdated`, `RecalculateAllRequested`, `CalculateNowRequested` (add/remove), `IEngineError.Message` setter, `IStringResultNode.Value` setter. The product record `StringResultNode` gets a settable `Value` to keep implementing the interface.
- Release IL: `python tools/validation/release_il.py --base main --project Quire.Calculation.Core --project Quire.Media3D` PASS, 1218 checked, 1209 exact, 0 failures, 0 Mathcad members without a Quire pair.
- `Quire.Media3D`: no stubs, no mismatches under `--all`.
- Remaining in `Quire.Calculation.Core`, all without a Mathcad counterpart (so `--all` still fails on them):
  - Parked, 18 generated stubs: explicit implementations of Quire-only interface members on mirrored classes. `IMatrixResultNode.Cols/DisplayValues/Units/ElementUnits/IsRangeTable` on `DisplayMatrixSimpleResultNode`, `EagerMatrixSimpleResultNode`, `LazyMatrixSimpleResultNode`; `IFunctionResultNode.Signature` on `FunctionSimpleResultNode`; `ITrace2DResultNode.Traces` on `Trace2DResults.Trace2DResultNode`; `IResultNode.ValueType` on `SimpleResultNode`. Blocker: Mathcad's interfaces have no such members, so there is no IL to match. They go away when the members leave the interfaces (as commit `750f656c0` did for `DisplayValue`), which needs edits outside this lane: `src/Quire.EquationEditor/Tree/Transform/ResultFormatter.cs` reads them through the interfaces, and 11 generated files in other assemblies carry `IResultNode.ValueType` stubs.
  - 169 product-only members (D21) of the product types in `ResultNodes.cs` (`CalculationResult`, `EmptyResultNode`, `UnitedValueResultNode`, `StringResultNode`, `MatrixResultNode`, `FunctionResultNode`, `Trace2DResultNode`, `TraceData`, `UnitPower`, `EngineError(string, string)`/`Symbol`) and the Quire-only interface members above.
