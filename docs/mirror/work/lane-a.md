# Lane A — Quire.Core

- Scope: all managed members in `src/Quire.Core`, including former C/D tier members (D18).
- Status: active; passing batches are being landed.
- Owner: Lane A.
- Branch: `mirror/a-core-20261021-quirecore`; worktree: `C:/Projects/Quire-wt/lane-a-core`.
- Original: Use the assembly map for all original assemblies consolidated into Quire.Core.
- Selected members: 346 accepted mechanical methods (+63 fields), plus 71 manually reconstructed methods in `DirectionalPair`, `NullableExtensions`, `ThicknessExtension`, three coordinate converters, `Pair<T>`, `EmbeddedResourceCopier`, `FileOperationsProvider`, `DirectionOps`, `AssertionResult.GetHashCode`, `IntegerCompareConverter.Convert` and `ToInt`, `DiscretePoint` X/Y accessors and `ToString`, two protection converters, `ProtectionDataExtension`, four Boolean/visibility converters, `NumberMatchToBooleanConverter`, `NumberToStringConverter`, `StringsMatchToBooleanConverter`, `StringFormatConverter`, `DateTimeFormatConverter`, `GeneralTransformToTransformConverter`, `ToolTipVisibilityConverter`, `StreamUtils`, `MouseFacade`, `Interval`, `PrintToFileException`, `FileUtils.GetAppDataPathWithBase`, two numbered `FileUtils` methods, `solveBlockRegionType`, six result-format DTO constructors, and 11 `UrlUtils` methods; 12 mechanical candidates rejected by MemberGen.
- Release IL report: `.mirror-cache/release-il-f07f0fb19c4e/release-il.json` — PASS, 2,340 checked, 0 mismatches.
- Remaining mismatches: 1,780 unfinished methods in the full Quire.Core inventory. Known current compiler IL blockers: `BooleanToLayoutTransformConverter.Convert`, `AppVariantToVisibilityConverter.Convert`, `HyperlinkExtensions.GetText`, and `WpfUtils.CombineRelativeUris`; `ApplicationLogger.ReplaceMultiSpaceWithSingleSpace` also differs by a current compiler call form; `FileUtils.GetProductNamePerVariant` still needs its unmapped Ptc.License dependency; assembly completion unconfirmed.

Use only the [five steps](../PLAN.md) and the [launch prompt](agent-prompts.md).
All former reservations are released. Preserve previously exact methods, land passing
batches and continue independently around blockers. Only this assembly and checkpoint
belong to this lane; shared tools/mappings belong to the maintainer. Never inspect Debug IL.
Native/unreadable counterparts remain incomplete. Do not spawn agents or threads.
