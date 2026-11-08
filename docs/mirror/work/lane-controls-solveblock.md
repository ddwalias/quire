# Lane Controls SolveBlock

- Owner: SolveBlock lane; branch: `mirror/solveblock-20261108`.
- Batches 1–5 passed and landed; batch 5 `9481c27a`.
- Batch 6 result/error passthrough properties: local PASS (`36 checked`); main gate first failed with MSB4166, retry PASS (`gate-il-183f85527bd2`), landed `e5f6a6e4`.
- Batch 7 `SolveBlockCalculatable.WrappedCalculatable`: PASS (`37 checked`; `.mirror-cache/release-il-548819f203cd`); landed rebased as `64b35a02`.
- Batch 8 `SolveBlockCalculatable` constructor: PASS (`38 checked`; `.mirror-cache/release-il-2fbec0758d5d`).
- `GetEquationTypeOfRl` remains stubbed; enum changes expose an unmatched `SolveBlockControl+<>c__DisplayClass249_0::<AddLine>b__4` closure (see earlier gate reports).
- Remaining: 383 stub markers in 18 generated types; no known mismatches in landed batches.
