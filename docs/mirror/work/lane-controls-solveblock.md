# Lane Controls SolveBlock

- Owner: SolveBlock lane; branch: `mirror/solveblock-20261108`.
- Batches 1–5 passed and landed; batch 5 `9481c27a`.
- Batch 6 result/error passthroughs: PASS (`36 checked`); first merge gate MSB4166, retry PASS and landed `e5f6a6e4`.
- Batch 7 `WrappedCalculatable`: PASS (`37 checked`); landed `64b35a02`.
- Batch 8 constructor: PASS (`38 checked`; `.mirror-cache/release-il-2fbec0758d5d`); landed `f1756f18`.
- Batch 9 `UpdateEquationTypeOfSimpleCalculatable`: PASS (`39 checked`; `.mirror-cache/release-il-2ac959072727`).
- `GetEquationTypeOfRl` remains stubbed because enum changes expose unmatched closure `SolveBlockControl+<>c__DisplayClass249_0::<AddLine>b__4`.
- Remaining: 382 stub markers in 18 generated types; no known mismatches in landed batches.
