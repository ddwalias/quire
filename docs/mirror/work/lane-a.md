# Lane A — Quire.Core

- Scope: all managed members in `src/Quire.Core`, including former C/D tier members (D18).
- Status: active; passing batches are being landed.
- Owner: Lane A.
- Branch: `mirror/a-core-20261021-quirecore`; worktree: `C:/Projects/Quire-wt/lane-a-core`.
- Selected members: 346 accepted mechanical methods (+63 fields), 157 manually reconstructed methods (+5 fields), and 24 abstract interface declarations in `IPositionProvider`, `IClipboard`, and `IEncryptor`; 12 mechanical candidates rejected by MemberGen.
- Release IL report: `.mirror-cache/release-il-b6632e808fb0/release-il.json` — PASS, 2,432 checked, 0 mismatches; Quire.Core has 1,671 unfinished methods.
- Remaining mismatches: 1,671 unfinished methods in the full Quire.Core inventory. Known current IL blockers include `EnumerableExtensions.IsNullOrEmpty` branch ordering, `DoubleComparer.GetDoubles(Point3D)` iterator naming, and `HashAlgorithmProvider.Dispose`; `BooleanToLayoutTransformConverter.Convert`, `AppVariantToVisibilityConverter.Convert`, `WpfUtils.CombineRelativeUris`, `DateFormatConverter.Convert`, and `BuildArchitectureToBitnessConverter.ConvertBack`; `FileUtils.GetProductNamePerVariant` needs its unmapped Ptc.License dependency; `VCardFactory.GetInstance` differs by compiler local initialization.

Use only the [five steps](../PLAN.md) and the [launch prompt](agent-prompts.md).
