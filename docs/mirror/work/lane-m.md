# Lane M — Quire.Calculation.Core

- Owner: Lane M (dispatch task_3c4e0e298dab). Branch `mirror/m-calc-core-20261108-d` from `main`.
- Exact batches 1–3: 17 methods (recorded in commit `ab8d2f8`). Batch 4: the nine interface properties `IFunctionResultNode.Description`; `IMatrixResultNode.Item/Count/Size/Columns`; `ITrace2DResultNode.RangeAxisInfo/ResultAxisInfo/DataPoints/TraceResultType`.
- Release IL: `python tools/validation/release_il.py --base main --project Quire.Calculation.Core` PASS, 592 checked, 0 failures. Report `.mirror-cache/release-il-9d2ec69a14ee/release-il.json`.
- Remaining: `ResultConverter.ConvertToIResultNode` is still a stub; the attempted body differed at the flat-matrix constructor, where Mathcad evaluates the leading `1` before `GetLength(0)` and the C# local form evaluates `GetLength(0)` first.
