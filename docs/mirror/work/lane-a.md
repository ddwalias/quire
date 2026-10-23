# Lane A — Quire.Core

- Owner: Lane A; branch `mirror/a-core-20261021-quirecore`; worktree `C:/Projects/Quire-wt/lane-a-core`.
- Members: `TableCoordinate`/`TableSize` formatting and equality methods; `BooleanToLayoutTransformConverter.Convert`; `TextElementAttachedProperties.SetStyleKey`; converter singleton initializers for `BooleanInverseConverter`, `DropDownListFontFamilyConverter`, `EnumMatchToVisibilityConverter`, `EnvironmentVariableInverseConverter`, `StyleNameToCategoryConverter`, and `ToolTipVisibilityConverter` passed Release IL.
- Release IL report: `.mirror-cache/release-il-70eaeea78b78/release-il.json` — PASS, 3,068 exact, 495 unfinished.
- Remaining mismatches: `headersContainerRegionCollectionSerializer.Serialize` callback IL; `DataObjectExtensions.GetTextWithEmbeddedItemsInfo`; `DebugTools.NotCalledFromApplicationLogger` callback ordinal; `WeakMultiCastDelegate.Remove` closure allocation IL; `WpfUtils.GroupTransformsAsFrozen`, `WpfUtils.IsLoaded`, `BooleanOperatorMultiConverter.Convert`, and `BuildArchitectureToBitnessConverter.ConvertBack` IL shape; `Encryptor.Encrypt` and `Decrypt` IL structure. `IO.FileUtils.GetProductNamePerVariant` depends on missing `LicenseManager` mapping.

Use only the [five steps](../PLAN.md) and the [launch prompt](agent-prompts.md).
