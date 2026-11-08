# Release IL gate timings

Owner: gate-speed worker; branch: `tools/gate-speed-20261108`.
Scope: timing instrumentation in release_il.py; verdict and build commands unchanged.

Project: Quire.Controls.SolveBlock; both runs use base `30f5c3a8760d52886cd088712ff2542de872d342`.

| Step | Before (s) | Repeat (s) |
|---|---:|---:|
| judge_build | 12.791 | 14.777 |
| base_checkout | 37.817 | 47.427 |
| base_build | 118.434 | 0.000 |
| base_scan | 4.240 | 0.000 |
| candidate_build | 84.428 | 111.556 |
| candidate_scan | 5.022 | 4.372 |
| compare | 0.029 | 0.028 |
| total | 278.820 | 193.772 |

Both gates PASS, 31 checked, 451 declarations. Before has a cold base inventory; repeat uses the existing inventory cache. This is cache warming, not an optimisation attributable to this patch.

Largest cold step: base build. Existing code already builds only selected projects and their reference closure, and NuGet uses its shared package cache by default. No additional build/restore shortcut has been established safe; no behaviour optimisation made. Judge caching and scan/compare changes do not address the largest measured step. Candidate --no-incremental remains because stale output has not been ruled out.

Validation: python -m unittest test_release_il: 39 tests PASS (413.651 s); focused timing test PASS after build-timer exception handling adjustment. git diff --check clean.

Reports: ignored .mirror-cache/gate-speed-before/release-il.json and .mirror-cache/gate-speed-after/release-il.json.
Remaining: coordinator decision on further optimisation; landing waits for the four active lane merges to settle.
