# Lane A — Quire.Core

- Scope: all managed members in `src/Quire.Core`, including former C/D tier members (D18).
- Status: active; passing batches are being landed.
- Owner: Lane A.
- Branch: `mirror/a-core-20261021-quirecore`; worktree: `C:/Projects/Quire-wt/lane-a-core`.
- Selected members: 346 accepted mechanical methods (+63 fields) and 148 manually reconstructed methods (+3 fields), including `AssertionResult`, `EnumerableExtensions`, `DoubleComparer`, persistent-data conversion flags, and `EnvironmentScope.Contains`; 12 mechanical candidates rejected by MemberGen.
- Release IL report: `.mirror-cache/release-il-afd1dc386427/release-il.json` — PASS, 2,418 checked, 0 mismatches.
- Remaining mismatches: 1,704 unfinished methods in the full Quire.Core inventory. Known compiler IL blockers include `EnumerableExtensions.IsNullOrEmpty` branch ordering and `DoubleComparer.GetDoubles(Point3D)` iterator naming; `BooleanToLayoutTransformConverter.Convert`, `AppVariantToVisibilityConverter.Convert`, `WpfUtils.CombineRelativeUris`, `DateFormatConverter.Convert`, and `BuildArchitectureToBitnessConverter.ConvertBack`; `FileUtils.GetProductNamePerVariant` needs its unmapped Ptc.License dependency; `VCardFactory.GetInstance` differs by compiler local initialization.

Use only the [five steps](../PLAN.md) and the [launch prompt](agent-prompts.md).
