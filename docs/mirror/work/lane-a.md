# Lane A — Quire.Core

- Scope: all managed members in `src/Quire.Core`, including former C/D tier members (D18).
- Status: active; passing batches are being landed.
- Owner: Lane A.
- Branch: `mirror/a-core-20261021-quirecore`; worktree: `C:/Projects/Quire-wt/lane-a-core`.
- Selected members: 346 accepted mechanical methods (+63 fields), 180 manually reconstructed methods (+5 fields), and 415 abstract interface declarations across Core, WPF, serialization, undo, quality agent, and service contracts; 12 mechanical candidates rejected by MemberGen.
- Release IL report: `.mirror-cache/release-il-311b863b3435/release-il.json` — Quire.Core has 0 mismatches; overall report has 72 Quire.Media3D failures because `main` advanced after the previous lane land. The inventory has 1,250 unfinished Quire.Core methods.
- Remaining mismatches: 1,250 unfinished methods in the full Quire.Core inventory. Known current IL blockers include `EnumerableExtensions.IsNullOrEmpty` branch ordering, `DoubleComparer.GetDoubles(Point3D)` iterator naming, and `HashAlgorithmProvider.Dispose`; `ArrayExtension.ToStringFormatted` and `ImageUtils.B16StringToBytes` compiler-generated closure names; `BooleanToLayoutTransformConverter.Convert`, `AppVariantToVisibilityConverter.Convert`, `WpfUtils.CombineRelativeUris`, `DateFormatConverter.Convert`, and `BuildArchitectureToBitnessConverter.ConvertBack`; `FileUtils.GetProductNamePerVariant` needs its unmapped Ptc.License dependency; `VCardFactory.GetInstance` differs by compiler local initialization.

Use only the [five steps](../PLAN.md) and the [launch prompt](agent-prompts.md).
