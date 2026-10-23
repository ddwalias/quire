# Lane A — Quire.Core

- Owner: Lane A; branch `mirror/a-core-20261021-quirecore`; worktree `C:/Projects/Quire-wt/lane-a-core`.
- Members: `TableCoordinate` and `TableSize` string formatting, equality, typed/object equality, and hash code methods; `BooleanToLayoutTransformConverter.Convert`; `TextElementAttachedProperties.SetStyleKey` passed Release IL.
- Release IL report: `.mirror-cache/release-il-e2fdd94daac2/release-il.json` — PASS, 3,062 exact, 495 unfinished.
- Remaining mismatches: `headersContainerRegionCollectionSerializer.Serialize` callback IL; `DataObjectExtensions.GetTextWithEmbeddedItemsInfo`; `DebugTools.NotCalledFromApplicationLogger` callback ordinal; `WeakMultiCastDelegate.Remove` closure allocation IL; `WpfUtils.GroupTransformsAsFrozen`, `WpfUtils.IsLoaded`, and `BooleanOperatorMultiConverter.Convert` IL shape; `Encryptor.Encrypt` and `Decrypt` IL structure. `IO.FileUtils.GetProductNamePerVariant` depends on missing `LicenseManager` mapping.

Use only the [five steps](../PLAN.md) and the [launch prompt](agent-prompts.md).
