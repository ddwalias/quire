# Lane A — Quire.Core

- Scope: all managed members in `src/Quire.Core`, including former C/D tier members (D18).
- Status: active; passing batches are being landed.
- Owner: Lane A.
- Branch: `mirror/a-core-20261021-quirecore`; worktree: `C:/Projects/Quire-wt/lane-a-core`.
- Selected members: 346 accepted mechanical methods (+63 fields), 185 manually reconstructed methods (+5 fields), and 415 abstract interface declarations across Core, WPF, serialization, undo, quality agent, and service contracts; 12 mechanical candidates rejected by MemberGen.
- Release IL report: `.mirror-cache/release-il-2273d2db533b/release-il.json` — PASS, 2,455 checked, 0 mismatches; Quire.Core has 1,245 unfinished methods.
- Remaining mismatches: 1,245 unfinished methods in the full Quire.Core inventory. Known current IL blockers include `EnumerableExtensions.IsNullOrEmpty` branch ordering; `DoubleComparer.GetDoubles(Point3D)` iterator naming; `HashAlgorithmProvider.Dispose`; `ArrayExtension.ToStringFormatted` and `ImageUtils.B16StringToBytes` generated closure names; converter bodies `BooleanOperatorMultiConverter.Convert`, `BooleansToVisibilityMultiConverter.Convert`, `InvertToGridPlacementConverter.Convert`, `BooleanToLayoutTransformConverter.Convert`, `AppVariantToVisibilityConverter.Convert`, `DateFormatConverter.Convert`, `BuildArchitectureToBitnessConverter.ConvertBack`, and `ValueConverterGroup.Convert` generated closures; `DataObjectExtensions.GetTextWithEmbeddedItemsInfo` nullable construction order; `WpfUtils.CombineRelativeUris`; `FileUtils.GetProductNamePerVariant` needs its unmapped Ptc.License dependency; `VCardFactory.GetInstance` differs by compiler local initialization.

Use only the [five steps](../PLAN.md) and the [launch prompt](agent-prompts.md).
