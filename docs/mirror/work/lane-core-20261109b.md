# Lane Core — Quire.Core (Ptc.Core.dll)

- Owner: dispatched worker task_59012864c6a3. Branch `mirror/core-20261109b` from `main`.
- Assembly assigned by maintainer; no WORK.md row.
- Batch 1: Small Constant, Singleton and Value types (`ProductInfo.Constants`, `Services.ApplicationBehaviorConstant`, `Services.ApplicationVariant`, `Services.ApplicationBehaviorOptions`, `Services.FunctionalityService`, `Services.FunctionalityServiceWrapper`, `DirectionalPair<TLeft, TRight>`, `Xml.xmlStandardElement`, `Xml.XpsCustomNames`, `Serialization.XamlConstants`, `Serialization.TextXamlConstants`). Gate: PASS, 3997 checked. Report: `.mirror-cache/release-il-bb683a81def7/release-il.json`.
  - Shapes: constants and private backing fields implemented in hand-written partials and removed from `.g.cs`. `SparseMatrix<TItem>._emptyPosition` was tried in `Mirrored/Matrices.SparseMatrix_1.cs` but caused Roslyn lambda numbering shift in `get_Rows` / `get_Columns` (`b__16_0` vs Mathcad `b__17_0`), so reverted for separate handling.
