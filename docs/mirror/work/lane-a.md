# Lane A — Quire.Core

- Owner: Lane A; branch `mirror/a-core-20261021-quirecore`; worktree `C:/Projects/Quire-wt/lane-a-core`.
- Members: `TableCoordinate`/`TableSize` formatting and equality methods; `BooleanToLayoutTransformConverter.Convert`; `TextElementAttachedProperties.SetStyleKey`; converter singleton and constant initializers for `BooleanInverse`, `DropDownListFontFamily`, `EnumMatchToVisibility`, `EnumToLocalizedString`, `EnvironmentVariableInverse`, `GeneralTransformToTransform`, `SettersToString`, `StyleImageToVisibility`, `StyleNameLegacy`, `StyleNameLegacyFontStyle`, `StyleNameToCategory`, `StyleToImage`, and `ToolTipVisibility` passed Release IL.
- Release IL report: `.mirror-cache/release-il-f66dde2b5c26/release-il.json` — PASS, 3,075 exact, 495 unfinished.
- Remaining mismatches: `headersContainerRegionCollectionSerializer.Serialize` callback IL; `DataObjectExtensions.GetTextWithEmbeddedItemsInfo`; `DebugTools.NotCalledFromApplicationLogger` callback ordinal; `WeakMultiCastDelegate.Remove` closure allocation IL; `WpfUtils.GroupTransformsAsFrozen`, `WpfUtils.IsLoaded`, `BooleanOperatorMultiConverter.Convert`, and `BuildArchitectureToBitnessConverter.ConvertBack` IL shape; `Encryptor.Encrypt` and `Decrypt` IL structure. `IO.FileUtils.GetProductNamePerVariant` depends on missing `LicenseManager` mapping.

Use only the [five steps](../PLAN.md) and the [launch prompt](agent-prompts.md).
