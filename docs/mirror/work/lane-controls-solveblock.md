# Lane Controls SolveBlock

- Owner: SolveBlock lane; branch: `mirror/solveblock-20261108`.
- Batches 1–5 passed and landed; batch 5 is `9481c27a`.
- Batch 6: result/error passthrough properties; local PASS (`36 checked`; `.mirror-cache/release-il-9f73bf1e407a`); first lane gate failed with MSB4166 child-node crashes (`gate-il-e73e19a5731e`); after queue cleared, retry PASS (`gate-il-183f85527bd2`), landed rebased as `e5f6a6e4` (ancestor of main).
- Batch 7: `SolveBlockCalculatable.WrappedCalculatable`; base accessor and setter then `UpdateEquationType(base.WrappedCalculatable)`; PASS (`37 checked`; `.mirror-cache/release-il-548819f203cd`).
- `GetEquationTypeOfRl` remains stubbed because enum changes expose unmatched `SolveBlockControl+<>c__DisplayClass249_0::<AddLine>b__4`; details in earlier gate logs.
- Remaining: 384 stub markers in 18 generated types; no known mismatches in landed batches.
