# Lane Controls SolveBlock

- Owner: SolveBlock lane; branch: `mirror/solveblock-20261108`.
- Batches 1–5 passed and landed: ProductInfo constants/constructor (`28`), help constant (`28`), `FindMatchingParenthesis` (`29`), regex fields (`30`), and `GetEquationTypeOfCalc` (`31`); batch 5 rebased as `9481c27a`.
- Batch 6: `SolveBlockCalculatable.EngineResult`, `InterceptAndUpdateResult`, `UseICalculatableToRestoreResult`, `CurrentError`; explicit type-pattern/early-return shapes; PASS (`36 checked`; `.mirror-cache/release-il-9f73bf1e407a`). Initial gate (`release-il-6dfa20062ede`) found opposite branch directions and was fixed.
- `GetEquationTypeOfRl` remains stubbed because changing the enum exposes unmatched `SolveBlockControl+<>c__DisplayClass249_0::<AddLine>b__4`; details remain in earlier gate logs.
- Remaining: 385 stub markers in 18 generated types; no known mismatches in landed batches.
