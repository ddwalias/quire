# Lane Controls SolveBlock

- Owner: SolveBlock lane; branch: `mirror/solveblock-20261108`.
- Batches 1–5 passed/landed; batch 5 `9481c27a`.
- Batch 6 result/error passthroughs: PASS (`36 checked`); initial MSB4166, retry PASS; landed `e5f6a6e4`.
- Batch 7 `WrappedCalculatable`: PASS (`37 checked`); landed `64b35a02`.
- Batch 8 constructor: PASS (`38 checked`); landed `f1756f18`.
- Batch 9 `UpdateEquationTypeOfSimpleCalculatable`: PASS (`39 checked`); landed `1a947618`.
- Batch 10 `UpdateEquationType`: PASS (`40 checked`); landed `adcea50f`.
- Batch 11 `_equationType` field: PASS (`40 checked`); landed `47c1dc7e`.
- Batch 12 `FirePropertyChanged`: PASS (`41 checked`); landed `91575596`.
- Batch 13 `HandleRequestEvaluation`: PASS (`42 checked`); landed `1ab5bfb`.
- Batch 14 `ContainsError`, `ShowError`: PASS (`44 checked`; `.mirror-cache/release-il-466962c91b83`).
- `GetEquationTypeOfRl` remains stubbed due enum/closure mismatch.
- Remaining: 376 stub markers in 18 generated types; no known mismatches in landed batches.
