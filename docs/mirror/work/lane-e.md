# Lane E — Quire.Mpl

- Owner: Lane E (Claude).
- Branch: `mirror/e-mpl-20261021-e7`.
- Selected members:
  - `Quire.Mcd.MatVal`: `get_RepHasPtrs`, `contains`, `get_IsScalarMatrix`, `get_IsRealMatrixMixed`, `DimActual` brought to exact Release IL match (5 methods).
- Release IL report: `python tools/validation/release_il.py --base main --project Quire.Mpl` PASS; 1,781 checked (`.mirror-cache/release-il-9d66a305c069/release-il.json`).
- Remaining managed members: 34 unfinished methods with Mathcad originals:
  - `Quire.Mcd.MatVal` (13): 6 display classes/iterators (`CastPtrs`, `offsets`, `element_types`, `get_doubles`, `get_data`), 5 legacy temp/loop patterns (`ThePtrNan`, `ReinsertPtrs`, `ReinsertPtrs1`, `get_type`, `get_data`), and 2 Hashtable loop differences (`IsUniformScalarPtrs`, `CanonicalizePtrs`).
  - `Quire.Mpl.Run.Delegates` (13): compiler display class numbering across `Premium_1..4`, `Closure_1..4`, `Wrap1..3`, `project21`, `project22`.
  - `Quire.Mpl.Run.Function` (7): compiler closure display class numbering across `CurryMatVal`, `UnCurryMatVal`, `VectorFunction`, and `AsF<...>`.
  - `Quire.Mcd.NumVal` (1): `LoadNaN` compiler temp allocation.
