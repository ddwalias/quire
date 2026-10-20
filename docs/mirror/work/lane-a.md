# Lane A — Quire.Core

- Scope: all managed members in `src/Quire.Core`, including former C/D tier members (D18).
- Status: active; passing batches are being landed.
- Owner: Lane A.
- Branch: `mirror/a-core-20261021-quirecore`; worktree: `C:/Projects/Quire-wt/lane-a-core`.
- Original: Use the assembly map for all original assemblies consolidated into Quire.Core.
- Selected members: 346 accepted mechanical methods (+63 fields), plus 86 manually reconstructed methods in `DirectionalPair`, `NullableExtensions`, `ThicknessExtension`, three coordinate converters, `Discrete2DCoordinateConverter.CanConvertFrom` and `ConvertTo`, `DiscretePoint` X/Y accessors and `ToString`, `DiscreteRect` Top/Left/Bottom/Right accessors and `Normalize`, `TextRange` constructor and `IsEquivalentTo`, `ProtectionData` password constructor and `GetProtectionPersistentData`, `Pair<T>`, `EmbeddedResourceCopier`, `FileOperationsProvider`, `DirectionOps`, `AssertionResult.GetHashCode`, `IntegerCompareConverter.Convert` and `ToInt`, two protection converters, `ProtectionDataExtension`, four Boolean/visibility converters, `NumberMatchToBooleanConverter`, `NumberToStringConverter`, `StringsMatchToBooleanConverter`, `StringFormatConverter`, `DateTimeFormatConverter`, `GeneralTransformToTransformConverter`, `ToolTipVisibilityConverter`, `StreamUtils`, `MouseFacade`, `Interval`, `PrintToFileException`, `FileUtils.GetAppDataPathWithBase`, two numbered `FileUtils` methods, `solveBlockRegionType`, six result-format DTO constructors, and 11 `UrlUtils` methods; 12 mechanical candidates rejected by MemberGen.
- Release IL report: `.mirror-cache/release-il-a2b10d6b5f22/release-il.json` — all Quire.Core members exact in this batch; overall FAIL is due to unrelated `Quire.Media3D` mismatches; 2,625 checked.
- Remaining mismatches: 1,765 unfinished methods in the full Quire.Core inventory. Known current compiler IL blockers: `BooleanToLayoutTransformConverter.Convert`, `AppVariantToVisibilityConverter.Convert`, `HyperlinkExtensions.GetText`, and `WpfUtils.CombineRelativeUris`; `ApplicationLogger.ReplaceMultiSpaceWithSingleSpace` also differs by a current compiler call form; `FileUtils.GetProductNamePerVariant` still needs its unmapped Ptc.License dependency; assembly completion unconfirmed.

Use only the [five steps](../PLAN.md) and the [launch prompt](agent-prompts.md).
All former reservations are released. Preserve previously exact methods, land passing
batches and continue independently around blockers. Only this assembly and checkpoint
belong to this lane; shared tools/mappings belong to the maintainer. Never inspect Debug IL.
Native/unreadable counterparts remain incomplete. Do not spawn agents or threads.
