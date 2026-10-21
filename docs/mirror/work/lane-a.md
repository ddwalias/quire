# Lane A — Quire.Core

- Scope: all managed members in `src/Quire.Core`, including former C/D tier members (D18).
- Status: active; passing batches are being landed.
- Owner: Lane A.
- Branch: `mirror/a-core-20261021-quirecore`; worktree: `C:/Projects/Quire-wt/lane-a-core`.
- Selected members: 346 accepted mechanical methods (+63 fields), 287 manually reconstructed methods (+13 fields), and 659 abstract interface declarations across Core, WPF, serialization, undo, quality agent, service contracts, WPF COM sinks, and XPS print interfaces; 12 mechanical candidates rejected by MemberGen. Latest member: `EnumHelper.TryParseWithCache<TEnum>`.
- Release IL report: `.mirror-cache/release-il-90eb5a9222b2/release-il.json` — PASS, 2,567 checked, 0 mismatches; Quire.Core has 897 unfinished methods.
- Remaining mismatches: 897 unfinished methods in the full Quire.Core inventory. Known current IL blockers include `DoubleComparer.GetDoubles(Point3D)` iterator naming; `ArrayExtension.Fill` branch layout and `ToStringFormatted` closure naming; `ImageUtils.B16StringToBytes` generated closure names; converter bodies `BooleanOperatorMultiConverter.Convert`, `BooleansToVisibilityMultiConverter.Convert`, `InvertToGridPlacementConverter.Convert`, `BooleanToLayoutTransformConverter.Convert`, `DateFormatConverter.Convert`, `BuildArchitectureToBitnessConverter.ConvertBack`, and `ValueConverterGroup.Convert` generated closures; `DataObjectExtensions.GetTextWithEmbeddedItemsInfo` nullable construction order; `MessageBoxTraceListener.Fail` generated closure; `WpfUtils.CombineRelativeUris`, `SetPropertyAndNotify<T>`, `IsLoaded`, `IsVisibleValid`, and `TryFindVisualAncestorByString`; `FileUtils.GetProductNamePerVariant` needs its unmapped Ptc.License dependency.

Use only the [five steps](../PLAN.md) and the [launch prompt](agent-prompts.md).
