# Lane M — Quire.Calculation.Core

- Owner: Lane M (dispatch task_55d65c9d912c). Branch `mirror/m-calc-core-20261107-c` from `main`.
- Exact batches: 1 (8 methods), 2 (3 methods), 3 (6 methods). Batch 3: `XmcdToIResultNodeTransform.BuildPlotTraceResult`, `BuildAxisInfo`, `BuildMatrix`, `BuildDisplayMatrix`, `BuildUnitedValue`; `ResultNodeToXmcdTransform.WriteDisplayMatrixXml`.
- Release IL: `python tools/validation/release_il.py --base main --project Quire.Calculation.Core` PASS, 572 checked, 0 failures. Report `.mirror-cache/release-il-5bead27548b6/release-il.json`.
- Remaining Mathcad-paired mismatches: `ResultConverter.ConvertToIResultNode` (flat-matrix constructor operand evaluation shape); interface stubs `IFunctionResultNode.Description`, `IMatrixResultNode.Item/Count/Size/Columns`, `ITrace2DResultNode.RangeAxisInfo/ResultAxisInfo/DataPoints/TraceResultType`.
- 218 Quire-only bodies remain unpaired and unchanged. `MirrorGen emit` was not run because it rewrites unrelated project files in this checkout; generated stubs were removed by hand where implemented.
