# Lane Controls-20261109 — Quire.Controls (Ptc.Controls.dll)

- Owner: dispatched worker task_c7d2fb85c470. Branch `mirror/controls-20261109` from `main`.
- Assembly assigned by maintainer; no WORK.md row.
- Batch 1: `IO.ScaledEvaluation` (ctor, Create, ExtractUnits, GetComputedValue x3, ConstructCurrentRl, `_builder`, `EvalExpressionRlFormat`, `EvalScaledExpressionRlFormat`). Gate: PASS, 333 checked. Report: `.mirror-cache/release-il-18c272f96625/release-il.json`.
  - Shapes that failed first: field initializer for `_builder` ran before the base ctor call (Release IL: base call first) → assigned in ctor body. `ExtractUnits` local order: result before unitedResult. `GetComputedValue` out-var declared as `IList<Unit> resultUnits;` before `result`-slot order fix → exact.
