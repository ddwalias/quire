# Lane Controls SolveBlock

- Owner: SolveBlock lane; branch: `mirror/solveblock-20261108`.
- Batches 1–5 passed/landed; batch 5 `9481c27a`.
- Batch 6 result/error passthroughs: PASS (`36 checked`); main gate first MSB4166, retry PASS; landed `e5f6a6e4`.
- Batch 7 `WrappedCalculatable`: PASS (`37 checked`); landed `64b35a02`.
- Batch 8 constructor: PASS (`38 checked`); landed `f1756f18`.
- Batch 9 `UpdateEquationTypeOfSimpleCalculatable`: PASS (`39 checked`); landed `1a947618`.
- Batch 10 `UpdateEquationType`: PASS (`40 checked`; `.mirror-cache/release-il-3b865ccd69d9`).
- `GetEquationTypeOfRl` remains stubbed due the enum/`SolveBlockControl` closure mismatch recorded in earlier gate reports.
- Remaining: 381 stub markers in 18 generated types; no known mismatches in landed batches.
