# Lane M — Quire.Calculation.Core

- Owner: Lane M (dispatch task_021e486c332f). Branch `mirror/m-calc-core-20261108-e` from `main`.
- Exact batches 1–3: 17 methods (recorded in commit `ab8d2f8`). Batch 4: the nine interface properties `IFunctionResultNode.Description`; `IMatrixResultNode.Item/Count/Size/Columns`; `ITrace2DResultNode.RangeAxisInfo/ResultAxisInfo/DataPoints/TraceResultType`.
- Exact batch 5: `ResultConverter.ConvertToIResultNode`; a local `rowCount = 1` before `GetLength(0)` reproduces Mathcad's operand evaluation order.
- Release IL: `python tools/validation/release_il.py --base main --project Quire.Calculation.Core` PASS, 573 checked, 0 failures. Report `.mirror-cache/release-il-1e744ea5d007/release-il.json`.
- Remaining Mathcad-paired mismatches from this lane: none of the members listed in batches 1–5.
